using MySqlConnector;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Pizzeria_Projekt_Schule
{
    public partial class Umsatzauswertung : Form
    {
        // Der Timer sorgt dafür, dass die Uhrzeit im Label immer aktuell bleibt
        private Timer timerClock;

        public Umsatzauswertung()
        {
            InitializeComponent();
            SetupTimer(); // Startet die Uhr, sobald das Fenster aufgeht
        }

        // Hier wird der Timer eingestellt, damit er jede Sekunde tickt
        private void SetupTimer()
        {
            timerClock = new Timer();
            timerClock.Interval = 1000; // 1000 Millisekunden sind genau eine Sekunde
            timerClock.Tick += (s, e) => UpdateClock();
            timerClock.Start();
            UpdateClock(); // Die Uhrzeit sofort beim ersten Mal anzeigen
        }

        private void UpdateClock()
        {
            // Holt die aktuelle Zeit und formatiert sie passend für Deutschland
            label11.Text = DateTime.Now.ToString("HH:mm:ss dddd, dd.MM.yyyy",
                System.Globalization.CultureInfo.GetCultureInfo("de-DE"));
        }

        private void Auswertung_Load(object sender, EventArgs e)
        {
            // Die Liste für den Zeitraum füllen, damit der Nutzer wählen kann
            Zeitraum_auswahl_comboBox1.Items.Clear();
            Zeitraum_auswahl_comboBox1.Items.AddRange(new object[] { "Heute", "Diese Woche", "Dieser Monat" });
            Zeitraum_auswahl_comboBox1.SelectedIndex = 0; // "Heute" ist am Anfang immer ausgewählt

            // Verhindern, dass man in der Tabelle direkt rumtippen oder Zeilen löschen kann
            Auswertungs_dataGridView2.ReadOnly = true;
            Auswertungs_dataGridView2.AllowUserToAddRows = false;

            LadeAlleDaten(); // Einmal alle Zahlen aus der Datenbank holen
        }

        // Diese Methode ruft nacheinander alle Funktionen auf, die Daten berechnen
        private void LadeAlleDaten()
        {
            try
            {
                LadeUmsatzSumme();
                LadeBeliebtesteSpeise();
                LadeBeliebtesteUhrzeit();
                LadeUmsatzProMitarbeiter(); // Standardmäßig zeigen wir den Umsatz der Mitarbeiter
            }
            catch (Exception ex)
            {
                // Wenn beim Laden der DB-Daten was schiefgeht, zeigen wir den Fehler an
                MessageBox.Show("Fehler beim Laden der Statistiken: " + ex.Message);
            }
        }

        // Hier wird der SQL-Filter erstellt, je nachdem was in der ComboBox gewählt wurde
        private string GetZeitraumFilter()
        {
            if (Zeitraum_auswahl_comboBox1.SelectedItem == null) return "1=1";
            string auswahl = Zeitraum_auswahl_comboBox1.SelectedItem.ToString();

            // Switch-Case schaut nach, welcher Text in der Box steht
            switch (auswahl)
            {
                case "Heute":
                    return "DATE(b.datum) = CURDATE()"; // Nur Datensätze von heute
                case "Diese Woche":
                    return "YEARWEEK(b.datum, 1) = YEARWEEK(CURDATE(), 1)"; // Datensätze der aktuellen Woche
                case "Dieser Monat":
                    return "MONTH(b.datum) = MONTH(CURDATE()) AND YEAR(b.datum) = YEAR(CURDATE())";
                default:
                    return "1=1"; // Falls irgendwas nicht klappt, nehmen wir einfach alles ohne Filter
            }
        }

        // Berechnet den Gesamtumsatz in Euro
        private void LadeUmsatzSumme()
        {
            string filter = GetZeitraumFilter();
            // Wir rechnen Menge mal Preis und summieren alles auf (SUM)
            string query = $@"SELECT IFNULL(SUM(p.menge * p.preis_beim_kauf), 0) 
                             FROM bestellungen b 
                             JOIN bestellposition p ON b.bestellnr = p.bestellnr_fk 
                             WHERE b.status = 'bezahlt' AND {filter}";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                // ExecuteScalar wird benutzt, weil wir nur ein einzelnes Ergebnis (die Summe) brauchen
                decimal umsatz = Convert.ToDecimal(cmd.ExecuteScalar() ?? 0);
                Umsatz_heute_textBox1.Text = umsatz.ToString("N2") + " €";
            }
        }

        // Schaut nach, welche Speise am häufigsten in der Tabelle vorkommt
        private void LadeBeliebtesteSpeise()
        {
            string filter = GetZeitraumFilter();
            // Wir gruppieren nach Speisename und sortieren nach der verkauften Menge
            string query = $@"SELECT s.speisename, SUM(p.menge) AS verkauft
                             FROM speisen s
                             JOIN bestellposition p ON s.speise_id = p.speise_id_fk
                             JOIN bestellungen b ON b.bestellnr = p.bestellnr_fk
                             WHERE b.status = 'bezahlt' AND {filter}
                             GROUP BY s.speisename
                             ORDER BY verkauft DESC LIMIT 1"; // LIMIT 1 gibt uns nur den obersten Platz

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                    beliebteste_speisse_textBox2.Text = $"{reader["speisename"]} ({reader["verkauft"]}x)";
                else
                    beliebteste_speisse_textBox2.Text = "Keine Daten";
            }
        }

        // Analysiert, in welcher Stunde die meisten Bestellungen gemacht wurden
        private void LadeBeliebtesteUhrzeit()
        {
            string filter = GetZeitraumFilter();
            string query = $@"SELECT HOUR(b.datum) AS Stunde, COUNT(*) AS Anzahl
                             FROM bestellungen b
                             WHERE b.status = 'bezahlt' AND {filter}
                             GROUP BY HOUR(b.datum)
                             ORDER BY Anzahl DESC LIMIT 1";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                    Beliebteste_uhrzeit_textBox3.Text = Convert.ToInt32(reader["Stunde"]).ToString("00") + ":00 Uhr";
                else
                    Beliebteste_uhrzeit_textBox3.Text = "Keine Daten";
            }
        }

        // Holt den Umsatz pro Mitarbeiter und zeigt ihn in der Tabelle
        private void LadeUmsatzProMitarbeiter()
        {
            string filter = GetZeitraumFilter();
            string query = $@"SELECT CONCAT(m.vorname, ' ', m.nachname) AS Mitarbeiter,
                             SUM(p.menge * p.preis_beim_kauf) AS Umsatz
                             FROM bestellungen b
                             JOIN bestellposition p ON b.bestellnr = p.bestellnr_fk
                             JOIN mitarbeiter m ON b.personalnr_fk = m.personalnr
                             WHERE b.status = 'bezahlt' AND {filter}
                             GROUP BY m.personalnr ORDER BY Umsatz DESC";
            FillTable(query);
        }

        // Listet den Umsatz für jeden Tisch auf
        private void LadeUmsatzProTisch()
        {
            string filter = GetZeitraumFilter();
            string query = $@"SELECT b.tisch_id_fk AS Tisch, SUM(p.menge * p.preis_beim_kauf) AS Umsatz
                             FROM bestellungen b
                             JOIN bestellposition p ON b.bestellnr = p.bestellnr_fk
                             WHERE b.status = 'bezahlt' AND {filter}
                             GROUP BY b.tisch_id_fk ORDER BY Umsatz DESC";
            FillTable(query);
        }

        // Berechnet den Umsatz für jeden Gast (Laufkunde oder registriert)
        private void LadeUmsatzProGast()
        {
            string filter = GetZeitraumFilter();
            string query = $@"SELECT CASE WHEN g.laufgast = 1 THEN 'Laufkunde' 
                             ELSE CONCAT(g.gastvorname, ' ', g.gastnachname) END AS Gast,
                             SUM(p.menge * p.preis_beim_kauf) AS Umsatz
                             FROM bestellungen b
                             JOIN bestellposition p ON b.bestellnr = p.bestellnr_fk
                             JOIN gast g ON b.gast_id_fk = g.gastid
                             WHERE b.status = 'bezahlt' AND {filter}
                             GROUP BY g.gastid ORDER BY Umsatz DESC";
            FillTable(query);
        }

        // Diese Methode nimmt die SQL-Ergebnisse und packt sie in das DataGridView
        private void FillTable(string query)
        {
            try
            {
                using (var conn = Database.GetConnection())
                using (var da = new MySqlDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt); // Daten in eine Tabelle im Speicher laden
                    Auswertungs_dataGridView2.DataSource = dt; // Tabelle im Fenster anzeigen
                    AuswertungGridDesign(); // Design anpassen
                    Auswertungs_dataGridView2.ClearSelection(); // Keine Zeile automatisch blau markieren
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Füllen der Tabelle: " + ex.Message);
            }
        }

        // Hier stellen wir ein, wie die Tabelle im Programm aussieht
        private void AuswertungGridDesign()
        {
            if (Auswertungs_dataGridView2.Columns.Count == 0) return;

            // Header Schriftart anpassen und die Spaltenbreite automatisch verteilen
            Auswertungs_dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            Auswertungs_dataGridView2.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;
            Auswertungs_dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Auswertungs_dataGridView2.RowHeadersVisible = false;

            // Die Umsatz-Spalte soll wie Geld (Währung) formatiert sein
            if (Auswertungs_dataGridView2.Columns.Contains("Umsatz"))
            {
                Auswertungs_dataGridView2.Columns["Umsatz"].DefaultCellStyle.Format = "C2"; // C2 steht für Currency (Währung)
                Auswertungs_dataGridView2.Columns["Umsatz"].DefaultCellStyle.ForeColor = Color.DarkGreen; // Grün für Geld
                Auswertungs_dataGridView2.Columns["Umsatz"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        // Wenn man auf Auswertung klickt, wird alles aktualisiert
        private void Auswertung_auswertung_button1_Click(object sender, EventArgs e) => LadeAlleDaten();

        // Button um zurück ins Menü zu kommen
        private void Zuruck_button3_Click(object sender, EventArgs e)
        {
            new Hauptmenu().Show();
            this.Close();
        }

        // Die Buttons unten schalten zwischen den verschiedenen Listen um
        private void Umsatz_p_mitarbeiter_button5_Click(object sender, EventArgs e) => LadeUmsatzProMitarbeiter();
        private void Umsatz_p_tisch_button6_Click(object sender, EventArgs e) => LadeUmsatzProTisch();
        private void Umsatz_p_gast_button7_Click(object sender, EventArgs e) => LadeUmsatzProGast();

        // Wenn man den Zeitraum ändert, werden die Zahlen sofort neu berechnet
        private void Zeitraum_auswahl_comboBox1_SelectedIndexChanged(object sender, EventArgs e) => LadeAlleDaten();
    }
}