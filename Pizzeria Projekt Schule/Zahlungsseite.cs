using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Pizzeria_Projekt_Schule
{
    public partial class Zahlungsseite : Form
    {
        private int bestellNr; // Speichert die aktuelle Haupt-Bestellnummer

        public Zahlungsseite()
        {
            InitializeComponent();
        }

        // --- LADEN DER SEITE ---
        private void Zahlung_Load(object sender, EventArgs e)
        {
            // Textfelder auf "Nur Lesen" setzen, da sie nur Ergebnisse anzeigen
            Summe_zahlen_textBox2.ReadOnly = true;
            gesamt_Zahlen_textBox3.ReadOnly = true;
            Summe_zahlen_textBox2.BackColor = Color.WhiteSmoke;
            gesamt_Zahlen_textBox3.BackColor = Color.WhiteSmoke;

            TischeLaden(); // Alle Tische mit offenen Rechnungen holen
        }

        // Holt alle Tische aus der DB, die mindestens eine 'offene' Bestellung haben
        private void TischeLaden()
        {
            string query = "SELECT DISTINCT tisch_id_fk AS tisch_id FROM bestellungen WHERE status = 'offen'";

            using (var conn = Database.GetConnection())
            using (var da = new MySqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Platzhalter-Zeile hinzufügen
                DataRow dr = dt.NewRow();
                dr["tisch_id"] = 0;
                dt.Rows.InsertAt(dr, 0);

                Tisch_zahlenseite_comboBox1.DisplayMember = "tisch_id";
                Tisch_zahlenseite_comboBox1.ValueMember = "tisch_id";
                Tisch_zahlenseite_comboBox1.DataSource = dt;
                Tisch_zahlenseite_comboBox1.SelectedIndex = 0;
            }
        }

        // Lädt alle Speisen eines Tisches in das Grid
        private void BestellungenLaden()
        {
            try
            {
                if (Tisch_zahlenseite_comboBox1.SelectedValue == null ||
                    Tisch_zahlenseite_comboBox1.SelectedValue is DataRowView) return;

                int tischId = Convert.ToInt32(Tisch_zahlenseite_comboBox1.SelectedValue);

                if (tischId == 0)
                {
                    dataGridView1.DataSource = null;
                    Summe_zahlen_textBox2.Text = "0,00";
                    gesamt_Zahlen_textBox3.Text = "0,00";
                    return;
                }

                List<int> alleNummern = HoleAlleOffenenBestellnummern(tischId);

                if (alleNummern.Count == 0)
                {
                    DataTable emptyTable = new DataTable();
                    emptyTable.Columns.Add("Speise");
                    emptyTable.Rows.Add("KEINE OFFENEN BESTELLUNGEN");
                    dataGridView1.DataSource = emptyTable;
                    return;
                }

                bestellNr = alleNummern[0]; // Referenz-ID für die Rechnung
                string filter = string.Join(",", alleNummern);

                string query = $@"SELECT bp.positionid, s.speisename AS Speise, bp.menge AS Menge, 
                                 bp.preis_beim_kauf AS Einzelpreis, (bp.menge * bp.preis_beim_kauf) AS Gesamt
                                 FROM bestellposition bp JOIN speisen s ON bp.speise_id_fk = s.speise_id
                                 WHERE bp.bestellnr_fk IN ({filter});";

                using (var conn = Database.GetConnection())
                using (var cmd = new MySqlCommand(query, conn))
                {
                    DataTable table = new DataTable();
                    new MySqlDataAdapter(cmd).Fill(table);
                    dataGridView1.DataSource = table;
                    ZahlungsGridDesign();
                }

                double summe = LadeSumme(alleNummern);
                Summe_zahlen_textBox2.Text = summe.ToString("N2") + " €";
                gesamt_Zahlen_textBox3.Text = summe.ToString("N2") + " €";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Bestellungen: " + ex.Message);
            }
        }

        // Berechnet die Gesamtsumme über alle offenen Bestellnummern eines Tisches
        private double LadeSumme(List<int> bestellNummern)
        {
            if (bestellNummern.Count == 0) return 0;
            string ids = string.Join(",", bestellNummern);
            string sql = $"SELECT SUM(menge * preis_beim_kauf) FROM bestellposition WHERE bestellnr_fk IN ({ids})";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(sql, conn))
            {
                object result = cmd.ExecuteScalar();
                return result == DBNull.Value ? 0 : Convert.ToDouble(result);
            }
        }

        // --- BEZAHLVORGANG (TRANSAKTION) ---
        private void Button1_Click(object sender, EventArgs e)
        {
            if (!Bargeld_zahlen_radioButton1.Checked && !Kartenzahlung_radioButton2.Checked)
            {
                MessageBox.Show("Bitte Zahlungsart auswählen!");
                return;
            }

            string zahlungsart = Bargeld_zahlen_radioButton1.Checked ? "Bar" : "Karte";

            // Parsen der Beträge (Vermeidung von Komma-Fehlern)
            double.TryParse(Summe_zahlen_textBox2.Text.Replace(" €", "").Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double gesamt);
            double.TryParse(Trinkgeld_Zahlen_TextBox1.Text.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double trinkgeld);

            using (var conn = Database.GetConnection())
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    // 1. Rechnung erstellen
                    string sqlRechnung = "INSERT INTO rechnungen (bestellnr_fk, gesamtpreis, datum, zahlungsart, trinkgeld) VALUES (@bestell, @gesamt, NOW(), @art, @tg)";
                    using (var cmd = new MySqlCommand(sqlRechnung, conn, trans))
                    {
                        cmd.Parameters.AddWithValue("@bestell", bestellNr);
                        cmd.Parameters.AddWithValue("@gesamt", gesamt);
                        cmd.Parameters.AddWithValue("@art", zahlungsart);
                        cmd.Parameters.AddWithValue("@tg", trinkgeld);
                        cmd.ExecuteNonQuery();
                    }

                    // 2. Alle Bestellungen dieses Tisches auf 'bezahlt' setzen
                    int tischId = Convert.ToInt32(Tisch_zahlenseite_comboBox1.SelectedValue);
                    List<int> alleNummern = HoleAlleOffenenBestellnummern(tischId);
                    string idsFilter = string.Join(",", alleNummern);

                    string updateStatus = $"UPDATE bestellungen SET status = 'bezahlt' WHERE bestellnr IN ({idsFilter})";
                    using (var statusCmd = new MySqlCommand(updateStatus, conn, trans))
                    {
                        statusCmd.ExecuteNonQuery();
                    }

                    // 3. Reservierung beenden, damit der Tisch wieder im System frei wird
                    string updateReservierung = @"UPDATE reservierungen SET zustand = 'beendet' 
                                                  WHERE tisch_id_fk = @tid AND (zustand = 'aktiv' OR zustand = 'offen')";
                    using (var resCmd = new MySqlCommand(updateReservierung, conn, trans))
                    {
                        resCmd.Parameters.AddWithValue("@tid", tischId);
                        resCmd.ExecuteNonQuery();
                    }

                    trans.Commit();
                    MessageBox.Show("Zahlung erfolgreich! Der Tisch ist nun wieder für neue Gäste bereit.");
                    Hauptmenu hauptmenu = new Hauptmenu();
                    this.Close();
                    hauptmenu.Show();

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("Fehler bei der Transaktion: " + ex.Message);
                }
            }
        }

        // --- STORNO-LOGIK ---
        private void Stornierungbutton_rechnungseite(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            int posId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["positionid"].Value);
            int menge = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Menge"].Value);

            DialogResult result = MessageBox.Show("Soll 1 Stück storniert werden? (Nein löscht die ganze Position)", "Storno", MessageBoxButtons.YesNoCancel);
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

        private List<int> HoleAlleOffenenBestellnummern(int tischId)
        {
            List<int> nummern = new List<int>();
            string query = "SELECT bestellnr FROM bestellungen WHERE tisch_id_fk = @tisch AND status = 'offen'";
            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@tisch", tischId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) { nummern.Add(reader.GetInt32("bestellnr")); }
                }
            }
            return nummern;
        }

        // Verhindert falsche Eingaben im Trinkgeld-Feld
        private void Trinkgeld_Zahlen_TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ',')) e.Handled = true;
            if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1)) e.Handled = true;
            if (e.KeyChar == '.') { e.KeyChar = ','; e.Handled = false; }
        }

        private void Tisch_zahlenseite_comboBox1_SelectedIndexChanged(object sender, EventArgs e) { BestellungenLaden(); }

        private void Button2_Click(object sender, EventArgs e)
        {
            Hauptmenu hauptmenu = new Hauptmenu();
            hauptmenu.Show();
            this.Close(); 
        }


        // Optische Gestaltung der Tabelle
        private void ZahlungsGridDesign()
        {
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 45);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            if (dataGridView1.Columns.Contains("positionid")) dataGridView1.Columns["positionid"].Visible = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ClearSelection();
        }

        private void Trinkgeld_Zahlen_TextBox1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            // 1. Erlaube Zahlen (0-9) und die Backspace-Taste (Löschen)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true; // Eingabe wird abgelehnt
            }

            // 2. Erlaube das Komma nur EINMAL
            if (e.KeyChar == ',' && (sender as TextBox).Text.Contains(","))
            {
                e.Handled = true; // Zweites Komma wird abgelehnt
            }
        }
    }
}