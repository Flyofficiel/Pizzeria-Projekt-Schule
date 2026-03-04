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
    public partial class Zahlungsseite : Form
    {
        int bestellNr;

        public Zahlungsseite()
        {
            InitializeComponent();
        }

        // Berechnet die aktuelle Summe der Bestellung aus der Datenbank
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

        private void Button1_Click(object sender, EventArgs e)
        {
            // Validierung: Zahlungsart gewählt?
            if (!Bargeld_zahlen_radioButton1.Checked && !Kartenzahlung_radioButton2.Checked)
            {
                MessageBox.Show("Bitte Zahlungsart auswählen!");
                return;
            }

            string zahlungsart = Bargeld_zahlen_radioButton1.Checked ? "Bar" : "Karte";

            // Beträge parsen (Kommata/Punkte Handling für Internationalisierung)
            double.TryParse(gesamt_Zahlen_textBox3.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double gesamt);
            double.TryParse(Trinkgeld_Zahlen_TextBox1.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double trinkgeld);

            if (bestellNr == 0)
            {
                MessageBox.Show("Keine gültige Bestellung ausgewählt!");
                return;
            }

            // --- TRANSAKTION STARTEN ---
            using (var conn = Database.GetConnection())
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    // 1. Rechnung in DB anlegen
                    string sqlRechnung = "INSERT INTO rechnungen (bestellnr_fk, gesamtpreis, datum, zahlungsart, trinkgeld) VALUES (@bestell, @gesamt, NOW(), @art, @tg)";
                    using (var cmd = new MySqlCommand(sqlRechnung, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@bestell", bestellNr);
                        cmd.Parameters.AddWithValue("@gesamt", gesamt);
                        cmd.Parameters.AddWithValue("@art", zahlungsart);
                        cmd.Parameters.AddWithValue("@tg", trinkgeld);
                        cmd.ExecuteNonQuery();
                    }

                    // 2. Bestellungs-Status auf 'bezahlt' setzen
                    string updateStatus = "UPDATE bestellungen SET status = 'bezahlt' WHERE bestellnr = @bnr";
                    using (var statusCmd = new MySqlCommand(updateStatus, conn, trans))
                    {
                        statusCmd.Parameters.AddWithValue("@bnr", bestellNr);
                        statusCmd.ExecuteNonQuery();
                    }

                    // 3. Tisch-Reservierung beenden (Tisch wird wieder frei für LadeTische())
                    string updateReservierung = @"UPDATE reservierungen SET zustand = 'beendet' 
                                          WHERE tisch_id_fk = (SELECT tisch_id_fk FROM bestellungen WHERE bestellnr = @bnr)
                                          AND (zustand = 'aktiv' OR zustand = 'offen')";
                    using (var resCmd = new MySqlCommand(updateReservierung, conn, trans))
                    {
                        resCmd.Parameters.AddWithValue("@bnr", bestellNr);
                        resCmd.ExecuteNonQuery();
                    }

                    trans.Commit(); // Alles okay? Dann speichern!
                    MessageBox.Show("Bezahlung abgeschlossen ✅. Der Tisch ist nun wieder frei.");

                    new Hauptmenu().Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    trans.Rollback(); // Bei Fehler: Alle Änderungen zurückrollen
                    MessageBox.Show("Fehler beim Bezahlen: " + ex.Message);
                }
            }
        }

        private void Zahlung_Load(object sender, EventArgs e)
        {
            TischeLaden();
            BestellungenLaden();
        }

        // Lädt alle Tische, die gerade eine 'offene' Bestellung haben
        private void TischeLaden()
        {
            string query = "SELECT DISTINCT tisch_id_fk AS tisch_id FROM bestellungen WHERE status = 'offen'";
            using (var conn = Database.GetConnection())
            using (var da = new MySqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                Tisch_zahlenseite_comboBox1.DisplayMember = "tisch_id";
                Tisch_zahlenseite_comboBox1.ValueMember = "tisch_id";
                Tisch_zahlenseite_comboBox1.DataSource = dt;
            }
        }

        private void BestellungenLaden()
        {
            if (Tisch_zahlenseite_comboBox1.SelectedValue == null) return;

            int tischId = Convert.ToInt32(Tisch_zahlenseite_comboBox1.SelectedValue);
            bestellNr = HoleOffeneBestellung(tischId);

            if (bestellNr == 0)
            {
                dataGridView1.DataSource = null;
                Summe_zahlen_textBox2.Text = "0.00";
                gesamt_Zahlen_textBox3.Text = "0.00";
                return;
            }

            string query = @"SELECT bp.positionid, s.speisename AS Speise, bp.menge AS Menge, 
                             bp.preis_beim_kauf AS Einzelpreis, (bp.menge * bp.preis_beim_kauf) AS Gesamt
                             FROM bestellposition bp JOIN speisen s ON bp.speise_id_fk = s.speise_id
                             WHERE bp.bestellnr_fk = @bnr;";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@bnr", bestellNr);
                DataTable table = new DataTable();
                new MySqlDataAdapter(cmd).Fill(table);
                dataGridView1.DataSource = table;

                if (dataGridView1.Columns.Contains("positionid"))
                    dataGridView1.Columns["positionid"].Visible = false;

                dataGridView1.Columns["Einzelpreis"].DefaultCellStyle.Format = "C2";
                dataGridView1.Columns["Gesamt"].DefaultCellStyle.Format = "C2";
            }

            double summe = LadeSumme();
            Summe_zahlen_textBox2.Text = summe.ToString("N2");
            gesamt_Zahlen_textBox3.Text = summe.ToString("N2");
        }

        private int HoleOffeneBestellung(int tischId)
        {
            string query = "SELECT bestellnr FROM bestellungen WHERE tisch_id_fk = @tisch AND status = 'offen' LIMIT 1;";
            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@tisch", tischId);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        
        // Automatisches Berechnen des Gesamtbetrags bei Trinkgeld-Eingabe
        private void Trinkgeld_Zahlen_TextBox1_TextChanged(object sender, EventArgs e)
        {
            // 1. Summe aus TextBox2 sicher einlesen (Inhalt von 'Summe:')
            // Wir nutzen 'CultureInfo.InvariantCulture', damit Punkte immer als Dezimaltrenner erkannt werden
            double.TryParse(
                Summe_zahlen_textBox2.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out double summe);

            // 2. Trinkgeld aus TextBox1 sicher einlesen (Inhalt von 'Trinkgeld:')
            double.TryParse(
                Trinkgeld_Zahlen_TextBox1.Text.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out double tg);

            // 3. Mathematische Addition der beiden Zahlen
            double gesamt = summe + tg;

            // 4. Ergebnis in TextBox3 schreiben (Inhalt von 'Gesamt:')
            // "N2" formatiert die Zahl auf 2 Nachkommastellen (z.B. 54.00)
            gesamt_Zahlen_textBox3.Text = gesamt.ToString("N2", System.Globalization.CultureInfo.InvariantCulture);
        }

        // Stornierungs-Logik: Ermöglicht das Abziehen einzelner Posten vor der Zahlung
        private void Stornierungbutton_rechnungseite(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            int posId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["positionid"].Value);
            int menge = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Menge"].Value);

            DialogResult result = MessageBox.Show("Nur 1 Stück stornieren?", "Storno", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Cancel) return;

            using (var conn = Database.GetConnection())
            {
                string sql = (result == DialogResult.Yes && menge > 1)
                    ? "UPDATE bestellposition SET menge = menge - 1 WHERE positionid = @id"
                    : "DELETE FROM bestellposition WHERE positionid = @id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", posId);
                    cmd.ExecuteNonQuery();
                }
            }
            BestellungenLaden();
        }

        private void Tisch_zahlenseite_comboBox1_SelectedIndexChanged(object sender, EventArgs e) { BestellungenLaden(); }
        private void button2_Click(object sender, EventArgs e) { new Hauptmenu().Show(); this.Close(); }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e) { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != '.') e.Handled = true; }

        private void Zahlenseite_dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void Summe_zahlen_textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void gesamt_Zahlen_textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void Bargeld_zahlen_radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Kartenzahlung_radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}