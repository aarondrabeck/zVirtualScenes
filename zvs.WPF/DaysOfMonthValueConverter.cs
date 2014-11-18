using System;
using System.Globalization;
using System.Windows.Data;
using zvs.DataModel.Tasks;

namespace zvs.WPF
{
    public class DaysOfMonthValueConverter : IValueConverter
    {
        private DaysOfMonth _target;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mask = (DaysOfMonth)parameter;
            _target = (DaysOfMonth)value;
            return ((mask & _target) != 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            _target ^= (DaysOfMonth)parameter;
            return _target;
        }
    }
}