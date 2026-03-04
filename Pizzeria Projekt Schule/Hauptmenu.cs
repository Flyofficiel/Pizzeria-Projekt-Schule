using System;
using System.Data;
using System.Windows.Forms;
using MySqlConnector;

namespace Pizzeria_Projekt_Schule
{
    // Das ist das Hauptmenü unserer Pizzeria-App.
    // Von hier aus gelangt man per Klick auf alle anderen Funktionen.
    public partial class Hauptmenu : Form
    {
        public Hauptmenu()
        {
            InitializeComponent();
        }

        // Sobald das Hauptmenü geladen wird, rufen wir eine Methode auf,
        // die uns die aktuellen Reservierungen aus der Datenbank zeigt.
        private void Hauptmenu_Load(object sender, EventArgs e)
        {
            LadeReservierungen();
        }

        // Platzhalter für Klicks auf Bilder oder Labels, falls man dort noch Logik braucht.
        private void hauptmenu_pictureBox1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }

        // --- NAVIGATION: Hier werden die anderen Fenster geöffnet ---

        // Öffnet die Speisekarte (z.B. um Preise zu ändern)
        private void speisemenü_Haupt_button1_Click(object sender, EventArgs e)
        {
            SpeisenMenu speisenpage = new SpeisenMenu();
            speisenpage.Show(); // Neue Seite anzeigen
            this.Close(); // Aktuelle Seite schließen
        }

        // Öffnet die Seite, um neue Bestellungen aufzunehmen
        private void Bestellungshaupt_button2_Click(object sender, EventArgs e)
        {
            Bestellungsseite bestellungspagerichtigpage = new Bestellungsseite();
            bestellungspagerichtigpage.Show();
            this.Close();
        }

        // Öffnet die Seite, um Tische zu reservieren
        private void reservierungenmenu_button3_Click(object sender, EventArgs e)
        {
            Reservierungsseite reservierungpage = new Reservierungsseite();
            reservierungpage.Show();
            this.Close();
        }

        // Zeigt an, welche Tische gerade im Restaurant belegt sind
        private void Tische_button4_Click(object sender, EventArgs e)
        {
            tischauswahl tischauswahlpage = new tischauswahl();
            tischauswahlpage.Show();
            this.Close();
        }

        // Öffnet das Kassen-System zum Bezahlen
        private void Zahlungsmenu_button5_Click(object sender, EventArgs e)
        {
            int bestellnr = 0; // Platzhalter für die Bestellung
            Zahlungsseite zahlungsartpage = new Zahlungsseite();
            zahlungsartpage.Show();
            this.Close();
        }

        // Zeigt die Statistik an, wie viel Geld wir verdient haben
        private void Auswertungsmenu_button7_Click(object sender, EventArgs e)
        {
            Umsatzauswertung auswertungpage = new Umsatzauswertung();
            auswertungpage.Show();
            this.Close();
        }

        // Loggt den aktuellen Mitarbeiter aus und geht zurück zum Login
        private void Abmelden_button8_Click(object sender, EventArgs e)
        {
            Loginpizzeriavesus loginform = new Loginpizzeriavesus();
            loginform.Show();
            this.Close();
        }

        // Öffnet die Verwaltung für das Personal
        private void Mitarbeiterverwaltung_button6_Click(object sender, EventArgs e)
        {
            Mitarbeiterverwaltung0 mitarbeiterverwaltungrichtigpage = new Mitarbeiterverwaltung0();
            mitarbeiterverwaltungrichtigpage.Show();
            this.Hide(); // Das Hauptmenü wird hier nur versteckt
        }

        private void Reservierung_huaptmenu_dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        // --- DATENBANK-LOGIK: Reservierungen anzeigen ---

        // Diese Methode holt die Reservierungen aus der Datenbank
        private void LadeReservierungen()
        {
            // SQL-Abfrage: Wir verbinden die Tabellen 'reservierungen' und 'gast',
            // damit wir den Namen des Gastes statt nur seiner ID sehen (JOIN).
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
                // Wir filtern nach dem Datum, das im Kalender (dateTimePicker) ausgewählt ist
                cmd.Parameters.AddWithValue("@datum", dateTimePicker1.Value.Date);

                DataTable dt = new DataTable();
                new MySqlDataAdapter(cmd).Fill(dt);

                // Die Tabelle im Fenster mit den Daten aus der Datenbank füllen
                Reservierung_huaptmenu_dataGridView1.DataSource = dt;
                Reservierung_huaptmenu_dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        // --- BUTTON: STORNIEREN ---
        private void Reservierung_stono_button9_Click(object sender, EventArgs e)
        {
            // 1. Erstmal prüfen, ob der Benutzer überhaupt eine Zeile angeklickt hat
            if (Reservierung_huaptmenu_dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Bitte wählen Sie zuerst eine Reservierung aus der Liste aus!", "Keine Auswahl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 2. Wir holen uns die ID der Reservierung aus der markierten Zeile
                int reservierungs_id = Convert.ToInt32(Reservierung_huaptmenu_dataGridView1.CurrentRow.Cells["reservierungs_id"].Value);

                // 3. Den Status in der Datenbank auf 'storniert' ändern
                string query = "UPDATE reservierungen SET zustand = 'storniert' WHERE reservierungs_id = @id";

                using (MySqlConnection conn = Database.GetConnection())
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", reservierungs_id);
                    cmd.ExecuteNonQuery(); // Befehl ausführen
                }

                // 4. Erfolg melden und die Liste aktualisieren
                MessageBox.Show("Reservierung wurde storniert ✔", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LadeReservierungen();
            }
            catch (Exception ex)
            {
                // Fehlermeldung, falls beim SQL-Befehl etwas schiefgeht
                MessageBox.Show("Fehler beim Stornieren: " + ex.Message);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e) { }

        // Wenn man im Kalender ein anderes Datum wählt, soll die Liste sofort neu laden
        private void hauptmenu_dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            LadeReservierungen();
        }
    }
}