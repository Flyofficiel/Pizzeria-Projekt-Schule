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
    public partial class tischauswahl : Form
    {
        public tischauswahl()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            LadeTische();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button41_Click(object sender, EventArgs e)
        {
            Hauptmenu mainmenupage = new Hauptmenu();
            mainmenupage.Show();
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void LadeTische()
        {

            MySqlConnection conn = Database.GetConnection();

            {
                string query = @"
        SELECT 
    t.tisch_id,
    t.max_personen,
    t.bereich,
    t.lage,
    t.ort,
    CONCAT(m.vorname, ' ', m.nachname) AS mitarbeiter
FROM tische t
LEFT JOIN mitarbeiter m 
    ON t.bereich = m.bereich
    AND m.rolle = 'service'
ORDER BY t.tisch_id;";

                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dataGridView1.DataSource = dt;
            }
        }
    }
}
