using PHRApp.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHRApp.Models.DTOs
{
    public class CreateEntryDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public EntryStatus Status { get; set; }
        public List<int> CategoryIds { get; set; } = new();
        public List<string> FilePaths { get; set; } = new();
    }
}
