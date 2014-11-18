using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace zvs.WPF
{
    public partial class ChromeStyledWindow : ResourceDictionary
    {
        public ChromeStyledWindow()
        {
            InitializeComponent();
        }
    }

    public class ResizeableToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mode = (ResizeMode)value;

            if (mode == ResizeMode.NoResize)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return WindowState.Normal;
        }
    }
}