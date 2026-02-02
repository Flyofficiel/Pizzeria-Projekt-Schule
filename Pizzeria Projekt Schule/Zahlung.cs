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
    public partial class Zahlung : Form
    {
        public Zahlung()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hauptmenu mainmenupage = new Hauptmenu();
            mainmenupage.Show();
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Zahlung_Load(object sender, EventArgs e)
        {
            BestellungenLaden();



        }
        private void BestellungenLaden()
        {
            if (comboBox1.SelectedValue == null)
                return;

            string connString = "server=localhost;uid=root;pwd=root;database=pizzaprojekt";

            string query = @"
        SELECT 
            b.bestellnr,
            s.speisename,
            bp.menge,
            bp.preis_beim_kauf,
            (bp.menge * bp.preis_beim_kauf) AS gesamtpreis
        FROM bestellungen b
        JOIN bestellposition bp ON b.bestellnr = bp.bestellnr_fk
        JOIN speisen s ON bp.speise_id_fk = s.speise_id
        JOIN reservierungen r 
            ON r.tisch_id_fk = b.tisch_id_fk 
           AND DATE(r.datum) = DATE(b.datum)
        WHERE 
            b.tisch_id_fk = @tisch
            AND DATE(b.datum) = @datum;
    ";

            using (MySqlConnection con = new MySqlConnection(connString))
            using (MySqlCommand cmd = new MySqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@tisch", comboBox1.SelectedValue);
                cmd.Parameters.AddWithValue("@datum", dateTimePicker1.Value.Date);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);

                dataGridView1.DataSource = table;
            }

            // 💶 Preis formatieren
            dataGridView1.Columns["preis_beim_kauf"].DefaultCellStyle.Format = "C2";
            dataGridView1.Columns["gesamtpreis"].DefaultCellStyle.Format = "C2";
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            BestellungenLaden();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

            BestellungenLaden();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            BestellungenLaden();
        }
    }
}