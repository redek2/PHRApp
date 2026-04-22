using PHRApp.Models.Entities;

namespace PHRApp.Models.JoinEntities
{
    public class EntryAttachment
    {
        public int EntryId { get; set; }
        public Entry Entry { get; set; } = null!;
        public int AttachmentId { get; set; }
        public Attachment Attachment { get; set; } = null!;
    }
}