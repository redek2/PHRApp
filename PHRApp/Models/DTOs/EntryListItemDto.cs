using PHRApp.Models.Enums;

namespace PHRApp.Models.DTOs
{
    public class EntryListItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime EventDate { get; set; }
        public EntryStatus Status { get; set; }
        public List<string> CategoryNames { get; set; } = new();
        public string CategoryNamesDisplay => string.Join(", ", CategoryNames);
    }
}
