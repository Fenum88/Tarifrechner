using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tarifrechner.Klassen
{
    public class Vertrag
    {
        public Vertragsteil Vertragsteil { get; set; }
        public Rechnungsgrundlage Rechnungsgrundlage { get; set; }
        public Vertrag() 
        {
            Vertragsteil = new();
            Rechnungsgrundlage = new();
        }
    }
}
