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

namespace Pizzeria_Projekt_Schule
{
    public partial class Bestellungspagerichtig : Form
    {
        public Bestellungspagerichtig()
        {
            InitializeComponent();

            button1.Click += button1_Click; // +
            button2.Click += button2_Click; // -
        }


        private void Bestellungspagerichtig_Load(object sender, EventArgs e)
        {
            comboBox1.DrawMode = DrawMode.OwnerDrawFixed;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.DrawItem += comboBox1_DrawItem;
            LadeTische();


            string connString = "server=localhost;uid=root;pwd=root;database=pizzaprojekt";
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




        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Bitte zuerst ein Gericht auswählen.");
                return;
            }

            var item = (WarenkorbItem)listBox1.SelectedItem;
            item.Menge++;
            WarenkorbAktualisieren();
        }
        private void mitarbeiterLaden()
        {
            string query = "SELECT vorname FROM mitarbeiter WHERE aktiv = 1";
            MySqlConnection conn = Database.GetConnection();
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);

            comboBox2.DataSource = table;
        }

        private void button2_Click(object sender, EventArgs e)
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
            string connString = "server=localhost;uid=root;pwd=root;database=pizzaprojekt";

            string query = @"
        INSERT INTO bestellungen (datum, tisch_id_fk, personalnr_fk)
        VALUES (NOW(), @tisch, @mitarbeiter);
        SELECT LAST_INSERT_ID();
    ";

            MySqlConnection conn = Database.GetConnection();
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@tisch", comboBox1.SelectedValue);
                cmd.Parameters.AddWithValue("@mitarbeiter", comboBox2.SelectedValue);


                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }


        private void button3_Click(object sender, EventArgs e)
        { // 1️⃣ Pflichtprüfungen
            if (warenkorb.Count == 0)
            {
                MessageBox.Show("Warenkorb ist leer!");
                return;
            }

            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Bitte einen Tisch auswählen!");
                return;
            }

            if (comboBox2.SelectedValue == null)
            {
                MessageBox.Show("Bitte einen Mitarbeiter auswählen!");
                return;
            }

            // 2️⃣ Tisch holen (DAS war deine Frage)
            TischItem tisch = (TischItem)comboBox1.SelectedItem;
            int tischId = tisch.TischId;

            // 3️⃣ Mitarbeiter holen
            int mitarbeiterId = Convert.ToInt32(comboBox2.SelectedValue);

            // 4️⃣ Bestellung anlegen
            int bestellNr = BestellungAnlegen();

            // 5️⃣ Bestellpositionen speichern
            using (var conn = Database.GetConnection())
            {
                foreach (var item in warenkorb)
                {
                    string query = @"INSERT INTO bestellposition
                             (bestellnr_fk, speise_id_fk, menge, preis_beim_kauf)
                             VALUES (@bnr, @sid, @menge, @preis)";

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

            // 6️⃣ Tisch auf BESetzt setzen 🔴
            string update = @"UPDATE tische
                      SET lage = 'Besetzt'
                      WHERE tisch_id = @tid";

            using (var cmd = new MySqlCommand(update, Database.GetConnection()))
            {
                cmd.Parameters.AddWithValue("@tid", tischId);
                cmd.ExecuteNonQuery();
            }

            // 7️⃣ Aufräumen & neu laden
            MessageBox.Show("Bestellung an Küche gesendet 🍕🔥");

            warenkorb.Clear();
            WarenkorbAktualisieren();
            LadeTische();
            comboBox1.SelectedIndex = -1;
        
        }

        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            int speiseId = Convert.ToInt32(row.Cells["speise_id"].Value);
            string name = row.Cells["speisename"].Value.ToString();
            double preis = Convert.ToDouble(row.Cells["preis"].Value);

            var item = warenkorb.FirstOrDefault(x => x.SpeiseId == speiseId);

            if (item != null)
            {
                item.Menge++;
            }
            else
            {
                warenkorb.Add(new WarenkorbItem
                {
                    SpeiseId = speiseId,
                    Name = name,
                    Preis = preis,
                    Menge = 1
                });
            }

            WarenkorbAktualisieren();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {


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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //textBox1 DefaultCellStyle.FormatProvider =
            //   System.Globalization.CultureInfo.GetCultureInfo("de-DE"); 
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void LadeTische()
        {

            comboBox1.Items.Clear();

            string query = "SELECT tisch_id, lage, berreich FROM tische WHERE aktiv = true";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    comboBox1.Items.Add(new TischItem
                    {
                        TischId = reader.GetInt32("tisch_id"),
                        Status = reader.GetString("lage").Trim(),   // 🔥 HIER
                        Bereich = reader.GetString("berreich")
                    });
                }
            }

            comboBox1.Invalidate(); // Erzwingt Neuzeichnen
        }


        private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            TischItem tisch = (TischItem)comboBox1.Items[e.Index];
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
            TischItem tisch = (TischItem)comboBox1.SelectedItem;
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

                // ❌ Nein → nichts tun, Auswahl zurücksetzen
                if (result == DialogResult.No)
                {
                    comboBox1.SelectedIndex = -1;
                    return;
                }

                // ✅ Ja → Tisch auf BESetzt setzen
                string update = @"UPDATE tische
                          SET lage = 'Besetzt'
                          WHERE tisch_id = @tid";

                using (var cmd = new MySqlCommand(update, Database.GetConnection()))
                {
                    cmd.Parameters.AddWithValue("@tid", tisch.TischId);
                    cmd.ExecuteNonQuery();
                }

                // Status im Objekt auch ändern
                tisch.Status = "Besetzt";

                // Farben neu zeichnen
                LadeTische();

                MessageBox.Show("Tisch wurde geöffnet und ist jetzt besetzt.");
            }

            // 🟢 Frei & 🔴 Besetzt → einfach erlauben

        }
    }
}
