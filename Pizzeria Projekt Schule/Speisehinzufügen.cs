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
    public partial class Speisehinzufügen : Form
    {
        public Speisehinzufügen()
        {
            InitializeComponent();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = Database.GetConnection())
            {
                // 1️⃣ Name prüfen
                if (string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("Speisename fehlt!");
                    return;
                }

                // 2️⃣ Preis prüfen
                if (!double.TryParse(textBox3.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out double preis))
                {
                    MessageBox.Show("Ungültiger Preis!");
                    return;
                }

                // 3️⃣ Typ prüfen
                if (comboBox1.SelectedItem == null)
                {
                    MessageBox.Show("Bitte Speisentyp auswählen!");
                    return;
                }

                string zutaten = textBox4.Text;

                // 4️⃣ INSERT (OHNE speise_id !!!)
                string query = @"INSERT INTO speisen 
                        (speisename, speisentyp, preis, zutaten, aktiv)
                        VALUES (@name, @typ, @preis, @zutaten, true)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", textBox2.Text);
                    cmd.Parameters.AddWithValue("@typ", comboBox1.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@preis", preis);
                    cmd.Parameters.AddWithValue("@zutaten", zutaten);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Speise hinzugefügt ✔");

                this.Close();
            }
        }


        private void speisenhin_Load(object sender, EventArgs e)
        {

        }
    }
}
