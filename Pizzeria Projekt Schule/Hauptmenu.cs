using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySqlConnector;

namespace Pizzeria_Projekt_Schule
{
    // Das ist die zentrale Seite unserer App. 
    // Von hier aus steuern wir alles: Bestellungen, Tische und Personal.
    public partial class Hauptmenu : Form
    {
        public Hauptmenu()
        {
            InitializeComponent();
            // Wir rufen das Design für die Tabelle direkt beim Start auf
            ReservierungGridDesign();
        }

        // Sobald das Fenster aufgeht, laden wir die Reservierungen aus der Datenbank
        private void Hauptmenu_Load(object sender, EventArgs e)
        {
            LadeReservierungen();
        }

        // Platzhalter für Klicks auf Bilder oder Texte (falls später noch was rein soll)
        private void Hauptmenu_pictureBox1_Click(object sender, EventArgs e) { }
        private void Label2_Click(object sender, EventArgs e) { }
        private void Label1_Click(object sender, EventArgs e) { }

        // --- HIER KOMMT DIE NAVIGATION ZU DEN ANDEREN SEITEN ---

        // Knopf für die Speisekarte: Öffnet die Verwaltung für Pizzen und Preise
        private void Speisemenü_Haupt_button1_Click(object sender, EventArgs e)
        {
            SpeisenMenu speisenpage = new SpeisenMenu();
            speisenpage.Show();
            this.Close(); // Altes Fenster zu, neues auf
        }

        // Knopf für neue Bestellungen
        private void Bestellungshaupt_button2_Click(object sender, EventArgs e)
        {
            Bestellungsseite bestellungspagerichtigpage = new Bestellungsseite();
            bestellungspagerichtigpage.Show();
            this.Close();
        }

        // Knopf um die Reservierungs-Übersicht zu öffnen
        private void Reservierungenmenu_button3_Click(object sender, EventArgs e)
        {
            Reservierungsseite reservierungpage = new Reservierungsseite();
            reservierungpage.Show();
            this.Close();
        }

        // Knopf für den Tischplan (zeigt welche Tische belegt sind)
        private void Tische_button4_Click(object sender, EventArgs e)
        {
            tischauswahl tischauswahlpage = new tischauswahl();
            tischauswahlpage.Show();
            this.Close();
        }

        // Knopf für das Kassen-System (Bezahlen)
        private void Zahlungsmenu_button5_Click(object sender, EventArgs e)
        {
            Zahlungsseite zahlungsartpage = new Zahlungsseite();
            zahlungsartpage.Show();
            this.Close();
        }

        // Knopf für die Umsatz-Auswertung (Statistik)
        private void Auswertungsmenu_button7_Click(object sender, EventArgs e)
        {
            Umsatzauswertung auswertungpage = new Umsatzauswertung();
            auswertungpage.Show();
            this.Close();
        }

        // Abmelden: Schickt uns zurück zum Login-Fenster
        private void Abmelden_button8_Click(object sender, EventArgs e)
        {
            Loginpizzeriavesus loginform = new Loginpizzeriavesus();
            loginform.Show();
            this.Close();
        }

        // Personalverwaltung öffnen
        private void Mitarbeiterverwaltung_button6_Click(object sender, EventArgs e)
        {
            Mitarbeiterverwaltung0 mitarbeiterverwaltungrichtigpage = new Mitarbeiterverwaltung0();
            mitarbeiterverwaltungrichtigpage.Show();
            this.Hide(); // Hier nur verstecken, falls man schnell zurück will
        }

        private void Reservierung_huaptmenu_dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        // --- DATENBANK-LOGIK: Reservierungen für heute anzeigen ---

