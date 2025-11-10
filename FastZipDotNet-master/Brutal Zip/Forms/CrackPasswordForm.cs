using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using Brutal_Zip.Classes;
using BrutalZip2025.BrutalControls;
using FastZipDotNet.Zip.Encryption;

namespace Brutal_Zip
{
    public partial class CrackPasswordForm : ModernForm
    {
        private readonly PasswordCrackContext _ctx;

        public string FoundPassword { get; private set; }

        // probe data
        private bool _isAes;
        private byte _aesStrength; // 1/2/3
        private byte[] _aesSalt;
        private byte[] _aesPwv;
        private byte[] _zipCryptoHdr12; // ciphertext header
        private byte _zipCryptoVerifier; // expected last byte

        private volatile bool _running;
        private CancellationTokenSource _cts;
        private AdjustableSemaphore _gate; // live-adjustable concurrency gate

        private byte _zipCryptoCheck10;   // expected decrypted header[10]
        private byte _zipCryptoCheck11;   // expected decrypted header[11]

        private long _attempts;
        private readonly Stopwatch _sw = new Stopwatch();

        public CrackPasswordForm(PasswordCrackContext ctx)
        {
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
            InitializeComponent();
            tbThreads.Maximum = Environment.ProcessorCount * 2;
            btnClose.Click += (s, e) => { if (!_running) Close(); };

            Text = "Crack Password — " + Path.GetFileName(_ctx.ZipPath);
            // default threads label
            lblThreadsVal.Text = tbThreads.Value.ToString();
            // Build probe immediately
            try
            {
                BuildProbe();
                lblStatus.Text = _isAes
                    ? $"Target: AES-{(_aesStrength == 1 ? 128 : _aesStrength == 2 ? 192 : 256)} (salt {(_aesSalt?.Length ?? 0)} bytes)"
                    : $"Target: ZipCrypto (12-byte header verify)";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Unable to prepare cracking probe:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        // Read minimal bytes needed to test passwords quickly
        private void BuildProbe()
        {
            var zfe = _ctx.TargetEntry;

            using var fs = new FileStream(
                _ctx.ZipPath,
                new FileStreamOptions
                {
                    Mode = FileMode.Open,
                    Access = FileAccess.Read,
                    Share = FileShare.ReadWrite | FileShare.Delete,
                    BufferSize = 1 << 20,
                    Options = FileOptions.RandomAccess
                });

            using var br = new BinaryReader(fs, Encoding.Default, leaveOpen: true);

            // Seek to local header
            fs.Seek((long)zfe.HeaderOffset, SeekOrigin.Begin);
            uint sig = br.ReadUInt32();
            if (sig != 0x04034b50) throw new InvalidDataException("Bad local header signature");

            ushort versionNeeded = br.ReadUInt16();
            ushort gpFlags = br.ReadUInt16();
            ushort methodLocal = br.ReadUInt16(); // may be 99 for AES
            ushort lastModTime = br.ReadUInt16();
            ushort lastModDate = br.ReadUInt16();
            uint crc32Local = br.ReadUInt32();
            uint compSizeLocal = br.ReadUInt32();
            uint uncompSizeLocal = br.ReadUInt32();
            ushort nameLen = br.ReadUInt16();
            ushort extraLen = br.ReadUInt16();

            if (nameLen > 0) br.ReadBytes(nameLen);
            if (extraLen > 0) br.ReadBytes(extraLen);

            bool hasDD = (gpFlags & 0x0008) != 0;
            bool isEncrypted = (gpFlags & 0x0001) != 0;
            if (!isEncrypted) throw new InvalidOperationException("Selected entry is not encrypted.");

            if (zfe.IsAes)
            {
                _isAes = true;
                _aesStrength = zfe.AesStrength != 0 ? zfe.AesStrength : (byte)3;
                int saltLen = WinZipAes.GetSaltLength(_aesStrength);

                _aesSalt = br.ReadBytes(saltLen);
                if (_aesSalt.Length != saltLen) throw new EndOfStreamException("Unexpected EOF reading AES salt");

                _aesPwv = br.ReadBytes(2);
                if (_aesPwv.Length != 2) throw new EndOfStreamException("Unexpected EOF reading AES PWV");

                // (AES PWV is checked later; nothing else to do here)
            }
            else
            {
                _isAes = false;

                // ZipCrypto: first 12 bytes of data is encryption header
                var hdr = br.ReadBytes(12);
                if (hdr.Length != 12) throw new EndOfStreamException("Unexpected EOF reading ZipCrypto header");
                _zipCryptoHdr12 = hdr;

                // Compute the expected verifier BYTES for header[10] and header[11]
                // If data descriptor present -> use lastModTime (little endian: low byte then high byte)
                // Else -> use high-16 bits of CRC-32 (order: (crc>>16)&0xff, then (crc>>24)&0xff)
                if (hasDD)
                {
                    _zipCryptoCheck10 = (byte)(lastModTime & 0xff);
                    _zipCryptoCheck11 = (byte)(lastModTime >> 8);
                }
                else
                {
                    _zipCryptoCheck10 = (byte)((zfe.Crc32 >> 16) & 0xff);
                    _zipCryptoCheck11 = (byte)((zfe.Crc32 >> 24) & 0xff);
                }
            }
        }

        private void tbThreads_ValueChanged(object sender, EventArgs e)
        {
            lblThreadsVal.Text = tbThreads.Value.ToString();
            if (_gate != null)
            {
                _gate.MaximumCount = tbThreads.Value;
            }
        }

        private void btnEstimate_Click(object sender, EventArgs e)
        {
            var chars = BuildCharset();
            long minL = (long)numMinLen.Value;
            long maxL = Math.Max(minL, (long)numMaxLen.Value);
            long N = chars.Length <= 0 ? 0 : chars.Length;
            // Calculate sum_{L=min}^{max} N^L (watch for overflow)
            try
            {
                System.Numerics.BigInteger total = 0;
                for (long L = minL; L <= maxL; L++)
                {
                    System.Numerics.BigInteger add = System.Numerics.BigInteger.One;
                    for (long i = 0; i < L; i++) add *= N;
                    total += add;
                }
                lblKeyspace.Text = $"Keyspace: ~ {total} combinations";
            }
            catch
            {
                lblKeyspace.Text = "Keyspace: very large";
            }
        }

        private void btnBrowseDict_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "Text files|*.txt;*.lst;*.dic|All files|*.*"
            };
            if (ofd.ShowDialog(this) == DialogResult.OK)
                txtDictPath.Text = ofd.FileName;
        }


