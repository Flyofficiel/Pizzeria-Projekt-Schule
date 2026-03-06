using MySqlConnector;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Pizzeria_Projekt_Schule
{
    public partial class Mitarbeiterverwaltung0 : Form
    {
        public Mitarbeiterverwaltung0()
        {
            InitializeComponent();
        }

        private void Mitarbeiterverwaltungrichtig_Load(object sender, EventArgs e)
        {
            MitarbeiterLaden();

            // Initialisierung der Rollen-Auswahl
            rolle_comboBox2.Items.Clear();
            rolle_comboBox2.Items.AddRange(new string[] { "service", "koch", "kasse", "admin", "management" });

            // Sicherstellen, dass beim Start alles korrekt gesperrt/eingestellt ist
            AktualisiereBereichsSperre();
        }

        // --- ZENTRALE LOGIK FÜR DIE BEREICHS-SPERRE ---
        private void AktualisiereBereichsSperre()
        {
            // Wir holen die Rolle aus der ComboBox
            string rolle = rolle_comboBox2.Text.ToLower();

            if (rolle == "service")
            {
                berreich_comboBox1.Enabled = true;

                // ALLES komplett zurücksetzen
                berreich_comboBox1.Items.Clear();
                berreich_comboBox1.Text = "";

                // Nur Tische hinzufügen
                berreich_comboBox1.Items.AddRange(new string[] {
        "Tische 1-10 (Innen vorne)",
        "Tische 11-20 (Innen hinten)",
        "Tische 21-30 (Terrasse)",
        "Tische 31-40 (VIP)"
    });

                berreich_comboBox1.SelectedIndex = -1; // nichts automatisch auswählen
            }
            else
            {
                // Bei allen anderen Rollen: Feld sperren (Disabled)
                berreich_comboBox1.Enabled = false;

                // Automatisch den passenden Text setzen
                if (rolle == "koch") berreich_comboBox1.Text = "Küche";
                else if (rolle == "kasse") berreich_comboBox1.Text = "Kasse";
                else if (rolle == "management") berreich_comboBox1.Text = "Management";
                else if (rolle == "admin") berreich_comboBox1.Text = "EDV / Admin";
            }
        }

        // --- DATEN LADEN & ANZEIGEN ---
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

            using (var conn = Database.GetConnection())
            using (var da = new MySqlDataAdapter(query, conn))
            {
                DataTable table = new DataTable();
                da.Fill(table);
                dataGridView1.DataSource = table;

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.ReadOnly = true;
                MitarbeiterGridDesign();
            }
        }

        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            var row = dataGridView1.CurrentRow;
            Miarbeiterverwaltung_name_textBox2.Text = row.Cells["vorname"].Value?.ToString();
            Miarbeiterverwaltung_nachname_textBox4.Text = row.Cells["nachname"].Value?.ToString();

            // Erst Rolle setzen, dann Sperre aktualisieren, dann Bereich anzeigen
            rolle_comboBox2.Text = row.Cells["rolle"].Value?.ToString();
            AktualisiereBereichsSperre();

            berreich_comboBox1.Text = row.Cells["bereich"].Value?.ToString();
        }

        // --- CRUD OPERATIONEN ---

        private void MitarbeiterHinzufuegen()
        {
            if (!ValidierungPruefen()) return;

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

            MessageBox.Show("Mitarbeiter hinzugefügt ✔");
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

            MessageBox.Show("Mitarbeiter aktualisiert ✔");
            MitarbeiterLaden();
        }

        private void MitarbeiterLoeschen()
        {
            string query = "UPDATE mitarbeiter SET aktiv = 0 WHERE personalnr = @personalnr";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@personalnr", dataGridView1.CurrentRow.Cells["personalnr"].Value);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Mitarbeiter deaktiviert ✔");
            MitarbeiterLaden();
            FelderLeeren();
        }

        // --- HILFSMETHODEN ---

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
            Miarbeiterverwaltung_name_textBox2.Clear(); Miarbeiterverwaltung_passwort_textBox3.Clear(); Miarbeiterverwaltung_nachname_textBox4.Clear();
            berreich_comboBox1.SelectedIndex = -1;
            rolle_comboBox2.SelectedIndex = -1;
            AktualisiereBereichsSperre();
        }

        private bool ValidierungPruefen()
        {
            if (string.IsNullOrWhiteSpace(Miarbeiterverwaltung_name_textBox2.Text) || string.IsNullOrWhiteSpace(Miarbeiterverwaltung_nachname_textBox4.Text))
            {
                MessageBox.Show("Vorname und Nachname fehlen!");
                return false;
            }
            return true;
        }

        // --- BUTTON EVENTS ---

        private void Mitarbeiter_hinzufugen_button1_Click(object sender, EventArgs e)
        {
            MitarbeiterHinzufuegen();
        }

        private void Mitarbeiterverwaltung_speichern_button2_Click(object sender, EventArgs e)
        {
            MitarbeiterUpdate();
        }

        private void Loeschen_button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;
            if (MessageBox.Show("Mitarbeiter wirklich löschen?", "Achtung", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                MitarbeiterLoeschen();
            }
        }

        private void Abbrechen_button4_Click(object sender, EventArgs e)
        {
            new Hauptmenu().Show();
            this.Close();
        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            AktualisiereBereichsSperre();
        }

        private void Showpassoword_CheckedChanged(object sender, EventArgs e)
        {
            Miarbeiterverwaltung_passwort_textBox3.PasswordChar = showpassoword.Checked ? '\0' : '●';
        }

        private void Mitarbeiterverwaltung_dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void mitarbeiterhinzufügen_Button1_Click_1(object sender, EventArgs e)
        {
            try
            {


                // 1. Zuerst alle Felder sauber leeren


                // 2. Die Auswahl im DataGridView komplett aufheben. 
                // Das ist entscheidend, damit MitarbeiterUpdate() nicht aus Versehen aufgerufen wird.
                /*dataGridView1.ClearSelection();
                if (dataGridView1.CurrentRow != null)
                {
                    dataGridView1.CurrentCell = null;
                }*/
                MitarbeiterHinzufuegen();
                // 3. Den Cursor in das Vorname-Feld setzen
                Miarbeiterverwaltung_name_textBox2.Focus();

              
                
            }
            catch(Exception ex)
            {
                MessageBox.Show("error" + ex);
            }
        }

        private void Miarbeiterverwaltung_name_textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Miarbeiterverwaltung_nachname_textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void Miarbeiterverwaltung_passwort_textBox3_TextChanged(object sender, EventArgs e)
        {

        }
        private void MitarbeiterGridDesign()
        {
            // Überschriften: Segoe UI, Größe 12, Fett
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnHeadersHeight = 45;

            // Zeilen: Segoe UI, Größe 11
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            dataGridView1.RowTemplate.Height = 35;

            // Hintergrundfarbe für jede zweite Zeile (bessere Lesbarkeit)
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(235, 235, 235);

            // Spaltenüberschriften umbenennen
            if (dataGridView1.Columns.Contains("personalnr"))
                dataGridView1.Columns["personalnr"].HeaderText = "ID";

            if (dataGridView1.Columns.Contains("vorname"))
                dataGridView1.Columns["vorname"].HeaderText = "Vorname";

            if (dataGridView1.Columns.Contains("nachname"))
                dataGridView1.Columns["nachname"].HeaderText = "Nachname";

            if (dataGridView1.Columns.Contains("rolle"))
                dataGridView1.Columns["rolle"].HeaderText = "Rolle";

            if (dataGridView1.Columns.Contains("bereich"))
                dataGridView1.Columns["bereich"].HeaderText = "Bereich";

            if (dataGridView1.Columns.Contains("Aktive_Tische"))
                dataGridView1.Columns["Aktive_Tische"].HeaderText = "Tische";

            if (dataGridView1.Columns.Contains("Offene_Bestellungen"))
                dataGridView1.Columns["Offene_Bestellungen"].HeaderText = "Offen";
        }
    }
}