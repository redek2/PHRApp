using Microsoft.Win32;
using PHRApp.ViewModels;
using System.Windows;

namespace PHRApp.Views
{
    public partial class AddEntryWindow : Window
    {
        private readonly AddEntryViewModel _viewModel;
        public AddEntryWindow(AddEntryViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = _viewModel;

            Loaded += async (_, __) => await _viewModel.LoadAsync();
        }

        private async void OnSaveClicked(object sender, RoutedEventArgs e)
        {
            var success = await _viewModel.SaveEntryAsync();

            if (success)
            {
                DialogResult = true;
                Close();
            }
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnAddFileClicked(object sender, RoutedEventArgs e)
        {
            if (_viewModel.IsBusy) return;

            var dialog = new OpenFileDialog
            {
                Filter = "Obsługiwane pliki (*.pdf;*.jpg;*.jpeg;*.png)|*.pdf;*.jpg;*.jpeg;*.png",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (var path in dialog.FileNames)
                {
                    _viewModel.AddFilePath(path);
                }
            }
        }

        private void OnRemoveFileClicked(object sender, RoutedEventArgs e)
        {
            if (_viewModel.IsBusy) return;

            if (AttachmentsListBox.SelectedItem is string selectedPath)
            {
                _viewModel.RemoveFilePath(selectedPath);
            }
        }
    }
}
