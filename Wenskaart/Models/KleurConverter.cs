using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Wenskaart.Models
{
    class KleurConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Brush)new BrushConverter().ConvertFromString(((Kleur)value).Naam);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Brush)
            {
                SolidColorBrush deKleur = (SolidColorBrush)value;
                Kleur kleurke = new Kleur
                {
                    Borstel = deKleur,
                    Naam = deKleur.Color.ToString(),
                    Hex = deKleur.ToString(),
                    Rood = deKleur.Color.R,
                    Groen = deKleur.Color.G,
                    Blauw = deKleur.Color.B
                };
                return kleurke;
            }
            return new Kleur();
        }
    }
}
