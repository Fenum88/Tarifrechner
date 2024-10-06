using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Accessibility;

namespace Tarifrechner.Klassen
{
    public class TafeldDetails
    {
        public string Name { get; set; }
        public double T_1 { get; set; }
        public double T_2 { get; set; }
        public string Ordnung { get; set; }

        public TafeldDetails(string name, double t_1, double t_2, string ordnung)
        {
            Name = name;
            T_1 = t_1;
            T_2 = t_2;
            Ordnung = ordnung;
        }
        // Standardkonstruktor, der den parameterisierten Konstruktor aufruft
        public TafeldDetails() : this("DAV2004", 5.0, 10.0, "1.O. Selektion")
        {
            // Hier könnte zusätzlicher Code stehen, wenn nötig
        }
    }
}
