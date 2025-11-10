using System.Collections.Concurrent;
using System.Runtime.InteropServices;

public static class FDirectory
{
    [Flags]
    public enum EntryKind
    {
        None = 0,
        Files = 1,
        Directories = 2,
        FilesAndDirectories = Files | Directories
    }

    public readonly struct FastEntry
    {
        public string FullPath { get; }
        public string Name { get; }
        public bool IsDirectory { get; }
        public long Size { get; } // files only; 0 for directories
        public DateTime LastWriteTimeUtc { get; }
        public DateTime CreationTimeUtc { get; }
        public FileAttributes Attributes { get; }

        public FastEntry(
            string fullPath,
            string name,
            bool isDirectory,
            long size,
            DateTime lastWriteUtc,
            DateTime creationUtc,
            FileAttributes attributes)
        {
            FullPath = fullPath;
            Name = name;
            IsDirectory = isDirectory;
            Size = size;
            LastWriteTimeUtc = lastWriteUtc;
            CreationTimeUtc = creationUtc;
            Attributes = attributes;
        }
    }

    // High-level helpers matching Directory-like behaviors
    public static IEnumerable<string> EnumerateFiles(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly, int maxDegreeOfParallelism = 0, int boundedCapacity = 8192, CancellationToken cancellationToken = default)
    {
        foreach (var e in EnumerateFileSystemEntriesTyped(path, searchPattern, searchOption, EntryKind.Files, maxDegreeOfParallelism, boundedCapacity, cancellationToken))
            yield return e.FullPath;
    }

