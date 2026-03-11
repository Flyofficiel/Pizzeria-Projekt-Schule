using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzeria_Projekt_Schule
{
    // Hilfsklasse für die Tisch-Objekte in der ComboBox
    public class TischItem
    {
        public int TischId { get; set; }
        public string Status { get; set; }
        public string Bereich { get; set; }

        public override string ToString()
        {
            // Das wird in der Liste angezeigt
            return $"Tisch {TischId} ({Status})";
        }
    }

    // Hilfsklasse für die Artikel im Warenkorb
    public class WarenkorbItem
    {
        public int SpeiseId { get; set; }
        public string Name { get; set; }
        public decimal Preis { get; set; }
        public int Menge { get; set; }
        public override string ToString() => $"{Name} x{Menge} ({Preis * Menge:0.00} €)";
    }
}

