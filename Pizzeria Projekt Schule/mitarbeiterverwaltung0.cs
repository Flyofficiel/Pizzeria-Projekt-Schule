using MySqlConnector;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Pizzeria_Projekt_Schule
{
    public partial class Mitarbeiterverwaltung0 : Form
    {
        public Mitarbeiterverwaltung0()
        {
            InitializeComponent();

            // Und vergiss nicht das hier, damit man keine Zahlen REINKOPIEREN kann:
            Miarbeiterverwaltung_nachname_textBox4.ShortcutsEnabled = false;
            Miarbeiterverwaltung_name_textBox2.ShortcutsEnabled = false;
        }

        private void Mitarbeiterverwaltungrichtig_Load(object sender, EventArgs e)
        {
            rolle_comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            berreich_comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            MitarbeiterLaden();

            rolle_comboBox2.Items.Clear();
            rolle_comboBox2.Items.AddRange(new string[] { "service", "koch", "kasse", "admin", "management" });

            AktualisiereBereichsSperre();
        }

        private void AktualisiereBereichsSperre()
        {
            string rolle = rolle_comboBox2.Text.ToLower();

            if (rolle == "service")
            {
                berreich_comboBox1.Enabled = true;
                berreich_comboBox1.Items.Clear();
                berreich_comboBox1.Text = "";

                berreich_comboBox1.Items.AddRange(new string[] {
                    "Tische 1-10 (Innen vorne)",
                    "Tische 11-20 (Innen hinten)",
                    "Tische 21-30 (Terrasse)",
                    "Tische 31-40 (VIP)"
                });

                berreich_comboBox1.SelectedIndex = -1;
            }
            else
            {
                berreich_comboBox1.Enabled = false;

                if (rolle == "koch") berreich_comboBox1.Text = "Küche";
                else if (rolle == "kasse") berreich_comboBox1.Text = "Kasse";
                else if (rolle == "management") berreich_comboBox1.Text = "Management";
                else if (rolle == "admin") berreich_comboBox1.Text = "EDV / Admin";
            }
        }

        private void MitarbeiterLaden()
        {
            string query = @"
                SELECT  
                    m.personalnr,
                    m.vorname,
                    m.nachname,
                    m.rolle,
                    m.bereich,
                    COUNT(DISTINCT b.tisch_id_fk) AS Aktive_Tische,
                    COUNT(b.bestellnr) AS Offene_Bestellungen
                FROM mitarbeiter m
                LEFT JOIN bestellungen b 
                    ON m.personalnr = b.personalnr_fk
                    AND b.status = 'offen'
                WHERE m.aktiv = 1
                GROUP BY m.personalnr";

            try
            {
                using (var conn = Database.GetConnection())
                using (var da = new MySqlDataAdapter(query, conn))
                {
                    DataTable table = new DataTable();
                    da.Fill(table);
                    dataGridView1.DataSource = table;

                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.ReadOnly = true;
                    MitarbeiterGridDesign();

                    dataGridView1.ClearSelection();
                    dataGridView1.CurrentCell = null;
                }
            }
            catch (Exception ex) { MessageBox.Show("Fehler beim Laden: " + ex.Message); }
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            var row = dataGridView1.CurrentRow;
            Miarbeiterverwaltung_name_textBox2.Text = row.Cells["vorname"].Value?.ToString();
            Miarbeiterverwaltung_nachname_textBox4.Text = row.Cells["nachname"].Value?.ToString();

            rolle_comboBox2.Text = row.Cells["rolle"].Value?.ToString();
            AktualisiereBereichsSperre();
            berreich_comboBox1.Text = row.Cells["bereich"].Value?.ToString();
        }

        private void MitarbeiterHinzufuegen()
        {
            if (!ValidierungPruefen()) return;

            string vorname = Miarbeiterverwaltung_name_textBox2.Text.Trim();
            string nachname = Miarbeiterverwaltung_nachname_textBox4.Text.Trim();

            if (ExistiertMitarbeiterBereits(vorname, nachname))
            {
                MessageBox.Show($"Der Mitarbeiter '{vorname} {nachname}' ist bereits registriert!",
                                "Warnung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var conn = Database.GetConnection())
            {
                string query = @"INSERT INTO mitarbeiter (vorname, nachname, bereich, passwort, rolle, aktiv)
                                 VALUES (@vorname, @nachname, @bereich, @passwort, @rolle, 1)";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    SetParams(cmd);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Mitarbeiter wurde erfolgreich angelegt! ✔");
            MitarbeiterLaden();
            FelderLeeren();
        }

        private void MitarbeiterUpdate()
        {
            if (dataGridView1.CurrentRow == null) return;

            string query = @"UPDATE mitarbeiter 
                             SET vorname = @vorname, nachname = @nachname, bereich = @bereich, 
                                 passwort = @passwort, rolle = @rolle
                             WHERE personalnr = @personalnr";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                SetParams(cmd);
                cmd.Parameters.AddWithValue("@personalnr", dataGridView1.CurrentRow.Cells["personalnr"].Value);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Daten wurden aktualisiert! ✔");
            MitarbeiterLaden();
        }

        private void MitarbeiterLoeschen()
        {
            if (dataGridView1.CurrentRow == null) return;
            string query = "UPDATE mitarbeiter SET aktiv = 0 WHERE personalnr = @personalnr";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@personalnr", dataGridView1.CurrentRow.Cells["personalnr"].Value);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Mitarbeiter wurde deaktiviert. ✔");
            MitarbeiterLaden();
            FelderLeeren();
        }

        private void SetParams(MySqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@vorname", Miarbeiterverwaltung_name_textBox2.Text);
            cmd.Parameters.AddWithValue("@nachname", Miarbeiterverwaltung_nachname_textBox4.Text);
            cmd.Parameters.AddWithValue("@bereich", berreich_comboBox1.Text);
            cmd.Parameters.AddWithValue("@passwort", Miarbeiterverwaltung_passwort_textBox3.Text);
            cmd.Parameters.AddWithValue("@rolle", rolle_comboBox2.Text);
        }

        private void FelderLeeren()
        {
            Miarbeiterverwaltung_name_textBox2.Clear();
            Miarbeiterverwaltung_passwort_textBox3.Clear();
            Miarbeiterverwaltung_nachname_textBox4.Clear();
            berreich_comboBox1.SelectedIndex = -1;
            rolle_comboBox2.SelectedIndex = -1;
            AktualisiereBereichsSperre();
        }

        private bool ValidierungPruefen()
        {
            if (string.IsNullOrWhiteSpace(Miarbeiterverwaltung_name_textBox2.Text) || Miarbeiterverwaltung_name_textBox2.Text.Length < 2)
            {
                MessageBox.Show("Bitte einen Vornamen eingeben.");
                return false;
            }
            if (rolle_comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Bitte eine Rolle zuweisen.");
                return false;
            }
            if (rolle_comboBox2.Text.ToLower() == "service" && (berreich_comboBox1.SelectedIndex == -1))
            {
                MessageBox.Show("Kellner brauchen einen Tischbereich!");
                return false;
            }
            if (Miarbeiterverwaltung_passwort_textBox3.Text.Length < 4)
            {
                MessageBox.Show("Das Passwort muss mindestens 4 Stellen haben.");
                return false;
            }
            return true;
        }

        private void Mitarbeiter_hinzufugen_button1_Click(object sender, EventArgs e) => MitarbeiterHinzufuegen();
        private void Mitarbeiterverwaltung_speichern_button2_Click(object sender, EventArgs e) => MitarbeiterUpdate();

        private void Abbrechen_button4_Click(object sender, EventArgs e)
        {
            new Hauptmenu().Show();
            this.Close();
        }

        private void ComboBoxRolle_SelectedIndexChanged(object sender, EventArgs e) => AktualisiereBereichsSperre();

        private void Showpassoword_CheckedChanged(object sender, EventArgs e)
        {
            Miarbeiterverwaltung_passwort_textBox3.PasswordChar = showpassoword.Checked ? '\0' : '●';
        }

        private void mitarbeiterhinzufügen_Button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Miarbeiterverwaltung_name_textBox2.Text))
                {
                    MitarbeiterHinzufuegen();
                }
                else
                {
                    FelderLeeren();
                    MessageBox.Show("Bitte gib die Daten für den neuen Mitarbeiter rechts ein.");
                }
            }
            catch (Exception ex) { MessageBox.Show("Fehler: " + ex.Message); }
        }

        private bool ExistiertMitarbeiterBereits(string vorname, string nachname)
        {
            string query = "SELECT COUNT(*) FROM mitarbeiter WHERE vorname = @vorname AND nachname = @nachname AND aktiv = 1";
            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@vorname", vorname);
                cmd.Parameters.AddWithValue("@nachname", nachname);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private void MitarbeiterGridDesign()
        {
            // 1. Grundlegende Optik
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.RowHeadersVisible = false; // Entfernt den leeren Balken ganz links
            dataGridView1.AllowUserToAddRows = false;

            // 2. Schriftarten und Farben (Zebra-Muster)
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dataGridView1.RowTemplate.Height = 35;

            // 3. Spalten-Automatik (Anpassung an Textinhalt)
            // Erstmal alle Spalten so breit wie ihr Inhalt machen
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // 4. Header-Texte verschönern
            if (dataGridView1.Columns.Contains("personalnr"))
                dataGridView1.Columns["personalnr"].HeaderText = "Personal-Nr.";

            if (dataGridView1.Columns.Contains("vorname"))
                dataGridView1.Columns["vorname"].HeaderText = "Vorname";

            if (dataGridView1.Columns.Contains("nachname"))
            {
                dataGridView1.Columns["nachname"].HeaderText = "Nachname";
                // Der Nachname darf den restlichen Platz ausfüllen
                dataGridView1.Columns["nachname"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            if (dataGridView1.Columns.Contains("rolle"))
                dataGridView1.Columns["rolle"].HeaderText = "Position / Rolle";

            if (dataGridView1.Columns.Contains("bereich"))
                dataGridView1.Columns["bereich"].HeaderText = "Zuständig für";

            if (dataGridView1.Columns.Contains("Aktive_Tische"))
                dataGridView1.Columns["Aktive_Tische"].HeaderText = "Tische";

            if (dataGridView1.Columns.Contains("Offene_Bestellungen"))
                dataGridView1.Columns["Offene_Bestellungen"].HeaderText = "Offene Best.";

            // 5. Sicherheit: Passwort niemals in der Tabelle anzeigen
            if (dataGridView1.Columns.Contains("passwort"))
                dataGridView1.Columns["passwort"].Visible = false;
        }

        private void loeschenbutton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;
            string v = dataGridView1.CurrentRow.Cells["vorname"].Value.ToString();
            string n = dataGridView1.CurrentRow.Cells["nachname"].Value.ToString();

            if (MessageBox.Show($"Soll {v} {n} deaktiviert werden?", "Bestätigung", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                MitarbeiterLoeschen();
            }
        }

        private void Miarbeiterverwaltung_name_textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Wir erlauben:
            // 1. Buchstaben (IsLetter)
            // 2. Das Leerzeichen (' ')
            // 3. Die Löschtaste (IsControl)
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != ' ' && !char.IsControl(e.KeyChar))
            {
                // Wenn es keines davon ist (also z.B. eine Zahl), blockieren:
                e.Handled = true;

                // Optional: Ein kurzes "Ding" zur Warnung
                System.Media.SystemSounds.Beep.Play();
            }
        }

        private void Miarbeiterverwaltung_nachname_textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Wir erlauben:
            // 1. Buchstaben (IsLetter)
            // 2. Das Leerzeichen (' ')
            // 3. Die Löschtaste (IsControl)
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != ' ' && !char.IsControl(e.KeyChar))
            {
                // Wenn es keines davon ist (also z.B. eine Zahl), blockieren:
                e.Handled = true;

                // Optional: Ein kurzes "Ding" zur Warnung
                System.Media.SystemSounds.Beep.Play();
            }
        }
    }
}