using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pizzeria_Projekt_Schule
{
    public partial class reservierung : Form
    {
        public reservierung()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hauptmenu mainmenupage = new Hauptmenu();
            mainmenupage.Show();
            this.Close();
        }
        void LadeTische(int personen)
        {
            string connString = "server=localhost;uid=root;pwd=root;database=pizzaprojekt";
            string sql = @"
        SELECT tisch_id, max_personen
        FROM tische
        WHERE aktiv = true
        AND max_personen >= @personen
        ORDER BY max_personen ASC";

            using (var con = new MySqlConnection(connString))
            using (var cmd = new MySqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@personen", personen);

                using (var da = new MySqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    comboBox2.DataSource = dt;
                    comboBox2.DisplayMember = "tisch_id";
                    comboBox2.ValueMember = "tisch_id";
                    dt.Columns.Add("anzeige", typeof(string));

                    foreach (DataRow row in dt.Rows)
                    {
                        row["anzeige"] = $"Tisch {row["tisch_id"]} ({row["max_personen"]} Personen)";
                    }

                    comboBox2.DisplayMember = "anzeige";
                    comboBox2.ValueMember = "tisch_id";

                }
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string connString = "server=localhost;uid=root;pwd=root;database=pizzaprojekt";

                const string guestadd = "INSERT INTO gast (gastvorname, gastnachname, telephonenr) VALUES (@vorname, @nachname, @telefon);";
                const string reservierungsinsert = "insert into reservierungen(datum,telephonenr) values (@datum,@telephonenr)";

             

                using (var conn = new MySqlConnection(connString))
                {
                    conn.Open();

                    // 1️⃣ GAST INSERT
                    int gastId;
                    string gastSql = @"
        INSERT INTO gast (gastvorname, gastnachname, telephonenr)
        VALUES (@vorname, @nachname, @telefon);
        SELECT LAST_INSERT_ID();";

                    using (var gastCmd = new MySqlCommand(gastSql, conn))
                    {

                        gastCmd.Parameters.AddWithValue("@vorname", "");
                        gastCmd.Parameters.AddWithValue("@nachname", textBox1.Text);
                        gastCmd.Parameters.AddWithValue("@telefon", textBox2.Text);

                        gastId = Convert.ToInt32(gastCmd.ExecuteScalar());
                    }

                    // 2️⃣ SLOT bestimmen
                    int slot;
                    switch (comboBox1.SelectedItem?.ToString())
                    {
                        case "12-15": slot = 1; break;
                        case "15-18": slot = 2; break;
                        case "18-21": slot = 3; break;
                        case "21-24": slot = 4; break;
                        default:
                            MessageBox.Show("Bitte Uhrzeit auswählen");
                            return;
                    }

                    // 3️⃣ RESERVIERUNG INSERT
                    string resSql = @"
                    INSERT INTO reservierungen
                    (tisch_id_fk, slot, datum, personenanzahl, gastid_fk, zustand)
                    VALUES
                    (@tisch, @slot, @datum, @personen, @gastid, 'offen')";

                    using (var resCmd = new MySqlCommand(resSql, conn))
                    {
                        if (Convert.ToInt16(numericUpDown1) == 0)
                        {
                            MessageBox.Show("geben sie eine personanzahl aus");
                        }
                        else
                        {
                            
                                resCmd.Parameters.AddWithValue("@tisch", Convert.ToInt32(comboBox2.SelectedValue));
                                resCmd.Parameters.AddWithValue("@slot", slot);
                                resCmd.Parameters.AddWithValue("@datum", dateTimePicker1.Value);

                                resCmd.Parameters.AddWithValue("@personen", Convert.ToInt32(numericUpDown1.Value));
                                resCmd.Parameters.AddWithValue("@gastid", gastId);

                                resCmd.ExecuteNonQuery();
                            
                        }

                    }
                }

                MessageBox.Show("Reservierung gespeichert ✅");

            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062) // Duplicate entry (UNIQUE constraint)
                {
                    MessageBox.Show(
                        "Dieser Tisch ist an diesem Datum und Slot bereits reserviert ❌",
                        "Reservierung nicht möglich",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
                else
                {
                    MessageBox.Show("Datenbankfehler: " + ex.Message);
                }
            }


        }

        

        private void reservierung_Load(object sender, EventArgs e)
        {
            
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            int personen = (int)numericUpDown1.Value;

            if (personen > 0)
                LadeTische(personen);
        }

    }
}
