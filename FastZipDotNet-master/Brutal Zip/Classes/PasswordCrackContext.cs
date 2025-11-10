namespace Brutal_Zip.Classes
{
    using static FastZipDotNet.Zip.Structure.ZipEntryStructs;

    public sealed class PasswordCrackContext
    {
        public string ZipPath { get; }
        public ZipFileEntry TargetEntry { get; }

        public PasswordCrackContext(string zipPath, ZipFileEntry entry)
        {
            ZipPath = zipPath ?? throw new ArgumentNullException(nameof(zipPath));
            TargetEntry = entry;
        }
    }
}
