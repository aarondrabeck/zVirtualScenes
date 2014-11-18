using System;
using System.Globalization;
using System.Windows.Data;

namespace zvs.WPF
{
    public class ActionableGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            return ((value is DataModel.Group && ((DataModel.Group) value).Devices.Count == 0) ||
                    value != null && value.ToString() == "{DataGrid.NewItemPlaceholder}");
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
