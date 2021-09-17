using System;
using System.Globalization;
using System.Windows.Data;

namespace Wenskaart.Models
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return "hidden";
            else
                return "visible";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == "hidden")
                return false;
            else
                return true;
        }
    }
}
