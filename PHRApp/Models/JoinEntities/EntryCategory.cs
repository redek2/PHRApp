using PHRApp.Models.Entities;

namespace PHRApp.Models.JoinEntities
{
    public class EntryCategory
    {
        public int EntryId { get; set; }
        public Entry Entry { get; set; } = null!;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}