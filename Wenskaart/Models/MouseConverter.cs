using GalaSoft.MvvmLight.Command;

namespace Wenskaart.Models
{
    public class MyModel
    {
        public object Sender { get; set; }
        public object E { get; set; }
    }
    class MouseConverter : IEventArgsConverter
    {
        public object Convert(object value, object parameter)
        {
            return new MyModel
            {
                Sender = value,
                E = parameter
            };
        }
    }
}
