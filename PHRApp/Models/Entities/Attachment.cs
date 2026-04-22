using PHRApp.Models.JoinEntities;

namespace PHRApp.Models.Entities
{
    public class Attachment
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public string StoredFileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long FileSize { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<EntryAttachment> EntryAttachments { get; set; } = new List<EntryAttachment>();
    }
}