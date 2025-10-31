using System.Text.Json.Serialization;

namespace Brutal_Zip.Classes
{
    public sealed class SfxBuildConfig
    {
        public string Title { get; set; } = "Self Extracting Archive";
        public string CompanyName { get; set; } = null;

        public string DefaultExtractDir { get; set; } = "%TEMP%\\SFX_%NAME%";
        public bool Silent { get; set; } = false;
        public bool Overwrite { get; set; } = true;
        public string RunAfter { get; set; } = null;
        public bool RequireElevation { get; set; } = false;
        public bool ShowCompletedDialog { get; set; } = true;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Password { get; set; }

        // License
        public string LicenseText { get; set; } = null;
        public bool RequireLicenseAccept { get; set; } = true;

        // Branding (base64)
        public string IconBase64 { get; set; } = null;         // bytes of .ico, base64
        public string BannerImageBase64 { get; set; } = null;  // bytes of .png/.jpg, base64

        // Theme color hex "#RRGGBB" or "RRGGBB"
        public string ThemeColor { get; set; } = null;

        // Extra UI
        public bool ShowFileList { get; set; } = true;
    }
}
