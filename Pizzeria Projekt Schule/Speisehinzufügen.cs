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
                string name = textBox2.Text.Trim();
                string zutaten = textBox4.Text.Trim();

                // 1️⃣ Name prüfen
                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("Speisename fehlt!");
                    return;
                }

                if (!name.Any(char.IsLetter))
                {
                    MessageBox.Show("Speisename muss mindestens einen Buchstaben enthalten!");
                    return;
                }

                // 2️⃣ Preis prüfen
                if (!decimal.TryParse(
                    textBox3.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal preis))
                {
                    MessageBox.Show("Ungültiger Preis!");
                    return;
                }

                if (preis <= 0 || preis > 999.99m)
                {
                    MessageBox.Show("Preis muss zwischen 0,01 € und 999,99 € liegen!");
                    return;
                }

                // 3️⃣ Typ prüfen
                if (comboBox1.SelectedItem == null)
                {
                    MessageBox.Show("Bitte Speisentyp auswählen!");
                    return;
                }

                // 4️⃣ Zutaten prüfen
                if (string.IsNullOrWhiteSpace(zutaten))
                {
                    MessageBox.Show("Bitte Zutaten eingeben!");
                    return;
                }

                // 5️⃣ INSERT
                string query = @"INSERT INTO speisen 
                (speisename, speisentyp, preis, zutaten, aktiv)
                VALUES (@name, @typ, @preis, @zutaten, true)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
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
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !char.IsDigit(e.KeyChar) &&
                e.KeyChar != ',' &&
                e.KeyChar != '.')
            {
                e.Handled = true;
            }

        }
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !char.IsLetter(e.KeyChar) &&
                e.KeyChar != ' ' &&
                e.KeyChar != ',')
            {
                e.Handled = true;
            }
        }



       
    }
}
