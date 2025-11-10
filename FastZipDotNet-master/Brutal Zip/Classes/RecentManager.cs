using BrutalZip;

namespace Brutal_Zip.Classes
{
    internal static class RecentManager
    {
        public static IReadOnlyList<string> GetList()
        {
            return SettingsService.Current.RecentArchives ??= new List<string>();
        }

        public static void Add(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;

            var list = SettingsService.Current.RecentArchives ??= new List<string>();

            // Normalize to full path
            try { path = Path.GetFullPath(path); } catch { }

            // Remove duplicate (case insensitive)
            list.RemoveAll(p => string.Equals(p, path, StringComparison.OrdinalIgnoreCase));

            // Insert at front
            list.Insert(0, path);

            // Trim to max
            int max = Math.Max(1, SettingsService.Current.RecentMax);
            if (list.Count > max) list.RemoveRange(max, list.Count - max);

            try { SettingsService.Save(); } catch { }
        }

        public static void Remove(string path)
        {
            var list = SettingsService.Current.RecentArchives ??= new List<string>();
            list.RemoveAll(p => string.Equals(p, path, StringComparison.OrdinalIgnoreCase));
            try { SettingsService.Save(); } catch { }
        }

        public static void Clear()
        {
            SettingsService.Current.RecentArchives?.Clear();
            try { SettingsService.Save(); } catch { }
        }
    }
}
