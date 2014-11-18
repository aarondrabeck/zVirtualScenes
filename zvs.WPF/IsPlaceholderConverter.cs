using System;
using System.Globalization;
using System.Windows.Data;

namespace zvs.WPF
{
    public class IsPlaceholderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            var val = value;
            if (val == null)
                return false;

            var itemName = val.ToString();
            return itemName.Equals("{DataGrid.NewItemPlaceholder}") || itemName.Equals("{NewItemPlaceholder}");
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
