using MySqlConnector;using System;using System.Collections.Generic;using System.ComponentModel;using System.Data;using System.Drawing;using System.Linq;using System.Text;using System.Threading.Tasks;using System.Windows.Forms;namespace Pizzeria_Projekt_Schule{    public partial class Zahlungsseite : Form    {        int bestellNr;
       

        public Zahlungsseite()
        {
            InitializeComponent();
        }        double LadeSumme()        {            string sql = @"        SELECT SUM(menge * preis_beim_kauf)        FROM bestellposition        WHERE bestellnr_fk = @bnr";            using (var conn = Database.GetConnection())            using (var cmd = new MySqlCommand(sql, conn))            {                cmd.Parameters.AddWithValue("@bnr", bestellNr);                object result = cmd.ExecuteScalar();                return result == DBNull.Value ? 0 : Convert.ToDouble(result);            }        }
        private void Button1_Click(object sender, EventArgs e)
        {
            if (!radioButton1.Checked && !radioButton2.Checked)
            {
                MessageBox.Show("Bitte Zahlungsart auswählen!");
                return;
            }

            string zahlungsart = radioButton1.Checked ? "Bar" : "Karte";

            // Beträge sicher einlesen
            double.TryParse(textBox3.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double gesamt);
            double.TryParse(textBox1.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double trinkgeld);

            if (bestellNr == 0)
            {
                MessageBox.Show("Keine gültige Bestellung ausgewählt!");
                return;
            }

            using (var conn = Database.GetConnection())
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    // 1. Rechnung speichern
                    string sqlRechnung = "INSERT INTO rechnungen (bestellnr_fk, gesamtpreis, datum, zahlungsart, trinkgeld) VALUES (@bestell, @gesamt, NOW(), @art, @tg)";
                    using (var cmd = new MySqlCommand(sqlRechnung, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@bestell", bestellNr);
                        cmd.Parameters.AddWithValue("@gesamt", gesamt);
                        cmd.Parameters.AddWithValue("@art", zahlungsart);
                        cmd.Parameters.AddWithValue("@tg", trinkgeld);
                        cmd.ExecuteNonQuery();
                    }

                    // 2. Bestellung abschließen (Statistik & Tisch-Logik)
                    string updateStatus = "UPDATE bestellungen SET status = 'bezahlt' WHERE bestellnr = @bnr";
                    using (var statusCmd = new MySqlCommand(updateStatus, conn, trans))
                    {
                        statusCmd.Parameters.AddWithValue("@bnr", bestellNr);
                        statusCmd.ExecuteNonQuery();
                    }

                    // 3. Reservierung beenden
                    // WICHTIG: Hier beenden wir die Reservierung, damit der Tisch im System wieder als "Frei" gilt
                    string updateReservierung = @"UPDATE reservierungen SET zustand = 'beendet' 
                                          WHERE tisch_id_fk = (SELECT tisch_id_fk FROM bestellungen WHERE bestellnr = @bnr)
                                          AND (zustand = 'aktiv' OR zustand = 'offen')";
                    using (var resCmd = new MySqlCommand(updateReservierung, conn, trans))
                    {
                        resCmd.Parameters.AddWithValue("@bnr", bestellNr);
                        resCmd.ExecuteNonQuery();
                    }

                    // Hinweis: Den 'UPDATE tische SET aktiv = true' lassen wir weg, 
                    // da 'aktiv' meistens für 'Tisch existiert noch' steht.

                    trans.Commit(); // ✅ Datenbank-Änderungen speichern
                    MessageBox.Show("Bezahlung abgeschlossen ✅. Der Tisch ist nun wieder frei.");

                    // 1. Neues Hauptmenü erstellen und anzeigen
                    Hauptmenu main = new Hauptmenu();
                    main.Show();

                    // 2. Dieses Fenster schließen
                    this.Close();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("Fehler beim Bezahlen: " + ex.Message);

                    // WICHTIG: Wenn ein Fehler passiert, darf Close() NICHT gerufen werden,
                    // damit der User den Fehler lesen kann und nicht ausgeloggt wird.
                }
            }
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)        {        }        private void Zahlung_Load(object sender, EventArgs e)        {

            TischeLaden();            BestellungenLaden();            double summe = LadeSumme();            textBox2.Text = summe.ToString("0.00");            textBox3.Text = summe.ToString("0.00");        }
        private void TischeLaden()
        {
            string query = @"
    SELECT DISTINCT tisch_id_fk AS tisch_id
    FROM bestellungen
    WHERE status = 'offen'";

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

            bestellNr = HoleOffeneBestellung(tischId);

            if (bestellNr == 0)
            {
                dataGridView1.DataSource = null;
                textBox2.Text = "0.00";
                textBox3.Text = "0.00";
                MessageBox.Show("Für diesen Tisch existiert keine offene Bestellung.");
                return;
            }

            string query = @"
SELECT 
    bp.positionid,
    s.speisename AS Speise,
    bp.menge AS Menge,
    bp.preis_beim_kauf AS Einzelpreis,
    (bp.menge * bp.preis_beim_kauf) AS Gesamt
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
                if (dataGridView1.Columns.Contains("positionid"))
                {
                    dataGridView1.Columns["positionid"].Visible = false;
                }

                dataGridView1.DataSource = table;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.Columns["Einzelpreis"].DefaultCellStyle.Format = "C2";
                dataGridView1.Columns["Gesamt"].DefaultCellStyle.Format = "C2";

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

        private void Stornierungbutton_rechnungseite(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Bitte Position auswählen!");
                return;
            }

            int posId = Convert.ToInt32(
    dataGridView1.CurrentRow.Cells["positionid"].Value
);
  

            int menge = Convert.ToInt32(
                dataGridView1.CurrentRow.Cells["Menge"].Value
            );

            DialogResult result = MessageBox.Show(
                "Nur 1 Stück stornieren?",
                "Storno",
                MessageBoxButtons.YesNoCancel
            );

            if (result == DialogResult.Cancel)
                return;

            using (var conn = Database.GetConnection())
            {
                if (result == DialogResult.Yes && menge > 1)
                {
                    // 🔹 Nur 1 Stück abziehen
                    string update = @"
UPDATE bestellposition
SET menge = menge - 1
WHERE positionid = @id";

                    using (var cmd = new MySqlCommand(update, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", posId);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // 🔹 Ganze Position löschen
                    string delete = @"
DELETE FROM bestellposition
WHERE positionid = @id";

                    using (var cmd = new MySqlCommand(delete, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", posId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            MessageBox.Show("Position storniert ✔");

            BestellungenLaden();   // Grid neu laden
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
