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
using Tarifrechner.Klassen;

namespace Tarifrechner
{

    /// <summary>
    /// Interaction logic for Eingabe.xaml
    /// </summary>
    public partial class Eingabe : Page
    {
        public string AnzeigeText { get; set; }
        public Vertrag Vertrag { get; set; }
        Ausgabe ausgabe;

        public Eingabe()
        {
            InitializeComponent();
            AnzeigeText = "Dynamischer Text";
            DataContext = this;  // Bindet die Daten an das XAML
            Vertrag = new();
            ausgabe = new();
        }
        public Vertrag Berechne()
        {
            Rechnungsgrundlage rg = Vertrag.Rechnungsgrundlage;
            Vertragsteil vt = Vertrag.Vertragsteil;
            TafeldDetails tafeldDetails = rg.TafeldDetails;

            vt.Policenbeginnjahr = int.Parse(PolicenbeginnjahrTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);
            vt.ea = int.Parse(EintrittsalterTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);
            vt.n = int.Parse(VersicherungsdauerTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);
            vt.t = int.Parse(BeitragszahldauerTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);
            vt.leistung1 = double.Parse(ErlebensfallleistungTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);
            vt.leistung2 = double.Parse(TodesfallleistungTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);
            vt.leistung3 = double.Parse(RentenleistungTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);

            rg.mannAnteil = double.Parse(MaenneranteilTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);
            rg.zins = double.Parse(ZinsTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);
            rg.alpha = double.Parse(AlphaKostenTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);
            rg.beta = double.Parse(betaTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);
            rg.gamma = double.Parse(gammaTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);
            rg.delta = double.Parse(deltaTextBox.Text, System.Globalization.CultureInfo.InvariantCulture);

            if (ZinsComboBox.SelectedItem != null)
            {
                rg.isKonstantZins = ZinsComboBox.SelectedItem.ToString().Contains("konstant");
            }
            else
            {
                rg.isKonstantZins = true;
            }

            //Details fuer die Tafelauswahl
            if (TafelBox.SelectedItem != null)
            {
                tafeldDetails.Name = TafelBox.SelectedItem.ToString().Contains("2004") ? "DAV2004" : "DAV2008";
            }
            else
            {
                tafeldDetails.Name = "DAV2004";
            }

            if (OrdnungSelektionBox != null)
            {
                if (OrdnungSelektionBox.SelectedItem.ToString().Contains("Selektion"))
                {
                    tafeldDetails.Ordnung = OrdnungSelektionBox.SelectedItem.ToString().Contains("1.") ? "1.O. Selektion" : "2.O. Selektion";
                }
                else
                {
                    tafeldDetails.Ordnung = OrdnungSelektionBox.SelectedItem.ToString().Contains("1.") ? "1.O. Aggregat" : "2.O. Aggregat";
                }
            }
            else
            {
                tafeldDetails.Ordnung = "1.O. Selektion";
            }

            tafeldDetails.T_1 = double.Parse(t1TextBox.Text, System.Globalization.CultureInfo.InvariantCulture);
            tafeldDetails.T_2 = double.Parse(t2TextBox.Text, System.Globalization.CultureInfo.InvariantCulture);


            return Vertrag;
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
