using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Brutal_Zip
{
    internal static class NativeBootstrap
    {
        private static readonly Dictionary<string, IntPtr> _map = new(StringComparer.OrdinalIgnoreCase);
        private static string _dir;
        private static bool _inited;

        public static void Initialize()
        {
            if (_inited) return;
            _inited = true;

            _dir = Path.Combine(Path.GetTempPath(), "BZ_Native_" + Process.GetCurrentProcess().Id + "_" + Guid.NewGuid().ToString("N"));
            try { Directory.CreateDirectory(_dir); } catch { }

            AppDomain.CurrentDomain.ProcessExit += (_, __) => Cleanup();
            AppDomain.CurrentDomain.DomainUnload += (_, __) => Cleanup();

            TryAddDllDirectory(_dir); // helps native dependency resolution (Lexilla for Scintilla)

            string rid = Rid();

            // Extract all we may need for this arch (if a resource is missing, Extract just returns)
            Extract("libdeflate.dll", $"native/{rid}/libdeflate.dll");
            Extract("libzstd.dll", $"native/{rid}/libzstd.dll");
            Extract("Scintilla.dll", $"native/{rid}/Scintilla.dll");   // or SciLexer.dll depending on your build
            Extract("SciLexer.dll", $"native/{rid}/SciLexer.dll");
            Extract("Lexilla.dll", $"native/{rid}/Lexilla.dll");
            Extract("WebView2Loader.dll", $"native/{rid}/WebView2Loader.dll");

            // Preload in an order that plays nicely with dependencies.
            // If SciLexer depends on Lexilla, either load Lexilla first or ensure it’s in the same folder.
            LoadOrIgnore("Lexilla.dll");
            LoadOrIgnore("Scintilla.dll", aliases: new[] { "Scintilla" }); // alias if the wrapper uses "Scintilla"
            LoadOrIgnore("SciLexer.dll", aliases: new[] { "SciLexer" });  // alias if the wrapper uses "SciLexer"
            LoadOrIgnore("libdeflate.dll");
            LoadOrIgnore("libzstd.dll", aliases: new[] { "libzstd" });     // alias because DllImport may be "libzstd"
            LoadOrIgnore("WebView2Loader.dll");

            // Register resolvers for assemblies that contain DllImports
            RegisterFor(Assembly.GetExecutingAssembly()); // this ziplib itself
            TryRegisterFor(typeof(FastZipDotNet.Zip.FastZipDotNet).Assembly); // FastZipDotNet -> libdeflate/libzstd

            // WebView2 DllImports live in Microsoft.Web.WebView2.Core
            TryRegisterByName("Microsoft.Web.WebView2.Core");

            // Scintilla wrapper assembly name varies ("ScintillaNET", "ScintillaNET.WPF", etc).
            TryRegisterByName("ScintillaNET");

            // Catch late loads
            AppDomain.CurrentDomain.AssemblyLoad += (_, e) =>
            {
                var n = e.LoadedAssembly.GetName().Name ?? "";
                if (n.StartsWith("FastZipDotNet", StringComparison.OrdinalIgnoreCase) ||
                    n.Equals("Microsoft.Web.WebView2.Core", StringComparison.OrdinalIgnoreCase) ||
                    n.StartsWith("Scintilla", StringComparison.OrdinalIgnoreCase) ||
                    n.StartsWith("ScintillaNET", StringComparison.OrdinalIgnoreCase))
                {
                    TryRegisterFor(e.LoadedAssembly);
                }
            };
        }

        private static void RegisterFor(Assembly asm)
        {
            try { NativeLibrary.SetDllImportResolver(asm, Resolve); } catch { }
        }

        private static void TryRegisterFor(Assembly asm)
        {
            if (asm == null) return;
            RegisterFor(asm);
        }

        private static void TryRegisterByName(string simpleName)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (string.Equals(asm.GetName().Name, simpleName, StringComparison.OrdinalIgnoreCase))
                {
                    RegisterFor(asm);
                    return;
                }
            }
        }

        private static IntPtr Resolve(string libraryName, Assembly asm, DllImportSearchPath? path)
        {
            if (_map.TryGetValue(libraryName, out var h)) return h;

            // A couple of common aliases
            if (libraryName.Equals("libzstd", StringComparison.OrdinalIgnoreCase) && _map.TryGetValue("libzstd.dll", out h)) return h;
            if (libraryName.Equals("Scintilla", StringComparison.OrdinalIgnoreCase) && _map.TryGetValue("Scintilla.dll", out h)) return h;
            if (libraryName.Equals("SciLexer", StringComparison.OrdinalIgnoreCase) && _map.TryGetValue("SciLexer.dll", out h)) return h;

            return IntPtr.Zero;
        }

        private static void Extract(string logicalName, string resPath)
        {
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                var resName = FindResourceName(asm, resPath);
                if (resName == null) return;

                var outPath = Path.Combine(_dir, logicalName);
                if (!File.Exists(outPath))
                {
                    using var s = asm.GetManifestResourceStream(resName);
                    using var fs = new FileStream(outPath, FileMode.Create, FileAccess.Write, FileShare.Read);
                    s.CopyTo(fs);
                }
            }
            catch { }
        }

        private static void LoadOrIgnore(string fileName, string[] aliases = null)
        {
            try
            {
                var path = Path.Combine(_dir, fileName);
                if (!File.Exists(path)) return;
                var h = NativeLibrary.Load(path);
                _map[fileName] = h;

                var stem = Path.GetFileNameWithoutExtension(fileName);
                _map[stem] = h;

                if (aliases != null)
                {
                    foreach (var a in aliases) _map[a] = h;
                }
            }
            catch { }
        }

        private static string Rid()
        {
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
            return "win-x64";
        }

        private static string FindResourceName(Assembly asm, string resourcePath)
        {
            string dotPath = resourcePath.Replace('/', '.').Replace('\\', '.');
            foreach (var n in asm.GetManifestResourceNames())
                if (n.EndsWith(dotPath, StringComparison.OrdinalIgnoreCase)) return n;
            return null;
        }

        private static void Cleanup()
        {
            try { if (!string.IsNullOrEmpty(_dir) && Directory.Exists(_dir)) Directory.Delete(_dir, true); } catch { }
        }

        // Optional: widen search so the loader can find dependent DLLs in _dir
        private const int LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetDefaultDllDirectories(int directoryFlags);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr AddDllDirectory(string lpPathName);

        private static void TryAddDllDirectory(string dir)
        {
            try
            {
                SetDefaultDllDirectories(LOAD_LIBRARY_SEARCH_DEFAULT_DIRS);
                AddDllDirectory(dir);
            }
            catch { }
        }
    }
}