    public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly, int maxDegreeOfParallelism = 0, int boundedCapacity = 8192, CancellationToken cancellationToken = default)
    {
        foreach (var e in EnumerateFileSystemEntriesTyped(path, searchPattern, searchOption, EntryKind.Directories, maxDegreeOfParallelism, boundedCapacity, cancellationToken))
            yield return e.FullPath;
    }

    public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly, int maxDegreeOfParallelism = 0, int boundedCapacity = 8192, CancellationToken cancellationToken = default)
    {
        foreach (var e in EnumerateFileSystemEntriesTyped(path, searchPattern, searchOption, EntryKind.FilesAndDirectories, maxDegreeOfParallelism, boundedCapacity, cancellationToken))
            yield return e.FullPath;
    }

    // Main API (parallelized with SemaphoreSlim)
    public static IEnumerable<FastEntry> EnumerateFileSystemEntriesTyped(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly,
        EntryKind kinds = EntryKind.FilesAndDirectories,
        int maxDegreeOfParallelism = 0,
        int boundedCapacity = 8192,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("path is null or empty", nameof(path));

        bool recursive = searchOption == SearchOption.AllDirectories;
        int dop = maxDegreeOfParallelism <= 0 ? Math.Max(1, Environment.ProcessorCount * 2) : maxDegreeOfParallelism;

        // Bounded collection for streaming results
        using var results = new BlockingCollection<FastEntry>(boundedCapacity);

        // Control parallelization with a semaphore
        using var sem = new SemaphoreSlim(dop, dop);

        var failed = new ConcurrentBag<string>();

        // Track outstanding directory scans so we know when to CompleteAdding
        int pending = 0;

        // Schedule a scan for a given directory
        void StartScan(string dir)
        {
            Interlocked.Increment(ref pending);

            Task.Run(() =>
            {
                bool taken = false;
                try
                {
                    // Acquire a parallel slot
                    sem.Wait(cancellationToken);
                    taken = true;

                    // Scan this directory (native fast path, managed fallback)
                    var subDirs = ScanDirectory(dir, results, kinds, searchPattern, recursive, cancellationToken, failed);

                    // Schedule children
                    if (recursive && subDirs != null)
                    {
                        foreach (var sd in subDirs)
                        {
                            if (cancellationToken.IsCancellationRequested) break;
                            StartScan(sd);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // ignore, we're stopping
                }
                catch
                {
                    failed.Add(dir);
                }
                finally
                {
                    if (taken) sem.Release();

                    if (Interlocked.Decrement(ref pending) == 0)
                    {
                        // No more work in-flight; mark results complete
                        results.CompleteAdding();
                    }
                }
            }, CancellationToken.None);
        }

        // Kick off root scan; if TopDirectoryOnly, we still go through same flow but ScanDirectory won't return children
        StartScan(path);

        // Consume results as they arrive
        foreach (var item in results.GetConsumingEnumerable(cancellationToken))
        {
            yield return item;
        }
    }

    // Scans one directory: emits entries to 'results' and returns child subdirectories (if recursive)
    private static List<string> ScanDirectory(
        string path,
        BlockingCollection<FastEntry> results,
        EntryKind kinds,
        string searchPattern,
        bool recursive,
        CancellationToken ct,
        ConcurrentBag<string> failedFolders)
    {
        // First try fast NT path
        if (TryScanDirectoryNt(path, results, kinds, searchPattern, recursive, ct, out var subdirs))
            return subdirs;

        // Fallback to managed
        try
        {
            var children = recursive ? new List<string>(64) : null;

            // Directories
            try
            {
                foreach (var dir in Directory.GetDirectories(path))
                {
                    if (ct.IsCancellationRequested) break;

                    try
                    {
                        var di = new DirectoryInfo(dir);
                        var name = di.Name;
                        bool isDir = true;
                        bool isReparse = (di.Attributes & FileAttributes.ReparsePoint) != 0;

                        // Emit directory if needed
                        if ((kinds & EntryKind.Directories) != 0 && FastWildcard.IsMatch(name, searchPattern, ignoreCase: true))
                        {
                            results.Add(new FastEntry(dir, name, isDir, 0L, di.LastWriteTimeUtc, di.CreationTimeUtc, di.Attributes), ct);
                        }

                        if (recursive && !isReparse)
                            children?.Add(dir);
                    }
                    catch
                    {
                        failedFolders.Add(dir);
                    }
                }
            }
            catch
            {
                failedFolders.Add(path);
            }

            // Files
            try
            {
                foreach (var file in Directory.GetFiles(path))
                {
                    if (ct.IsCancellationRequested) break;

                    try
                    {
                        var fi = new FileInfo(file);
                        var name = fi.Name;

                        if ((kinds & EntryKind.Files) != 0 && FastWildcard.IsMatch(name, searchPattern, ignoreCase: true))
                        {
                            long size = SafeLength(fi);
                            results.Add(new FastEntry(file, name, isDirectory: false, size, fi.LastWriteTimeUtc, fi.CreationTimeUtc, fi.Attributes), ct);
                        }
                    }
                    catch
                    {
                        failedFolders.Add(file);
                    }
                }
            }
            catch
            {
                failedFolders.Add(path);
            }

            return children;
        }
        catch
        {
            failedFolders.Add(path);
            return null;
        }
    }

    private static bool TryScanDirectoryNt(
        string path,
        BlockingCollection<FastEntry> results,
        EntryKind kinds,
        string searchPattern,
        bool recursive,
        CancellationToken ct,
        out List<string> subdirs)
    {
        subdirs = recursive ? new List<string>(128) : null;

        IntPtr pPath = IntPtr.Zero;
        IntPtr pUStr = IntPtr.Zero;
        IntPtr hFile = IntPtr.Zero;
        IntPtr pBuffer = IntPtr.Zero;

        const int BufferSize = 64 * 1024;

        try
        {
            string ntPath = ToNtPath(path);
            if (!ntPath.EndsWith("\\", StringComparison.Ordinal))
                ntPath += "\\";

            // Build UNICODE_STRING and OBJECT_ATTRIBUTES
            pPath = Marshal.StringToHGlobalUni(ntPath);
            var us = new UNICODE_STRING
            {
                Length = (ushort)(ntPath.Length * 2),
                MaximumLength = (ushort)(ntPath.Length * 2),
                Buffer = pPath
            };
            pUStr = Marshal.AllocHGlobal(Marshal.SizeOf<UNICODE_STRING>());
            Marshal.StructureToPtr(us, pUStr, false);

            var oa = new OBJECT_ATTRIBUTES
            {
                Length = (uint)Marshal.SizeOf<OBJECT_ATTRIBUTES>(),
                RootDirectory = IntPtr.Zero,
                ObjectName = pUStr,
                Attributes = OBJ_CASE_INSENSITIVE,
                SecurityDescriptor = IntPtr.Zero,
                SecurityQualityOfService = IntPtr.Zero
            };

            var iosb = new IO_STATUS_BLOCK();

            int status = NtOpenFile(ref hFile,
                                    FILE_LIST_DIRECTORY | SYNCHRONIZE,
                                    ref oa,
                                    ref iosb,
                                    FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE,
                                    FILE_DIRECTORY_FILE | FILE_SYNCHRONOUS_IO_NONALERT | FILE_OPEN_FOR_BACKUP_INTENT);

            if (status != 0)
            {
                // Open failed (e.g., network/permission): fallback to managed
                return false;
            }

            pBuffer = Marshal.AllocHGlobal(BufferSize);
            bool restart = true;

            while (true)
            {
                if (ct.IsCancellationRequested)
                    break;

                status = NtQueryDirectoryFile(hFile,
                                              IntPtr.Zero,
                                              IntPtr.Zero,
                                              IntPtr.Zero,
                                              ref iosb,
                                              pBuffer,
                                              (uint)BufferSize,
                                              FILE_INFORMATION_CLASS.FileBothDirectoryInformation,
                                              false, // batched
                                              IntPtr.Zero,
                                              restart);

                if (status == STATUS_NO_MORE_FILES)
                    break;
                if (status < 0)
                    throw new ExternalException("NtQueryDirectoryFile failed", status);

                restart = false;

                unsafe
                {
                    byte* basePtr = (byte*)pBuffer;

                    for (; ; )
                    {
                        var entry = (FILE_BOTH_DIR_INFORMATION_UNMANAGED*)basePtr;

                        int nameCharCount = (int)(entry->FileNameLength / 2);
                        char* namePtr = entry->ShortName + 12; // FileName follows ShortName[12]

                        // Trim any trailing NUL
                        while (nameCharCount > 0 && namePtr[nameCharCount - 1] == '\0')
                            nameCharCount--;

                        if (nameCharCount > 0)
                        {
                            string name = new string(namePtr, 0, nameCharCount);
                            if (name != "." && name != "..")
                            {
                                string fullPath =
                                    (path.Length != 0 && path[path.Length - 1] == '\\')
                                    ? string.Concat(path, name)
                                    : string.Concat(path, "\\", name);

                                bool isDir = (entry->FileAttributes & (uint)FileAttributeDirectory) != 0;
                                bool isReparse = (entry->FileAttributes & (uint)FileAttributeReparsePoint) != 0;

                                bool want = (isDir && (kinds & EntryKind.Directories) != 0) ||
                                            (!isDir && (kinds & EntryKind.Files) != 0);

                                if (want && FastWildcard.IsMatch(name, searchPattern, ignoreCase: true))
                                {
                                    var attrs = (FileAttributes)entry->FileAttributes;
                                    long size = isDir ? 0L : entry->EndOfFile;
                                    DateTime lw = DateTime.FromFileTimeUtc(entry->LastWriteTime);
                                    DateTime cr = DateTime.FromFileTimeUtc(entry->CreationTime);

                                    results.Add(new FastEntry(fullPath, name, isDir, size, lw, cr, attrs), ct);
                                }

                                if (recursive && isDir && !isReparse)
                                {
                                    subdirs?.Add(fullPath);
                                }
                            }
                        }

                        if (entry->NextEntryOffset == 0)
                            break;

                        basePtr += entry->NextEntryOffset;
                    }
                }
            }

            return true;
        }
        catch
        {
            // Any failure: let caller fallback to managed
            subdirs = null;
            return false;
        }
        finally
        {
            if (pBuffer != IntPtr.Zero) Marshal.FreeHGlobal(pBuffer);
            if (hFile != IntPtr.Zero) NtClose(hFile);
            if (pUStr != IntPtr.Zero) Marshal.FreeHGlobal(pUStr);
            if (pPath != IntPtr.Zero) Marshal.FreeHGlobal(pPath);
        }
    }

    private static long SafeLength(FileInfo fi)
    {
        try { return fi.Length; } catch { return 0; }
    }

    // Fast wildcard matcher (no regex). Supports * and ? with ordinal case-insensitive option.
    private static class FastWildcard
    {
        public static bool IsMatch(string text, string pattern, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(pattern) || pattern == "*") return true;
            if (pattern == "*.*") pattern = "*"; // common Windows semantics

            int t = 0, p = 0;
            int starIdx = -1;
            int match = 0;

            while (t < text.Length)
            {
                if (p < pattern.Length && (pattern[p] == '?' || CharEquals(text[t], pattern[p], ignoreCase)))
                {
                    t++; p++;
                }
                else if (p < pattern.Length && pattern[p] == '*')
                {
                    starIdx = p;
                    match = t;
                    p++;
                }
                else if (starIdx != -1)
                {
                    p = starIdx + 1;
                    match++;
                    t = match;
                }
                else
                {
                    return false;
                }
            }

            while (p < pattern.Length && pattern[p] == '*')
                p++;

            return p == pattern.Length;
        }

        private static bool CharEquals(char a, char b, bool ignoreCase)
        {
            if (a == b) return true;
            if (!ignoreCase) return false;

            if (a <= 127 && b <= 127)
            {
                if (a >= 'a' && a <= 'z') a = (char)(a - 32);
                if (b >= 'a' && b <= 'z') b = (char)(b - 32);
                return a == b;
            }

            return char.ToUpperInvariant(a) == char.ToUpperInvariant(b);
        }
    }

    // NT path normalization (C:\ -> \??\C:\, UNC -> \??\UNC\server\share, etc.)
    private static string ToNtPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException("Path is null or empty", nameof(path));

        string p = path;
        if (p.IndexOf('/') >= 0)
            p = p.Replace('/', '\\');

        int len = p.Length;
        if (len < 2)
        {
            string abs = Path.GetFullPath(p);
            return ToNtPath(abs);
        }

        // Already NT or device namespace
        if (len >= 4 && p[0] == '\\' && p[1] == '?' && p[2] == '?' && p[3] == '\\')
            return p;
        if (len >= 8 && p[0] == '\\' && p[1] == 'D' && p[2] == 'e' && p[3] == 'v' &&
            p[4] == 'i' && p[5] == 'c' && p[6] == 'e' && p[7] == '\\')
            return p;

        // \\?\ -> \??\
        if (len >= 4 && p[0] == '\\' && p[1] == '\\' && p[2] == '?' && p[3] == '\\')
            return BuildWithPrefix(@"\??\", p, 4);

        // UNC \\server\share -> \??\UNC\server\share
        if (p[0] == '\\' && p[1] == '\\')
            return BuildWithPrefix(@"\??\UNC\", p, 2);

        // Drive-letter C:\...
        if (len >= 3)
        {
            char c0 = p[0];
            if (((c0 >= 'A' && c0 <= 'Z') || (c0 >= 'a' && c0 <= 'z')) && p[1] == ':' && p[2] == '\\')
                return BuildWithPrefix(@"\??\", p, 0);
        }

        string abs2 = Path.GetFullPath(p);
        return ToNtPath(abs2);
    }

    private static string BuildWithPrefix(string ntPrefix, string path, int skip)
    {
        int tailLen = path.Length - skip;
        int totalLen = ntPrefix.Length + tailLen;
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        return string.Create(totalLen, (ntPrefix, path, skip), (dest, state) =>
        {
            state.ntPrefix.AsSpan().CopyTo(dest);
            state.path.AsSpan(state.skip).CopyTo(dest.Slice(state.ntPrefix.Length));
        });
#else
var chars = new char[totalLen];
ntPrefix.CopyTo(0, chars, 0, ntPrefix.Length);
path.CopyTo(skip, chars, ntPrefix.Length, tailLen);
return new string(chars);
#endif
    }

    // Interop
    private const int STATUS_NO_MORE_FILES = unchecked((int)0x80000006);

    private const uint FILE_LIST_DIRECTORY = 0x0001;
    private const uint SYNCHRONIZE = 0x00100000;

    private const uint FILE_SHARE_READ = 0x00000001;
    private const uint FILE_SHARE_WRITE = 0x00000002;
    private const uint FILE_SHARE_DELETE = 0x00000004;

    private const uint FILE_DIRECTORY_FILE = 0x00000001;
    private const uint FILE_SYNCHRONOUS_IO_NONALERT = 0x00000020;
    private const uint FILE_OPEN_FOR_BACKUP_INTENT = 0x00004000;

    private const uint OBJ_CASE_INSENSITIVE = 0x00000040;

    private const uint FileAttributeDirectory = 0x00000010;
    private const uint FileAttributeReparsePoint = 0x00000400;

    private enum FILE_INFORMATION_CLASS
    {
        FileDirectoryInformation = 1,
        FileFullDirectoryInformation = 2,
        FileBothDirectoryInformation = 3,
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct IO_STATUS_BLOCK
    {
        public IntPtr Status;
        public IntPtr Information;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct UNICODE_STRING
    {
        public ushort Length;
        public ushort MaximumLength;
        public IntPtr Buffer;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct OBJECT_ATTRIBUTES
    {
        public uint Length;
        public IntPtr RootDirectory;
        public IntPtr ObjectName; // PUNICODE_STRING
        public uint Attributes;
        public IntPtr SecurityDescriptor;
        public IntPtr SecurityQualityOfService;
    }

    [DllImport("ntdll.dll")]
    private static extern int NtOpenFile(
        ref IntPtr FileHandle,
        uint DesiredAccess,
        ref OBJECT_ATTRIBUTES ObjectAttributes,
        ref IO_STATUS_BLOCK IoStatusBlock,
        uint ShareAccess,
        uint OpenOptions);

    [DllImport("ntdll.dll")]
    private static extern int NtClose(IntPtr Handle);

    [DllImport("ntdll.dll")]
    private static extern int NtQueryDirectoryFile(
        IntPtr FileHandle,
        IntPtr Event,
        IntPtr ApcRoutine,
        IntPtr ApcContext,
        ref IO_STATUS_BLOCK IoStatusBlock,
        IntPtr FileInformation,
        uint Length,
        FILE_INFORMATION_CLASS FileInformationClass,
        bool ReturnSingleEntry,
        IntPtr FileName,
        bool RestartScan);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private unsafe struct FILE_BOTH_DIR_INFORMATION_UNMANAGED
    {
        public uint NextEntryOffset;
        public uint FileIndex;
        public long CreationTime;
        public long LastAccessTime;
        public long LastWriteTime;
        public long ChangeTime;
        public long EndOfFile;
        public long AllocationSize;
        public uint FileAttributes;
        public uint FileNameLength;
        public uint EaSize;
        public byte ShortNameLength;
        public fixed char ShortName[12];
        // WCHAR FileName[] immediately follows
    }
}