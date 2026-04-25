namespace PHRApp.Models.DTOs
{
    public class StoredFileResult
    {
        public string OriginalFileName { get; set; } = null!;
        public string StoredFileName { get; set; } = null!;
        public string RelativePath { get; set; } = null!;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = null!;
    }
}