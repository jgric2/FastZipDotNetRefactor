namespace FastZipDotNet.Zip
{
    public enum ZipOperation { Extract, Build, Test, Repair }

    public sealed class ZipProgress
    {
        public ZipOperation Operation { get; set; }
        public string CurrentFile { get; set; }

        // Totals
        public int TotalFiles { get; set; }
        public long TotalBytesUncompressed { get; set; }
        public long TotalBytesCompressed { get; set; } // sum of archive entry compressed sizes (for extract)

        // Completed so far
        public int FilesProcessed { get; set; }
        public long BytesProcessedUncompressed { get; set; } // uncompressed bytes processed
        public long BytesProcessedCompressed { get; set; }   // compressed bytes processed (approx while extracting)

        // Time
        public TimeSpan Elapsed { get; set; }

        // Speeds
        public double SpeedBytesPerSec { get; set; }
        public double FilesPerSec { get; set; }

        // Derived metrics
        public double Percent => TotalBytesUncompressed > 0 ? (100.0 * BytesProcessedUncompressed / TotalBytesUncompressed) : 0.0;

        public TimeSpan? ETA => SpeedBytesPerSec > 1e-6
            ? TimeSpan.FromSeconds(Math.Max(0.0, (TotalBytesUncompressed - BytesProcessedUncompressed) / SpeedBytesPerSec))
            : (TimeSpan?)null;

        // Archive compression ratio (overall, from the ZIP)
        public double ArchiveCompressionRatio =>
            TotalBytesUncompressed > 0 ? (double)TotalBytesCompressed / TotalBytesUncompressed : 0.0;

        // Compression ratio for processed data so far (during extraction this is an approximation)
        public double CurrentCompressionRatio =>
            BytesProcessedUncompressed > 0 ? (double)BytesProcessedCompressed / BytesProcessedUncompressed : 0.0;

        // Back-compat aliases if you already use these names
        public long BytesProcessed => BytesProcessedUncompressed;
        public long TotalBytes => TotalBytesUncompressed;
    }
}