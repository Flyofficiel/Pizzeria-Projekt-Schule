using MySqlConnector;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Pizzeria_Projekt_Schule
{
    // In diesem Fenster verwalten wir die Speisekarte (Speisen anschauen, hinzufügen, ändern)
    public partial class SpeisenMenu : Form
    {
        public SpeisenMenu()
        {
            InitializeComponent();
        }

        // Logik zum "Löschen" einer Speise
        private void SpeiseLoeschen()
        {
            // WICHTIG: Wir löschen nicht mit DELETE, weil die Speise noch in alten Rechnungen stehen könnte.
            // Stattdessen setzen wir aktiv auf 0 (das nennt man Soft Delete).
            string query = @"
                UPDATE speisen
                SET aktiv = 0
                WHERE speise_id = @speise_id";

            using (MySqlConnection conn = Database.GetConnection())
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                // Die ID holen wir uns aus der Zeile, die im Gitter gerade angeklickt ist
                cmd.Parameters.AddWithValue(
                    "@speise_id",
                    Speissen_menu_dataGridView1.CurrentRow.Cells["speise_id"].Value
                );

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Speise wurde aus der Karte entfernt ✔");
            SpeisenLaden(); // Liste neu laden, damit die Speise verschwindet
        }

        // Zurück zum Hauptmenü Knopf
        private void Zuruck_Hauptmenu_button4_Click(object sender, EventArgs e)
        {
            Hauptmenu mainmenupage = new Hauptmenu();
            mainmenupage.Show();
            this.Close();
        }

        // Wenn das Fenster öffnet, laden wir direkt die Liste
        private void Speissen_Load(object sender, EventArgs e)
        {
            SpeisenLaden();
        }

        // Öffnet das Fenster, um eine ganz neue Speise einzutippen
        private void Hinzufugen_button2_Click(object sender, EventArgs e)
        {
            Speisehinzufügen speisenhinzufügen = new Speisehinzufügen();
            speisenhinzufügen.Show();
        }

        // Der Löschen-Button mit Sicherheitsabfrage
        private void Loschen_button3_Click(object sender, EventArgs e)
        {
            // Prüfen, ob überhaupt was ausgewählt wurde
            if (Speissen_menu_dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Bitte wähle erst eine Speise aus der Liste aus.");
                return;
            }

            // Den Nutzer nochmal fragen, damit er nicht aus Versehen löscht
            DialogResult result = MessageBox.Show(
                "Soll diese Speise wirklich von der Karte genommen werden?",
                "Sicherheitsabfrage",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                SpeiseLoeschen();
            }
        }

        // Holt die Speisen-Daten aus der Datenbank
        private void SpeisenLaden()
        {
            try
            {
                // Wir laden nur Speisen, die 'aktiv = 1' sind
                string query = @"
                    SELECT speise_id, speisename, speisentyp, 
                    COALESCE(preis, 0.00) as preis, zutaten, aktiv
                    FROM speisen
                    WHERE aktiv = 1";

                using (MySqlConnection conn = Database.GetConnection())
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    Speissen_menu_dataGridView1.DataSource = table;
                }

                // Den Preis schön als Euro-Betrag formatieren
                if (Speissen_menu_dataGridView1.Columns.Contains("preis"))
                {
                    Speissen_menu_dataGridView1.Columns["preis"].DefaultCellStyle.Format = "C2";
                    Speissen_menu_dataGridView1.Columns["preis"].DefaultCellStyle.FormatProvider =
                        System.Globalization.CultureInfo.GetCultureInfo("de-DE");
                    DataGridDesign(); // Das Aussehen der Tabelle anpassen
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden: " + ex.Message);
            }
        }

        // Filter: Entweder nur aktive Speisen oder alles (auch alte) anzeigen
        private void Aktive_speissen_checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            string query;

            if (Aktive_speissen_checkBox1.Checked)
            {
                query = "SELECT speise_id, speisename, speisentyp, preis, zutaten FROM speisen WHERE aktiv = 1";
            }
            else
            {
                query = "SELECT speise_id, speisename, speisentyp, preis, zutaten, aktiv FROM speisen";
            }

            using (var conn = Database.GetConnection())
            using (var adapter = new MySqlDataAdapter(query, conn))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                Speissen_menu_dataGridView1.DataSource = table;
            }
            DataGridDesign();
        }

        // Einfacher Refresh-Button
        private void Aktualisieren_button5_Click(object sender, EventArgs e)
        {
            SpeisenLaden();
        }

        // Öffnet das Bearbeiten-Fenster für die gewählte Speise
        private void Update_button1_Click_1(object sender, EventArgs e)
        {
            if (Speissen_menu_dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Bitte wähle erst eine Speise aus.");
                return;
            }

            // Wir ziehen uns alle Infos aus der Tabelle, um sie dem anderen Fenster zu geben
            int id = Convert.ToInt32(Speissen_menu_dataGridView1.CurrentRow.Cells["speise_id"].Value);
            string name = Speissen_menu_dataGridView1.CurrentRow.Cells["speisename"].Value.ToString();
            string typ = Speissen_menu_dataGridView1.CurrentRow.Cells["speisentyp"].Value.ToString();

            decimal preis = 0;
            if (Speissen_menu_dataGridView1.CurrentRow.Cells["preis"].Value != null)
            {
                decimal.TryParse(Speissen_menu_dataGridView1.CurrentRow.Cells["preis"].Value.ToString(),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.GetCultureInfo("de-DE"), out preis);
            }
            string zutaten = Speissen_menu_dataGridView1.CurrentRow.Cells["zutaten"].Value.ToString();

            // Das Bearbeiten-Fenster öffnen
            speiupd updateForm = new speiupd(id, name, typ, preis, zutaten);
            updateForm.ShowDialog(); // Das Programm wartet hier, bis das Fenster zu ist

            SpeisenLaden(); // Danach Liste aktualisieren
        }

        // Hier wird eingestellt, wie das Gitter (DataGridView) optisch aussieht
        private void DataGridDesign()
        {
            Speissen_menu_dataGridView1.ReadOnly = true; // Keiner darf direkt in die Zellen schreiben

            // Schriftart für die Köpfe der Tabelle
            Speissen_menu_dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            Speissen_menu_dataGridView1.ColumnHeadersHeight = 45;

            // Schriftart für die Zeilen
            Speissen_menu_dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            Speissen_menu_dataGridView1.RowTemplate.Height = 35;

            // Spaltenbreite automatisch so einstellen, dass alles draufpasst
            Speissen_menu_dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // Die Zutaten-Spalte darf den restlichen Platz verbrauchen (sie ist meist am längsten)
            if (Speissen_menu_dataGridView1.Columns.Contains("zutaten"))
            {
                Speissen_menu_dataGridView1.Columns["zutaten"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            // Die Spaltennamen im Fenster schöner benennen als in der Datenbank
            if (Speissen_menu_dataGridView1.Columns.Contains("speisename"))
                Speissen_menu_dataGridView1.Columns["speisename"].HeaderText = "Name der Speise";

            if (Speissen_menu_dataGridView1.Columns.Contains("preis"))
                Speissen_menu_dataGridView1.Columns["preis"].HeaderText = "Preis (€)";
        }

        // Platzhalter für Klick-Events, falls du sie später brauchst
        private void Label1_Click(object sender, EventArgs e) { }
        private void Speissen_menu_dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void Label2_Click(object sender, EventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
    }
}