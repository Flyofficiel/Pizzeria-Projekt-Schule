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
            StammgastLaden();
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
    SELECT 
        t.tisch_id,
        t.bereich,
        t.max_personen,

        CASE
            WHEN EXISTS (
                SELECT 1 FROM bestellungen b
                WHERE b.tisch_id_fk = t.tisch_id
                AND b.slot = @slot
                AND DATE(b.datum) = @datum
                AND b.status = 'offen'
            ) THEN 'Besetzt'

         WHEN EXISTS (
    SELECT 1 FROM reservierungen r
    WHERE r.tisch_id_fk = t.tisch_id
    AND r.slot = @slot
    AND DATE(r.datum) = @datum
    AND r.zustand = 'aktiv' 
) THEN 'Aktiv'

WHEN EXISTS (
    SELECT 1 FROM reservierungen r
    WHERE r.tisch_id_fk = t.tisch_id
    AND r.slot = @slot
    AND DATE(r.datum) = @datum
    AND r.zustand = 'offen' -- NUR wenn sie noch nicht eingecheckt/beendet sind
) THEN 'Reserviert'

            ELSE 'Frei'
        END AS status

    FROM tische t
    WHERE t.aktiv = true
    AND t.max_personen >= @personen
    ORDER BY t.max_personen ASC";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@personen", personen);
                cmd.Parameters.AddWithValue("@datum", dateTimePicker1.Value.Date);
                cmd.Parameters.AddWithValue("@slot", slot);

                DataTable dt = new DataTable();
                new MySqlDataAdapter(cmd).Fill(dt);

                if (!dt.Columns.Contains("Anzeige"))
                    dt.Columns.Add("Anzeige", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    row["Anzeige"] =
                        "Tisch " + row["tisch_id"] +
                        " - " + row["bereich"] +
                        " (" + row["max_personen"] + " Pers.)" +
                        " - " + row["status"];
                }

                comboBox2.DataSource = null;

                if (dt.Rows.Count > 0)
                {
                    comboBox2.DisplayMember = "Anzeige";
                    comboBox2.ValueMember = "tisch_id";
                    comboBox2.DataSource = dt;
                    comboBox2.SelectedIndex = 0;
                }
                else
                {
                    comboBox2.Items.Clear();
                    MessageBox.Show("Kein passender Tisch verfügbar ❌");
                }
            }
        }
        private void AktualisiereTischeAuto()
        {
            if (numericUpDown1.Value > 0 &&
                comboBox1.SelectedItem != null)
            {
                LadeTische((int)numericUpDown1.Value);
            }
        }




        private void Reservierungspeichern_Button(object sender, EventArgs e)
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
                // 🔥 Wenn Reservierung für heute → Uhrzeit prüfen
                if (dateTimePicker1.Value.Date == DateTime.Today)
                {
                    int gewaehlterSlot = HoleSlot();
                    int aktuelleStunde = DateTime.Now.Hour;

                    int aktuellerSlot = 0;

                    if (aktuelleStunde < 12|| aktuelleStunde > 14.5)
                        aktuellerSlot = 1;
                    else if (aktuelleStunde < 15|| aktuelleStunde > 17.5)
                        aktuellerSlot = 2;
                    else if (aktuelleStunde < 18 || aktuelleStunde > 20.5)
                        aktuellerSlot = 3;
                    else if (aktuelleStunde < 21 || aktuelleStunde > 21.5 )
                        aktuellerSlot = 4;
                    else
                    {
                        MessageBox.Show("Heute sind keine Reservierungen mehr möglich!");
                        return;
                    }

                    if (gewaehlterSlot < aktuellerSlot)
                    {
                        MessageBox.Show("Reservierung für vergangene Uhrzeit nicht möglich!");
                        return;
                    }
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
            // 🔥 WICHTIG: Datum auf heute setzen
            dateTimePicker1.Value = DateTime.Now;

            // 🔥 SLOT ZEITEN LADEN
            comboBox1.Items.Clear();
            comboBox1.Items.Add("12-15");
            comboBox1.Items.Add("15-18");
            comboBox1.Items.Add("18-21");
            comboBox1.Items.Add("21-24");
            comboBox1.SelectedIndex = 0;

            // 🔥 FARBEN AKTIVIEREN
            comboBox2.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox2.DrawItem += comboBox2_DrawItem;

            // Optional: Tische sofort laden, basierend auf dem neuen Datum
            AktualisiereTischeAuto();
        }





        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            AktualisiereTischeAuto();
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
            AktualisiereTischeAuto();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            AktualisiereTischeAuto();
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Nur Zahlen und Backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
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
        private void comboBox2_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();

            // 🔥 WICHTIG: Prüfen, ob das Element wirklich eine DataRowView ist (Daten aus DB)
            if (comboBox2.Items[e.Index] is DataRowView row)
            {
                string status = row["status"].ToString().ToLower();
                Color farbe = Color.Black;

                switch (status)
                {
                    case "frei": farbe = Color.Green; break;
                    case "reserviert": farbe = Color.Orange; break;
                    case "besetzt": farbe = Color.Red; break;
                    case "aktiv": farbe = Color.MediumPurple; break;
                }

                using (Brush brush = new SolidBrush(farbe))
                {
                    e.Graphics.DrawString(
                        row["Anzeige"].ToString(),
                        e.Font,
                        brush,
                        e.Bounds.Left,
                        e.Bounds.Top
                    );
                }
            }
            else
            {
                // 🔥 Fallback: Wenn es nur ein String ist (z.B. "Kein Tisch frei")
                e.Graphics.DrawString(
                    comboBox2.Items[e.Index].ToString(),
                    e.Font,
                    Brushes.Black, // Oder eine Farbe deiner Wahl
                    e.Bounds.Left,
                    e.Bounds.Top
                );
            }

            e.DrawFocusRectangle();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
            
        }
        private void StammgastLaden()
        {
            MySqlConnection conn = Database.GetConnection();
            {
                string query = @"SELECT gastvorname,gastnachname,telephonenr FROM gast ";
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridView1.DataSource = table;
                }
            }
        }

        private void guestuebernehmen_Click(object sender, EventArgs e)
        {
            dataGridView1.CurrentRow.Selected = true;
            textBox1.Text = dataGridView1.CurrentRow.Cells["gastvorname"].Value.ToString() + " " + dataGridView1.CurrentRow.Cells["gastnachname"].Value.ToString();
            textBox2.Text = dataGridView1.CurrentRow.Cells["telephonenr"].Value.ToString();
        }
    }
}
