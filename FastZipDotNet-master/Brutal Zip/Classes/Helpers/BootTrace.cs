using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Brutal_Zip.Classes.Helpers
{
    internal static class BootTrace
    {
        private static readonly object _lock = new object();
        private static readonly Stopwatch _sw = new Stopwatch();
        private static string _path;
        private static bool _enabled;

        // Call this once very early in Program.Main
        public static void Init(string tag = null, bool enabled = true)
        {
            _enabled = enabled;
            if (!_enabled) return;

            try
            {
                var pid = Process.GetCurrentProcess().Id;
                var name = "BrutalZip_Startup_" + pid + (string.IsNullOrWhiteSpace(tag) ? "" : "_" + tag) + ".log";
                _path = Path.Combine(Path.GetTempPath(), name);
                _sw.Restart();
                WriteLine("==== BrutalZip startup trace ====");
                WriteLine("PID: " + pid + "  Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            catch
            {
                _enabled = false;
            }
        }

        public static void Mark(string label)
        {
            if (!_enabled) return;
            try
            {
                WriteLine($"{_sw.ElapsedMilliseconds,7} ms | {label}");
            }
            catch { }
        }

        public static void Exception(Exception ex, string label = null)
        {
            if (!_enabled) return;
            try
            {
                WriteLine($"{_sw.ElapsedMilliseconds,7} ms | EXCEPTION {label ?? ""}: {ex.GetType().Name}: {ex.Message}");
                WriteLine(ex.StackTrace ?? "");
            }
            catch { }
        }

        public static string LogPath => _path;

        private static void WriteLine(string line)
        {
            lock (_lock)
            {
                File.AppendAllText(_path, line + Environment.NewLine, Encoding.UTF8);
            }
        }
    }
}