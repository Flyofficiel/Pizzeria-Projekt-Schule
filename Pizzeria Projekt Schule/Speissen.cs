using MySqlConnector;
using System;
using System.Collections;
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
    public partial class Speissen : Form
    {
        public Speissen()
        {
            InitializeComponent();
        }
        private void SpeiseLoeschen()
        {
            string connString = "server=localhost;uid=root;pwd=root;database=pizzaprojekt";

            string query = @"
        UPDATE speisen
        SET aktiv = 0
        WHERE speise_id = @speise_id
    ";

            using (MySqlConnection con = new MySqlConnection(connString))
            using (MySqlCommand cmd = new MySqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue(
                    "@speise_id",
                    dataGridView1.CurrentRow.Cells["speise_id"].Value
                );

                con.Open();
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Speise gelöscht ✔");
            SpeisenLaden(); // 🔥 Grid aktualisieren
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Hauptmenu mainmenupage = new Hauptmenu();
            mainmenupage.Show();
            this.Close();
        }

        private void Speissen_Load(object sender, EventArgs e)
        {
            SpeisenLaden();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            speisenhin speisenhinzufügen = new speisenhin();
            speisenhinzufügen.Show();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Bitte zuerst eine Speise auswählen.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Möchten Sie diese Speise wirklich löschen?",
                "Bestätigung",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                SpeiseLoeschen();
            }
        }
        private void SpeisenLaden()
        {
            string connString = "server=localhost;uid=root;pwd=root;database=pizzaprojekt";
            string query = "SELECT speise_id, speisename, speisentyp, preis, zutaten FROM speisen WHERE aktiv = 1";

            using (MySqlConnection con = new MySqlConnection(connString))
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, con);
                DataTable table = new DataTable();
                adapter.Fill(table);

                dataGridView1.DataSource = table;
            }

            // 💶 Preis formatieren
            dataGridView1.Columns["preis"].DefaultCellStyle.Format = "C2";
            dataGridView1.Columns["preis"].DefaultCellStyle.FormatProvider =
                System.Globalization.CultureInfo.GetCultureInfo("de-DE");

            dataGridView1.Columns["speise_id"].HeaderText = "ID";
            dataGridView1.Columns["speisename"].HeaderText = "Name";
            dataGridView1.Columns["speisentyp"].HeaderText = "Typ";
            dataGridView1.Columns["preis"].HeaderText = "Preis";
            dataGridView1.Columns["zutaten"].HeaderText = "Zutaten";
        }

    }
}
