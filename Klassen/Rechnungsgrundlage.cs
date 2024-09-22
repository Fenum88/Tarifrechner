using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tarifrechner.Klassen
{
    public class Rechnungsgrundlage
    {
        //Rechnungszins
        public double zins { get; set; }
        // Männeranteil
        public double mannAnteil { get; set; }
        //Mit konstanten Zins oder variablen Zins
        public bool isKonstantZins { get; set; }
        public Rechnungsgrundlage() { }
    }
}
