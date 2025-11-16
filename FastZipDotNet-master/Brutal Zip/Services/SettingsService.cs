using System;
using System.IO;
using System.Text.Json;

namespace BrutalZip
{
    public static class SettingsService
    {
        public static AppSettings Current { get; private set; } = new AppSettings();


        private static string SettingsFolder =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BrutalSoftware", "BrutalZip");

        private static string SettingsPath => Path.Combine(SettingsFolder, "settings.json");

        public static void Load()
        {
            try
            {
                if (!File.Exists(SettingsPath))
                {
                    Directory.CreateDirectory(SettingsFolder);
                    Current = new AppSettings();
                    Save();
                    return;
                }

                var json = File.ReadAllText(SettingsPath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);
                Current = settings ?? new AppSettings();
            }
            catch
            {
                Current = new AppSettings();
            }
        }

        public static void Save()
        {
            Directory.CreateDirectory(SettingsFolder);
            var json = JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsPath, json);
        }
    }

    public class AppSettings
    {
        public string DefaultMethod { get; set; } = "Deflate";
        public int DefaultLevel { get; set; } = 6;

        public bool ThreadsAuto { get; set; } = true;
        public int Threads { get; set; } = Math.Max(1, Environment.ProcessorCount * 2);

        public string ExtractDefault { get; set; } = "Smart";
        public bool OpenExplorerAfterCreate { get; set; } = true;
        public bool OpenExplorerAfterExtract { get; set; } = false;

        public bool AddContextMenu { get; set; } = false;

        // NEW:
        public List<string> RecentArchives { get; set; } = new List<string>();
        public int RecentMax { get; set; } = 10;

        // NEW: global default encryption for new archives
        public bool EncryptNewArchivesByDefault { get; set; } = false;
        // Accepted values: "ZipCrypto", "AES128", "AES192", "AES256"
        public string DefaultEncryptAlgorithm { get; set; } = "ZipCrypto";

        public string LastExtractDir { get; set; } = "";
    }
}