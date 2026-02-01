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
        }
        

        private void Bestellungspagerichtig_Load(object sender, EventArgs e)
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

        private void button5_Click(object sender, EventArgs e)
        {
            Hauptmenu hauptmenu = new Hauptmenu();
            hauptmenu.Show();
            this.Close();
        }
    }
}
