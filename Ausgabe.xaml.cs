using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics.Metrics;
using Tarifrechner.Klassen;

namespace Tarifrechner
{
    public class Tafel
    {
        public double QX { get; set; }
        public double QY { get; set; }
        public double ZINS { get; set; }
    }
    public class GesamtOutput
    {
        public int m {  get; set; }
        public int alter { get; set; }
        public double qx { get; set; }
        public double zins { get; set; }
        public double aufzinsfaktor { get; set; }
        public double diskontierungsfaktor { get; set; }
        public double lbw_tod { get; set; }
    }
    /// <summary>
    /// Interaction logic for Ausgabe.xaml
    /// </summary>
    public partial class Ausgabe : Page
    {
        public List<Tafel> TafelListe { get; set; }
        public List<GesamtOutput> GesamtOutput { get; set; }
        public Ausgabe()
        {
            InitializeComponent();
            TafelListe = LeseSterbetafel("DAV2008.csv");
            GesamtOutput = new();
            DataContext = this;  // Bindet die Daten an das XAML
        }


        private List<Tafel> LeseSterbetafel(string dateiName)
        {
            // Kombiniere den BaseDirectory-Pfad mit dem relativen Pfad
            string dateiPfad = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", dateiName);
            string dateiZins = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Zinsstruktur.csv");

            var datenpunkte = new List<Tafel>(); 
            var zeilenSterbetafel = File.ReadAllLines(dateiPfad);
            var zeilenZinsStruktur = File.ReadAllLines(dateiZins);

            

            for (int index =1 ; index < zeilenSterbetafel.Length; index++)
            {
                var zeile = zeilenSterbetafel[index];
                var spalten = zeile.Split(';');
                datenpunkte.Add(new Tafel
                {
                    QX = double.Parse(spalten[0], System.Globalization.CultureInfo.InvariantCulture),
                    QY = double.Parse(spalten[1], System.Globalization.CultureInfo.InvariantCulture),
                    ZINS = double.Parse(zeilenZinsStruktur[index], System.Globalization.CultureInfo.InvariantCulture),

                });
            }

            return datenpunkte;
        }



        public void Berechne(Vertrag vertrag)
        {
            GesamtOutput = new();
            BerechneQx(vertrag);
            GesamtAusgabeBox.ItemsSource = null;
            GesamtAusgabeBox.ItemsSource = GesamtOutput;
            Beitrag.Text = BerechneBeitrag(vertrag).ToString();
        }
        public double BerechneBeitrag(Vertrag vertrag)
        {
            double beitrag = GesamtOutput[0].lbw_tod * vertrag.Vertragsteil.leistung1;
            return beitrag;
        }
        public void BerechneQx(Vertrag vertrag)
        {
            Vertragsteil vt = vertrag.Vertragsteil;
            Rechnungsgrundlage rg = vertrag.Rechnungsgrundlage;
            int index = 0;
            for(int i = vt.ea; i< TafelListe.Count;i++) 
            {
                Tafel zeile = TafelListe[i];
                GesamtOutput wert=new();
                wert.qx = zeile.QX * rg.mannAnteil + (1 - rg.mannAnteil) * zeile.QY;
                wert.m = index;
                wert.alter = index + vt.ea;
                wert.zins = rg.isKonstantZins? rg.zins : TafelListe[i- vt.ea].ZINS;
                wert.aufzinsfaktor = wert.zins + 1;
                wert.diskontierungsfaktor = 1 / wert.aufzinsfaktor;
                index++;
                GesamtOutput.Add(wert);
            }

            GesamtOutput[vt.n].lbw_tod= 0;
            for (int i = vt.n - 1; i > -1; i--)
            {
                GesamtOutput[i].lbw_tod = (GesamtOutput[i + 1].lbw_tod * (1 - GesamtOutput[i].qx) + GesamtOutput[i].qx) * GesamtOutput[i].diskontierungsfaktor;
            }
        }

    }
}
