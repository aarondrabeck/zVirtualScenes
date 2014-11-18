using System;
using System.Windows.Data;

namespace zvs.WPF.Converters
{
    public class NullableIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            var strVal = value.ToString();

            if (string.IsNullOrEmpty(strVal))
                return null;
            else
            {            
                var v = 0;
                if (int.TryParse(strVal, out v))
                    return v;
                else
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }
    }
}
