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
    public partial class Zahlung : Form
    {
        int bestellNr;
        public Zahlung(int bestellnummer)
        {
            InitializeComponent();
            bestellNr = bestellnummer;
        }
        double LadeSumme()
        {
            string sql = @"
        SELECT SUM(menge * preis_beim_kauf)
        FROM bestellposition
        WHERE bestellnr_fk = @bnr";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@bnr", bestellNr);
                object result = cmd.ExecuteScalar();
                return result == DBNull.Value ? 0 : Convert.ToDouble(result);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (!radioButton1.Checked && !radioButton2.Checked)
            {
                MessageBox.Show("Bitte Zahlungsart auswählen!");
                return;
            }

            string zahlungsart = radioButton1.Checked ? "Bar" : "Karte";

            double gesamt = double.Parse(textBox3.Text);
            double trinkgeld = string.IsNullOrWhiteSpace(textBox1.Text)
                ? 0
                : double.Parse(textBox1.Text);

            // Rechnung speichern
            string sql = @"
        INSERT INTO rechnungen
        (bestellnr_fk, gesamtpreis, datum, zahlungsart, trinkgeld)
        VALUES
        (@bestell, @gesamt, NOW(), @art, @tg)";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@bestell", bestellNr);
                cmd.Parameters.AddWithValue("@gesamt", gesamt);
                cmd.Parameters.AddWithValue("@art", zahlungsart);
                cmd.Parameters.AddWithValue("@tg", trinkgeld);
                cmd.ExecuteNonQuery();
            }

            // Tisch wieder frei machen
            string frei = @"
        UPDATE tische
        SET lage = 'Frei'
        WHERE tisch_id = (
            SELECT tisch_id_fk FROM bestellungen WHERE bestellnr = @bnr
        )";

            using (var cmd = new MySqlCommand(frei, Database.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@bnr", bestellNr);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Bezahlung abgeschlossen ✅");
            Hauptmenu mainmenupage = new Hauptmenu();
            mainmenupage.Show();
            this.Close();


        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Zahlung_Load(object sender, EventArgs e)
        {
            BestellungenLaden();
            double summe = LadeSumme();
            textBox2.Text = summe.ToString("0.00");
            textBox3.Text = summe.ToString("0.00");


        }
        private void BestellungenLaden()
        {
            if (comboBox1.SelectedValue == null)
                return;

            string connString = "server=localhost;uid=root;pwd=root;database=pizzaprojekt";

            string query = @"
        SELECT 
            b.bestellnr,
            s.speisename,
            bp.menge,
            bp.preis_beim_kauf,
            (bp.menge * bp.preis_beim_kauf) AS gesamtpreis
        FROM bestellungen b
        JOIN bestellposition bp ON b.bestellnr = bp.bestellnr_fk
        JOIN speisen s ON bp.speise_id_fk = s.speise_id
        JOIN reservierungen r 
            ON r.tisch_id_fk = b.tisch_id_fk 
           AND DATE(r.datum) = DATE(b.datum)
        WHERE 
            b.tisch_id_fk = @tisch
            AND DATE(b.datum) = @datum;
    ";

            MySqlConnection conn = Database.GetConnection();
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@tisch", comboBox1.SelectedValue);
                cmd.Parameters.AddWithValue("@datum", dateTimePicker1.Value.Date);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);

                dataGridView1.DataSource = table;
            }

            // 💶 Preis formatieren
            dataGridView1.Columns["preis_beim_kauf"].DefaultCellStyle.Format = "C2";
            dataGridView1.Columns["gesamtpreis"].DefaultCellStyle.Format = "C2";
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            BestellungenLaden();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

            BestellungenLaden();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            BestellungenLaden();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hauptmenu mainmenupage = new Hauptmenu();
            mainmenupage.Show();
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            double summe = double.Parse(textBox2.Text);

            if (!double.TryParse(textBox1.Text.Replace(",", "."), out double tg))
                tg = 0;

            textBox3.Text = (summe + tg).ToString("0.00");
        }
    }
}