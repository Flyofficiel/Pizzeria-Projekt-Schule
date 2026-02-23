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
using System.Transactions;
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

            button3.Click += Button3_Click_aa;
            tischauswahl.SelectionChangeCommitted += tischauswahl_SelectionChangeCommitted;

          
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
            tischauswahl.DrawItem += tischauswahl_DrawItem;
            tischauswahl.DropDownStyle = ComboBoxStyle.DropDownList;
           
            dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged;


            mitarbeiterLaden();

            if (comboBox2.Items.Count > 0)
            {
                comboBox2.SelectedIndex = 0;
            }

            AktualisiereTische();



            string query = "SELECT speise_id, speisename, preis FROM speisen WHERE aktiv = 1";

            MySqlConnection conn = Database.GetConnection();
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;

                
                dataGridView1.ClearSelection();
                dataGridView1.CurrentCell = null;
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

                       
        }

    


        private void Button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Bitte zuerst eine Speise auswählen!");
                return;
            }

            int rowIndex = dataGridView1.SelectedRows[0].Index;

            WarenkorbAdd(rowIndex);
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
            else
            {
                MessageBox.Show("Bitte zuerst ein Produkt im Warenkorb auswählen!");
            }
        }






        private void Button3_Click_aa(object sender, EventArgs e)
        {
            if (warenkorb.Count == 0)
            {
                MessageBox.Show("Warenkorb ist leer!");
                return;
            }

            if (!(tischauswahl.SelectedItem is TischItem tisch))
            {
                MessageBox.Show("Bitte einen Tisch auswählen!");
                return;
            }

            if (comboBox2.SelectedValue == null)
            {
                MessageBox.Show("Bitte einen Mitarbeiter auswählen!");
                return;
            }


           

            using (var conn = Database.GetConnection())
            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    // 1️⃣ Bestellung speichern
                    string bestellQuery = @"
INSERT INTO bestellungen 
(datum, gast_id_fk, tisch_id_fk, personalnr_fk, status, slot)
VALUES 
(@datum, @gast, @tisch, @mitarbeiter, 'offen', @slot);
SELECT LAST_INSERT_ID();";

                    int gastId = 1; // Laufkunde ID
                    int bestellNr;   // WICHTIG → deklarieren!

                    using (var cmd = new MySqlCommand(bestellQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@datum", dateTimePicker1.Value.Date);
                        cmd.Parameters.AddWithValue("@gast", gastId); // 🔥 DAS HAT GEFEHLT
                        cmd.Parameters.AddWithValue("@tisch", tisch.TischId);
                        cmd.Parameters.AddWithValue("@mitarbeiter", comboBox2.SelectedValue);
                        cmd.Parameters.AddWithValue("@slot", HoleSlot());

                        bestellNr = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // 2️⃣ Positionen speichern
                    foreach (var item in warenkorb)
                    {
                        string posQuery = @"
INSERT INTO bestellposition
(bestellnr_fk, speise_id_fk, menge, preis_beim_kauf)
VALUES (@bnr, @sid, @menge, @preis)";

                        using (var cmd = new MySqlCommand(posQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@bnr", bestellNr);
                            cmd.Parameters.AddWithValue("@sid", item.SpeiseId);
                            cmd.Parameters.AddWithValue("@menge", item.Menge);
                            cmd.Parameters.AddWithValue("@preis", item.Preis);
                            cmd.ExecuteNonQuery();
                        }
                    }

                  

                   
                    

                    // 4️⃣ Alles speichern
                    transaction.Commit();
                    AktualisiereTische();
                    tischauswahl.Refresh();

                    MessageBox.Show("Bestellung gespeichert 🍕");

                    warenkorb.Clear();
                    WarenkorbAktualisieren();
                   
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Fehler beim Speichern: " + ex.Message);
                }
            }
        }
        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            dataGridView1.Rows[e.RowIndex].Selected = true;



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


       

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            //textBox1 DefaultCellStyle.FormatProvider =
            //   System.Globalization.CultureInfo.GetCultureInfo("de-DE"); 
        }

        
      private void combobox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            AktualisiereTische();
        }


        private void LadeTische(string bereich)
        {
            
            tischauswahl.Items.Clear();

            DateTime datum = dateTimePicker1.Value.Date;
            string query = @"
SELECT 
    t.tisch_id,
    t.bereich,
 CASE
    -- 🔴 1. Hat offene Bestellung?
    WHEN EXISTS (
        SELECT 1
        FROM bestellungen b
        WHERE b.tisch_id_fk = t.tisch_id
        AND b.slot = @slot
        AND b.status = 'offen'
    ) THEN 'Besetzt'

    -- 🔴 2. Reservierung ist AKTIV (Gast sitzt)
    WHEN EXISTS (
        SELECT 1
        FROM reservierungen r
        WHERE r.tisch_id_fk = t.tisch_id
        AND DATE(r.datum) = @datum
        AND r.slot = @slot
        AND r.zustand = 'aktiv'
    ) THEN 'Besetzt'

    -- 🟠 3. Reservierung nur offen (noch nicht da)
    WHEN EXISTS (
        SELECT 1
        FROM reservierungen r
        WHERE r.tisch_id_fk = t.tisch_id
        AND DATE(r.datum) = @datum
        AND r.slot = @slot
        AND r.zustand = 'offen'
    ) THEN 'Reserviert'

    -- 🟢 4. Sonst frei
    ELSE 'Frei'
END AS status

FROM tische t
WHERE t.aktiv = true
AND t.bereich = @bereich
ORDER BY t.tisch_id";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@datum", datum);
                cmd.Parameters.AddWithValue("@slot", HoleSlot());
                cmd.Parameters.AddWithValue("@bereich", bereich);

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
            if (comboBox1.SelectedIndex == -1)
                return 0;

            switch (comboBox1.SelectedIndex)
            {
                case 0: return 1;
                case 1: return 2;
                case 2: return 3;
                case 3: return 4;
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










        private void tischauswahl_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            TischItem tisch = (TischItem)tischauswahl.Items[e.Index];

            e.DrawBackground();

            Color farbe = Color.Green; // Standard Frei

            switch (tisch.Status.ToLower())
            {
                case "besetzt":
                    farbe = Color.Red;
                    break;

                case "reserviert":
                    farbe = Color.Orange;
                    break;

                case "frei":
                    farbe = Color.Green;
                    break;
            }

            using (Brush brush = new SolidBrush(farbe))
            {
                e.Graphics.DrawString(
                    tisch.ToString(),
                    e.Font,
                    brush,
                    e.Bounds.Left,
                    e.Bounds.Top
                );
            }

            e.DrawFocusRectangle();
        }




        private void tischauswahl_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (tischauswahl.SelectedItem == null) return;

            TischItem tisch = (TischItem)tischauswahl.SelectedItem;

           

            string status = tisch.Status.ToLower();

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
                    string updateReservierung = @"
            UPDATE reservierungen
            SET zustand = 'aktiv'
            WHERE tisch_id_fk = @tid
            AND DATE(datum) = @datum
            AND slot = @slot
            AND zustand = 'offen'";

                    using (var cmd = new MySqlCommand(updateReservierung, conn))
                    {
                        cmd.Parameters.AddWithValue("@tid", tisch.TischId);
                        cmd.Parameters.AddWithValue("@datum", dateTimePicker1.Value.Date);
                        cmd.Parameters.AddWithValue("@slot", HoleSlot());
                        cmd.ExecuteNonQuery();
                    }
                }

                AktualisiereTische();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

       

        private void tischauswahl_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}


