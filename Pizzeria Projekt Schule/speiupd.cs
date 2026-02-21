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
                cmd.Parameters.AddWithValue("@preis", Convert.ToDecimal(textBox3.Text));
                cmd.Parameters.AddWithValue("@zutaten", textBox4.Text);
                cmd.Parameters.AddWithValue("@id", speiseId);

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Speise erfolgreich aktualisiert ✔");
            this.Close(); // 🔥 Wichtig!




        }

        
    }
}
