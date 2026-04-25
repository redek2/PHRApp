using PHRApp.Models.DTOs;
using PHRApp.Models.Enums;
using PHRApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PHRApp.ViewModels
{
    public class EntryListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

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

        public EntryListViewModel(IEntryService entryService, ICategoryService categoryService)
        {
            _entryService = entryService;
            _categoryService = categoryService;
        }

        public async Task LoadAsync()
        {

            var query = new EntryQueryDto
            {
                SearchTerm = SearchTerm,
                Status = SelectedStatus,
                CategoryId = SelectedCategoryId == 0 ? null : SelectedCategoryId
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
    }
}