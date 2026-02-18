using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Pizzeria_Projekt_Schule.Bestellungsseite;

namespace Pizzeria_Projekt_Schule
{
    public partial class Bestellungsseite : Form
    {
        public Bestellungsseite()
        {
            InitializeComponent();

            //button1.Click += Button1_Click; // +
           // button2.Click += Button2_Click; // -
        }


        private void Bestellungspagerichtig_Load(object sender, EventArgs e)
        {
            tischauswahl.DrawMode = DrawMode.OwnerDrawFixed;
            tischauswahl.DropDownStyle = ComboBoxStyle.DropDownList;
            tischauswahl.DrawItem += comboBox1_DrawItem;


            LadeTische();


            
            string query = "SELECT speise_id, speisename, preis FROM speisen WHERE aktiv = 1";

            MySqlConnection conn = Database.GetConnection();
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
            }

            // 🔥 DAS ist entscheidend
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            dataGridView1.Columns["preis"].DefaultCellStyle.Format = "C2";
            dataGridView1.Columns["preis"].DefaultCellStyle.FormatProvider =
                System.Globalization.CultureInfo.GetCultureInfo("de-DE");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Hauptmenu hauptmenu = new Hauptmenu();
            hauptmenu.Show();
            this.Close();
        }
        public class WarenkorbItem
        {
            public int SpeiseId { get; set; }
            public string Name { get; set; }
            public double Preis { get; set; }
            public int Menge { get; set; }

            public override string ToString()
            {
                return $"{Name} x{Menge}  ({Preis * Menge:0.00} €)";
            }
        }
        List<WarenkorbItem> warenkorb = new List<WarenkorbItem>();
        private void WarenkorbAktualisieren()
        {
            listBox1.Items.Clear();

            double summe = 0;

            foreach (var item in warenkorb)
            {
                listBox1.Items.Add(item);
                summe += item.Preis * item.Menge;
            }

            textBox1.Text = summe.ToString("0.00 €");

            // 🔥 AUTOMATISCHES AUSWÄHLEN
            if (listBox1.Items.Count > 0)
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }




        private void Button1_Click(object sender, EventArgs e)
        {
            

            WarenkorbAdd(dataGridView1.CurrentCell.RowIndex);
            WarenkorbAktualisieren();
        }
        private void mitarbeiterLaden()
        {
            // 🔒 Nur Servicekräfte laden
            string query = @"
        SELECT personalnr,
               CONCAT(vorname,' ',nachname) AS name
        FROM mitarbeiter
        WHERE rolle = 'service'
        AND aktiv = true";

            using (var conn = Database.GetConnection())
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);

