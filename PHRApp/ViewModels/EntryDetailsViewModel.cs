using PHRApp.Models.DTOs;
using PHRApp.Services.Interfaces;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace PHRApp.ViewModels
{
    public class EntryDetailsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly IEntryService _entryService;
        private readonly IFileStorageService _fileStorageService;

        private EntryDetailsDto? _entry;
        private string _errorMessage = string.Empty;
        private bool _isBusy;

        public EntryDetailsDto? Entry
        {
            get => _entry;
            private set { if (_entry != value) { _entry = value; OnPropertyChanged(); } }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            private set { if (_errorMessage != value) { _errorMessage = value; OnPropertyChanged(); } }
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        public EntryDetailsViewModel(IEntryService entryService, IFileStorageService fileStorageService)
        {
            _entryService = entryService;
            _fileStorageService = fileStorageService;
        }

        public async Task LoadAsync(int entryId)
        {
            ErrorMessage = string.Empty;
            IsBusy = true;
            try
            {
                Entry = await _entryService.GetEntryByIdAsync(entryId);
                if (Entry == null)
                    ErrorMessage = "Nie znaleziono wpisu.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Błąd podczas ładowania wpisu: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        public string? ResolveAttachmentPath(AttachmentDto attachment)
        {
            var fullPath = _fileStorageService.ResolveFullPath(attachment.FilePath);
            return File.Exists(fullPath) ? fullPath : null;
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}