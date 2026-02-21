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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
            comboBox1.Items.Clear();
            comboBox1.Items.Add("12-15");
            comboBox1.Items.Add("15-18");
            comboBox1.Items.Add("18-21");
            comboBox1.Items.Add("21-24");
            comboBox1.SelectedIndex = 0;
            tischauswahl.DrawMode = DrawMode.OwnerDrawFixed;
            tischauswahl.DropDownStyle = ComboBoxStyle.DropDownList;
            tischauswahl.DrawItem += comboBox1_DrawItem;
            dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged;

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
            DateTime ausgewaehltesDatum = dateTimePicker1.Value.Date;
            string query = @"
INSERT INTO bestellungen (datum, tisch_id_fk, personalnr_fk, status, slot)
VALUES (@datum, @tisch, @mitarbeiter, 'offen', @slot);

SELECT LAST_INSERT_ID();
";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@datum", ausgewaehltesDatum);
                cmd.Parameters.AddWithValue("@tisch", tisch.TischId);
                cmd.Parameters.AddWithValue("@mitarbeiter", comboBox2.SelectedValue);
                cmd.Parameters.AddWithValue("@slot", HoleSlot());

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


        private void combobox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AktualisiereTische();
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

        
      private void combobox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            AktualisiereTische();
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

            DateTime datum = dateTimePicker1.Value.Date;

            string query = @"
SELECT t.tisch_id,
       t.bereich,

CASE
    WHEN EXISTS (
        SELECT 1 FROM bestellungen b
        WHERE b.tisch_id_fk = t.tisch_id
        AND DATE(b.datum) = @datum
        AND b.status = 'offen'
        AND b.slot = @slot
    ) THEN 'Besetzt'

    WHEN EXISTS (
        SELECT 1 FROM reservierungen r
        WHERE r.tisch_id_fk = t.tisch_id
        AND DATE(r.datum) = @datum
        AND r.slot = @slot
        AND r.zustand = 'offen'
    ) THEN 'Reserviert'

    ELSE 'Frei'
END AS status

FROM tische t
WHERE t.aktiv = true
AND t.bereich = @bereich";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@bereich", bereich);
                cmd.Parameters.AddWithValue("@datum", datum);
                cmd.Parameters.AddWithValue("@slot", HoleSlot());

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tischauswahl.Items.Add(new TischItem
                        {
                            TischId = reader.GetInt32("tisch_id"),
                            Status = reader.GetString("status"),
                            Bereich = reader.GetString("bereich")
                        });
                    }
                }
            }
        }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            AktualisiereTische();
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
        private void AktualisiereTische()
        {
            if (comboBox2.SelectedValue == null)
            {
                tischauswahl.Items.Clear();
                return;
            }

            if (HoleSlot() == 0)
            {
                tischauswahl.Items.Clear();
                return;
            }

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
                    LadeTische(bereich);
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
AND DATE(datum) = @datum
AND slot = @slot
AND zustand = 'offen'";

                    using (var cmd2 = new MySqlCommand(updateReservierung, conn))
                    {
                        cmd2.Parameters.AddWithValue("@tid", tisch.TischId);
                        cmd2.Parameters.AddWithValue("@datum", dateTimePicker1.Value.Date);
                        cmd2.Parameters.AddWithValue("@slot", HoleSlot());


                    }
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void combobox1SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedValue == null)
                return;

            if (HoleSlot() == 0)
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
                    LadeTische(bereich);
                }
            }
        }

        
    }
}


