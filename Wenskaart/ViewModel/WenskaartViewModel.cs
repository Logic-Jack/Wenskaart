using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wenskaart.Models;

namespace Wenskaart.ViewModels
{
    class WenskaartViewModel : ViewModelBase
    {
        #region Properties
        public BitmapImage CanvasImage { get => canvasImage; private set { canvasImage = value; RaisePropertyChanged(nameof(CanvasImage)); } }
        private ObservableCollection<Bal> ballen = new ObservableCollection<Bal>();
        public ObservableCollection<Bal> Ballen { get => ballen; set { ballen = value; RaisePropertyChanged(nameof(Ballen)); } }
        public List<string> ListOfPics { get; set; }
        private Visibility _isvisible;
        private string _statusText = "";
        private Guid id;
        private bool isEnable;
        public bool IsEnable { get => isEnable; set { isEnable = value; RaisePropertyChanged(nameof(IsEnable)); } }
        private FontFamily _selectedLetterType;

        public FontFamily SelectedLetterType
        {
            get { return _selectedLetterType; }
            set { _selectedLetterType = value; RaisePropertyChanged(nameof(SelectedLetterType)); }
        }

        private string _wens;

        public string Wens
        {
            get { return _wens; }
            set { _wens = value; RaisePropertyChanged(nameof(Wens)); }
        }

        public string StatusText
        {
            get { return _statusText; }
            private set { _statusText = value; RaisePropertyChanged(nameof(StatusText)); }
        }

        public Visibility Isvisible
        {
            get { return _isvisible; }
            private set { _isvisible = value; RaisePropertyChanged(nameof(Isvisible)); }
        }

