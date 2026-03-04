using MySqlConnector;
using System;
using System.Data;
using System.Windows.Forms;

namespace Pizzeria_Projekt_Schule
{
    public partial class Umsatzauswertung : Form
    {
        // Timer für die Echtzeit-Uhr im Formular
        private Timer timer1;

        public Umsatzauswertung()
        {
            InitializeComponent();

            // Timer-Einstellungen: Jede Sekunde (1000ms) das Ereignis auslösen
            timer1 = new Timer();
            timer1.Interval = 1000;
            timer1.Tick += Timer1_Tick;
            timer1.Start();

            Timenow(); // Uhrzeit sofort beim Start anzeigen
        }

        // Wird beim Laden der Seite ausgeführt
        private void Auswertung_Load(object sender, EventArgs e)
        {
            // Auswahlmöglichkeiten für den Zeitraum festlegen
            Zeitraum_auswahl_comboBox1.Items.Clear();
            Zeitraum_auswahl_comboBox1.Items.Add("Heute");
            Zeitraum_auswahl_comboBox1.Items.Add("Diese Woche");
            Zeitraum_auswahl_comboBox1.Items.Add("Dieser Monat");

            // Standardmäßig "Heute" auswählen (Index 0)
            Zeitraum_auswahl_comboBox1.SelectedIndex = 0;

            // Grundeinstellungen für die Tabelle (DataGridView)
            Auswertungs_dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Auswertungs_dataGridView2.ReadOnly = true;
            Auswertungs_dataGridView2.AllowUserToAddRows = false;

            // Erste Daten beim Laden abrufen
            LadeAlleDaten();
        }

        // Hilfsmethode, um alle Statistiken auf einmal zu aktualisieren
        private void LadeAlleDaten()
        {
            LadeUmsatzNachZeitraum();
            LadeBeliebtesteSpeise();
            LadeBeliebtesteUhrzeit();
            LadeUmsatzProMitarbeiter();
        }

        // Button: Manuelle Auswertung starten
        private void Auswertung_auswertung_button1_Click(object sender, EventArgs e)
        {
            if (Zeitraum_auswahl_comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Bitte Zeitraum auswählen!");
                return;
            }
            LadeAlleDaten();
        }

        // Button: Zurück zum Hauptmenü
        private void Zuruck_button3_Click(object sender, EventArgs e)
        {
            Hauptmenu mainmenupage = new Hauptmenu();
            mainmenupage.Show();
            this.Close();
        }

        // --- STATISTIK LOGIK ---

