using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzeria_Projekt_Schule
{
    public class TischItem
    {
        public int TischId { get; set; }
        public string Status { get; set; }   // Frei / Besetzt / Reserviert
        public string Bereich { get; set; }

        public override string ToString()
        {
            return $"Tisch {TischId} - {Bereich} ({Status})";
        }
    }
}
