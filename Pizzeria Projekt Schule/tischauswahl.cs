using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pizzeria_Projekt_Schule
{
    public partial class tischauswahl : Form
    {
        public tischauswahl()
        {
            InitializeComponent();
        }

        // Das hier passiert sofort, wenn das Fenster geöffnet wird
        private void Form3_Load(object sender, EventArgs e)
        {
            // Wir rufen die Methode auf, die alle Tische aus der Datenbank holt
            LadeTische();
        }

        // Der Zurück-Button schließt die Tischübersicht und öffnet wieder das Hauptmenü
        private void Zuruck_button41_Click(object sender, EventArgs e)
        {
            Hauptmenu mainmenupage = new Hauptmenu();
            mainmenupage.Show();
            this.Close(); // Schließt das aktuelle Fenster
        }

        // Hier passiert die eigentliche Arbeit mit der Datenbank
        private void LadeTische()
        {
            // Wir holen uns die offene Verbindung aus unserer Database-Klasse
            MySqlConnection conn = Database.GetConnection();

            try
            {
                // In diesem SQL-Befehl verknüpfen wir die Tische mit den Mitarbeitern (LEFT JOIN)
                // So wird direkt angezeigt, welcher Kellner für welchen Bereich (z.B. Terrasse) zuständig ist
                string query = @"
                    SELECT  
                        t.tisch_id AS 'Tisch Nr.',
                        t.max_personen AS 'Plätze',
                        t.bereich AS 'Bereich',
                        t.lage AS 'Lage',
                        t.ort AS 'Ort',
                        CONCAT(m.vorname, ' ', m.nachname) AS 'Zuständiger Service'
                    FROM tische t
                    LEFT JOIN mitarbeiter m 
                        ON t.bereich = m.bereich
                        AND m.rolle = 'service'
                    ORDER BY t.tisch_id;";

                // Der Adapter führt den Befehl aus und holt die Daten
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable(); // Eine leere Tabelle im Arbeitsspeicher erstellen
                adapter.Fill(dt); // Die Tabelle mit den Datenbank-Ergebnissen befüllen

                // Die befüllte Tabelle wird jetzt einfach im DataGridView (dem Gitter) angezeigt
                Tischauswahl_dataGridView1.DataSource = dt;

                // Hier machen wir die Tabelle noch ein bisschen hübscher
                Tischauswahl_dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Spalten füllen das Fenster aus
                Tischauswahl_dataGridView1.AllowUserToAddRows = false; // Verhindert, dass man unten neue Zeilen von Hand eintippt
                Tischauswahl_dataGridView1.ReadOnly = true; // Sperrt das Bearbeiten, da es eine reine Info-Seite ist

                // Wir nehmen die Standard-Markierung der ersten Zelle raus
                Tischauswahl_dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                // Falls der Server nicht erreichbar ist oder der SQL-Befehl einen Fehler hat
                MessageBox.Show("Fehler beim Laden der Tischübersicht: " + ex.Message);
            }
        }

        // Leere Event-Methoden können hier stehen bleiben, damit es im Designer keine Fehler gibt
        private void label1_Click(object sender, EventArgs e) { }
        private void Tischauswahl_dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }
}