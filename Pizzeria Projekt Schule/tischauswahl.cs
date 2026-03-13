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
            MySqlConnection conn = Database.GetConnection();

            try
            {
                string query = @"
        SELECT 
            t.tisch_id AS 'Nr.', 
            t.ort AS 'Lage/Bereich', 
            t.max_personen AS 'Plätze',
            -- Mitarbeiter-Info (wer ist für diesen Bereich zuständig?)
            IFNULL(CONCAT(m.vorname, ' ', m.nachname), 'Kein Personal') AS 'Kellner',
            CASE
                -- 1. BESETZT: Offene Bestellung im aktuellen Slot
                WHEN EXISTS (SELECT 1 FROM bestellungen b 
                             WHERE b.tisch_id_fk = t.tisch_id 
                             AND b.slot = @slot 
                             AND DATE(b.datum) = @datum 
                             AND b.status = 'offen') THEN '🔴 BESETZT'

                -- 2. GAST DA: Reservierung ist bereits 'aktiv'
                WHEN EXISTS (SELECT 1 FROM reservierungen r 
                             WHERE r.tisch_id_fk = t.tisch_id 
                             AND DATE(r.datum) = @datum 
                             AND r.slot = @slot 
                             AND r.zustand = 'aktiv') THEN '🟡 AKTIV'

                -- 3. RESERVIERT: Zukünftige Reservierung für diesen Slot
                WHEN EXISTS (SELECT 1 FROM reservierungen r 
                             WHERE r.tisch_id_fk = t.tisch_id 
                             AND DATE(r.datum) = @datum 
                             AND r.slot = @slot 
                             AND r.zustand = 'offen') THEN '🔵 RESERVIERT'

                ELSE '🟢 FREI'
            END AS 'Status'
        FROM tische t
        -- Join über das Feld 'bereich' (z.B. 'Tische 1-10')
        LEFT JOIN mitarbeiter m ON t.bereich = m.bereich AND m.aktiv = true
        WHERE t.aktiv = true 
        ORDER BY t.tisch_id ASC"; // Sortiert von Tisch 1 bis 40

                MySqlCommand cmd = new MySqlCommand(query, conn);

                // Parameter für den aktuellen Zeit-Check
                cmd.Parameters.AddWithValue("@slot", 1); 
                cmd.Parameters.AddWithValue("@datum", DateTime.Now.ToString("yyyy-MM-dd"));

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                Tischauswahl_dataGridView1.DataSource = dt;

                //  OPTIK-FINISH
                Tischauswahl_dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                Tischauswahl_dataGridView1.RowHeadersVisible = false;
                Tischauswahl_dataGridView1.AllowUserToAddRows = false;

                // Damit man die Zeilen besser unterscheiden kann (Zebra-Muster)
                Tischauswahl_dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden aller Tische: " + ex.Message);
            }
        }

       

    }
}