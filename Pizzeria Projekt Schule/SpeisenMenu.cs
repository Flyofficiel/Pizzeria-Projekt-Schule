using MySqlConnector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Pizzeria_Projekt_Schule
{
    // Dieses Formular zeigt die Liste aller Speisen an.
    // Man kann hier Speisen hinzufügen, bearbeiten oder löschen.
    public partial class SpeisenMenu : Form
    {
        public SpeisenMenu()
        {
            // Initialisiert die Fenster-Oberfläche
            InitializeComponent();
        }

        // --- SPEISE LÖSCHEN (Logik) ---
        // Wir löschen die Speise nicht wirklich aus der Tabelle (wegen alter Bestellungen),
        // sondern setzen nur den Status 'aktiv' auf 0. Das nennt man "Soft Delete".
        private void SpeiseLoeschen()
        {
            string query = @"
                UPDATE speisen
                SET aktiv = 0
                WHERE speise_id = @speise_id";

            using (MySqlConnection conn = Database.GetConnection())
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                // Wir holen uns die ID der Speise aus der aktuell angeklickten Zeile
                cmd.Parameters.AddWithValue(
                    "@speise_id",
                    Speissen_menu_dataGridView1.CurrentRow.Cells["speise_id"].Value
                );

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Speise gelöscht ✔");
            SpeisenLaden(); // Die Liste aktualisieren, damit die gelöschte Speise verschwindet
        }

        private void Label1_Click(object sender, EventArgs e) { }
        private void Speissen_menu_dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void Label2_Click(object sender, EventArgs e) { }

        // --- NAVIGATION ---
        // Schließt diese Seite und geht zurück zum Hauptmenü
        private void Zuruck_Hauptmenu_button4_Click(object sender, EventArgs e)
        {
            Hauptmenu mainmenupage = new Hauptmenu();
            mainmenupage.Show();
            this.Close();
        }

        // Beim Starten des Fensters laden wir sofort alle Speisen aus der Datenbank
        private void Speissen_Load(object sender, EventArgs e)
        {
            SpeisenLaden();
        }

        // Öffnet das kleine Fenster, um eine neue Pizza/Speise anzulegen
        private void Hinzufugen_button2_Click(object sender, EventArgs e)
        {
            Speisehinzufügen speisenhinzufügen = new Speisehinzufügen();
            speisenhinzufügen.Show();
        }

        // --- BUTTON: LÖSCHEN ---
        private void Loschen_button3_Click(object sender, EventArgs e)
        {
            // Erst prüfen, ob überhaupt eine Speise in der Liste markiert ist
            if (Speissen_menu_dataGridView1.CurrentRow == null ||
                Speissen_menu_dataGridView1.CurrentRow.Cells["speise_id"].Value == null)
            {
                MessageBox.Show("Bitte zuerst eine Speise auswählen.");
                return;
            }

            // Wir fragen zur Sicherheit nochmal nach, bevor wir löschen
            DialogResult result = MessageBox.Show(
                "Möchten Sie diese Speise wirklich löschen?",
                "Bestätigung",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            // Nur wenn der Benutzer auf 'Ja' klickt, wird die Methode SpeiseLoeschen ausgeführt
            if (result == DialogResult.Yes)
            {
                SpeiseLoeschen();
            }
        }

        // --- DATEN LADEN ---
        // Holt alle aktiven Speisen aus der Datenbank und zeigt sie in der Tabelle an
        private void SpeisenLaden()
        {
            try
            {
                // SQL: Wir laden ID, Name, Typ, Preis und Zutaten.
                // COALESCE(preis, 0.00) sorgt dafür, dass kein Fehler kommt, falls mal kein Preis eingetragen ist.
                string query = @"
                    SELECT speise_id, speisename, speisentyp, 
                    COALESCE(preis, 0.00) as preis, zutaten ,aktiv
                    FROM speisen
                    WHERE aktiv = 1";

                using (MySqlConnection conn = Database.GetConnection())
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    Speissen_menu_dataGridView1.DataSource = table; // Daten an die Tabelle binden
                }

                // Hier stellen wir ein, dass die Preis-Spalte als Währung (€) angezeigt wird
                if (Speissen_menu_dataGridView1.Columns.Contains("preis"))
                {
                    Speissen_menu_dataGridView1.Columns["preis"].DefaultCellStyle.Format = "C2"; // C2 steht für Currency (Währung)
                    Speissen_menu_dataGridView1.Columns["preis"].DefaultCellStyle.FormatProvider =
                        System.Globalization.CultureInfo.GetCultureInfo("de-DE"); // Deutsches Format (Komma statt Punkt)
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Speisen: " + ex.Message);
            }
        }

        // --- FILTER: NUR AKTIVE ODER ALLE SPEISEN ---
        private void Aktive_speissen_checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            string query;

            // Je nachdem ob der Haken gesetzt ist, laden wir nur aktive oder wirklich alle Speisen
            if (Aktive_speissen_checkBox1.Checked)
            {
                query = "SELECT speise_id, speisename, speisentyp, preis, zutaten FROM speisen WHERE aktiv = 1";
            }
            else
            {
                query = "SELECT speise_id, speisename, speisentyp, preis, zutaten, aktiv FROM speisen";
            }

            using (MySqlConnection conn = Database.GetConnection())
            using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                Speissen_menu_dataGridView1.DataSource = table;
            }

            // Währungsformatierung wieder anwenden
            Speissen_menu_dataGridView1.Columns["preis"].DefaultCellStyle.Format = "C2";
            Speissen_menu_dataGridView1.Columns["preis"].DefaultCellStyle.FormatProvider =
                System.Globalization.CultureInfo.GetCultureInfo("de-DE");
        }

        // Button zum manuellen Aktualisieren der Liste
        private void Aktualisieren_button5_Click(object sender, EventArgs e)
        {
            SpeisenLaden();
        }

        // --- BUTTON: BEARBEITEN / UPDATE ---
        private void Update_button1_Click_1(object sender, EventArgs e)
        {
            // Prüfen, ob eine Zeile ausgewählt wurde
            if (Speissen_menu_dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Bitte zuerst eine Speise auswählen.");
                return;
            }

            // Wir lesen alle Daten aus der markierten Zeile aus
            int id = Convert.ToInt32(Speissen_menu_dataGridView1.CurrentRow.Cells["speise_id"].Value);
            string name = Speissen_menu_dataGridView1.CurrentRow.Cells["speisename"].Value.ToString();
            string typ = Speissen_menu_dataGridView1.CurrentRow.Cells["speisentyp"].Value.ToString();

            // Preis sicher umwandeln (beachtet Komma und Punkt)
            decimal preis = 0;
            if (Speissen_menu_dataGridView1.CurrentRow.Cells["preis"].Value != null)
            {
                decimal.TryParse(Speissen_menu_dataGridView1.CurrentRow.Cells["preis"].Value.ToString(),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.GetCultureInfo("de-DE"), out preis);
            }
            string zutaten = Speissen_menu_dataGridView1.CurrentRow.Cells["zutaten"].Value.ToString();

            // Wir öffnen das Update-Fenster und übergeben die Daten der Speise
            speiupd updateForm = new speiupd(id, name, typ, preis, zutaten);
            updateForm.ShowDialog(); // ShowDialog pausiert dieses Fenster, bis das andere geschlossen wird

            SpeisenLaden(); // Wenn das Update-Fenster zugeht, laden wir die Liste neu
        }
    }
}