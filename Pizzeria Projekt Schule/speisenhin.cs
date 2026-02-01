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
            string connString = "server=localhost;uid=root;pwd=root;database=pizzaprojekt";
            string query = @"insert into speisen (speise_id,speisename,speisentyp, preis, zutaten) values (@speise_id,@speisename,@speisentyp, @preis, @zutaten)";

            MySqlConnector.MySqlConnection con = new MySqlConnector.MySqlConnection(connString);
            MySqlCommand cmd = new MySqlCommand(query, con);
            {
                cmd.Parameters.AddWithValue("@speise_id", textBox1.Text);
                cmd.Parameters.AddWithValue("@speisename", textBox2.Text);
                cmd.Parameters.AddWithValue("@speisentyp", comboBox1.Text);
                cmd.Parameters.AddWithValue("@preis", textBox3.Text);
                cmd.Parameters.AddWithValue("@zutaten", textBox4.Text);
                con.Open();


                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Speise hinzugefügt ✔");
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Speise-ID existiert bereits!");
                }

            }
        }
    }
}
