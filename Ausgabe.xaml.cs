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
        public int m { get; set; }
        public int alter { get; set; }
        public double qx { get; set; }
        public double zins { get; set; }
        public double aufzinsfaktor { get; set; }
        public double diskontierungsfaktor { get; set; }
        //Barwerte
        public double lbw_tod { get; set; }
        public double lbw_erleben { get; set; }
        public double lbw_rente { get; set; }
        public double lbw_praemie { get; set; }
        public double lbw_aufschubszeit { get; set; }
        //Deckungsrueckstellungen
        //Netto
        public double mVx { get; set; }
        //Zillmer
        public double mVx_z { get; set; }
        //Ausreichende
        public double mVx_a { get; set; }
        //Ausreichende gezillmert
        public double mVx_a_gez { get; set; }
        //Bilanzielle
        public double mVx_B { get; set; }
        //Inkassokosten
        public double beta_kosten { get; set; }
        //Verwaltungskosten
        public double gamma_kosten { get; set; }
        //Rentenbezugskosten
        public double delta_kosten { get; set; }
        //alpha_gamma_kosten
        public double alpha_gamma_kosten { get; set; }
        //Gesamtkosten
        public double gesamtkosten { get; set; }



    }
    /// <summary>
    /// Interaction logic for Ausgabe.xaml
    /// </summary>
    public partial class Ausgabe : Page
    {
        public List<Tafel> TafelListe { get; set; }
        public List<GesamtOutput> GesamtOutput { get; set; }
        public DAV2004 DAV2004 { get; set; }
        public Ausgabe()
        {
            InitializeComponent();
            GesamtOutput = new();
            DAV2004 = new();
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

            for (int index = 1; index < zeilenSterbetafel.Length; index++)
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
            TafelListe = LeseSterbetafel("DAV2008.csv");
            TafeldDetails tafeldDetails = vertrag.Rechnungsgrundlage.TafeldDetails;
            if (tafeldDetails.Name == "DAV2004")
                DAV2004 = new DAV2004(tafeldDetails.T_1, tafeldDetails.T_2, vertrag.Vertragsteil.Policenbeginnjahr, vertrag.Vertragsteil.ea, vertrag.Vertragsteil.n, TafelListe, tafeldDetails.Ordnung);
            GesamtOutput = new();
            BerechneQx(vertrag);

            Nettobeitrag.Text = BerechneNettobeitrag(vertrag).ToString();
            Bruttobeitrag.Text = BerechneBruttobeitrag(vertrag).ToString();
            Zillmerprämie.Text = BerechnZillmerbeitrag(vertrag).ToString();
            (double alpha_z1, double alpha_gamma1) = BerechneAbschlusskosten(vertrag);
            (alpha_z.Text, alpha_gamma.Text) = (alpha_z1.ToString(), alpha_gamma1.ToString());
            BerechneKostenverlauf(vertrag);
            BerechneDeckungsrueckstellungen(vertrag);
            GesamtAusgabeBox.ItemsSource = null;
            GesamtAusgabeBox.ItemsSource = GesamtOutput;

        }
        public double BerechneNettobeitrag(Vertrag vertrag)
        {
            // Berechnung fuer Todesfallleistung
            double beitrag = GesamtOutput[0].lbw_tod * vertrag.Vertragsteil.leistung2 / GesamtOutput[0].lbw_praemie;
            // Berechnung fuer Erlebensfallleistung
            beitrag += GesamtOutput[0].lbw_erleben * vertrag.Vertragsteil.leistung1 / GesamtOutput[0].lbw_praemie;
            // Berechnung fuer Rente
            beitrag += GesamtOutput[0].lbw_rente * vertrag.Vertragsteil.leistung3 / GesamtOutput[0].lbw_praemie;
            return beitrag;
        }

        public double BerechneBruttobeitrag(Vertrag vertrag)
        {
            // Berechnung fuer lbw
            double lbw_tod = GesamtOutput[0].lbw_tod * vertrag.Vertragsteil.leistung2;
            double lbw_erleben = GesamtOutput[0].lbw_erleben * vertrag.Vertragsteil.leistung1;
            double lbw_rente = GesamtOutput[0].lbw_rente * vertrag.Vertragsteil.leistung3;
            double lbw_aufschubszeit = GesamtOutput[0].lbw_aufschubszeit;
            double lbw_praemie = GesamtOutput[0].lbw_praemie;
            // Berechnungen bbw
            double bbw = GesamtOutput[0].lbw_praemie;
            int t = vertrag.Vertragsteil.t;

            // Kostensaetze
            double alpha = vertrag.Rechnungsgrundlage.alpha;
            (double alpha_z1, double alpha_gamma1) = BerechneAbschlusskosten(vertrag);
            double beta = vertrag.Rechnungsgrundlage.beta;
            double gamma = vertrag.Rechnungsgrundlage.gamma;
            double delta = vertrag.Rechnungsgrundlage.delta;

            double lbw_gesamt = (lbw_tod + lbw_erleben + lbw_rente * (1 + delta));
            double bbw_gesamt = bbw * (1 - t * alpha / lbw_praemie - beta - t * gamma * lbw_aufschubszeit / lbw_praemie);

            return lbw_gesamt / bbw_gesamt;
        }

        public double BerechnZillmerbeitrag(Vertrag vertrag)
        {
            double nettopraemie = BerechneNettobeitrag(vertrag);
            double bruttopraemie = BerechneBruttobeitrag(vertrag);
            // Berechnungen bbw
            double bbw = GesamtOutput[0].lbw_praemie;
            int t = vertrag.Vertragsteil.t;

            // Kostensaetze
            (double alpha_z1, double alpha_gamma1) = BerechneAbschlusskosten(vertrag);

            double wert = nettopraemie + alpha_z1 * bruttopraemie * t / bbw;
            return wert;
        }

        public void BerechneDeckungsrueckstellungen(Vertrag vertrag)
        {
            // Berechnung Praemien
            double nettopraemie = BerechneNettobeitrag(vertrag);
            double bruttopraemie = BerechneBruttobeitrag(vertrag);
            double zillmerpraemie = BerechnZillmerbeitrag(vertrag);

            (double alpha_z1, double alpha_gamma1) = BerechneAbschlusskosten(vertrag);
            double delta = vertrag.Rechnungsgrundlage.delta;

            //Gesamte Abschlusskoten
            double abKo = alpha_z1 * vertrag.Vertragsteil.t * bruttopraemie / GesamtOutput[0].lbw_praemie;

            for (int i = 0; i < vertrag.Vertragsteil.n; i++)
            {
                // Berechnung fuer lbw
                double lbw_tod = GesamtOutput[i].lbw_tod * vertrag.Vertragsteil.leistung2;
                double lbw_erleben = GesamtOutput[i].lbw_erleben * vertrag.Vertragsteil.leistung1;
                double lbw_rente = GesamtOutput[i].lbw_rente * vertrag.Vertragsteil.leistung3;
                double lbw = lbw_tod + lbw_erleben + lbw_rente + (1 + delta);
                double lbw_netto = lbw_tod + lbw_erleben + lbw_rente;

                //Barwerte
                double bbw = GesamtOutput[i].lbw_praemie;

                double mVx = lbw_netto - nettopraemie * bbw;
                double mVx_z = mVx - abKo * bbw;
                //double mVx_a_gez = 0.0;
                //double mVx_B = 0.0;
                GesamtOutput[i].mVx = mVx;
                GesamtOutput[i].mVx_z = mVx_z;
                if (i == 0)
                {
                    GesamtOutput[i].mVx_a = -vertrag.Rechnungsgrundlage.alpha * vertrag.Vertragsteil.t * bruttopraemie;
                    GesamtOutput[i].mVx_a_gez = mVx_z;

                }
                else
                {
                    GesamtOutput[i].mVx_a = lbw_netto + GesamtOutput[i].gesamtkosten - GesamtOutput[i].alpha_gamma_kosten - bbw * bruttopraemie;
                    GesamtOutput[i].mVx_a_gez = lbw_netto + GesamtOutput[i].gesamtkosten - bbw * bruttopraemie;
                }
                GesamtOutput[i].mVx_B = Math.Max(GesamtOutput[i].mVx_a_gez, 0);
            }
        }

        public (double, double) BerechneAbschlusskosten(Vertrag vertrag)
        {
            double alpha_z = Math.Min(vertrag.Rechnungsgrundlage.alpha, 0.025);
            double alpha_gamma = (vertrag.Rechnungsgrundlage.alpha - alpha_z) / GesamtOutput[0].lbw_aufschubszeit;
            return (alpha_z, alpha_gamma);
        }

        public void BerechneKostenverlauf(Vertrag vertrag)
        {
            Vertragsteil vt = vertrag.Vertragsteil;
            Rechnungsgrundlage rg = vertrag.Rechnungsgrundlage;
            double nettopraemie = BerechneNettobeitrag(vertrag);
            double bruttopraemie = BerechneBruttobeitrag(vertrag);

            // Berechnungen bbw
            int t = vertrag.Vertragsteil.t;

            // Kostensaetze
            (double alpha_z1, double alpha_gamma1) = BerechneAbschlusskosten(vertrag);
            double beta = rg.beta;
            double gamma = rg.gamma;
            double delta = rg.delta;

            double abKlo = bruttopraemie * vt.t * alpha_gamma1;

            //Berechnung Kostenverlauf
            GesamtOutput[vt.n].beta_kosten = 0;
            GesamtOutput[vt.n].gamma_kosten = 0;
            GesamtOutput[vt.n].delta_kosten = 0;
            GesamtOutput[vt.n].alpha_gamma_kosten = 0;
            GesamtOutput[vt.n].gesamtkosten = 0;
            for (int i = vt.n - 1; i > -1; i--)
            {
                double bbw = GesamtOutput[i].lbw_praemie;
                double lbw_erleben = GesamtOutput[i].lbw_erleben;
                double lbw_rente = GesamtOutput[i].lbw_rente * vt.leistung3;
                double lbw_aufschubszeit = GesamtOutput[i].lbw_aufschubszeit;
                if (i < vt.t)
                    GesamtOutput[i].beta_kosten = bbw * beta * bruttopraemie;
                GesamtOutput[i].gamma_kosten = lbw_aufschubszeit * gamma * bruttopraemie * vt.t;
                GesamtOutput[i].delta_kosten = lbw_rente * delta;
                GesamtOutput[i].alpha_gamma_kosten = abKlo * lbw_aufschubszeit;
                GesamtOutput[i].gesamtkosten = GesamtOutput[i].beta_kosten + GesamtOutput[i].gamma_kosten + GesamtOutput[i].delta_kosten + GesamtOutput[i].alpha_gamma_kosten;
            }
        }
        public void BerechneQx(Vertrag vertrag)
        {
            Vertragsteil vt = vertrag.Vertragsteil;
            Rechnungsgrundlage rg = vertrag.Rechnungsgrundlage;
            int index = 0;
            for (int i = vt.ea; i < TafelListe.Count; i++)
            {
                Tafel zeile = TafelListe[i];
                GesamtOutput wert = new();
                wert.qx = zeile.QX * rg.mannAnteil + (1 - rg.mannAnteil) * zeile.QY;
                wert.m = index;
                wert.alter = index + vt.ea;
                wert.zins = rg.isKonstantZins ? rg.zins : TafelListe[i - vt.ea].ZINS;
                wert.aufzinsfaktor = wert.zins + 1;
                wert.diskontierungsfaktor = 1 / wert.aufzinsfaktor;
                index++;
                GesamtOutput.Add(wert);
            }

            // Berechnung von LBW Tod
            GesamtOutput[vt.n].lbw_tod = 0;
            for (int i = vt.n - 1; i > -1; i--)
            {
                GesamtOutput[i].lbw_tod = (GesamtOutput[i + 1].lbw_tod * (1 - GesamtOutput[i].qx) + GesamtOutput[i].qx) * GesamtOutput[i].diskontierungsfaktor;
            }
            //Berechnung von LBW Erlebensfall lbw_erleben
            GesamtOutput[vt.n].lbw_erleben = 1;
            for (int i = vt.n - 1; i > -1; i--)
            {
                double wertVorjahr = GesamtOutput[i + 1].lbw_erleben;
                double px = (1 - GesamtOutput[i].qx);
                double v = GesamtOutput[i].diskontierungsfaktor;
                GesamtOutput[i].lbw_erleben = wertVorjahr * px * v;
            }
            //Berechnung von LBW Praemie
            GesamtOutput[vt.t].lbw_praemie = 0;
            for (int i = vt.t - 1; i > -1; i--)
            {
                double wertVorjahr = GesamtOutput[i + 1].lbw_praemie;
                double px = (1 - GesamtOutput[i].qx);
                double v = GesamtOutput[i].diskontierungsfaktor;
                GesamtOutput[i].lbw_praemie = wertVorjahr * px * v + 1;
            }
            //Berechnung von LBW Aufschubszeit lbw_aufschubszeit
            GesamtOutput[vt.n].lbw_aufschubszeit = 0;
            for (int i = vt.n - 1; i > -1; i--)
            {
                double wertVorjahr = GesamtOutput[i + 1].lbw_aufschubszeit;
                double px = (1 - GesamtOutput[i].qx);
                double v = GesamtOutput[i].diskontierungsfaktor;
                GesamtOutput[i].lbw_aufschubszeit = wertVorjahr * px * v + 1;
            }
            //Berechnung von LBW Rente lbw_rente
            GesamtOutput[GesamtOutput.Count - 1].lbw_rente = 1;
            for (int i = GesamtOutput.Count - 2; i >= 0; i--)
            {
                double wertVorjahr = GesamtOutput[i + 1].lbw_rente;
                double px = (1 - GesamtOutput[i].qx);
                double v = GesamtOutput[i].diskontierungsfaktor;
                wertVorjahr *= px * v;
                if (i >= vt.n)
                    wertVorjahr += 1;
                GesamtOutput[i].lbw_rente = wertVorjahr;
            }

        }

    }
}
