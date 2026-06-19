using PHRApp.Models.DTOs;
using PHRApp.Models.Enums;
using PHRApp.Services.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PHRApp.ViewModels
{
    public class EntryListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public EntryListViewModel(IEntryService entryService, ICategoryService categoryService)
        {
            _entryService = entryService;
            _categoryService = categoryService;
            SelectedCategoryId = 0;
            SelectedStatus = null;
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private readonly IEntryService _entryService;
        private readonly ICategoryService _categoryService;

        public ObservableCollection<EntryListItemDto> Entries { get; } = new();
        public ObservableCollection<CategoryDto> Categories { get; } = new();

        private string? _searchTerm;
        public string? SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (_searchTerm != value)
                {
                    _searchTerm = value;
                    OnPropertyChanged();
                }
            }
        }

        private EntryStatus? _selectedStatus;
        public EntryStatus? SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (_selectedStatus != value)
                {
                    _selectedStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        private int? _selectedCategoryId;
        public int? SelectedCategoryId
        {
            get => _selectedCategoryId;
            set
            {
                if (_selectedCategoryId != value)
                {
                    _selectedCategoryId = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _fromDate;
        public DateTime? FromDate
        {
            get => _fromDate;
            set
            {
                if (_fromDate != value)
                {
                    _fromDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _toDate;
        public DateTime? ToDate
        {
            get => _toDate;
            set
            {
                if (_toDate != value)
                {
                    _toDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task LoadAsync()
        {
            await _entryService.UpdateExpiredEntriesAsync();

            var query = new EntryQueryDto
            {
                SearchTerm = SearchTerm,
                Status = SelectedStatus,
                CategoryId = SelectedCategoryId == 0 ? null : SelectedCategoryId,
                FromDate = FromDate,
                ToDate = ToDate.HasValue ? ToDate.Value.Date.AddDays(1).AddTicks(-1) : null
            };

            var results = await _entryService.GetEntriesAsync(query);

            Entries.Clear();

            foreach (var entry in results)
            {
                Entries.Add(entry);
            }
        }

        public async Task LoadCategoriesAsync()
        {
            Categories.Clear();

            Categories.Add(new CategoryDto
            {
                Id = 0,
                Name = "Wszystkie"
            });

            var categories = await _categoryService.GetAllCategoriesAsync();

            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }

        public async Task InitializeAsync()
        {
            await LoadCategoriesAsync();
            await LoadAsync();
        }

        public EntryListItemDto? SelectedEntry { get; set; }
        public IEnumerable<object> AvailableStatuses { get; } = new List<object>
        {
            new { Label = "Wszystkie", Value = (EntryStatus?)null },
            new { Label = "Planned", Value = (EntryStatus?)EntryStatus.Planned },
            new { Label = "Completed", Value = (EntryStatus?)EntryStatus.Completed },
            new { Label = "Cancelled", Value = (EntryStatus?)EntryStatus.Cancelled }
        };
    }
}