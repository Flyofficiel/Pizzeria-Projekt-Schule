using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySqlConnector;

namespace Pizzeria_Projekt_Schule
{
    public partial class auswertung01 : Form
    {
        public auswertung01()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Hauptmenu mainmenupage = new Hauptmenu();
            mainmenupage.Show();
            this.Close();
        }

        private void auswertung01_Load(object sender, EventArgs e)
        {
            LadeUmsatzHeute();
            LadeBeliebtesteSpeise();
            LadeUmsatzProMitarbeiter();
           

        }
        private void LadeUmsatzHeute()
        {
            string query = @"
        SELECT IFNULL(SUM(p.menge * p.preis_beim_kauf),0)
        FROM bestellungen b
        JOIN bestellposition p ON b.bestellnr = p.bestellnr_fk
        WHERE DATE(b.datum) = CURDATE()
        AND b.status = 'bezahlt'";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                object result = cmd.ExecuteScalar();

                decimal umsatz = Convert.ToDecimal(result);

                textBox1.Text = umsatz.ToString("0.00 €");
            }
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }
        private void LadeBeliebtesteSpeise()
        {
            string query = @"
        SELECT s.speisename, SUM(p.menge) AS verkauft
        FROM speisen s
        JOIN bestellposition p ON s.speise_id = p.speise_id_fk
        JOIN bestellungen b ON b.bestellnr = p.bestellnr_fk
        WHERE b.status = 'bezahlt'
        GROUP BY s.speisename
        ORDER BY verkauft DESC
        LIMIT 1";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    string name = reader.GetString("speisename");
                    int menge = reader.GetInt32("verkauft");

                    textBox2.Text = name + " (" + menge + "x)";
                }
                else
                {
                    {
                        textBox2.Text = "Keine Daten";
                    }
                }
            }
        }

                private void LadeUmsatzProMitarbeiter()
        {
            string query = @"
        SELECT CONCAT(m.vorname,' ',m.nachname) AS Mitarbeiter,
               IFNULL(SUM(p.menge * p.preis_beim_kauf),0) AS Umsatz
        FROM mitarbeiter m
        LEFT JOIN bestellungen b ON m.personalnr = b.personalnr_fk
        LEFT JOIN bestellposition p ON b.bestellnr = p.bestellnr_fk
        WHERE b.status = 'bezahlt'
        GROUP BY Mitarbeiter
        ORDER BY Umsatz DESC";

            using (var conn = Database.GetConnection())
            using (var da = new MySqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

                
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }

}


    