        public enum CrackType
        {
            BruteForce,
            Dictionary
        }

        CrackType selectedTabCrackType = CrackType.BruteForce;

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (_running) return;

            // sanity check
            if (selectedTabCrackType == CrackType.BruteForce)
            {
                if (BuildCharset().Length == 0)
                {
                    MessageBox.Show(this, "Select at least one character group or provide custom characters.", "Error");
                    return;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txtDictPath.Text) || !File.Exists(txtDictPath.Text))
                {
                    MessageBox.Show(this, "Select a valid dictionary file.", "Error");
                    return;
                }
            }

            buttonBruteForce.Enabled = false;
            buttonDictionary.Enabled = false;

            _running = true;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            _cts = new CancellationTokenSource();
            _gate = new AdjustableSemaphore(tbThreads.Value);
            _attempts = 0;
            _sw.Restart();
            lblFound.Text = "Found: -";
            lblStatus.Text = "Working…";

            if (selectedTabCrackType == CrackType.BruteForce)
                _ = RunBruteforceAsync(_cts.Token);
            else
                _ = RunDictionaryAsync(_cts.Token);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (!_running) return;
            _cts?.Cancel();

            buttonBruteForce.Enabled = true;
            buttonDictionary.Enabled = true;
        }

        private void StopUi(string msg)
        {
            _sw.Stop();
            _running = false;
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            lblStatus.Text = msg;
        }

        // Build character set
        private char[] BuildCharset()
        {
            var sb = new StringBuilder();
            if (chkLower.Checked) sb.Append("abcdefghijklmnopqrstuvwxyz");
            if (chkUpper.Checked) sb.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            if (chkDigits.Checked) sb.Append("0123456789");
            if (chkSymbols.Checked) sb.Append("!@#$%^&*()-_=+[]{};:'\",.<>/?\\|`~");
            if (!string.IsNullOrEmpty(txtCustom.Text)) sb.Append(txtCustom.Text);
            // remove duplicates
            var seen = new HashSet<char>();
            var list = new List<char>();
            foreach (var ch in sb.ToString())
                if (seen.Add(ch)) list.Add(ch);
            return list.ToArray();
        }

        // Test a password candidate using probe
        private bool TestCandidate(string pwd)
        {
            if (_isAes)
            {
                return TestCandidateAES(pwd);
            }
            else
            {
                return TestCandidatePK(pwd);
            }
        }


        private bool TestCandidateAES(string pwd)
        {
            var (enc, mac, pwv) = WinZipAes.DeriveKeys(pwd ?? string.Empty, _aesSalt, _aesStrength);

            bool ok = pwv.Length >= 2 &&
                      _aesPwv.Length >= 2 &&
                      pwv[0] == _aesPwv[0] &&
                      pwv[1] == _aesPwv[1];

            Array.Clear(enc, 0, enc.Length);
            Array.Clear(mac, 0, mac.Length);
            Array.Clear(pwv, 0, pwv.Length);
            return ok;
        }

        private bool TestCandidatePK(string pwd)
        {
            if (_zipCryptoHdr12 == null || _zipCryptoHdr12.Length != 12)
                return false;

            var tmp = ArrayPool<byte>.Shared.Rent(12);
            try
            {
                Buffer.BlockCopy(_zipCryptoHdr12, 0, tmp, 0, 12);

                var tc = new TraditionalZipCrypto(pwd ?? string.Empty);
                tc.Decrypt(tmp, 0, 12);

                // Verify both header[10] and header[11] against the expected two-byte check.
                return tmp[10] == _zipCryptoCheck10 && tmp[11] == _zipCryptoCheck11;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(tmp);
            }
        }



        private void UpdateStatusUi(string extra = null)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(UpdateStatusUi), extra);
                return;
            }
            double secs = Math.Max(0.001, _sw.Elapsed.TotalSeconds);
            long attempts = Interlocked.Read(ref _attempts);
            double aps = attempts / secs;
            lblStatus.Text = $"Attempts: {attempts:N0}   Speed: {aps:N0}/s   Elapsed: {_sw.Elapsed:hh\\:mm\\:ss}" +
                             (extra != null ? $"   {extra}" : "");

            labelAttempts.Text = $"Attempts: {attempts:N0}";
            labelSpeed.Text = $"Speed: {aps:N0}/s";
            labelElapsed.Text = $"Elapsed: {_sw.Elapsed:hh\\:mm\\:ss}";

        }

        // Dictionary runner
        private async Task RunDictionaryAsync(CancellationToken ct)
        {
            try
            {
                var bc = new BlockingCollection<string>(boundedCapacity: 8192);
                var producer = Task.Run(() => DictProducer(bc, ct), ct);

                int maxWorkers = Math.Max(1, Environment.ProcessorCount * 2);
                var tasks = new List<Task>(maxWorkers);

                if (_isAes)
                {
                    for (int i = 0; i < maxWorkers; i++)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            while (!ct.IsCancellationRequested)
                            {
                                // concurrency gate
                                _gate.WaitOne();
                                try
                                {
                                    if (!bc.TryTake(out var pwd, 50, ct))
                                        continue;

                                    if (TestCandidateAES(pwd))
                                    {
                                        FoundPassword = pwd;
                                        lblFound.BeginInvoke(new Action(() => lblFound.Text = "Found: " + pwd));
                                        _cts.Cancel();
                                        MessageBox.Show("Password found: " + pwd);
                                        return;
                                    }
                                    Interlocked.Increment(ref _attempts);
                                }
                                finally
                                {
                                    _gate.Release();
                                }

                                if ((_attempts & 0x1FFF) == 0) UpdateStatusUi();
                                await Task.Yield();
                            }
                        }, ct));
                    }
                }
                else
                {
                    for (int i = 0; i < maxWorkers; i++)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            while (!ct.IsCancellationRequested)
                            {
                                // concurrency gate
                                _gate.WaitOne();
                                try
                                {
                                    if (!bc.TryTake(out var pwd, 50, ct))
                                        continue;

                                    if (TestCandidatePK(pwd))
                                    {
                                        FoundPassword = pwd;
                                        lblFound.BeginInvoke(new Action(() => lblFound.Text = "Found: " + pwd));
                                        _cts.Cancel();
                                        MessageBox.Show("Password found: " + pwd);
                                        return;
                                    }
                                    Interlocked.Increment(ref _attempts);
                                }
                                finally
                                {
                                    _gate.Release();
                                }

                                if ((_attempts & 0x1FFF) == 0) UpdateStatusUi();
                                await Task.Yield();
                            }
                        }, ct));
                    }
                }





                await producer;
                await Task.WhenAll(tasks);
                if (!ct.IsCancellationRequested && string.IsNullOrEmpty(FoundPassword))
                    StopUi("Finished. No match.");
                else if (!string.IsNullOrEmpty(FoundPassword))
                {
                    StopUi("Match found.");
                    DialogResult = DialogResult.OK;
                }
            }
            catch (OperationCanceledException)
            {
                StopUi(string.IsNullOrEmpty(FoundPassword) ? "Cancelled." : "Cancelled (match found).");
                if (!string.IsNullOrEmpty(FoundPassword)) DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                StopUi("Error: " + ex.Message);
            }
        }

        private void DictProducer(BlockingCollection<string> bc, CancellationToken ct)
        {
            try
            {
                using var sr = new StreamReader(txtDictPath.Text, Encoding.UTF8, true, 1 << 16);
                string line;
                while (!ct.IsCancellationRequested && (line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.Length == 0) continue;

                    // Base word
                    bc.Add(line, ct);

                    if (chkMutateCase.Checked)
                    {
                        bc.Add(line.ToLowerInvariant(), ct);
                        bc.Add(line.ToUpperInvariant(), ct);
                        // Title case simple:
                        if (line.Length > 1)
                            bc.Add(char.ToUpper(line[0]) + line.Substring(1).ToLowerInvariant(), ct);
                    }

                    if (chkAppendDigits.Checked)
                    {
                        for (int n = 0; n <= 999; n++)
                        {
                            bc.Add(line + n.ToString("D"), ct);
                            if (ct.IsCancellationRequested) break;
                        }
                    }
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                bc.CompleteAdding();
            }
        }

        // Brute force runner
        private async Task RunBruteforceAsync(CancellationToken ct)
        {
            try
            {
                var chars = BuildCharset();
                var prefix = txtPrefix.Text ?? "";
                long minL = (long)numMinLen.Value;
                long maxL = Math.Max(minL, (long)numMaxLen.Value);

                // Precompute per-length totals
                var totals = new System.Numerics.BigInteger[maxL - minL + 1];
                for (long L = minL, i = 0; L <= maxL; L++, i++)
                {
                    System.Numerics.BigInteger t = 1;
                    for (long k = 0; k < L; k++) t *= chars.Length;
                    totals[i] = t;
                }

                // global index
                System.Numerics.BigInteger global = 0;
                System.Numerics.BigInteger globalMax = 0;
                foreach (var t in totals) globalMax += t;

                // We can’t Interlocked increment BigInteger, so we partition by chunks
                // Strategy: use a shared long chunk counter over the “last length” dimension only – enough for practical runs.
                // We’ll map chunk indices to (length, offset) at runtime. ChunkSize chosen to amortize overhead.
                const int ChunkSize = 5000;
                long chunkCursor = 0; // counts chunks across all lengths approximated by using double mapping

                // Build an array of length info (length L and its total combos)
                var lens = new (long L, System.Numerics.BigInteger Count)[totals.Length];
                for (int i = 0; i < totals.Length; i++)
                    lens[i] = (minL + i, totals[i]);

                int maxWorkers = Math.Max(1, Environment.ProcessorCount * 2);
                var tasks = new List<Task>(maxWorkers);

                if (_isAes)
                {
                    for (int w = 0; w < maxWorkers; w++)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            var rnd = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId));
                            var buf = new char[128];

                            while (!ct.IsCancellationRequested)
                            {
                                _gate.WaitOne();
                                try
                                {
                                    long chunkId = Interlocked.Add(ref chunkCursor, 1) - 1;

                                    // Map chunkId to a (length, baseIndex) by cycling lengths
                                    int iLen = (int)(chunkId % lens.Length);
                                    long L = lens[iLen].L;
                                    var totalForLen = lens[iLen].Count;

                                    // Convert chunkId to big starting index approximated by chunkId * ChunkSize modulo total
                                    var baseIndex = (System.Numerics.BigInteger)(chunkId / lens.Length) * ChunkSize;
                                    if (baseIndex >= totalForLen) baseIndex %= totalForLen;

                                    // Iterate ChunkSize candidates in this length
                                    for (int j = 0; j < ChunkSize && !ct.IsCancellationRequested; j++)
                                    {
                                        var idx = baseIndex + j;
                                        if (idx >= totalForLen) break;

                                        // convert idx to string of length L from chars[]
                                        int plen = prefix.Length;
                                        for (int x = 0; x < plen; x++) buf[x] = prefix[x];

                                        var n = idx;
                                        for (long pos = L - 1; pos >= 0; pos--)
                                        {
                                            var rem = (int)(n % chars.Length);
                                            buf[plen + pos] = chars[rem];
                                            n /= chars.Length;
                                        }
                                        string candidate = new string(buf, 0, plen + (int)L);

                                        if (TestCandidateAES(candidate))
                                        {
                                            FoundPassword = candidate;
                                            lblFound.BeginInvoke(new Action(() => lblFound.Text = "Found: " + candidate));
                                            _cts.Cancel();
                                            return;
                                        }

                                        Interlocked.Increment(ref _attempts);
                                        if ((_attempts & 0x1FFF) == 0) UpdateStatusUi();
                                    }
                                }
                                finally
                                {
                                    _gate.Release();
                                }

                                await Task.Yield();
                            }
                        }, ct));
                    }
                }
                else
                {
                    for (int w = 0; w < maxWorkers; w++)
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            var rnd = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId));
                            var buf = new char[128];

                            while (!ct.IsCancellationRequested)
                            {
                                _gate.WaitOne();
                                try
                                {
                                    long chunkId = Interlocked.Add(ref chunkCursor, 1) - 1;

                                    // Map chunkId to a (length, baseIndex) by cycling lengths
                                    int iLen = (int)(chunkId % lens.Length);
                                    long L = lens[iLen].L;
                                    var totalForLen = lens[iLen].Count;

                                    // Convert chunkId to big starting index approximated by chunkId * ChunkSize modulo total
                                    var baseIndex = (System.Numerics.BigInteger)(chunkId / lens.Length) * ChunkSize;
                                    if (baseIndex >= totalForLen) baseIndex %= totalForLen;

                                    // Iterate ChunkSize candidates in this length
                                    for (int j = 0; j < ChunkSize && !ct.IsCancellationRequested; j++)
                                    {
                                        var idx = baseIndex + j;
                                        if (idx >= totalForLen) break;

                                        // convert idx to string of length L from chars[]
                                        int plen = prefix.Length;
                                        for (int x = 0; x < plen; x++) buf[x] = prefix[x];

                                        var n = idx;
                                        for (long pos = L - 1; pos >= 0; pos--)
                                        {
                                            var rem = (int)(n % chars.Length);
                                            buf[plen + pos] = chars[rem];
                                            n /= chars.Length;
                                        }
                                        string candidate = new string(buf, 0, plen + (int)L);

                                        if (TestCandidatePK(candidate))
                                        {
                                            //if (candidate == "asd")
                                            //{
                                            //    var tt = "";
                                            //}

                                            FoundPassword = candidate;
                                            lblFound.BeginInvoke(new Action(() => lblFound.Text = "Found: " + candidate));
                                            _cts.Cancel();
                                            return;
                                        }

                                        Interlocked.Increment(ref _attempts);
                                        if ((_attempts & 0x1FFF) == 0) UpdateStatusUi();
                                    }
                                }
                                finally
                                {
                                    _gate.Release();
                                }

                                await Task.Yield();
                            }
                        }, ct));
                    }
                }


                await Task.WhenAll(tasks);
                if (!ct.IsCancellationRequested && string.IsNullOrEmpty(FoundPassword))
                    StopUi("Finished. No match.");
                else if (!string.IsNullOrEmpty(FoundPassword))
                {
                    StopUi("Match found.");
                    DialogResult = DialogResult.OK;
                }
            }
            catch (OperationCanceledException)
            {
                StopUi(string.IsNullOrEmpty(FoundPassword) ? "Cancelled." : "Cancelled (match found).");
                if (!string.IsNullOrEmpty(FoundPassword)) DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                StopUi("Error: " + ex.Message);
            }
        }

        private void CrackPasswordForm_Load(object sender, EventArgs e)
        {

        }

        private void buttonBruteForce_Click(object sender, EventArgs e)
        {
            panelBruteForce.Show();
            panelDictionary.Hide();
            panelJob.Hide();

            selectedTabCrackType = CrackType.BruteForce;
        }

        private void buttonDictionary_Click(object sender, EventArgs e)
        {
            panelBruteForce.Hide();
            panelDictionary.Show();
            panelJob.Hide();

            selectedTabCrackType = CrackType.Dictionary;
        }

        private void buttonJob_Click(object sender, EventArgs e)
        {
            panelBruteForce.Hide();
            panelDictionary.Hide();
            panelJob.Show();

        }

        private void panelDictionary_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
