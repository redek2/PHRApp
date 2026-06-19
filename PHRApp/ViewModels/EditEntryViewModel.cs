using PHRApp.Models.DTOs;
using PHRApp.Models.Enums;
using PHRApp.Services.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PHRApp.ViewModels
{
    public class EditEntryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly IEntryService _entryService;
        private readonly ICategoryService _categoryService;

        private int _entryId;
        private string _title = string.Empty;
        private string? _description;
        private DateTime _eventDate = DateTime.Now;
        private EntryStatus _status = EntryStatus.Planned;
        private string _errorMessage = string.Empty;
        private bool _isBusy;
        private readonly List<int> _removedExistingIds = new();
        private TimeSpan _eventTime = new TimeSpan(8, 0, 0);

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

        public IEnumerable<EntryStatus> AvailableStatuses => Enum.GetValues<EntryStatus>();
        public ObservableCollection<CategorySelectionItem> AvailableCategories { get; } = new();
        public ObservableCollection<AttachmentItem> Attachments { get; } = new();

        public EditEntryViewModel(IEntryService entryService, ICategoryService categoryService)
        {
            _entryService = entryService;
            _categoryService = categoryService;
        }

        public async Task LoadAsync(int entryId)
        {
            _entryId = entryId;
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                AvailableCategories.Clear();
                foreach (var c in categories)
                    AvailableCategories.Add(CategorySelectionItem.FromDto(c));

                var entry = await _entryService.GetEntryByIdAsync(entryId);
                if (entry == null)
                {
                    ErrorMessage = "Nie znaleziono wpisu.";
                    return;
                }

                Title = entry.Title;
                Description = entry.Description;
                EventDate = entry.EventDate.Date;
                EventTime = entry.EventDate.TimeOfDay;
                Status = entry.Status;

                foreach (var cat in AvailableCategories)
                    cat.IsSelected = entry.CategoryNames.Contains(cat.Name);

                _removedExistingIds.Clear();
                Attachments.Clear();
                foreach (var a in entry.Attachments)
                    Attachments.Add(new AttachmentItem
                    {
                        ExistingId = a.Id,
                        DisplayName = a.FileName
                    });
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Błąd podczas ładowania: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void AddAttachment(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            if (!Attachments.Any(a => a.NewFilePath == filePath))
                Attachments.Add(new AttachmentItem
                {
                    NewFilePath = filePath,
                    DisplayName = fileName
                });
        }

        public void RemoveAttachment(AttachmentItem item)
        {
            if (!item.IsNew && item.ExistingId.HasValue)
                _removedExistingIds.Add(item.ExistingId.Value);
            Attachments.Remove(item);
        }

        public async Task<bool> SaveAsync()
        {
            ErrorMessage = string.Empty;

            if (!Validate()) return false;

            IsBusy = true;
            try
            {
                var dto = new UpdateEntryDto
                {
                    Id = _entryId,
                    Title = Title.Trim(),
                    Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
                    EventDate = EventDate.Date + EventTime,
                    Status = Status,
                    CategoryIds = AvailableCategories
                        .Where(c => c.IsSelected)
                        .Select(c => c.Id)
                        .ToList(),
                    AttachmentIdsToRemove = _removedExistingIds.ToList(),
                    NewFilePaths = Attachments
                        .Where(a => a.IsNew && a.NewFilePath != null)
                        .Select(a => a.NewFilePath!)
                        .ToList()
                };

                await _entryService.UpdateEntryAsync(dto);
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

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}