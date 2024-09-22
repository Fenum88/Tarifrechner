using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tarifrechner
{
    public class Vertragsteil
    {

        // Eintrittsalter
        public int ea { get; set; }
        //Versicherungsdauer
        public int n { get; set; }
        //Beitragszahldauer
        public int t { get; set; }
        //Erlebensfallleistung
        public double leistung1 { get; set; }
        //Todesfallleistung
        public double leistung2 { get; set; }

    }
}
