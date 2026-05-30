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
    }
}
