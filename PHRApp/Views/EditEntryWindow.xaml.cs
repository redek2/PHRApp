using Microsoft.Win32;
using PHRApp.Models.DTOs;
using PHRApp.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace PHRApp.Views
{
    public partial class EditEntryWindow : Window
    {
        private readonly EditEntryViewModel _viewModel;

        public EditEntryWindow(EditEntryViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        public async Task LoadAsync(int entryId)
        {
            await _viewModel.LoadAsync(entryId);
        }

        private async void OnSaveClicked(object sender, RoutedEventArgs e)
        {
            if (_viewModel.IsBusy) return;

            var success = await _viewModel.SaveAsync();
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
                    _viewModel.AddAttachment(path);
            }
        }

        private void OnRemoveFileClicked(object sender, RoutedEventArgs e)
        {
            if (_viewModel.IsBusy) return;

            if (AttachmentsListBox.SelectedItem is AttachmentItem item)
                _viewModel.RemoveAttachment(item);
        }
    }
}