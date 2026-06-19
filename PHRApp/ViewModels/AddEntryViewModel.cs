using PHRApp.Infrastructure;
using PHRApp.Models.DTOs;
using PHRApp.Models.Enums;
using PHRApp.Services.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PHRApp.ViewModels
{
    public class AddEntryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand AddCategoryCommand { get; }

        private readonly IEntryService _entryService;
        private readonly ICategoryService _categoryService;

        private string _title = string.Empty;
        private string? _description;
        private DateTime _eventDate = DateTime.Now;
        private EntryStatus _status = EntryStatus.Planned;
        private string _errorMessage = string.Empty;
        private bool _isBusy;
        private string _newCategoryName = string.Empty;
        private TimeSpan _eventTime = new TimeSpan(8, 0, 0);

        public AddEntryViewModel(IEntryService entryService, ICategoryService categoryService)
        {
            _entryService = entryService;
            _categoryService = categoryService;
            AddCategoryCommand = new RelayCommand(
                execute: async _ => await AddCategoryAsync(),
                canExecute: _ => !string.IsNullOrWhiteSpace(NewCategoryName) && !IsBusy
            );
        }

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
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string NewCategoryName
        {
            get => _newCategoryName;
            set { if (_newCategoryName != value) { _newCategoryName = value; OnPropertyChanged(); } }
        }

        public ObservableCollection<CategorySelectionItem> AvailableCategories { get; } = new();
        public ObservableCollection<string> SelectedFilePaths { get; } = new();
        public IEnumerable<EntryStatus> AvailableStatuses => Enum.GetValues<EntryStatus>();

        public async Task LoadAsync()
        {
            await LoadCategoriesAsync();
        }

        public async Task<bool> SaveEntryAsync()
        {
            ErrorMessage = string.Empty;

            if (!Validate()) return false;

            IsBusy = true;
            try
            {
                var dto = BuildCreateEntryDto();
                await _entryService.CreateEntryAsync(dto);
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

        private async Task LoadCategoriesAsync()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            AvailableCategories.Clear();
            foreach (var category in categories)
                AvailableCategories.Add(CategorySelectionItem.FromDto(category));
        }

        private CreateEntryDto BuildCreateEntryDto()
        {
            return new CreateEntryDto
            {
                Title = Title.Trim(),
                Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
                EventDate = EventDate.Date + EventTime,
                Status = Status,
                CategoryIds = AvailableCategories
                .Where(c => c.IsSelected)
                .Select(c => c.Id)
                .ToList(),
                FilePaths = SelectedFilePaths.ToList()
            };
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private async Task AddCategoryAsync()
        {
            ErrorMessage = string.Empty;

            var name = NewCategoryName.Trim();

            IsBusy = true;
            try
            {
                var dto = new CreateCategoryDto
                {
                    Name = name
                };

                var newId = await _categoryService.CreateCategoryAsync(dto);

                var newItem = new CategorySelectionItem
                {
                    Id = newId,
                    Name = name,
                    IsSelected = true
                };

                var insertAt = AvailableCategories
                    .ToList()
                    .FindIndex(c => string.Compare(c.Name, newItem.Name, StringComparison.OrdinalIgnoreCase) > 0);

                if (insertAt < 0)
                    AvailableCategories.Add(newItem);
                else
                    AvailableCategories.Insert(insertAt, newItem);

                NewCategoryName = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Nie można dodać kategorii: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        public TimeSpan EventTime
        {
            get => _eventTime;
            set
            {
                if (_eventTime != value)
                {
                    _eventTime = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Tytuł wpisu jest wymagany.";
                return false;
            }

            if (Status == EntryStatus.Planned && EventDate.Date + EventTime <= DateTime.Now)
            {
                ErrorMessage = "Data i godzina zdarzenia dla wpisu 'Planned' musi być w przyszłości.";
                return false;
            }

            if (Status == EntryStatus.Completed && EventDate.Date + EventTime > DateTime.Now)
            {
                ErrorMessage = "Data i godzina zdarzenia dla wpisu 'Completed' nie może być w przyszłości.";
                return false;
            }

            return true;
        }
    }
}