                comboBox2.DisplayMember = "name";       // Anzeigename
                comboBox2.ValueMember = "personalnr";   // Wichtige ID
                comboBox2.DataSource = table;
            }
        }


        private void Button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem is WarenkorbItem item)
            {
                item.Menge--;
                if (item.Menge <= 0)
                    warenkorb.Remove(item);

                WarenkorbAktualisieren();
            }
        }
        private int BestellungAnlegen()
        {
            TischItem tisch = (TischItem)tischauswahl.SelectedItem;

            string query = @"
    INSERT INTO bestellungen (datum, tisch_id_fk, personalnr_fk, status)
    VALUES (NOW(), @tisch, @mitarbeiter, 'offen');

    SELECT LAST_INSERT_ID();
    ";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@tisch", tisch.TischId);
                cmd.Parameters.AddWithValue("@mitarbeiter", comboBox2.SelectedValue);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }





        private void Button3_Click_aa(object sender, EventArgs e)
        {
            // 1️⃣ Prüfungen
            if (warenkorb.Count == 0)
            {
                MessageBox.Show("Warenkorb ist leer!");
                return;
            }

            if (tischauswahl.SelectedItem == null)

            {
                MessageBox.Show("Bitte einen Tisch auswählen!");
                return;
            }

            if (comboBox2.SelectedValue == null)
            {
                MessageBox.Show("Bitte einen Mitarbeiter auswählen!");
                return;
            }

            // 2️⃣ Werte holen
            TischItem tisch = (TischItem)tischauswahl.SelectedItem;
            int tischId = tisch.TischId;

            int mitarbeiterId = Convert.ToInt32(comboBox2.SelectedValue);

            DateTime jetzt = DateTime.Now;

            // 3️⃣ Bestellung anlegen
            int bestellNr = BestellungAnlegen();

            // 4️⃣ Positionen speichern
            using (var conn = Database.GetConnection())
            {
                foreach (var item in warenkorb)
                {
                    string query = @"INSERT INTO bestellposition
                             (bestellnr_fk,   speise_id_fk, menge, preis_beim_kauf)
                             VALUES (@bnr ,@sid, @menge, @preis)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@bnr", bestellNr);
                        cmd.Parameters.AddWithValue("@sid", item.SpeiseId);
                        cmd.Parameters.AddWithValue("@menge", item.Menge);
                        cmd.Parameters.AddWithValue("@preis", item.Preis);
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            // 5️⃣ Tisch auf Besetzt setzen
            string update = @"UPDATE tische
                      SET lage = 'Besetzt'
                      WHERE tisch_id = @tid";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(update, conn))
            {
                cmd.Parameters.AddWithValue("@tid", tischId);
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Bestellung gespeichert 🍕");

            // 6️⃣ Aufräumen
            warenkorb.Clear();
            WarenkorbAktualisieren();
            tischauswahl.SelectedIndex = -1;
        }
        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;



            //WarenkorbAdd(e.RowIndex);

            WarenkorbAktualisieren();
        }

        private void WarenkorbAdd(int quelle)
           
        {
            if (quelle < 0) return;
            DataGridViewRow row = dataGridView1.Rows[quelle];
            int speiseId = Convert.ToInt32(row.Cells["speise_id"].Value);
            string name = row.Cells["speisename"].Value.ToString();
            double preis = Convert.ToDouble(row.Cells["preis"].Value);

            var item = warenkorb.FirstOrDefault(x => x.SpeiseId == speiseId);

            if (item != null)
                item.Menge++;
            else
                warenkorb.Add(new WarenkorbItem
                {
                    SpeiseId = speiseId,
                    Name = name,
                    Preis = preis,
                    Menge = 1
                });
        }


        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedValue == null)
                return;

            int personalNr = Convert.ToInt32(comboBox2.SelectedValue);

            string query = "SELECT bereich FROM mitarbeiter WHERE personalnr = @pnr";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@pnr", personalNr);

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    string bereich = result.ToString();

                    // 🔥 Nur Tische dieses Bereichs laden
                    LadeTische(bereich);
                }
            }
        }


        private void comboBox2_DropDown(object sender, EventArgs e)
        {
            string query = @"SELECT personalnr,
                     CONCAT(vorname,' ',nachname) AS name
                     FROM mitarbeiter
                     WHERE rolle = 'service'
                     AND aktiv = true";

            using (var conn = Database.GetConnection())
            {
                MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                comboBox2.DisplayMember = "name";
                comboBox2.ValueMember = "personalnr";
                comboBox2.DataSource = dt;
            }
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            //textBox1 DefaultCellStyle.FormatProvider =
            //   System.Globalization.CultureInfo.GetCultureInfo("de-DE"); 
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void LadeTische()
        {
            tischauswahl.Items.Clear();

            string query = "SELECT tisch_id, lage, bereich FROM tische WHERE aktiv = true";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    tischauswahl.Items.Add(new TischItem
                    {
                        TischId = reader.GetInt32("tisch_id"),
                        Status = reader.GetString("lage"),
                        Bereich = reader.GetString("bereich")
                    });
                }
            }
        }
        private void LadeTische(string bereich)
        {
            tischauswahl.Items.Clear();

            string query = @"
        SELECT tisch_id, lage, bereich
        FROM tische
        WHERE aktiv = true
        AND bereich = @bereich";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@bereich", bereich);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tischauswahl.Items.Add(new TischItem
                        {
                            TischId = reader.GetInt32("tisch_id"),
                            Status = reader.GetString("lage"),
                            Bereich = reader.GetString("bereich")
                        });
                    }
                }
            }
        }






        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            TischItem tisch = (TischItem)tischauswahl.Items[e.Index];
            e.DrawBackground();

            string status = tisch.Status.Trim().ToLower();

            Color farbe = Color.Black;

            if (status == "frei")
                farbe = Color.Green;
            else if (status == "reserviert")
                farbe = Color.Orange;
            else if (status == "besetzt")
                farbe = Color.Red;

            using (Brush brush = new SolidBrush(farbe))
            {
                e.Graphics.DrawString(
                    tisch.ToString(),
                    e.Font,
                    brush,
                    e.Bounds
                );
            }

            e.DrawFocusRectangle();
        }




        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            TischItem tisch = (TischItem)tischauswahl.SelectedItem;
            string status = tisch.Status.ToLower();

            // 🟠 FALL: RESERVIERT
            if (status == "reserviert")
            {
                var result = MessageBox.Show(
                    "Dieser Tisch ist reserviert.\nSind die Gäste da und soll der Tisch geöffnet werden?",
                    "Reservierung öffnen",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.No)
                {
                    tischauswahl.SelectedIndex = -1;
                    return;
                }

                using (var conn = Database.GetConnection())
                {
                    // 1️⃣ Tisch auf Besetzt setzen
                    string updateTisch = @"UPDATE tische
                               SET lage = 'Besetzt'
                               WHERE tisch_id = @tid";

                    using (var cmd = new MySqlCommand(updateTisch, conn))
                    {
                        cmd.Parameters.AddWithValue("@tid", tisch.TischId);
                        cmd.ExecuteNonQuery();
                    }

                    // 2️⃣ Reservierung auf AKTIV setzen
                    string updateReservierung = @"
            UPDATE reservierungen
            SET zustand = 'aktiv'
            WHERE tisch_id_fk = @tid
            AND DATE(datum) = CURDATE()
            AND zustand = 'offen'";

                    using (var cmd2 = new MySqlCommand(updateReservierung, conn))
                    {
                        cmd2.Parameters.AddWithValue("@tid", tisch.TischId);
                        cmd2.ExecuteNonQuery();
                    }
                }

                tisch.Status = "Besetzt";
                LadeTische();

                MessageBox.Show("Reservierung wurde aktiviert und Tisch ist jetzt besetzt.");
            }


        }
    }
}


