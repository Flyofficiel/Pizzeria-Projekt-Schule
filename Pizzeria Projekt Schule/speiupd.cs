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
    public partial class speiupd : Form
    {
        public speiupd(int id, string name, string typ, decimal preis, string zutaten)
        {
            InitializeComponent();

            speiseId = id;

            textBox2.Text = name;
            comboBox1.Text = typ;
            textBox3.Text = preis.ToString();
            textBox4.Text = zutaten;

        }
        private int speiseId;
        private void button1_Click(object sender, EventArgs e)
        {
            // Name darf nicht leer sein
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Speisename darf nicht leer sein!");
                return;
            }

            // Name darf nicht nur aus Zahlen bestehen
            if (textBox2.Text.All(char.IsDigit))
            {
                MessageBox.Show("Speisename darf nicht nur Zahlen enthalten!");
                return;
            }


            string query = @"
        UPDATE speisen
        SET speisename = @name,
            speisentyp = @typ,
            preis = @preis,
            zutaten = @zutaten
        WHERE speise_id = @id
    ";

            MySqlConnection conn = Database.GetConnection();

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@name", textBox2.Text);
                cmd.Parameters.AddWithValue("@typ", comboBox1.Text);
                if (!decimal.TryParse(
     textBox3.Text.Replace(",", "."),
     System.Globalization.NumberStyles.Any,
     System.Globalization.CultureInfo.InvariantCulture,
     out decimal preis))
                {
                    MessageBox.Show("Ungültiger Preis!");
                    return;
                }

                cmd.Parameters.AddWithValue("@preis", preis);
                cmd.Parameters.AddWithValue("@zutaten", textBox4.Text);
                cmd.Parameters.AddWithValue("@id", speiseId);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Speise erfolgreich aktualisiert ✔");
            this.Close(); } //  Wichtig!
            private void button2_Click(object sender, EventArgs e)
        {
            // Schließt einfach das aktuelle Fenster ohne zu speichern
            this.Close();
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

            // Punkt automatisch zu Komma machen (optional)
            if (e.KeyChar == '.')
            {
                e.KeyChar = ',';
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

        private void speiupd_Load(object sender, EventArgs e)
        {

        }
    }
}
