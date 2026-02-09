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
    public partial class speisenhin : Form
    {
        public speisenhin()
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
                // 1️⃣ Speise-ID
                if (!int.TryParse(textBox1.Text, out int speiseId))
                {
                    MessageBox.Show("Speise-ID muss eine Zahl sein!");
                    return;
                }

                // Existenz prüfen
                string checkQuery = "SELECT COUNT(*) FROM speisen WHERE speise_id = @id";
                using (var checkCmd = new MySqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@id", speiseId);
                    if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                    {
                        MessageBox.Show("Speise-ID existiert bereits!");
                        return;
                    }
                }

                // 2️⃣ Name
                if (string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("Speisename fehlt!");
                    return;
                }

                // 3️⃣ Preis
                if (!double.TryParse(textBox3.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out double preis))
                {
                    MessageBox.Show("Ungültiger Preis!");
                    return;
                }

                // 4️⃣ INSERT
                string query = @"INSERT INTO speisen 
            (speise_id, speisename, speisentyp, preis, zutaten)
            VALUES (@id, @name, @typ, @preis, @zutaten)";

                if (comboBox1.SelectedItem == null)
                {
                    MessageBox.Show("Bitte Speisentyp auswählen!");
                    return;
                }
                if(comboBox1.SelectedItem.ToString() != "🥤 GETRÄNKE" && textBox4 != null)
                {
                    MessageBox.Show("Bitte fügen Sie die Zutaten hinzu");
                    return;
                }
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", speiseId);
                    cmd.Parameters.AddWithValue("@name", textBox2.Text);
                    cmd.Parameters.AddWithValue("@typ", comboBox1.SelectedItem);
                    cmd.Parameters.AddWithValue("@preis", preis);
                    cmd.Parameters.AddWithValue("@zutaten", textBox4.Text);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Speise hinzugefügt ✔");
                }
            }
        }

        private void speisenhin_Load(object sender, EventArgs e)
        {

        }
    }
}
