namespace BL.Dtos
{
    public class CVFileDto
    {
        public Guid Id { get; set; }
        public Guid JobSeekerId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string BlobUrl { get; set; } = string.Empty;
        public int FileSizeBytes { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime UploadedAt { get; set; }

        // Helper property for display
        public string FileSizeDisplay => FileSizeBytes < 1024 * 1024
            ? $"{FileSizeBytes / 1024} KB"
            : $"{FileSizeBytes / (1024 * 1024)} MB";
    }
}