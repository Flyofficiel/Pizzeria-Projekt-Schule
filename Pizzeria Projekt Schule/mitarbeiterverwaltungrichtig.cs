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
    public partial class mitarbeiterverwaltungrichtig : Form
    {
        public mitarbeiterverwaltungrichtig()
        {
            InitializeComponent();
        }

        private void mitarbeiterverwaltungrichtig_Load(object sender, EventArgs e)
        {
            string connString = "server=localhost;uid=root;pwd=root;database=pizzaprojekt";
            string query = "SELECT * FROM Mitarbeiter";

            using (MySqlConnection con = new MySqlConnection(connString))
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, con);
                DataTable table = new DataTable();

                adapter.Fill(table);

                dataGridView1.DataSource = table;
            }

        }
       

        private void dataGridView1_SelectionChanged_1(object sender, EventArgs e)
        {
            // Prüfen, ob überhaupt eine Zeile ausgewählt ist
            if (dataGridView1.CurrentRow == null)
                return;

            DataGridViewRow row = dataGridView1.CurrentRow;

            textBox1.Text = row.Cells["personalnr"].Value?.ToString();
            textBox2.Text = row.Cells["vorname"].Value + " " + row.Cells["nachname"].Value;
            comboBox1.Text = row.Cells["bereich"].Value?.ToString();
            textBox3.Text = row.Cells["passwort"].Value?.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }
        private void MitarbeiterUpdate()
        {
            string connString = "server=localhost;uid=root;pwd=root;database=pizzaprojekt";

            string query = @"
        UPDATE mitarbeiter
        SET 
            vorname = @vorname,
            nachname = @nachname,
            bereich = @bereich,
            passwort = @passwort
        WHERE personalnr = @personalnr
    ";

            using (MySqlConnection con = new MySqlConnection(connString))
            using (MySqlCommand cmd = new MySqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@vorname", textBox2.Text);
                cmd.Parameters.AddWithValue("@nachname", "");
                cmd.Parameters.AddWithValue("@bereich", comboBox1.Text);
                cmd.Parameters.AddWithValue("@passwort", textBox3.Text);
                cmd.Parameters.AddWithValue("@personalnr", textBox1.Text);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Mitarbeiter gespeichert ✔");
            MitarbeiterLaden();
        }
        private void MitarbeiterLaden()
        {
            string connString = "server=localhost;uid=root;pwd=root;database=pizzaprojekt";
            string query = "SELECT personalnr, vorname, nachname, bereich, passwort FROM mitarbeiter WHERE aktiv = 1";

            using (MySqlConnection con = new MySqlConnection(connString))
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, con);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Bitte zuerst einen Mitarbeiter auswählen.");
                return;
            }

            MitarbeiterUpdate();
        }
    }
}
