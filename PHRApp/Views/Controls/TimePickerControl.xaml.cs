using System.Windows;
using System.Windows.Controls;

namespace PHRApp.Views.Controls
{
    public partial class TimePickerControl : UserControl
    {
        public static readonly DependencyProperty SelectedTimeProperty =
            DependencyProperty.Register(
                nameof(SelectedTime),
                typeof(TimeSpan),
                typeof(TimePickerControl),
                new FrameworkPropertyMetadata(
                    new TimeSpan(8, 0, 0),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedTimeChanged));

        public TimeSpan SelectedTime
        {
            get => (TimeSpan)GetValue(SelectedTimeProperty);
            set => SetValue(SelectedTimeProperty, value);
        }

        private bool _isUpdating;

        public TimePickerControl()
        {
            InitializeComponent();

            for (int h = 0; h < 24; h++)
                HoursComboBox.Items.Add(h.ToString("D2"));

            foreach (var m in new[] { 0, 15, 30, 45 })
                MinutesComboBox.Items.Add(m.ToString("D2"));

            HoursComboBox.SelectedIndex = 8;
            MinutesComboBox.SelectedIndex = 0;
        }

        private static void OnSelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TimePickerControl)d;
            if (control._isUpdating) return;

            var time = (TimeSpan)e.NewValue;
            control._isUpdating = true;

            control.HoursComboBox.SelectedItem = time.Hours.ToString("D2");

            var roundedMinutes = (time.Minutes / 15) * 15;
            control.MinutesComboBox.SelectedItem = roundedMinutes.ToString("D2");

            control._isUpdating = false;
        }

        private void OnTimeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdating) return;
            if (HoursComboBox.SelectedItem == null || MinutesComboBox.SelectedItem == null) return;

            var hours = int.Parse((string)HoursComboBox.SelectedItem);
            var minutes = int.Parse((string)MinutesComboBox.SelectedItem);

            _isUpdating = true;
            SelectedTime = new TimeSpan(hours, minutes, 0);
            _isUpdating = false;
        }
    }
}