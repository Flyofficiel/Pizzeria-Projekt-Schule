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
            string connString = "server=localhost;uid=root;pwd=root;database=pizzaprojekt";
            string query = "SELECT * FROM speisen";

            using (MySqlConnection con = new MySqlConnection(connString))
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, con);
                DataTable table = new DataTable();

                adapter.Fill(table);

                dataGridView1.DataSource = table;
            }
        }
    }
}
