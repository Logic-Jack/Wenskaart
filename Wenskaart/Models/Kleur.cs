using System.Windows.Media;

namespace Wenskaart.Models
{
    public class Kleur
    {
        public SolidColorBrush Borstel { get; set; }
        public string Naam { get; set; }
        public string Hex { get; set; }
        public byte Rood { get; set; }
        public byte Groen { get; set; }
        public byte Blauw { get; set; }
    }

}