        private void LadeReservierungen()
        {
            // Wir holen uns die Daten und nutzen 'JOIN', damit wir direkt den Namen des Gastes 
            // aus der Gast-Tabelle haben und nicht nur die ID-Nummer.
            // Außerdem wandeln wir die Slots (1, 2, 3...) in echte Uhrzeiten um (CASE).
            string query = @"
                SELECT 
                    r.reservierungs_id,
                    r.tisch_id_fk AS Tisch,
                    DATE(r.datum) AS Datum,

                    CASE r.slot
                        WHEN 1 THEN '12:00 - 15:00'
                        WHEN 2 THEN '15:00 - 18:00'
                        WHEN 3 THEN '18:00 - 21:00'
                        WHEN 4 THEN '21:00 - 24:00'
                    END AS Slot,

                    r.personenanzahl AS Personen,
                    CONCAT(g.gastvorname,' ',g.gastnachname) AS Gast,
                    g.telephonenr AS Telefon

                FROM reservierungen r
                JOIN gast g ON r.gastid_fk = g.gastid
                WHERE r.zustand = 'offen'
                AND DATE(r.datum) = @datum
                ORDER BY r.slot ASC";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                // Hier filtern wir nach dem Datum, das der User im Kalender angeklickt hat
                cmd.Parameters.AddWithValue("@datum", dateTimePicker1.Value.Date);

                DataTable dt = new DataTable();
                new MySqlDataAdapter(cmd).Fill(dt);

                // Daten in die Tabelle schieben
                Reservierung_huaptmenu_dataGridView1.DataSource = dt;
                ReservierungGridDesign();
                Reservierung_huaptmenu_dataGridView1.ClearSelection(); // Auswahl am Anfang aufheben
            }
        }

        // Schickes Layout für die Tabelle, damit man alles gut lesen kann
        private void ReservierungGridDesign()
        {
            // Header schick machen (Fett, groß, zentriert)
            var grid = Reservierung_huaptmenu_dataGridView1;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.ColumnHeadersHeight = 45;

            // Zeilen-Design
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            grid.RowTemplate.Height = 35;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(235, 235, 235); // Jede zweite Zeile grau

            // Spaltenbreite automatisch an den Text anpassen
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // Verhindert, dass der User direkt in der Tabelle rumschreibt
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Spaltennamen für Menschen verständlich machen
            if (grid.Columns.Contains("reservierungs_id")) grid.Columns["reservierungs_id"].HeaderText = "ID";
            if (grid.Columns.Contains("Tisch")) grid.Columns["Tisch"].HeaderText = "Tisch-Nr.";

            if (grid.Columns.Contains("Gast"))
            {
                grid.Columns["Gast"].HeaderText = "Kunde";
                // Die Gast-Spalte füllt den restlichen Platz aus
                grid.Columns["Gast"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            if (grid.Columns.Contains("Telefon")) grid.Columns["Telefon"].HeaderText = "Tel. Nummer";
        }

        // --- BUTTON: STORNIEREN ---
        private void Reservierung_storno_button9_Click(object sender, EventArgs e)
        {
            // Erst checken, ob überhaupt eine Zeile ausgewählt wurde
            if (Reservierung_huaptmenu_dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Bitte wähle erst eine Reservierung aus!", "Nichts ausgewählt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Die ID der Reservierung aus der gewählten Zeile holen
                int reservierungs_id = Convert.ToInt32(Reservierung_huaptmenu_dataGridView1.CurrentRow.Cells["reservierungs_id"].Value);

                // Update-Befehl: Wir löschen nicht, sondern setzen den Zustand auf 'storniert'
                string query = "UPDATE reservierungen SET zustand = 'storniert' WHERE reservierungs_id = @id";

                using (MySqlConnection conn = Database.GetConnection())
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", reservierungs_id);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Reservierung wurde storniert!", "Erfolg");
                LadeReservierungen(); // Liste neu laden, damit die stornierte weg ist
            }
            catch (Exception ex)
            {
                MessageBox.Show("Da gab es ein Problem beim Stornieren: " + ex.Message);
            }
        }

        private void Panel1_Paint(object sender, PaintEventArgs e) { }

        // Wenn man das Datum im Kalender ändert, laden wir die Liste neu (z.B. Reservierungen für morgen schauen)
        private void Hauptmenu_dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            LadeReservierungen();
        }

        private void pictureBox1_Click(object sender, EventArgs e) { }
    }
}