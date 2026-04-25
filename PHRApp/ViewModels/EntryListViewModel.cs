using PHRApp.Models.DTOs;
using PHRApp.Models.Enums;
using PHRApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHRApp.ViewModels
{
    public class EntryListViewModel
    {
        private readonly IEntryService _entryService;

        public ObservableCollection<EntryListItemDto> Entries { get; } = new();

        public string? SearchTerm { get; set; }
        public EntryStatus? SelectedStatus { get; set; }
        public int? SelectedCategoryId { get; set; }

        public EntryListViewModel(IEntryService entryService)
        {
            _entryService = entryService;
        }

        public async Task LoadAsync()
        {
            var query = new EntryQueryDto
            {
                SearchTerm = SearchTerm,
                Status = SelectedStatus,
                CategoryId = SelectedCategoryId
            };

            var results = await _entryService.GetEntriesAsync(query);

            Entries.Clear();

            foreach (var entry in results)
            {
                Entries.Add(entry);
            }
        }
    }
}
