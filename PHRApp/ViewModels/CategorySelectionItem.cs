using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PHRApp.ViewModels
{
    public class CategorySelectionItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isSelected;

        public int Id { get; init; }
        public string Name { get; init; } = null!;

        public bool IsSelected
        {
            get => _isSelected;
            set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
        }

        public static CategorySelectionItem FromDto(Models.DTOs.CategoryDto dto) =>
            new() { Id = dto.Id, Name = dto.Name };

        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
