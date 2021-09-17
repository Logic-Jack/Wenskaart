using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Wenskaart.Models
{
    public class FontConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FontFamily fontFamily)
            {
                return fontFamily.Source;
            }
            return "Arial Narrow";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return new FontFamily("Arial Narrow");
            return new FontFamily(value.ToString());
            
        }
    }
}
