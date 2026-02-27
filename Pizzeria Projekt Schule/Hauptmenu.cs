using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySqlConnector;
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
            Zahlungsseite zahlungsartpage = new Zahlungsseite();
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
            Mitarbeiterverwaltung0 mitarbeiterverwaltungrichtigpage = new Mitarbeiterverwaltung0();
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
                cmd.Parameters.AddWithValue("@datum", dateTimePicker1.Value.Date);

                DataTable dt = new DataTable();
                new MySqlDataAdapter(cmd).Fill(dt);

                dataGridView1.DataSource = dt;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            int reservierungs_id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["reservierungs_id"].Value);
            int Tisch = Convert.ToInt32(dataGridView1.CurrentRow.Cells["tisch"].Value);
            DateTime datum = Convert.ToDateTime(dataGridView1.CurrentRow.Cells["Datum"].Value);
            string slot = dataGridView1.CurrentRow.Cells["Slot"].Value.ToString();
            int personen = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Personen"].Value);
            string gast = dataGridView1.CurrentRow.Cells["Gast"].Value.ToString(); 
            long telephonenr = Convert.ToInt64(dataGridView1.CurrentRow.Cells["Telefon"].Value);
            string query = @"
UPDATE reservierungen
SET zustand = 'storniert'
WHERE reservierungs_id = @id
";

            MySqlConnection conn = Database.GetConnection();

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", reservierungs_id);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Reservierung wurde storniert ✔");
            LadeReservierungen(); // deine Reload Methode
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            LadeReservierungen();
        }
    }
}
