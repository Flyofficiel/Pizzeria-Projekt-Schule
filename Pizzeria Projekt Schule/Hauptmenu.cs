using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pizzeria_Projekt_Schule;

namespace Pizzeria_Projekt_Schule
{
    // Das ist das Hauptmenü der Anwendung.
    // Von hier aus kann man zu allen anderen Seiten navigieren.
    public partial class Hauptmenu : Form
    {
        public Hauptmenu()
        {
            InitializeComponent();
        }

        // Beim Start des Formulars werden automatisch
        // die heutigen offenen Reservierungen geladen.
        private void Hauptmenu_Load(object sender, EventArgs e)
        {
            LadeReservierungen();
        }

        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }

        // Öffnet das Speisen-Menü
        private void button1_Click(object sender, EventArgs e)
        {
            SpeisenMenu speisenpage = new SpeisenMenu();
            speisenpage.Show();
            this.Close();
        }

        // Öffnet die Bestellungsseite
        private void button2_Click(object sender, EventArgs e)
        {
            Bestellungsseite bestellungspagerichtigpage = new Bestellungsseite();
            bestellungspagerichtigpage.Show();
            this.Close();
        }

        // Öffnet die Reservierungsseite
        private void button3_Click(object sender, EventArgs e)
        {
            Reservierungsseite reservierungpage = new Reservierungsseite();
            reservierungpage.Show();
            this.Close();
        }

        // Öffnet die Tischübersicht
        private void button4_Click(object sender, EventArgs e)
        {
            tischauswahl tischauswahlpage = new tischauswahl();
            tischauswahlpage.Show();
            this.Close();
        }

        // Öffnet die Zahlungsseite
        private void button5_Click(object sender, EventArgs e)
        {
            int bestellnr = 0; // wird später mit echter Bestellnummer gefüllt
            Zahlungsseite zahlungsartpage = new Zahlungsseite(bestellnr);
            zahlungsartpage.Show();
            this.Close();
        }

        // Öffnet die Umsatzauswertung
        private void button7_Click(object sender, EventArgs e)
        {
            Umsatzauswertung auswertungpage = new Umsatzauswertung();
            auswertungpage.Show();
            this.Close();
        }

        // Öffnet das Login-Formular
        private void button8_Click(object sender, EventArgs e)
        {
            Loginpizzeriavesus loginform = new Loginpizzeriavesus();
            loginform.Show();
            this.Close();
        }

        // Öffnet die Mitarbeiterverwaltung
        private void button6_Click(object sender, EventArgs e)
        {
            mitarbeiterverwaltung0 mitarbeiterverwaltungrichtigpage = new mitarbeiterverwaltung0();
            mitarbeiterverwaltungrichtigpage.Show();
            this.Hide();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        // Diese Methode lädt alle offenen Reservierungen für heute
        // und zeigt sie im DataGridView an.
        private void LadeReservierungen()
        {
            string query = @"
    SELECT 
        r.reservierungs_id,
        r.tisch_id_fk AS Tisch,
        DATE(r.datum) AS Datum,
        r.slot AS Slot,
        r.personenanzahl AS Personen,
        CONCAT(g.gastvorname,' ',g.gastnachname) AS Gast,
        g.telephonenr AS Telefon
    FROM reservierungen r
    JOIN gast g ON r.gastid_fk = g.gastid
    WHERE r.zustand = 'offen'
    AND DATE(r.datum) = CURDATE()
    AND NOT EXISTS (
        SELECT 1
        FROM bestellungen b
        WHERE b.tisch_id_fk = r.tisch_id_fk
        AND DATE(b.datum) = DATE(r.datum)
        AND b.status = 'bezahlt'
    )
    ORDER BY r.slot ASC";

            // Verbindung zur Datenbank herstellen
            using (var conn = Database.GetConnection())
            using (var da = new MySqlConnector.MySqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Daten im Grid anzeigen
                dataGridView1.DataSource = dt;

                // Spaltenbreite automatisch anpassen
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }
    }
}
