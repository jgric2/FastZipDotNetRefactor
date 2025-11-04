using System.Text.Json.Serialization;

namespace SfxStub
{
    public sealed class SfxConfig
    {
        public string Title { get; set; } = "Self Extracting Archive";
        public string CompanyName { get; set; } = null;

        public string DefaultExtractDir { get; set; } = "%TEMP%\\SFX_%NAME%";
        public bool Silent { get; set; } = false;
        public bool Overwrite { get; set; } = true;
        public string RunAfter { get; set; } = null;  // relative path inside extracted files
        public bool RequireElevation { get; set; } = false;
        public bool ShowCompletedDialog { get; set; } = true;

        // Password for encrypted zip (optional). If null/empty and zip is encrypted, the stub will prompt.
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Password { get; set; }

        // License
        public string LicenseText { get; set; } = null;          // If set and not Silent, show and require accept
        public bool RequireLicenseAccept { get; set; } = true;   // Whether to require Accept if LicenseText != null

        // UI branding (base64-encoded)
        public string IconBase64 { get; set; } = null;           // .ico; optional
        public string BannerImageBase64 { get; set; } = null;    // png/jpg; optional

        // Theme color as hex "#RRGGBB" or "RRGGBB"
        public string ThemeColor { get; set; } = null;
        public string ThemeColorEnd { get; set; } = null;

        // Extra UI options
        public bool ShowFileList { get; set; } = true;
    }
}