using PHRApp.Models.DTOs;
using PHRApp.Models.Enums;
using PHRApp.Services.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PHRApp.ViewModels
{
    public class AddEntryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly IEntryService _entryService;
        private readonly ICategoryService _categoryService;
        private readonly IDataRefreshService _refreshService;

        private string _title = string.Empty;
        private string? _description;
        private DateTime _eventDate = DateTime.Now;
        private EntryStatus _status = EntryStatus.Planned;
        private string _errorMessage = string.Empty;
        private bool _isBusy;

        public string Title
        {
            get => _title;
            set { if (_title != value) { _title = value; OnPropertyChanged(); } }
        }

        public string? Description
        {
            get => _description;
            set { if (_description != value) { _description = value; OnPropertyChanged(); } }
        }

        public DateTime EventDate
        {
            get => _eventDate;
            set { if (_eventDate != value) { _eventDate = value; OnPropertyChanged(); } }
        }

        public EntryStatus Status
        {
            get => _status;
            set { if (_status != value) { _status = value; OnPropertyChanged(); } }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { if (_errorMessage != value) { _errorMessage = value; OnPropertyChanged(); } }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        public ObservableCollection<CategoryDto> AvailableCategories { get; } = new();
        public ObservableCollection<CategoryDto> SelectedCategories { get; } = new();
        public ObservableCollection<string> SelectedFilePaths { get; } = new();
        public IEnumerable<EntryStatus> AvailableStatuses => Enum.GetValues<EntryStatus>();

        public AddEntryViewModel(IEntryService entryService, ICategoryService categoryService, IDataRefreshService refreshService)
        {
            _entryService = entryService;
            _categoryService = categoryService;
            _refreshService = refreshService;

        }
        public async Task LoadAsync()
        {
            await LoadCategoriesAsync();
        }

        public async Task<bool> SaveEntryAsync()
        {
            ErrorMessage = string.Empty;

            IsBusy = true;
            try
            {
                var dto = BuildCreateEntryDto();
                await _entryService.CreateEntryAsync(dto);
                _refreshService.NotifyEntriesChanged();
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void AddFilePath(string fullPath)
        {
            if (!string.IsNullOrWhiteSpace(fullPath) && !SelectedFilePaths.Contains(fullPath))
                SelectedFilePaths.Add(fullPath);
        }

        public void RemoveFilePath(string fullPath)
        {
            SelectedFilePaths.Remove(fullPath);
        }

        public void ToggleCategorySelection(CategoryDto category)
        {
            if (SelectedCategories.Any(c => c.Id == category.Id))
                SelectedCategories.Remove(SelectedCategories.First(c => c.Id == category.Id));
            else
                SelectedCategories.Add(category);
        }
        private async Task LoadCategoriesAsync()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            AvailableCategories.Clear();
            foreach (var category in categories)
                AvailableCategories.Add(category);
        }

        private CreateEntryDto BuildCreateEntryDto()
        {
            return new CreateEntryDto
            {
                Title = Title.Trim(),
                Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
                EventDate = EventDate,
                Status = Status,
                CategoryIds = SelectedCategories.Select(c => c.Id).ToList(),
                FilePaths = SelectedFilePaths.ToList()
            };
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
