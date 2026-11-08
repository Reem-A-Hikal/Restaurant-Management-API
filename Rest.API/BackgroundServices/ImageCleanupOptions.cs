namespace Rest.API.BackgroundServices
{
    public sealed class ImageCleanupOptions
    {
        public const string SectionName = "ImageCleanup";
        public bool Enabled { get; set; } = true;

        /// <summary>false = log what would be deleted, delete nothing.</summary>
        public bool DeleteFiles { get; set; } = false;

        /// <summary>Never touch files younger than this (upload-then-save window).</summary>
        public int MinAgeHours { get; set; } = 24;

        public int IntervalHours { get; set; } = 24;
    }
}
