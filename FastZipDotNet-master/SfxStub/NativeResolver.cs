using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SfxStub // or your main app namespace if you add there too
{
    internal static class NativeResolver
    {
        // Map names used in DllImport to loaded handles
        private static readonly Dictionary<string, IntPtr> _handles = new(StringComparer.OrdinalIgnoreCase);
        private static bool _registered;
        private static string _extractDir;

        public static void Register()
        {
            if (_registered) return;
            _registered = true;

            // Process-private folder to hold native DLLs
            _extractDir = Path.Combine(
                Path.GetTempPath(),
                "Native_" + (Process.GetCurrentProcess().Id.ToString("D")) + "_" + Guid.NewGuid().ToString("N"));

            try { Directory.CreateDirectory(_extractDir); } catch { }

            AppDomain.CurrentDomain.ProcessExit += (_, __) => SafeDeleteExtractDir();
            AppDomain.CurrentDomain.DomainUnload += (_, __) => SafeDeleteExtractDir();

            // Extract & load the native libs we need based on arch
            // Adjust resource names to your embedding (see csproj snippet below)
            string arch = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant(); // x64, arm64, x86
                                                                                                // Common P/Invoke names in your FastZip code:
                                                                                                //   - libdeflate: "libdeflate.dll"
                                                                                                //   - libzstd: "libzstd"
            TryExtractAndLoad("libdeflate.dll", $"native/{OsRidArch()}/libdeflate.dll");
            TryExtractAndLoad("libzstd", $"native/{OsRidArch()}/libzstd.dll");  // DllImport("libzstd") resolves to libzstd.dll

            // Register global resolver for this assembly
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), Resolve);
        }

        private static string OsRidArch()
        {
            // Build a RID-like folder name for resources: win-x64, win-x86, win-arm64
            // We target Windows for this repo.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return RuntimeInformation.ProcessArchitecture switch
                {
                    Architecture.X64 => "win-x64",
                    Architecture.X86 => "win-x86",
                    Architecture.Arm64 => "win-arm64",
                    Architecture.Arm => "win-arm",
                    _ => "win-x64"
                };
            }
            // If you also plan Unix later, extend here.
            return "win-x64";
        }

        private static void TryExtractAndLoad(string logicalName, string resourcePath)
        {
            try
            {
                // Retrieve embedded resource stream
                var asm = Assembly.GetExecutingAssembly();
                // Resource names use dots; normalize
                // Example: if you embed with LogicalName="native/win-x64/libdeflate.dll"
                // internally it might be "SfxStub.native.win-x64.libdeflate.dll" (assemblyname + dots)
                string resName = FindResourceName(asm, resourcePath);
                if (resName == null) return;

                string fileName = Path.GetFileName(resourcePath);
                string outPath = Path.Combine(_extractDir, fileName);

                if (!File.Exists(outPath))
                {
                    using var s = asm.GetManifestResourceStream(resName);
                    using var fs = new FileStream(outPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                    s.CopyTo(fs);
                }

                // Load and keep handle
                IntPtr h = NativeLibrary.Load(outPath);
                _handles[logicalName] = h;

                // For libzstd, DllImport uses "libzstd", not "libzstd.dll"
                if (logicalName.Equals("libzstd", StringComparison.OrdinalIgnoreCase))
                    _handles["libzstd.dll"] = h;
            }
            catch
            {
                // ignored; if not found we won't resolve
            }
        }

        private static string FindResourceName(Assembly asm, string resourcePath)
        {
            // Convert slashes to dots
            string dotPath = resourcePath.Replace('/', '.').Replace('\\', '.');
            foreach (var name in asm.GetManifestResourceNames())
            {
                if (name.EndsWith(dotPath, StringComparison.OrdinalIgnoreCase))
                    return name;
            }
            return null;
        }

        private static IntPtr Resolve(string libraryName, Assembly assembly, DllImportSearchPath? path)
        {
            // If we preloaded a handle for this library name, return it
            if (_handles.TryGetValue(libraryName, out var h))
                return h;

            // In some cases, "libzstd" -> preloaded under "libzstd.dll"
            if (libraryName.Equals("libzstd", StringComparison.OrdinalIgnoreCase) &&
                _handles.TryGetValue("libzstd.dll", out h))
                return h;

            return IntPtr.Zero;
        }

        private static void SafeDeleteExtractDir()
        {
            try
            {
                if (!string.IsNullOrEmpty(_extractDir) && Directory.Exists(_extractDir))
                    Directory.Delete(_extractDir, true);
            }
            catch { }
        }
    }
}