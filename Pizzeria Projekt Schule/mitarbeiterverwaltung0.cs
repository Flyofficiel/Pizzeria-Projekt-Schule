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

namespace Pizzeria_Projekt_Schule
{
    public partial class mitarbeiterverwaltung0 : Form
    {
        public mitarbeiterverwaltung0()
        {
            InitializeComponent();
        }

        private void mitarbeiterverwaltungrichtig_Load(object sender, EventArgs e)
        {
            MitarbeiterLaden();

            // 🔥 BEREICH ENUM WERTE
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Innen vorne");
            comboBox1.Items.Add("Innen hinten");
            comboBox1.Items.Add("Terrasse");
            comboBox1.Items.Add("Terrasse groß");
            comboBox1.Items.Add("VIP / Gruppen");
            comboBox1.Items.Add("Küche");
            comboBox1.Items.Add("Kasse");
            comboBox1.Items.Add("EDV");
            comboBox1.Items.Add("Management");

            // 🔥 ROLLE ENUM WERTE
            comboBox2.Items.Clear();
            comboBox2.Items.Add("service");
            comboBox2.Items.Add("koch");
            comboBox2.Items.Add("kasse");
            comboBox2.Items.Add("admin");
            comboBox2.Items.Add("management");
        }



        private void dataGridView1_SelectionChanged_1(object sender, EventArgs e)
        {
            // Prüfen, ob überhaupt eine Zeile ausgewählt ist
            if (dataGridView1.CurrentRow == null)
                return;

            DataGridViewRow row = dataGridView1.CurrentRow;

            textBox2.Text = row.Cells["vorname"].Value + " " + row.Cells["nachname"].Value;
            comboBox1.Text = row.Cells["bereich"].Value?.ToString();
            if (dataGridView1.Columns.Contains("passwort"))
            {
                textBox3.Text = row.Cells["passwort"].Value?.ToString();
            }
            else
            {
                textBox3.Text = "";
            }

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
            if (dataGridView1.CurrentRow == null)
                return;

            string query = @"
    UPDATE mitarbeiter
    SET 
        vorname = @vorname,
        nachname = @nachname,
        bereich = @bereich,
        passwort = @passwort,
        rolle = @rolle
    WHERE personalnr = @personalnr
    ";

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

            MessageBox.Show("Mitarbeiter gespeichert ✔");
            MitarbeiterLaden();
        }

        private void MitarbeiterLaden()
        {
           
            string query = "SELECT personalnr, vorname, nachname, bereich, passwort FROM mitarbeiter WHERE aktiv = 1";

            MySqlConnection conn = Database.GetConnection();
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
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
            if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Bitte Bereich und Rolle auswählen!");
                return;
            }

            string query = @"
    INSERT INTO mitarbeiter
    (vorname, nachname, bereich, passwort, rolle, aktiv)
    VALUES
    (@vorname, @nachname, @bereich, @passwort, @rolle, 1)
    ";

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
            FelderLeeren();
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
    }
}
