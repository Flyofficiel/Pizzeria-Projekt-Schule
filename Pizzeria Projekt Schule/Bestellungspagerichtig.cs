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
    public partial class Bestellungspagerichtig : Form
    {
        public Bestellungspagerichtig()
        {
            InitializeComponent();

            button1.Click += button1_Click; // +
            button2.Click += button2_Click; // -
        }


        private void Bestellungspagerichtig_Load(object sender, EventArgs e)
        {
            string connString = "server=localhost;uid=root;pwd=root;database=pizzaprojekt";
            string query = "SELECT speise_id, speisename, preis FROM speisen WHERE aktiv = 1";

            using (MySqlConnection con = new MySqlConnection(connString))
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, con);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
            }

            // 🔥 DAS ist entscheidend
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Hauptmenu hauptmenu = new Hauptmenu();
            hauptmenu.Show();
            this.Close();
        }
        public class WarenkorbItem
        {
            public int SpeiseId { get; set; }
            public string Name { get; set; }
            public double Preis { get; set; }
            public int Menge { get; set; }

            public override string ToString()
            {
                return $"{Name} x{Menge}  ({Preis * Menge:0.00} €)";
            }
        }
        List<WarenkorbItem> warenkorb = new List<WarenkorbItem>();
        private void WarenkorbAktualisieren()
        {
            listBox1.Items.Clear();

            double summe = 0;

            foreach (var item in warenkorb)
            {
                listBox1.Items.Add(item);
                summe += item.Preis * item.Menge;
            }

            textBox1.Text = summe.ToString("0.00 €");

            // 🔥 AUTOMATISCHES AUSWÄHLEN
            if (listBox1.Items.Count > 0)
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }


        

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Bitte zuerst ein Gericht auswählen.");
                return;
            }

            var item = (WarenkorbItem)listBox1.SelectedItem;
            item.Menge++;
            WarenkorbAktualisieren();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem is WarenkorbItem item)
            {
                item.Menge--;
                if (item.Menge <= 0)
                    warenkorb.Remove(item);

                WarenkorbAktualisieren();
            }
        }
        private int BestellungAnlegen()
        {
            string connString = "server=localhost;uid=root;pwd=root;database=pizzaprojekt";

            string query = @"
        INSERT INTO bestellungen (datum, tisch_id_fk, personalnr_fk)
        VALUES (NOW(), @tisch, @mitarbeiter);
        SELECT LAST_INSERT_ID();
    ";

            using (MySqlConnection con = new MySqlConnection(connString))
            using (MySqlCommand cmd = new MySqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@tisch", comboBox1.SelectedValue);
                cmd.Parameters.AddWithValue("@mitarbeiter", comboBox2.SelectedValue);

                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (warenkorb.Count == 0)
            {
                MessageBox.Show("Warenkorb ist leer!");
                return;
            }

            int bestellNr = BestellungAnlegen();

            string connString = "server=localhost;uid=root;pwd=root;database=pizzaprojekt";

            using (MySqlConnection con = new MySqlConnection(connString))
            {
                con.Open();

                foreach (var item in warenkorb)
                {
                    string query = @"
                INSERT INTO bestellposition
                (bestellnr_fk, speise_id_fk, menge, preis_beim_kauf)
                VALUES
                (@bestellnr, @speise, @menge, @preis)
            ";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@bestellnr", bestellNr);
                        cmd.Parameters.AddWithValue("@speise", item.SpeiseId);
                        cmd.Parameters.AddWithValue("@menge", item.Menge);
                        cmd.Parameters.AddWithValue("@preis", item.Preis);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            MessageBox.Show("Bestellung an Küche gesendet 🍕🔥");
            warenkorb.Clear();
            WarenkorbAktualisieren();
        }

        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            int speiseId = Convert.ToInt32(row.Cells["speise_id"].Value);
            string name = row.Cells["speisename"].Value.ToString();
            double preis = Convert.ToDouble(row.Cells["preis"].Value);

            var item = warenkorb.FirstOrDefault(x => x.SpeiseId == speiseId);

            if (item != null)
            {
                item.Menge++;
            }
            else
            {
                warenkorb.Add(new WarenkorbItem
                {
                    SpeiseId = speiseId,
                    Name = name,
                    Preis = preis,
                    Menge = 1
                });
            }

            WarenkorbAktualisieren();
        }
    }
    

}
