using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pizzeria_Projekt_Schule
{
    public partial class Reservierungsseite : Form
    {
        public Reservierungsseite()
        {
            InitializeComponent();
            StammgastLaden(); // Lädt die Liste der bereits bekannten Gäste
                              // Verhindert Strg+V, Strg+C und das Standard-Rechtsklick-Menü
            Name_textBox1.ShortcutsEnabled = false;
            Telefon_textBox2.ShortcutsEnabled = false;
            
        }
           
        

        private void Abbrechen_button2_Click(object sender, EventArgs e)
        {
            Hauptmenu mainmenupage = new Hauptmenu();
            mainmenupage.Show();
            this.Close();
        }

        //  TISCHE FILTERN 
        // Lädt alle Tische, die für die Personenanzahl groß genug und im Slot frei sind
        void LadeTische(int personen)
        {
            if (Uhrzeit_comboBox1.SelectedItem == null) return;

            int slot = HoleSlot();
            if (slot == 0) return;

            // Komplexe SQL-Abfrage: Prüft den Status (Frei, Reserviert, Besetzt, Aktiv) pro Tisch
            string sql = @"
    SELECT 
        t.tisch_id,
        t.bereich,
        t.max_personen,
        CASE
            WHEN EXISTS (
                SELECT 1 FROM bestellungen b
                WHERE b.tisch_id_fk = t.tisch_id AND b.slot = @slot AND DATE(b.datum) = @datum AND b.status = 'offen'
            ) THEN 'Besetzt'
            WHEN EXISTS (
                SELECT 1 FROM reservierungen r
                WHERE r.tisch_id_fk = t.tisch_id AND r.slot = @slot AND DATE(r.datum) = @datum AND r.zustand = 'aktiv' 
            ) THEN 'Aktiv'
            WHEN EXISTS (
                SELECT 1 FROM reservierungen r
                WHERE r.tisch_id_fk = t.tisch_id AND r.slot = @slot AND DATE(r.datum) = @datum AND r.zustand = 'offen'
            ) THEN 'Reserviert'
            ELSE 'Frei'
        END AS status
    FROM tische t
    WHERE t.aktiv = true
    AND t.max_personen >= @personen
    ORDER BY t.max_personen ASC";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@personen", personen);
                cmd.Parameters.AddWithValue("@datum", dateTimePicker1.Value.Date);
                cmd.Parameters.AddWithValue("@slot", slot);

                DataTable dt = new DataTable();
                new MySqlDataAdapter(cmd).Fill(dt);

                // Hilfsspalte für die Anzeige in der ComboBox erstellen
                if (!dt.Columns.Contains("Anzeige"))
                    dt.Columns.Add("Anzeige", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    row["Anzeige"] = $"Tisch {row["tisch_id"]} - {row["bereich"]} ({row["max_personen"]} Pers.) - {row["status"]}";
                }

                Tischauswahl_comboBox2.DataSource = null;

                if (dt.Rows.Count > 0)
                {
                    Tischauswahl_comboBox2.DisplayMember = "Anzeige";
                    Tischauswahl_comboBox2.ValueMember = "tisch_id";
                    Tischauswahl_comboBox2.DataSource = dt;
                    Tischauswahl_comboBox2.SelectedIndex = 0;
                }
                else
                {
                    Tischauswahl_comboBox2.Items.Clear();
                    MessageBox.Show("Kein passender Tisch verfügbar ❌");
                }
            }
        }

        private void AktualisiereTischeAuto()
        {
            if (nureservierung_personenzahl_numericUpDown1.Value > 0 && Uhrzeit_comboBox1.SelectedItem != null)
            {
                LadeTische((int)nureservierung_personenzahl_numericUpDown1.Value);
            }
        }

        // RESERVIERUNG SPEICHERN 
        private void Reservierungspeichern_Button(object sender, EventArgs e)
        {
            try
            {
                string name = Name_textBox1.Text.Trim();
                string telefon = Telefon_textBox2.Text.Trim();

                // Validierung: Datum
                if (dateTimePicker1.Value.Date < DateTime.Today)
                {
                    MessageBox.Show("Reservierung in der Vergangenheit nicht möglich!");
                    return;
                }

                // Validierung: Uhrzeit/Slots bei Reservierungen für den heutigen Tag
                if (dateTimePicker1.Value.Date == DateTime.Today)
                {
                    int gewaehlterSlot = HoleSlot();
                    int aktuelleStunde = DateTime.Now.Hour;
                    int aktuellerSlot = 0;

                    // Bestimmung des aktuellen Zeitfensters
                    if (aktuelleStunde < 15) aktuellerSlot = 1;
                    else if (aktuelleStunde < 18) aktuellerSlot = 2;
                    else if (aktuelleStunde < 21) aktuellerSlot = 3;
                    else if (aktuelleStunde < 24) aktuellerSlot = 4;

                    if (gewaehlterSlot < aktuellerSlot && aktuellerSlot != 0)
                    {
                        MessageBox.Show("Dieser Zeitraum ist für heute bereits abgelaufen!");
                        return;
                    }
                }

                // Pflichtfelder prüfen
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(telefon) || Tischauswahl_comboBox2.SelectedItem == null)
                {
                    MessageBox.Show("Bitte alle Pflichtfelder ausfüllen!");
                    return;
                }

                // NEU: Prüfung auf Mindestlänge der Telefonnummer
                if (telefon.Length < 8)
                {
                    MessageBox.Show("Die Telefonnummer muss mindestens 8 Ziffern lang sein! 📞");
                    return;
                }

                string connString = "server=localhost;uid=root;pwd=root;database=pizzaprojekt";

                using (var conn = new MySqlConnection(connString))
                {
                    conn.Open();

                    // 1. Gast-Logik: Existiert die Telefonnummer schon?
                    int gastId;
                    string checkGast = "SELECT gastid FROM gast WHERE telephonenr = @telefon LIMIT 1";

                    using (var checkCmd = new MySqlCommand(checkGast, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@telefon", telefon);
                        var result = checkCmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            gastId = Convert.ToInt32(result); // Stammgast gefunden
                        }
                        else
                        {
                            // Neuen Gast anlegen
                            string gastSql = @"INSERT INTO gast (gastvorname, gastnachname, telephonenr)
                                              VALUES (@vorname, @nachname, @telefon);
                                              SELECT LAST_INSERT_ID();";

                            using (var gastCmd = new MySqlCommand(gastSql, conn))
                            {
                                string[] teile = name.Split(' ');
                                string vorname = teile[0];
                                string nachname = teile.Length > 1 ? teile[1] : "";

                                gastCmd.Parameters.AddWithValue("@vorname", vorname);
                                gastCmd.Parameters.AddWithValue("@nachname", nachname);
                                gastCmd.Parameters.AddWithValue("@telefon", telefon);
                                gastId = Convert.ToInt32(gastCmd.ExecuteScalar());
                            }
                        }
                    }

                    // 2. Reservierung in DB schreiben
                    string resSql = @"INSERT INTO reservierungen (tisch_id_fk, slot, datum, personenanzahl, gastid_fk, zustand)
                                      VALUES (@tisch, @slot, @datum, @personen, @gastid, 'offen')";

                    using (var resCmd = new MySqlCommand(resSql, conn))
                    {
                        resCmd.Parameters.AddWithValue("@tisch", Tischauswahl_comboBox2.SelectedValue);
                        resCmd.Parameters.AddWithValue("@slot", HoleSlot());
                        resCmd.Parameters.AddWithValue("@datum", dateTimePicker1.Value.Date);
                        resCmd.Parameters.AddWithValue("@personen", nureservierung_personenzahl_numericUpDown1.Value);
                        resCmd.Parameters.AddWithValue("@gastid", gastId);

                        resCmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Reservierung erfolgreich gespeichert ✅");
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062) // UNIQUE Constraint (Tisch, Datum, Slot darf nur 1x existieren)
                    MessageBox.Show("Dieser Tisch ist leider schon belegt!");
                else
                    MessageBox.Show("Fehler: " + ex.Message);
            }
        }

        private void Reservierung_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = DateTime.Now;
            Uhrzeit_comboBox1.Items.Clear();
            Uhrzeit_comboBox1.Items.AddRange(new string[] { "12-15", "15-18", "18-21", "21-24" });
            Uhrzeit_comboBox1.SelectedIndex = 0;

            Tischauswahl_comboBox2.DrawMode = DrawMode.OwnerDrawFixed;
            Tischauswahl_comboBox2.DrawItem += Tischauswahl_comboBox2_DrawItem;
            AktualisiereTischeAuto();
        }

        private void Reservierung_personenzahl_numericUpDown1_ValueChanged(object sender, EventArgs e) { AktualisiereTischeAuto(); }

        private int HoleSlot()
        {
            if (Uhrzeit_comboBox1.SelectedItem == null) return 0;
            return Uhrzeit_comboBox1.SelectedIndex + 1;
        }

        private void Reservierung_dateTimePicker1_ValueChanged(object sender, EventArgs e) { AktualisiereTischeAuto(); }
        private void Uhrzeit_comboBox1_SelectedIndexChanged(object sender, EventArgs e) { AktualisiereTischeAuto(); }

        // --- VALIDIERUNG DER EINGABE ---
        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void Name_textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;
                System.Media.SystemSounds.Beep.Play();
            }
        }

        //  FARBIGE ANZEIGE DER TISCHE 
        private void Tischauswahl_comboBox2_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            e.DrawBackground();

            if (Tischauswahl_comboBox2.Items[e.Index] is DataRowView row)
            {
                string status = row["status"].ToString().ToLower();
                Color farbe = Color.Black;

                switch (status)
                {
                    case "frei": farbe = Color.Green; break;
                    case "reserviert": farbe = Color.Orange; break;
                    case "besetzt": farbe = Color.Red; break;
                    case "aktiv": farbe = Color.MediumPurple; break;
                }

                using (Brush brush = new SolidBrush(farbe))
                {
                    e.Graphics.DrawString(row["Anzeige"].ToString(), e.Font, brush, e.Bounds.Left, e.Bounds.Top);
                }
            }
            e.DrawFocusRectangle();
        }

        // --- STAMMGAST-FUNKTION 
        private void StammgastLaden()
        {
            using (MySqlConnection conn = Database.GetConnection())
            {
                string query = "SELECT gastvorname, gastnachname, telephonenr FROM gast";
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                stammgaste_dataGridView1.DataSource = table;
            }
        }

        private void Guestuebernehmen_Click(object sender, EventArgs e)
        {
            if (stammgaste_dataGridView1.CurrentRow != null)
            {
                var row = stammgaste_dataGridView1.CurrentRow;
                Name_textBox1.Text = $"{row.Cells["gastvorname"].Value} {row.Cells["gastnachname"].Value}";
                Telefon_textBox2.Text = row.Cells["telephonenr"].Value.ToString();
            }
        }

        private void Name_textBox1_TextChanged(object sender, EventArgs e) { }
        private void Panel2_Paint(object sender, PaintEventArgs e) { }
        private void TabPage2_Click(object sender, EventArgs e) { }
        private void TabPage1_Click(object sender, EventArgs e) { }

        private void Tischauswahl_comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

       
    }
}