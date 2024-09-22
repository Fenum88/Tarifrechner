using System.Text;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Runtime.InteropServices;

namespace Tarifrechner
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // DllImport zum Öffnen der Konsole
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();

        Eingabe eingabe;
        Ausgabe ausgabe;
        public List<string> Qx { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            //AllocConsole();
            // Starte mit der ersten Seite
            eingabe = new Eingabe();
            ausgabe= new Ausgabe();
            MainFrame.Navigate(eingabe);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        // Event-Handler für Navigation zu Seite 1
        private void NavigateToEingabe(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(eingabe);
        }

        // Event-Handler für Navigation zu Seite 2
        private void NavigateToAusgabe(object sender, RoutedEventArgs e)
        {
            ausgabe.Berechne(eingabe.Berechne());
            MainFrame.Navigate(ausgabe);
        }
    }
}