        private List<Kleur> _kleuren;
        public List<Kleur> Kleuren
        {
            get { return _kleuren; }
            private set { _kleuren = value; }
        }
        private Kleur _selectedColor;
        public Kleur SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                _selectedColor = value;
                RaisePropertyChanged(nameof(SelectedColor));
            }
        }

        private int fontSize;
        private BitmapImage canvasImage;

        public int FontSize
        {
            get => fontSize;
            private set { fontSize = value; RaisePropertyChanged(nameof(FontSize)); }
        }
        #endregion
        #region ICommand
        public RelayCommand<string> RButton
        {
            get { return new RelayCommand<string>(RepeatButton); }
        }
        public RelayCommand<string> NewTemplate
        {
            get { return new RelayCommand<string>(NewTemplat); }
        }
        public RelayCommand Openen
        {
            get { return new RelayCommand(OpenenBestand); }
        }
        public RelayCommand Opslaan
        {
            get { return new RelayCommand(OpslaanBestand); }
        }
        public RelayCommand Afsluiten
        {
            get { return new RelayCommand(AfsluitenApp); }
        }
        public RelayCommand<CancelEventArgs> ClosingCommand
        {
            get { return new RelayCommand<CancelEventArgs>(OnWindowClosing); }
        }

        public RelayCommand<MouseEventArgs> Mouse_Move
        {
            get { return new RelayCommand<MouseEventArgs>(MouseMove); }
        }
        public RelayCommand<MouseEventArgs> MouseEnter
        {
            get { return new RelayCommand<MouseEventArgs>(Mouse_Enter); }
        }
        public RelayCommand<MouseEventArgs> MouseLeave
        {
            get { return new RelayCommand<MouseEventArgs>(Mouse_Leave); }
        }


        public RelayCommand<DragEventArgs> DropCommand
        {
            get { return new RelayCommand<DragEventArgs>(Drop); }
        }
        #endregion


        public WenskaartViewModel()
        {
            SelectedLetterType = new FontFamily("Arial");
            Kleuren = new List<Kleur>();
            CanvasImage = new BitmapImage(new Uri("pack://application:,,,/Images/empty.png", UriKind.Absolute));
            ListOfPics = new List<string> { "kerstkaart", "geboortekaart" };
            foreach (PropertyInfo info in typeof(Colors).GetProperties())
            {
                BrushConverter bc = new BrushConverter();
                SolidColorBrush deKleur =
                (SolidColorBrush)bc.ConvertFromString(info.Name);
                Kleur kleurke = new Kleur
                {
                    Borstel = deKleur,
                    Naam = info.Name,
                    Hex = deKleur.ToString(),
                    Rood = deKleur.Color.R,
                    Groen = deKleur.Color.G,
                    Blauw = deKleur.Color.B
                };
                Kleuren.Add(kleurke);
                if (kleurke.Naam == "White")
                    SelectedColor = kleurke;
            }
            FontSize = 10;
            Isvisible = Visibility.Hidden;
            IsEnable = false;
        }
        private void RepeatButton(string parameter)
        {

            if (parameter == "+" && FontSize < 40)
                FontSize++;
            else if (parameter == "-" && FontSize > 10)
                FontSize--;
        }
        private void NewTemplat(string parameter)
        {
            Isvisible = Visibility.Visible;
            IsEnable = true;
            FontSize = 10;
            if (!string.IsNullOrEmpty(parameter))
                CanvasImage = new BitmapImage(new Uri($"pack://application:,,,/Images/{parameter}.jpg", UriKind.Absolute));
            else
                CanvasImage = new BitmapImage(new Uri("pack://application:,,,/Images/empty.png", UriKind.Absolute));
            StatusText = "Nieuw";
            Wens = "";
            ballen.Clear();
        }

        public void OnWindowClosing(CancelEventArgs e)
        {
            if (MessageBox.Show("Afsluiten", "Wilt u het programma sluiten ?",
            MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) ==
            MessageBoxResult.No)
                e.Cancel = true;
        }

        #region Bestanden
        private void OpslaanBestand()
        {
            var Sballen = JsonConvert.SerializeObject(Ballen);
            try
            {
                SaveFileDialog dlg = new SaveFileDialog
                {
                    FileName = "wenskaart",
                    DefaultExt = ".krt",
                    Filter = "Textbox documents |*.krt"
                };
                if (dlg.ShowDialog() == true)
                {
                    using (StreamWriter bestand = new StreamWriter(dlg.FileName))
                    {
                        bestand.WriteLine(CanvasImage);
                        bestand.WriteLine(Sballen);
                        bestand.WriteLine(Wens);
                        bestand.WriteLine(FontSize);
                        bestand.WriteLine(SelectedLetterType.Source);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("opslaan mislukt : " + ex.Message);
            }
        }
        private void OpenenBestand()
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog
                {
                    FileName = "",
                    DefaultExt = ".krt",
                    Filter = "Textbox documents |*.krt"
                };
                if (dlg.ShowDialog() == true)
                {
                    using (StreamReader bestand = new StreamReader(dlg.FileName))
                    {
                        CanvasImage = new BitmapImage(new Uri(bestand.ReadLine(), UriKind.Absolute));
                        Ballen = JsonConvert.DeserializeObject<ObservableCollection<Bal>>(bestand.ReadLine());
                        Wens = bestand.ReadLine();
                        FontSize = Int32.Parse(bestand.ReadLine());
                        SelectedLetterType = new FontFamily(bestand.ReadLine());
                    }
                    StatusText = dlg.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("openen mislukt : " + ex.Message);
            }
        }
        private void AfsluitenApp()
        {
            Application.Current.MainWindow.Close();
        }
        #endregion

        #region DragAndDrop



        private void MouseMove(MouseEventArgs e)
        {
            if (e.Source is Ellipse ellipse)
            {

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (!string.IsNullOrEmpty(ellipse.Tag?.ToString()))
                        id = (Guid)ellipse.Tag;

                    DragDrop.DoDragDrop(ellipse, ellipse.Fill.ToString(), DragDropEffects.Move);
                }
            }
        }
        private void Mouse_Enter(MouseEventArgs e)
        {
            if (e.Source is Ellipse ellipse)
                ellipse.StrokeThickness = 8;
        }
        private void Mouse_Leave(MouseEventArgs e)
        {
            if (e.Source is Ellipse ellipse)
                ellipse.StrokeThickness = 5;
        }


        private void Drop(DragEventArgs e)
        {
            if (e.OriginalSource is Canvas canvas)
            {
                if (e.Data.GetDataPresent(DataFormats.StringFormat))
                {
                    string dataString = (string)e.Data.GetData(DataFormats.StringFormat);
                    BrushConverter converter = new BrushConverter();
                    if (converter.IsValid(dataString))
                    {
                        if (Ballen.Where(x => x.Id == id).ToList().Count != 0)
                            Ballen.Remove(Ballen.Where(x => x.Id == id).ToList().First());
                        Ballen.Add(new Bal
                        {
                            Kleur = (Brush)converter.ConvertFromString(dataString),
                            XPositie = e.GetPosition(canvas).X - 20,
                            YPositie = e.GetPosition(canvas).Y - 20,
                            Id = Guid.NewGuid()
                        });
                    }
                }
            }
            if (e.OriginalSource is Image image)
            {
                if (Ballen.Where(x => x.Id == id).ToList().Count != 0)
                    Ballen.Remove(Ballen.Where(x => x.Id == id).ToList().First());

            }
        }
        #endregion
    }
}
