using PHRApp.Models.DTOs;
using PHRApp.ViewModels;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PHRApp.Views
{
    public partial class EntryDetailsWindow : Window
    {
        private readonly EntryDetailsViewModel _viewModel;

        public EntryDetailsWindow(EntryDetailsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        public async Task LoadAsync(int entryId)
        {
            await _viewModel.LoadAsync(entryId);
        }

        private void OnCloseClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnOpenFileClicked(object sender, RoutedEventArgs e)
        {
            if (AttachmentsListBox.SelectedItem is not AttachmentDto attachment)
                return;

            OpenAttachment(attachment);
        }

        private void OpenAttachment(AttachmentDto attachment)
        {
            var fullPath = _viewModel.ResolveAttachmentPath(attachment);
            if (fullPath == null)
            {
                MessageBox.Show("Plik nie został znaleziony na dysku.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
        }

        private void OnAttachmentDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem { DataContext: AttachmentDto attachment })
                OpenAttachment(attachment);
        }
    }
}