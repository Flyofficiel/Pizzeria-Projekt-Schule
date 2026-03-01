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
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

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

            //  BEREICH ENUM WERTE
            comboBox1.Items.Clear();




            //  ROLLE ENUM WERTE
            comboBox2.Items.Clear();
            comboBox2.Items.Add("service");
            comboBox2.Items.Add("koch");
            comboBox2.Items.Add("kasse");
            comboBox2.Items.Add("admin");
            comboBox2.Items.Add("management");
        }



        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
                return;

            var row = dataGridView1.CurrentRow;

            textBox2.Text = row.Cells["vorname"].Value?.ToString();
            textBox4.Text = row.Cells["nachname"].Value?.ToString();
            comboBox2.Text = row.Cells["rolle"].Value?.ToString();
            comboBox1.Text = row.Cells["bereich"].Value?.ToString();
        }


        private void MitarbeiterLoeschen()
        {


            string query = @"
            UPDATE mitarbeiter
            SET aktiv = 0
            WHERE personalnr = @personalnr
            ";

            MySqlConnection conn = Database.GetConnection();
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@personalnr",
                    dataGridView1.CurrentRow.Cells["personalnr"].Value
                );


                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Mitarbeiter gelöscht ✔");

            MitarbeiterLaden();
            FelderLeeren();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Bitte zuerst einen Mitarbeiter auswählen.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Möchten Sie diesen Mitarbeiter wirklich löschen?",
                "Bestätigung",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                MitarbeiterLoeschen();
            }
        }
        private void MitarbeiterUpdate()
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Name darf nicht leer sein!");
                return;
            }
            if (dataGridView1.CurrentRow == null)
                return;

            string query = @"
    UPDATE mitarbeiter
    SET vorname = @vorname,
        nachname = @nachname,
        bereich = @bereich,
        passwort = @passwort,
        rolle = @rolle
    WHERE personalnr = @personalnr";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@vorname", textBox2.Text);
                cmd.Parameters.AddWithValue("@nachname", textBox4.Text);
                cmd.Parameters.AddWithValue("@bereich", comboBox1.Text);
                cmd.Parameters.AddWithValue("@passwort", textBox3.Text);
                cmd.Parameters.AddWithValue("@rolle", comboBox2.Text);
                cmd.Parameters.AddWithValue("@personalnr",
                    dataGridView1.CurrentRow.Cells["personalnr"].Value);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Mitarbeiter aktualisiert ✔");
            MitarbeiterLaden();
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



        private void FelderLeeren()
        {
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
        }

        private void MitarbeiterHinzufuegen()
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text) ||
    string.IsNullOrWhiteSpace(textBox4.Text) ||
    string.IsNullOrWhiteSpace(textBox3.Text) ||
    comboBox1.SelectedItem == null ||
    comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Bitte alle Felder ausfüllen!");
                return;
            }
            if (textBox3.Text.Length < 4)
            {
                MessageBox.Show("Passwort muss mindestens 4 Zeichen haben!");
                return;
            }


            string check = "SELECT COUNT(*) FROM mitarbeiter WHERE vorname = @v AND nachname = @n";

            using (var conn = Database.GetConnection())


            using (var checkCmd = new MySqlCommand(check, conn))
            {
                checkCmd.Parameters.AddWithValue("@v", textBox2.Text);
                checkCmd.Parameters.AddWithValue("@n", textBox4.Text);

                int existiert = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (existiert > 0)
                {
                    MessageBox.Show("Mitarbeiter existiert bereits!");
                    return;
                }
            }
            string query = @"
    INSERT INTO mitarbeiter
    (vorname, nachname, bereich, passwort, rolle, aktiv)
    VALUES
    (@vorname, @nachname, @bereich, @passwort, @rolle, 1)";
            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@vorname", textBox2.Text);
                cmd.Parameters.AddWithValue("@nachname", textBox4.Text);
                cmd.Parameters.AddWithValue("@bereich", comboBox1.Text);
                cmd.Parameters.AddWithValue("@passwort", textBox3.Text);
                cmd.Parameters.AddWithValue("@rolle", comboBox2.Text);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Mitarbeiter hinzugefügt ✔");
            MitarbeiterLaden();
        }





        private void button2_Click(object sender, EventArgs e)
        {


            MitarbeiterUpdate();
        }

        private void Button1_Click(object sender, EventArgs e)
        {


            MitarbeiterHinzufuegen();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Hauptmenu hauptmenu = new Hauptmenu();
            hauptmenu.Show();
            this.Close();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string rolle = comboBox2.SelectedItem?.ToString();

            comboBox1.Items.Clear();
            comboBox1.Enabled = true;   // erstmal aktivieren

            if (rolle == "service")
            {
                comboBox1.Items.Add("Innen vorne Tische 1-10");
                comboBox1.Items.Add("Innen hinten Tische 11-20");
                comboBox1.Items.Add("Terrasse Tische 21-30");
                comboBox1.Items.Add("Terrasse groß Tische 31-35");
                comboBox1.Items.Add("VIP / Gruppen Tische 36-40");
            }
            else if (rolle == "kasse")
            {
                comboBox1.Items.Add("Kasse");
                comboBox1.SelectedIndex = 0;
                comboBox1.Enabled = false;
            }
            else if (rolle == "admin")
            {
                comboBox1.Items.Add("EDV");
                comboBox1.SelectedIndex = 0;
                comboBox1.Enabled = false;
            }
            else if (rolle == "management")
            {
                comboBox1.Items.Add("Management");
                comboBox1.SelectedIndex = 0;
                comboBox1.Enabled = false;
            }
            else if (rolle == "koch")
            {
                comboBox1.Items.Add("Küche");
                comboBox1.SelectedIndex = 0;
                comboBox1.Enabled = false;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            {
                // Erlaubt: Buchstaben, Kontrolltasten (wie Backspace) und Leerzeichen
                if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
                {
                    e.Handled = true; // Verwirft die Eingabe, wenn es eine Zahl oder Sonderzeichen ist
                }
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            {
                // Erlaubt: Buchstaben, Kontrolltasten (wie Backspace) und Leerzeichen
                if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
                {
                    e.Handled = true; // Verwirft die Eingabe, wenn es eine Zahl oder Sonderzeichen ist
                }
            }
        }

        private void showpassoword_CheckedChanged(object sender, EventArgs e)
        {
            if (showpassoword.Checked)
            {
                textBox3.PasswordChar = '\0'; // Kein Maskierungszeichen, Passwort wird sichtbar
            }
            else
            {
                textBox3.PasswordChar = '●';
            }
        }
    }
    }

