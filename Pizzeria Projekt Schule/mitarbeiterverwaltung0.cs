using MySqlConnector;
using System;
using System.Data;
using System.Windows.Forms;

namespace Pizzeria_Projekt_Schule
{
    public partial class Mitarbeiterverwaltung0 : Form
    {
        public Mitarbeiterverwaltung0()
        {
            InitializeComponent();
        }

        private void mitarbeiterverwaltungrichtig_Load(object sender, EventArgs e)
        {
            MitarbeiterLaden();

            // Initialisierung der Rollen-Auswahl
            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(new string[] { "service", "koch", "kasse", "admin", "management" });

            // Sicherstellen, dass beim Start alles korrekt gesperrt/eingestellt ist
            AktualisiereBereichsSperre();
        }

        // --- ZENTRALE LOGIK FÜR DIE BEREICHS-SPERRE ---
        private void AktualisiereBereichsSperre()
        {
            // Wir holen die Rolle aus der ComboBox
            string rolle = comboBox2.Text.ToLower();

            if (rolle == "service")
            {
                // Nur beim Service darf man den Tisch-Bereich wählen
                comboBox1.Enabled = true;

                // Falls die Liste leer ist, Tische für Service laden
                if (comboBox1.Items.Count <= 1)
                {
                    comboBox1.Items.Clear();
                    comboBox1.Items.AddRange(new string[] {
                        "Tische 1-10 (Innen vorne)",
                        "Tische 11-20 (Innen hinten)",
                        "Tische 21-30 (Terrasse)",
                        "Tische 31-40 (VIP)"
                    });
                }
            }
            else
            {
                // Bei allen anderen Rollen: Feld sperren (Disabled)
                comboBox1.Enabled = false;

                // Automatisch den passenden Text setzen
                if (rolle == "koch") comboBox1.Text = "Küche";
                else if (rolle == "kasse") comboBox1.Text = "Kasse";
                else if (rolle == "management") comboBox1.Text = "Management";
                else if (rolle == "admin") comboBox1.Text = "EDV / Admin";
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
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            var row = dataGridView1.CurrentRow;
            textBox2.Text = row.Cells["vorname"].Value?.ToString();
            textBox4.Text = row.Cells["nachname"].Value?.ToString();

            // Erst Rolle setzen, dann Sperre aktualisieren, dann Bereich anzeigen
            comboBox2.Text = row.Cells["rolle"].Value?.ToString();
            AktualisiereBereichsSperre();

            comboBox1.Text = row.Cells["bereich"].Value?.ToString();
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
            cmd.Parameters.AddWithValue("@vorname", textBox2.Text);
            cmd.Parameters.AddWithValue("@nachname", textBox4.Text);
            cmd.Parameters.AddWithValue("@bereich", comboBox1.Text);
            cmd.Parameters.AddWithValue("@passwort", textBox3.Text);
            cmd.Parameters.AddWithValue("@rolle", comboBox2.Text);
        }

        private void FelderLeeren()
        {
            textBox2.Clear(); textBox3.Clear(); textBox4.Clear();
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            AktualisiereBereichsSperre();
        }

        private bool ValidierungPruefen()
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text) || string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Vorname und Nachname fehlen!");
                return false;
            }
            return true;
        }

        // --- BUTTON EVENTS ---

        private void button1_Click(object sender, EventArgs e)
        {
            MitarbeiterHinzufuegen();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MitarbeiterUpdate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;
            if (MessageBox.Show("Mitarbeiter wirklich löschen?", "Achtung", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                MitarbeiterLoeschen();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new Hauptmenu().Show();
            this.Close();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            AktualisiereBereichsSperre();
        }

        private void showpassoword_CheckedChanged(object sender, EventArgs e)
        {
            textBox3.PasswordChar = showpassoword.Checked ? '\0' : '●';
        }
    }
}