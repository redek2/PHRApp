using PHRApp.Models.Enums;

namespace PHRApp.Models.DTOs
{
    public class EntryQueryDto
    {
        public int? CategoryId { get; set; }
        public EntryStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SearchTerm { get; set; }
    }
}
