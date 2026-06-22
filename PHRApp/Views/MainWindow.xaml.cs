using Microsoft.Extensions.DependencyInjection;
using PHRApp.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace PHRApp.Views
{
    public partial class MainWindow : Window
    {

        private readonly EntryListViewModel _viewModel;
        private readonly IServiceProvider _serviceProvider;
        public MainWindow(EntryListViewModel viewModel, IServiceProvider serviceProvider)
        {
            InitializeComponent();

            _viewModel = viewModel;
            _serviceProvider = serviceProvider;

            DataContext = _viewModel;

            Loaded += async (_, __) => await _viewModel.InitializeAsync();
        }

        public async void OnSearchClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                await _viewModel.LoadAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void OnAddEntryClicked(object sender, RoutedEventArgs e)
        {
            var window = _serviceProvider.GetRequiredService<AddEntryWindow>();

            window.Owner = this;

            var result = window.ShowDialog();

            if (result == true)
            {
                try
                {
                    await _viewModel.LoadAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void OnEntryDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            if (_viewModel.SelectedEntry == null) return;

            var window = _serviceProvider.GetRequiredService<EntryDetailsWindow>();
            window.Owner = this;
            await window.LoadAsync(_viewModel.SelectedEntry.Id);
            window.ShowDialog();

            try
            {
                await _viewModel.LoadAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnClearDatesClicked(object sender, RoutedEventArgs e)
        {
            _viewModel.FromDate = null;
            _viewModel.ToDate = null;
        }
    }
}