using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using System.Text.Json;

namespace Brutal_Zip.Classes
{
    internal static class SingleInstance
    {
        private static readonly string MutexName = @"Global\BrutalZip_SingleBurst";
        private static Mutex _mutex;
        private static CancellationTokenSource _cts;
        private static Task _serverTask;
        private static DateTime _lastActivityUtc;
        private static TimeSpan _idleTimeout = TimeSpan.FromMilliseconds(1200);

        private static string PipeName => $"BrutalZipPipe_{WindowsIdentity.GetCurrent()?.User?.Value ?? "Default"}";

        // Try to become burst-primary immediately (no blocking).
        public static bool TryBecomePrimary()
        {
            try
            {
                // Create or open the mutex
                _mutex = new Mutex(false, MutexName);
                // Attempt to take ownership without waiting
                if (!_mutex.WaitOne(0))
                {
                    // Somebody else already owns the burst
                    _mutex.Close();
                    _mutex = null;
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        // Keep trying to forward args to a running primary for up to totalWaitMs
        public static bool TryForwardToPrimary(string[] args, int totalWaitMs = 1500, int attemptIntervalMs = 120, int connectTimeoutMs = 200)
        {
            var deadline = Environment.TickCount + totalWaitMs;
            do
            {
                try
                {
                    using var client = new NamedPipeClientStream(".", PipeName, PipeDirection.Out);
                    client.Connect(connectTimeoutMs); // short connect
                    var json = JsonSerializer.Serialize(args ?? Array.Empty<string>());
                    var bytes = Encoding.UTF8.GetBytes(json);
                    client.Write(bytes, 0, bytes.Length);
                    return true;
                }
                catch
                {
                    Thread.Sleep(attemptIntervalMs);
                }
            } while (Environment.TickCount < deadline);

            return false;
        }

        // Run a short-lived server; stop after idleTimeoutMs with no new messages
        public static void StartServer(MainForm form, int idleTimeoutMs = 1200)
        {
            _idleTimeout = TimeSpan.FromMilliseconds(Math.Max(300, idleTimeoutMs));
            _cts = new CancellationTokenSource();
            _lastActivityUtc = DateTime.UtcNow;

            _serverTask = Task.Run(async () =>
            {
                try
                {
                    while (!_cts.IsCancellationRequested)
                    {
                        using var server = new NamedPipeServerStream(
                            PipeName,
                            PipeDirection.In,
                            8, // allow several client connects during burst
                            PipeTransmissionMode.Message,
                            PipeOptions.Asynchronous);

                        var wait = server.WaitForConnectionAsync(_cts.Token);

                        // Idle watchdog
                        while (!wait.IsCompleted && !_cts.IsCancellationRequested)
                        {
                            await Task.Delay(50, _cts.Token).ConfigureAwait(false);
                            if (DateTime.UtcNow - _lastActivityUtc > _idleTimeout)
                            {
                                _cts.Cancel();
                                break;
                            }
                        }

                        if (_cts.IsCancellationRequested)
                            break;

                        try { await wait.ConfigureAwait(false); }
                        catch { break; }

                        // Got a connection
                        _lastActivityUtc = DateTime.UtcNow;

                        try
                        {
                            using var ms = new MemoryStream();
                            var buf = new byte[4096];
                            do
                            {
                                int r = await server.ReadAsync(buf, 0, buf.Length, _cts.Token).ConfigureAwait(false);
                                if (r <= 0) break;
                                ms.Write(buf, 0, r);
                            } while (!server.IsMessageComplete);

                            var json = Encoding.UTF8.GetString(ms.ToArray());
                            var args = JsonSerializer.Deserialize<string[]>(json) ?? Array.Empty<string>();

                            _lastActivityUtc = DateTime.UtcNow;

                            form.BeginInvoke(new Action(async () =>
                            {
                                try { await form.HandleCommandAsync(args); } catch { }
                            }));
                        }
                        catch
                        {
                            // Ignore message errors, continue
                        }
                    }
                }
                catch
                {
                    // swallow and exit
                }
                finally
                {
                    try { _mutex?.ReleaseMutex(); } catch { }
                    try { _mutex?.Close(); } catch { }
                    _mutex = null;
                }
            }, _cts.Token);
        }

        public static void StopServer()
        {
            try { _cts?.Cancel(); } catch { }
            try { _serverTask?.Wait(200); } catch { }
            _cts = null;
            _serverTask = null;

            try { _mutex?.ReleaseMutex(); } catch { }
            try { _mutex?.Close(); } catch { }
            _mutex = null;
        }
    }
}