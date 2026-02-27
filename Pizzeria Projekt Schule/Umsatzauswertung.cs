using System;
using System.Data;
using System.Windows.Forms;
using MySqlConnector;

namespace Pizzeria_Projekt_Schule
{
    public partial class Umsatzauswertung : Form
    {
        public Umsatzauswertung()
        {
            InitializeComponent();
        }

        // 🔥 WICHTIG: exakt so schreiben!
        private void auswertung01_Load(object sender, EventArgs e)
        {
            // Zeitraum ComboBox füllen
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Heute");
            comboBox1.Items.Add("Diese Woche");
            comboBox1.Items.Add("Dieser Monat");
            comboBox1.Items.Add("Alle");

            comboBox1.SelectedIndex = 0; // 🔥 WICHTIG

            LadeUmsatzNachZeitraum();
            LadeBeliebtesteSpeise();
            LadeBeliebtesteUhrzeit();
            LadeUmsatzProMitarbeiter();


            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.ReadOnly = true;
            dataGridView2.AllowUserToAddRows = false;

        }


        // 🔥 Button "Auswertung"
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Bitte Zeitraum auswählen!");
                return;
            }

            LadeUmsatzNachZeitraum();
            LadeBeliebtesteSpeise();
            LadeBeliebtesteUhrzeit();
            LadeUmsatzProMitarbeiter();

        }





        // 🔥 Zurück Button
        private void button3_Click(object sender, EventArgs e)
        {
            Hauptmenu mainmenupage = new Hauptmenu();
            mainmenupage.Show();
            this.Close();
        }

        // 🔥 Label Click (leer lassen)
        private void label1_Click(object sender, EventArgs e)
        {

        }



        private void LadeBeliebtesteSpeise()
        {
            string filter = GetZeitraumFilter();

            string query = $@"
SELECT s.speisename, SUM(p.menge) AS verkauft
FROM speisen s
JOIN bestellposition p ON s.speise_id = p.speise_id_fk
JOIN bestellungen b ON b.bestellnr = p.bestellnr_fk
WHERE b.status = 'bezahlt'
AND {filter}
GROUP BY s.speisename
ORDER BY verkauft DESC
LIMIT 1";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    string name = reader.GetString("speisename");
                    int menge = reader.GetInt32("verkauft");
                    textBox2.Text = name + " (" + menge + "x)";
                }
                else
                {
                    textBox2.Text = "Keine Daten";
                }
            }
        }
        private void LadeBeliebtesteUhrzeit()
        {
            string filter = GetZeitraumFilter();

            string query = $@"
SELECT HOUR(b.datum) AS Stunde, COUNT(*) AS Anzahl
FROM bestellungen b
WHERE b.status = 'bezahlt'
AND {filter}
GROUP BY Stunde
ORDER BY Anzahl DESC
LIMIT 1";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    int stunde = reader.GetInt32("Stunde");
                    labeluhrzeit.Text = "Beliebteste Uhrzeit: " + stunde + ":00 Uhr";
                }
                else
                {
                    labeluhrzeit.Text = "Beliebteste Uhrzeit: Keine Daten";
                }
            }
        }



        private void LadeUmsatzProMitarbeiter()
        {
            string filter = GetZeitraumFilter();

            string query = $@"
SELECT CONCAT(m.vorname,' ',m.nachname) AS Mitarbeiter,
       IFNULL(SUM(p.menge * p.preis_beim_kauf),0) AS Umsatz
FROM mitarbeiter m
LEFT JOIN bestellungen b 
    ON m.personalnr = b.personalnr_fk
LEFT JOIN bestellposition p 
    ON b.bestellnr = p.bestellnr_fk
WHERE b.status = 'bezahlt'
AND {filter}
GROUP BY Mitarbeiter
ORDER BY Umsatz DESC";

            using (var conn = Database.GetConnection())
            using (var da = new MySqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView2.DataSource = dt;
            }
        }
        private void LadeUmsatzProTisch()
        {
            string filter = GetZeitraumFilter();

            string query = $@"
SELECT b.tisch_id_fk AS Tisch,
       IFNULL(SUM(p.menge * p.preis_beim_kauf),0) AS Umsatz
FROM bestellungen b
JOIN bestellposition p ON b.bestellnr = p.bestellnr_fk
WHERE b.status = 'bezahlt'
AND {filter}
GROUP BY Tisch
ORDER BY Umsatz DESC";

            using (var conn = Database.GetConnection())
            using (var da = new MySqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView2.DataSource = dt;
            }
        }
        private void LadeUmsatzProGast()
        {
            string filter = GetZeitraumFilter();

            // Wir starten bei 'bestellungen', um JEDE bezahlte Bestellung zu finden, 
            // egal ob eine Reservierung vorlag oder nicht.
            string query = $@"
SELECT 
    CASE 
        WHEN g.laufgast = 1 THEN 'Laufkunde (unregistriert)'
        ELSE CONCAT(g.gastvorname, ' ', g.gastnachname) 
    END AS Gast,
    IFNULL(SUM(p.menge * p.preis_beim_kauf), 0) AS Umsatz
FROM bestellungen b
JOIN bestellposition p ON b.bestellnr = p.bestellnr_fk
JOIN gast g ON b.gast_id_fk = g.gastid
WHERE b.status = 'bezahlt'
AND {filter}
GROUP BY g.gastid, Gast
ORDER BY Umsatz DESC";

            using (var conn = Database.GetConnection())
            using (var da = new MySqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView2.DataSource = dt;
            }
        }





        private string GetZeitraumFilter()
        {

            if (comboBox1.SelectedItem == null)
                return "1=1";

            string z = comboBox1.SelectedItem.ToString();

            if (z == "Heute")
                return "DATE(b.datum) = CURDATE()";

            if (z == "Diese Woche")
                return "YEARWEEK(b.datum, 1) = YEARWEEK(CURDATE(), 1)";

            if (z == "Dieser Monat")
                return "MONTH(b.datum) = MONTH(CURDATE()) AND YEAR(b.datum) = YEAR(CURDATE())";

            return "1=1";

        }

        private void LadeUmsatzNachZeitraum()
        {
            string filter = GetZeitraumFilter();

            string query = $@"
SELECT IFNULL(SUM(p.menge * p.preis_beim_kauf),0)
FROM bestellungen b
JOIN bestellposition p ON b.bestellnr = p.bestellnr_fk
WHERE b.status = 'bezahlt'
AND {filter}";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                object result = cmd.ExecuteScalar();

                decimal umsatz = 0;

                if (result != null && result != DBNull.Value)
                    umsatz = Convert.ToDecimal(result);

                textBox1.Text = umsatz.ToString("0.00 €");
            }
        }



        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }





        private void button5_Click(object sender, EventArgs e)
        {
            LadeUmsatzProMitarbeiter();
        }


        private void button7_Click(object sender, EventArgs e)
        {
            LadeUmsatzProGast();
        }



        private void button6_Click(object sender, EventArgs e)
        {
            LadeUmsatzProTisch();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.PerformClick();
        }


    }
}


