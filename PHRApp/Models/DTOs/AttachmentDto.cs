namespace PHRApp.Models.DTOs
{
    public class AttachmentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = null!;
    }
}
