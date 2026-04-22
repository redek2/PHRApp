using PHRApp.Models.Enums;
using PHRApp.Models.JoinEntities;

namespace PHRApp.Models.Entities
{
    public class Entry
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public EntryStatus Status { get; set; } = EntryStatus.Planned;
        public bool IsArchived { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<EntryCategory> EntryCategories { get; set; } = new List<EntryCategory>();
        public ICollection<EntryAttachment> EntryAttachments { get; set; } = new List<EntryAttachment>();
    }
}