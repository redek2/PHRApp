using PHRApp.Models.Enums;

namespace PHRApp.Models.DTOs
{
    public class EntryDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public EntryStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<string> CategoryNames { get; set; } = new();
        public List<AttachmentDto> Attachments { get; set; } = new();
        public string CategoryNameDisplay => string.Join(", ", CategoryNames);
        public string StatusDisplay => Status.ToDisplayString();
    }
}
