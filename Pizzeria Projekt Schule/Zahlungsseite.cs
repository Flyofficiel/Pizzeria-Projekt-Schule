using MySqlConnector;using System;using System.Collections.Generic;using System.ComponentModel;using System.Data;using System.Drawing;using System.Linq;using System.Text;using System.Threading.Tasks;using System.Windows.Forms;namespace Pizzeria_Projekt_Schule{    public partial class Zahlungsseite : Form    {        int bestellNr;        public Zahlungsseite(int bestellnummer)        {            InitializeComponent();            bestellNr = bestellnummer;        }        double LadeSumme()        {            string sql = @"        SELECT SUM(menge * preis_beim_kauf)        FROM bestellposition        WHERE bestellnr_fk = @bnr";            using (var conn = Database.GetConnection())            using (var cmd = new MySqlCommand(sql, conn))            {                cmd.Parameters.AddWithValue("@bnr", bestellNr);                object result = cmd.ExecuteScalar();                return result == DBNull.Value ? 0 : Convert.ToDouble(result);            }        }
        private void Button1_Click(object sender, EventArgs e)
        {
            if (!radioButton1.Checked && !radioButton2.Checked)
            {
                MessageBox.Show("Bitte Zahlungsart auswählen!");
                return;
            }

            string zahlungsart = radioButton1.Checked ? "Bar" : "Karte";

            double.TryParse(textBox3.Text.Replace(",", "."),
    System.Globalization.NumberStyles.Any,
    System.Globalization.CultureInfo.InvariantCulture,
    out double gesamt);

            double.TryParse(textBox1.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out double trinkgeld);


            using (var conn = Database.GetConnection())
            {
                // 1️⃣ Rechnung speichern
                string sqlRechnung = @"
        INSERT INTO rechnungen
        (bestellnr_fk, gesamtpreis, datum, zahlungsart, trinkgeld)
        VALUES
        (@bestell, @gesamt, NOW(), @art, @tg)";

                using (var cmd = new MySqlCommand(sqlRechnung, conn))
                {
                    cmd.Parameters.AddWithValue("@bestell", bestellNr);
                    cmd.Parameters.AddWithValue("@gesamt", gesamt);
                    cmd.Parameters.AddWithValue("@art", zahlungsart);
                    cmd.Parameters.AddWithValue("@tg", trinkgeld);
                    cmd.ExecuteNonQuery();
                }

                // 2️⃣ Bestellung auf bezahlt setzen
                string updateStatus = @"
        UPDATE bestellungen
        SET status = 'bezahlt'
        WHERE bestellnr = @bnr";

                using (var statusCmd = new MySqlCommand(updateStatus, conn))
                {
                    statusCmd.Parameters.AddWithValue("@bnr", bestellNr);
                    statusCmd.ExecuteNonQuery();
                }

                // 3️⃣ Tisch wieder frei machen
                string frei = @"
        UPDATE tische
        SET lage = 'Frei'
        WHERE tisch_id = (
            SELECT tisch_id_fk FROM bestellungen WHERE bestellnr = @bnr
        )";

                using (var freiCmd = new MySqlCommand(frei, conn))
                {
                    freiCmd.Parameters.AddWithValue("@bnr", bestellNr);
                    freiCmd.ExecuteNonQuery();
                }

               


                MessageBox.Show("Bezahlung abgeschlossen ✅");

                Hauptmenu mainmenupage = new Hauptmenu();
                mainmenupage.Show();
                this.Close();
            }
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)        {        }        private void Zahlung_Load(object sender, EventArgs e)        {

            TischeLaden();            BestellungenLaden();            double summe = LadeSumme();            textBox2.Text = summe.ToString("0.00");            textBox3.Text = summe.ToString("0.00");        }
        private void TischeLaden()
        {
            string query = "SELECT tisch_id FROM tische WHERE lage = 'Besetzt'";

            using (var conn = Database.GetConnection())
            using (var da = new MySqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

                comboBox1.DisplayMember = "tisch_id";
                comboBox1.ValueMember = "tisch_id";
                comboBox1.DataSource = dt;
            }
        }




        private int HoleOffeneBestellung(int tischId)
        {
            string query = @"
        SELECT bestellnr
        FROM bestellungen
        WHERE tisch_id_fk = @tisch
        AND status = 'offen'
        ORDER BY datum DESC
        LIMIT 1;";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@tisch", tischId);

                object result = cmd.ExecuteScalar();

                if (result != null)
                    return Convert.ToInt32(result);
                else
                    return 0;
            }
        }
        private void BestellungenLaden()
        {
            if (comboBox1.SelectedValue == null)
                return;

            int tischId = Convert.ToInt32(comboBox1.SelectedValue);

            // 🔥 Hier holen wir die offene Bestellung
            bestellNr = HoleOffeneBestellung(tischId);

            if (bestellNr == 0)
            {
                dataGridView1.DataSource = null;
                textBox2.Text = "0.00";
                textBox3.Text = "0.00";
                return;
            }

            string query = @"
        SELECT 
            s.speisename,
            bp.menge,
            bp.preis_beim_kauf,
            (bp.menge * bp.preis_beim_kauf) AS gesamtpreis
        FROM bestellposition bp
        JOIN speisen s ON bp.speise_id_fk = s.speise_id
        WHERE bp.bestellnr_fk = @bnr;";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@bnr", bestellNr);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);

                dataGridView1.DataSource = table;
            }

            double summe = LadeSumme();
            textBox2.Text = summe.ToString("0.00");
            textBox3.Text = summe.ToString("0.00");
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)        {            BestellungenLaden();        }        private void DateTimePicker1_ValueChanged(object sender, EventArgs e)        {            BestellungenLaden();        }        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)        {            BestellungenLaden();        }        private void button2_Click(object sender, EventArgs e)        {            Hauptmenu mainmenupage = new Hauptmenu();            mainmenupage.Show();            this.Close();        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(
                textBox2.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out double summe);

            double.TryParse(
                textBox1.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out double tg);

            double gesamt = summe + tg;

            textBox3.Text = gesamt.ToString("0.00");
        }

       
            private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !char.IsDigit(e.KeyChar) &&
                e.KeyChar != ',' &&
                e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
