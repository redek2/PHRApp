using Microsoft.Extensions.DependencyInjection;
using PHRApp.Models.DTOs;
using PHRApp.Services.Interfaces;
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
        private readonly IServiceProvider _serviceProvider;
        private readonly IEntryService _entryService;

        public EntryDetailsWindow(EntryDetailsViewModel viewModel, IServiceProvider serviceProvider, IEntryService entryService)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _serviceProvider = serviceProvider;
            _entryService = entryService;
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

        private async void OnEditClicked(object sender, RoutedEventArgs e)
        {
            var window = _serviceProvider.GetRequiredService<EditEntryWindow>();
            window.Owner = this;
            await _entryService.UpdateExpiredEntriesAsync();
            await window.LoadAsync(_viewModel.Entry!.Id);
            var result = window.ShowDialog();

            if (result == true)
            {
                await _viewModel.LoadAsync(_viewModel.Entry.Id);
            }
        }

        private async void OnDeleteClicked(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Czy na pewno chcesz usunąć ten wpis? Ta operacja jest nieodwracalna.",
                "Potwierdzenie usunięcia",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                await _entryService.ArchiveEntryAsync(_viewModel.Entry!.Id);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas usuwania wpisu: {ex.Message}", "Błąd",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}