        // Findet das meistverkaufte Gericht im gewählten Zeitraum
        private void LadeBeliebtesteSpeise()
        {
            string filter = GetZeitraumFilter();
            string query = $@"
                SELECT s.speisename, SUM(p.menge) AS verkauft
                FROM speisen s
                JOIN bestellposition p ON s.speise_id = p.speise_id_fk
                JOIN bestellungen b ON b.bestellnr = p.bestellnr_fk
                WHERE b.status = 'bezahlt' AND {filter}
                GROUP BY s.speisename
                ORDER BY verkauft DESC LIMIT 1";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    beliebteste_speisse_textBox2.Text = $"{reader.GetString("speisename")} ({reader.GetInt32("verkauft")}x)";
                }
                else beliebteste_speisse_textBox2.Text = "Keine Daten";
            }
        }

        // Analysiert, zu welcher Stunde die meisten Bestellungen eingehen
        private void LadeBeliebtesteUhrzeit()
        {
            string filter = GetZeitraumFilter();
            string query = $@"
                SELECT HOUR(b.datum) AS Stunde, COUNT(*) AS Anzahl
                FROM bestellungen b
                WHERE b.status = 'bezahlt' AND {filter}
                GROUP BY HOUR(b.datum)
                ORDER BY Anzahl DESC LIMIT 1";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    Beliebteste_uhrzeit_textBox3.Text = reader.GetInt32("Stunde").ToString("00") + ":00 Uhr";
                }
                else Beliebteste_uhrzeit_textBox3.Text = "Keine Daten";
            }
        }

        // Listet den Umsatz pro Mitarbeiter in der Tabelle auf
        private void LadeUmsatzProMitarbeiter()
        {
            string filter = GetZeitraumFilter();
            string query = $@"
                SELECT CONCAT(IFNULL(m.vorname, 'Kein Name'), ' ', IFNULL(m.nachname, '')) AS Mitarbeiter,
                       IFNULL(SUM(p.menge * p.preis_beim_kauf), 0) AS Umsatz
                FROM bestellungen b
                JOIN bestellposition p ON b.bestellnr = p.bestellnr_fk
                LEFT JOIN mitarbeiter m ON b.personalnr_fk = m.personalnr
                WHERE b.status = 'bezahlt' AND {filter}
                GROUP BY b.personalnr_fk, m.vorname, m.nachname
                ORDER BY Umsatz DESC";

            FillTable(query);
        }

        // Listet den Umsatz pro Tisch auf
        private void LadeUmsatzProTisch()
        {
            string filter = GetZeitraumFilter();
            string query = $@"
                SELECT b.tisch_id_fk AS Tisch, IFNULL(SUM(p.menge * p.preis_beim_kauf), 0) AS Umsatz
                FROM bestellungen b
                JOIN bestellposition p ON b.bestellnr = p.bestellnr_fk
                WHERE b.status = 'bezahlt' AND {filter}
                GROUP BY Tisch ORDER BY Umsatz DESC";

            FillTable(query);
        }

        // Listet den Umsatz pro Gast auf (Laufkunden vs. Registrierte)
        private void LadeUmsatzProGast()
        {
            string filter = GetZeitraumFilter();
            string query = $@"
                SELECT CASE WHEN g.laufgast = 1 THEN 'Laufkunde' 
                       ELSE CONCAT(g.gastvorname, ' ', g.gastnachname) END AS Gast,
                       IFNULL(SUM(p.menge * p.preis_beim_kauf), 0) AS Umsatz
                FROM bestellungen b
                JOIN bestellposition p ON b.bestellnr = p.bestellnr_fk
                JOIN gast g ON b.gast_id_fk = g.gastid
                WHERE b.status = 'bezahlt' AND {filter}
                GROUP BY g.gastid, Gast ORDER BY Umsatz DESC";

            FillTable(query);
        }

        // Zentrale Methode, um die SQL-Ergebnisse in das Grid zu füllen
        private void FillTable(string query)
        {
            using (var conn = Database.GetConnection())
            using (var da = new MySqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                Auswertungs_dataGridView2.DataSource = dt;
                // Euro-Zeichen in der Umsatz-Spalte formatieren
                if (Auswertungs_dataGridView2.Columns.Contains("Umsatz"))
                    Auswertungs_dataGridView2.Columns["Umsatz"].DefaultCellStyle.Format = "C2";
            }
        }

        // Erzeugt den passenden WHERE-Teil für die SQL-Abfrage
        private string GetZeitraumFilter()
        {
            if (Zeitraum_auswahl_comboBox1.SelectedItem == null) return "1=1";
            string z = Zeitraum_auswahl_comboBox1.SelectedItem.ToString();

            if (z == "Heute") return "DATE(b.datum) = CURDATE()";
            if (z == "Diese Woche") return "YEARWEEK(b.datum, 1) = YEARWEEK(CURDATE(), 1)";
            if (z == "Dieser Monat") return "MONTH(b.datum) = MONTH(CURDATE()) AND YEAR(b.datum) = YEAR(CURDATE())";

            return "1=1";
        }

        // Berechnet den Gesamtumsatz als Zahl für die Haupt-Anzeige
        private void LadeUmsatzNachZeitraum()
        {
            string filter = GetZeitraumFilter();
            string query = $@"
                SELECT IFNULL(SUM(p.menge * p.preis_beim_kauf), 0)
                FROM bestellungen b
                JOIN bestellposition p ON b.bestellnr = p.bestellnr_fk
                WHERE b.status = 'bezahlt' AND {filter}";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                decimal umsatz = Convert.ToDecimal(cmd.ExecuteScalar() ?? 0);
                Umsatz_heute_textBox1.Text = umsatz.ToString("N2") + " €";
            }
        }

        // --- UI EVENTS ---

        private void Umsatz_p_mitarbeiter_button5_Click(object sender, EventArgs e) => LadeUmsatzProMitarbeiter();
        private void Umsatz_p_tisch_button6_Click(object sender, EventArgs e) => LadeUmsatzProTisch();
        private void Umsatz_p_gast_button7_Click(object sender, EventArgs e) => LadeUmsatzProGast();

        // Wenn der Zeitraum geändert wird, alles neu laden
        private void Zeitraum_auswahl_comboBox1_SelectedIndexChanged(object sender, EventArgs e) => LadeAlleDaten();

        // Timer-Ereignis für die Uhr
        private void Timer1_Tick(object sender, EventArgs e) => Timenow();

        private void Timenow()
        {
            // Deutsche Formatierung für Datum und Uhrzeit
            label11.Text = DateTime.Now.ToString("HH:mm:ss dddd, dd.MM.yyyy",
                System.Globalization.CultureInfo.GetCultureInfo("de-DE"));
        }

        private void Label1_Click(object sender, EventArgs e) { }
        private void Auswertungs_dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void TextBox5_TextChanged(object sender, EventArgs e) { }

        private void Zeitraum_auswahl_comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void Umsatz_heute_textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Beliebteste_speisse_textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Beliebteste_uhrzeit_textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}