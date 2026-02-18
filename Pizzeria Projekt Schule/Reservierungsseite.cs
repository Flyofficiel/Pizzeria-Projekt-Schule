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
    public partial class Reservierungsseite : Form
    {
        public Reservierungsseite()
        {
            InitializeComponent();
        }



        private void button2_Click(object sender, EventArgs e)
        {
            Hauptmenu mainmenupage = new Hauptmenu();
            mainmenupage.Show();
            this.Close();
        }
        void LadeTische(int personen)
        {
            if (comboBox1.SelectedItem == null)
                return;

            int slot = HoleSlot();
            if (slot == 0)
                return;

            string sql = @"
    SELECT t.tisch_id, t.max_personen
    FROM tische t
    WHERE t.aktiv = true
    AND t.max_personen >= @personen
    AND t.tisch_id NOT IN
    (
        SELECT r.tisch_id_fk
        FROM reservierungen r
        WHERE DATE(r.datum) = @datum
        AND r.slot = @slot
        AND r.zustand = 'offen'
    )
    ORDER BY t.max_personen ASC";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@personen", personen);
                cmd.Parameters.AddWithValue("@datum", dateTimePicker1.Value.Date);
                cmd.Parameters.AddWithValue("@slot", slot);

                DataTable dt = new DataTable();
                new MySqlDataAdapter(cmd).Fill(dt);

                comboBox2.DisplayMember = "tisch_id";
                comboBox2.ValueMember = "tisch_id";
                comboBox2.DataSource = dt;
            }
        }




        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // 🔥 Verbesserte Eingabeprüfung
                string name = textBox1.Text.Trim();
                string telefon = textBox2.Text.Trim();

                if (dateTimePicker1.Value.Date < DateTime.Today)
                {
                    MessageBox.Show("Reservierung in der Vergangenheit nicht möglich!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(name) ||
                    string.IsNullOrWhiteSpace(telefon) ||
                    numericUpDown1.Value == 0 ||
                    comboBox1.SelectedItem == null ||
                    comboBox2.SelectedItem == null)
                {
                    MessageBox.Show("Bitte alle Pflichtfelder ausfüllen!");
                    return;
                }

                // Telefonnummer Länge prüfen
                if (telefon.Length < 8 || telefon.Length > 15)
                {
                    MessageBox.Show("Telefonnummer ungültig!");
                    return;
                }



                string connString = "server=localhost;uid=root;pwd=root;database=pizzaprojekt";

                const string guestadd = "INSERT INTO gast (gastvorname, gastnachname, telephonenr) VALUES (@vorname, @nachname, @telefon);";
                const string reservierungsinsert = "insert into reservierungen(datum,telephonenr) values (@datum,@telephonenr)";



                using (var conn = new MySqlConnection(connString))
                {
                    conn.Open();

                    // 1️⃣ GAST INSERT
                    // 🔥 Gast prüfen oder neu anlegen
                    int gastId;
                    string checkGast = "SELECT gastid FROM gast WHERE telephonenr = @telefon LIMIT 1";

                    using (var checkCmd = new MySqlCommand(checkGast, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@telefon", textBox2.Text);
                        var result = checkCmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            gastId = Convert.ToInt32(result);
                        }
                        else
                        {
                            string gastSql = @"
        INSERT INTO gast (gastvorname, gastnachname, telephonenr)
        VALUES (@vorname, @nachname, @telefon);
        SELECT LAST_INSERT_ID();";

                            using (var gastCmd = new MySqlCommand(gastSql, conn))
                            {
                                string[] teile = name.Split(' ');

                                string vorname = teile[0];
                                string nachname = teile.Length > 1 ? teile[1] : "";

                                gastCmd.Parameters.AddWithValue("@vorname", vorname);
                                gastCmd.Parameters.AddWithValue("@nachname", nachname);
                                gastCmd.Parameters.AddWithValue("@telefon", telefon);


                                gastId = Convert.ToInt32(gastCmd.ExecuteScalar());
                            }
                        }
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


                        resCmd.Parameters.AddWithValue("@tisch", Convert.ToInt32(comboBox2.SelectedValue));
                        resCmd.Parameters.AddWithValue("@slot", slot);
                        resCmd.Parameters.AddWithValue("@datum", dateTimePicker1.Value);

                        resCmd.Parameters.AddWithValue("@personen", Convert.ToInt32(numericUpDown1.Value));
                        resCmd.Parameters.AddWithValue("@gastid", gastId);

                        resCmd.ExecuteNonQuery();



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
            if (numericUpDown1.Value > 0)
                LadeTische((int)numericUpDown1.Value);
        }

        private int HoleSlot()
        {
            switch (comboBox1.SelectedItem?.ToString())
            {
                case "12-15": return 1;
                case "15-18": return 2;
                case "18-21": return 3;
                case "21-24": return 4;
                default: return 0;
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value > 0)
                LadeTische((int)numericUpDown1.Value);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value > 0)
                LadeTische((int)numericUpDown1.Value);
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Nur Zahlen und Backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress_1(object sender, KeyPressEventArgs e)
        {

        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !char.IsLetter(e.KeyChar) &&
                e.KeyChar != ' ')
            {
                e.Handled = true;
                System.Media.SystemSounds.Beep.Play();
            }
        }

        


        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        
    }
}
