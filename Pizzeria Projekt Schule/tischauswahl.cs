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

        private void Form3_Load(object sender, EventArgs e)
        {
            // Beim Laden der Form werden sofort alle Tischdaten aus der DB geholt
            LadeTische();
        }

        // --- NAVIGATION ---
        private void button41_Click(object sender, EventArgs e)
        {
            Hauptmenu mainmenupage = new Hauptmenu();
            mainmenupage.Show();
            this.Close();
        }

        // --- DATEN LADEN ---
        private void LadeTische()
        {
            // Verbindung über deine zentrale Database-Klasse holen
            MySqlConnection conn = Database.GetConnection();

            try
            {
                // SQL-Abfrage mit Verknüpfung (Join) der Mitarbeiter-Tabelle
                // So sieht man sofort, welcher Kellner für welchen Bereich eingeteilt ist
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

                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Das DataGridView wird automatisch mit den Spalten aus dem SQL-Query befüllt
                dataGridView1.DataSource = dt;

                // Optisches Tuning für das Grid
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.AllowUserToAddRows = false; // Verhindert leere Zeile am Ende
                dataGridView1.ReadOnly = true;            // Nur zum Anschauen gedacht
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Tischübersicht: " + ex.Message);
            }
        }

        // Platzhalter für Event-Handler (können gelöscht werden, wenn nicht genutzt)
        private void label1_Click(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }
}