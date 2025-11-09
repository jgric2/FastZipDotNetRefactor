using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Brutal_Zip.Classes.Helpers
{
    public static class MemoryTrimmer
    {
        // OPTIONS TYPE
        public sealed class Options
        {
            // GC-related
            public bool WaitForFinalizers { get; set; } = true;
            public bool CompactLargeObjectHeap { get; set; } = true;
            public GCCollectionMode CollectionMode { get; set; } = GCCollectionMode.Aggressive;
            public bool Blocking { get; set; } = true;

            // Native + OS-related
            public bool TrimNativeHeaps { get; set; } = true;
            public bool TrimWorkingSet { get; set; } = true; // Windows-only
            public bool FreeUnusedCOMLibraries { get; set; } = false; // Windows-only, opt-in

            // Convenience toggle for a “lighter” pass (no working-set trim and no native-heap trim)
            public bool LightPass { get; set; } = false;
        }


        public static Task ForceFullGCAsync(bool compactLargeObjectHeap = true, bool waitForFinalizers = true, GCCollectionMode mode = GCCollectionMode.Aggressive, bool blocking = true, CancellationToken cancellationToken = default)
        => Task.Run(() => ForceFullGC(compactLargeObjectHeap, waitForFinalizers, mode, blocking), cancellationToken);

        public static Task<bool> TryTrimWorkingSetAsync(CancellationToken cancellationToken = default)
        => Task.Run(() => TryTrimWorkingSet(), cancellationToken);

        public static Task<bool> TryTrimNativeHeapsAsync(CancellationToken cancellationToken = default)
        => Task.Run(() => TryTrimNativeHeaps(), cancellationToken);

        public static Task<bool> TryFreeUnusedCOMLibrariesAsync(CancellationToken cancellationToken = default)
        => Task.Run(() => TryFreeUnusedCOMLibraries(), cancellationToken);

        public static async Task<bool> CollectIfHighMemoryLoadAsync(
        double thresholdRatio = 0.85,
        Options? options = null,
        CancellationToken cancellationToken = default)
        {
            var info = GC.GetGCMemoryInfo();
            if (info.HighMemoryLoadThresholdBytes > 0 && info.MemoryLoadBytes > 0)
            {
                double ratio = (double)info.MemoryLoadBytes / info.HighMemoryLoadThresholdBytes;
                if (ratio < thresholdRatio)
                    return false;
            }

            await MinimizeFootprintAsync(options, cancellationToken).ConfigureAwait(false);
            return true;
        }

        // MAIN ONE-SHOT HELPER: does finalizers → compacting GC → native trim → WS trim
        public static void MinimizeFootprint(Options? options = null)
        {
            options ??= new Options();

            if (options.LightPass)
            {
                ForceFullGC(options.CompactLargeObjectHeap, options.WaitForFinalizers, options.CollectionMode, options.Blocking);
                return;
            }

            // 1) Finalizers + compacting GC (can return free segments to the OS)
            ForceFullGC(options.CompactLargeObjectHeap, options.WaitForFinalizers, options.CollectionMode, options.Blocking);

            // 2) Trim native heaps (may return native pages to the OS if libraries used malloc/CRT heaps)
            if (options.TrimNativeHeaps)
                TryTrimNativeHeaps();

            // 3) Ask COM loader to unload any idle COM DLLs
            if (options.FreeUnusedCOMLibraries)
                TryFreeUnusedCOMLibraries();

            // 4) Trim the working set (Windows-only) to drop resident pages
            if (options.TrimWorkingSet)
                TryTrimWorkingSet();
        }

        // Async wrapper if you’d rather not block the caller/UI thread
        public static Task MinimizeFootprintAsync(Options? options = null, CancellationToken cancellationToken = default)
            => Task.Run(() => MinimizeFootprint(options), cancellationToken);

        public static Task MinimizeFootprintMaximumAsync(CancellationToken cancellationToken = default)
        {
            var options = new MemoryTrimmer.Options
            {
                WaitForFinalizers = true,
                CompactLargeObjectHeap = true,
                TrimWorkingSet = true,      // Windows-only
                TrimNativeHeaps = true,     // Win/Linux
                FreeUnusedCOMLibraries = false // Set true only if you actually use COM and want it
            };
            return Task.Run(() => MinimizeFootprint(options), cancellationToken);
        }

        public static void MinimizeFootprintMaximum()
        {
            var options = new MemoryTrimmer.Options
            {
                WaitForFinalizers = true,
                CompactLargeObjectHeap = true,
                TrimWorkingSet = true,      // Windows-only
                TrimNativeHeaps = true,     // Win/Linux
                FreeUnusedCOMLibraries = false // Set true only if you actually use COM and want it
            };
            MinimizeFootprint(options);
        }

        // Forces a full, compacting GC (and optionally LOH compaction + finalizers)
        public static void ForceFullGC(
            bool compactLargeObjectHeap = true,
            bool waitForFinalizers = true,
            GCCollectionMode mode = GCCollectionMode.Aggressive,
            bool blocking = true)
        {
            try
            {
                if (compactLargeObjectHeap)
                {
                    // LOH compaction happens on the next full GC only
                    System.Runtime.GCSettings.LargeObjectHeapCompactionMode =
                        System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
                }

                // Typical “finalizers then collect again” pattern to reclaim objects
                // that were only released by finalizers.
                GC.Collect(GC.MaxGeneration, mode, blocking, compacting: true);
                if (waitForFinalizers)
                {
                    GC.WaitForPendingFinalizers();
                    GC.Collect(GC.MaxGeneration, mode, blocking, compacting: true);
                }
            }
            catch
            {
                // Swallow by design; trimming is best-effort
            }
        }

        // Windows-only: working set trim. Falls back across two APIs for robustness.
        public static bool TryTrimWorkingSet()
        {
            if (!OperatingSystem.IsWindows())
                return false;

            try
            {
                using var p = Process.GetCurrentProcess();
                var h = p.Handle;

                // First try the classic "reset to defaults and empty" call.
                if (SetProcessWorkingSetSize(h, new IntPtr(-1), new IntPtr(-1)))
                    return true;

                // Fallback to EmptyWorkingSet if needed.
                return EmptyWorkingSet(h);
            }
            catch
            {
                return false;
            }
        }

        // Native heap trim: Windows (CRT heap compact), Linux (glibc malloc_trim), macOS not supported.
        public static bool TryTrimNativeHeaps()
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    var heap = GetProcessHeap();
                    if (heap != IntPtr.Zero)
                    {
                        // Returns 0 on failure; non-zero means "largest free block size", which implies success
                        var res = HeapCompact(heap, 0);
                        return res != UIntPtr.Zero;
                    }
                }
                else if (OperatingSystem.IsLinux())
                {
                    // Returns 1 if memory was actually released, 0 if not possible
                    int rc = malloc_trim(0);
                    return rc == 1;
                }
            }
            catch
            {
                // best-effort
            }
            return false;
        }

        // Optional: if your app uses COM components and you want idle ones unloaded
        public static bool TryFreeUnusedCOMLibraries()
        {
            if (!OperatingSystem.IsWindows())
                return false;

            try
            {
                // Unload immediately (0-delay) any unused COM libraries
                CoFreeUnusedLibrariesEx(0, 0);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Only trim aggressively when memory load is high.
        // Returns true if a trim was performed.
        public static bool CollectIfHighMemoryLoad(double thresholdRatio = 0.85, Options? options = null)
        {
            // GC.GetGCMemoryInfo is cross-platform. We compare current load vs GC's high-load threshold.
            var info = GC.GetGCMemoryInfo();
            if (info.HighMemoryLoadThresholdBytes <= 0 || info.MemoryLoadBytes <= 0)
            {
                // Fallback heuristic: just collect if we can't read metrics
                MinimizeFootprint(options);
                return true;
            }

            double ratio = (double)info.MemoryLoadBytes / info.HighMemoryLoadThresholdBytes;
            if (ratio >= thresholdRatio)
            {
                MinimizeFootprint(options);
                return true;
            }
            return false;
        }

        // Small helpers to observe memory numbers
        public static long GetWorkingSetBytes()
        {
            using var p = Process.GetCurrentProcess();
            return p.WorkingSet64;
        }

        public static long GetPrivateBytes()
        {
            using var p = Process.GetCurrentProcess();
            return p.PrivateMemorySize64;
        }

        // ------------------ P/Invoke and OS-specific wiring ------------------

        // Windows: working set APIs
        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetProcessWorkingSetSize(IntPtr hProcess, IntPtr dwMinimumWorkingSetSize, IntPtr dwMaximumWorkingSetSize);

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [DllImport("psapi.dll", SetLastError = true)]
        private static extern bool EmptyWorkingSet(IntPtr hProcess);

        // Windows: CRT heap APIs
        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcessHeap();

        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern UIntPtr HeapCompact(IntPtr hHeap, uint dwFlags);

        // Windows: COM unload
        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [DllImport("ole32.dll", ExactSpelling = true)]
        private static extern void CoFreeUnusedLibrariesEx(uint dwUnloadDelay, uint dwReserved);

        // Linux: glibc trim
        [Obfuscation(Exclude = true, StripAfterObfuscation = true)]
        [DllImport("libc", EntryPoint = "malloc_trim")]
        private static extern int malloc_trim(ulong pad);
    }
}
