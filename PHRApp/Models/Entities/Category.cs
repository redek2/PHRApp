using PHRApp.Models.JoinEntities;

namespace PHRApp.Models.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        // Navigation properties
        public ICollection<EntryCategory> EntryCategories { get; set; } = new List<EntryCategory>();

    }
}