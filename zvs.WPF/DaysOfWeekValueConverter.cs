using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using zvs.DataModel.Tasks;

namespace zvs.WPF
{
    public class DaysOfWeekValueConverter : IValueConverter
    {
        private DaysOfWeek _target;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mask = (DaysOfWeek)parameter;
            _target = (DaysOfWeek)value;
            return ((mask & _target) != 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            _target ^= (DaysOfWeek)parameter;
            return _target;
        }
    }
}