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
        private Timer timer1; // Timer für die Echtzeit-Uhr oben rechts

        public Bestellungsseite()
        {
            InitializeComponent();

            // Verknüpfung der Events und Start des Timers für die Uhrzeit
            tischauswahl.SelectionChangeCommitted += tischauswahl_SelectionChangeCommitted;
            timer1 = new Timer();
            timer1.Interval = 1000; // 1 Sekunde
            timer1.Tick += Timer1_Tick;
            timer1.Start();

            timenow(); // Uhrzeit sofort beim Start anzeigen
        }

        // Methode, um die aktuelle Uhrzeit schön formatiert anzuzeigen
        private void timenow()
        {
            label7.Text = DateTime.Now.ToString("HH:mm:ss dddd, dd.MM.yyyy",
            System.Globalization.CultureInfo.GetCultureInfo("de-DE"));
        }

        private void Bestellungspagerichtig_Load(object sender, EventArgs e)
        {
            // Zeitslots für die Pizzeria festlegen
            slot_comboBox1_.Items.Clear();
            slot_comboBox1_.Items.Add("12-15");
            slot_comboBox1_.Items.Add("15-18");
            slot_comboBox1_.Items.Add("18-21");
            slot_comboBox1_.Items.Add("21-24");
            slot_comboBox1_.SelectedIndex = 0;

            // Tischauswahl optisch anpassen (OwnerDraw damit wir Farben nutzen können)
            tischauswahl.DrawMode = DrawMode.OwnerDrawFixed;
            tischauswahl.DrawItem += tischauswahl_DrawItem;
            tischauswahl.DropDownStyle = ComboBoxStyle.DropDownList;

            dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged;

            mitarbeiterLaden(); // Lädt nur Service-Mitarbeiter in die ComboBox

            if (tischauswahl_comboBox2.Items.Count > 0)
            {
                tischauswahl_comboBox2.SelectedIndex = 0;
            }

            AktualisiereTische(); // Zeigt an, welche Tische frei/besetzt sind

            // Speisekarte aus der Datenbank in das Grid laden
            string query = "SELECT speise_id, speisename, preis FROM speisen WHERE aktiv = 1";

            using (var conn = Database.GetConnection())
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;

                dataGridView1.ClearSelection();
                dataGridView1.CurrentCell = null;
            }

            // Einstellungen für das Grid: Man wählt immer die ganze Zeile aus
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            // Preisspalte als Währung (€) formatieren
            dataGridView1.Columns["preis"].DefaultCellStyle.Format = "C2";
            dataGridView1.Columns["preis"].DefaultCellStyle.FormatProvider =
                System.Globalization.CultureInfo.GetCultureInfo("de-DE");
        }

        private void abbrechen_button5_Click(object sender, EventArgs e)
        {
            // Zurück zum Hauptmenü
            Hauptmenu hauptmenu = new Hauptmenu();
            hauptmenu.Show();
            this.Close();
        }

        // Hilfsklasse für die Artikel im Warenkorb
        public class WarenkorbItem
        {
            public int SpeiseId { get; set; }
            public string Name { get; set; }
            public decimal Preis { get; set; }
            public int Menge { get; set; }

            // Wie das Item in der ListBox angezeigt wird
            public override string ToString()
            {
                return $"{Name} x{Menge}  ({Preis * Menge:0.00} €)";
            }
        }

        List<WarenkorbItem> warenkorb = new List<WarenkorbItem>();

        // Aktualisiert die Anzeige der Liste und berechnet die Gesamtsumme
        private void WarenkorbAktualisieren()
        {
            Bestellkorb_listBox1.Items.Clear();
            decimal summe = 0;

            foreach (var item in warenkorb)
            {
                Bestellkorb_listBox1.Items.Add(item);
                summe += item.Preis * item.Menge;
            }

            summe_TextBox1.Text = summe.ToString("C2",
            System.Globalization.CultureInfo.GetCultureInfo("de-DE"));
        }

        // --- EVENT: SPEISE HINZUFÜGEN ---
        private void hinzufugen_Button1_Click(object sender, EventArgs e)
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

        // Lädt die Mitarbeiter mit der Rolle 'service' aus der DB
        private void mitarbeiterLaden()
        {
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

                tischauswahl_comboBox2.DisplayMember = "name";
                tischauswahl_comboBox2.ValueMember = "personalnr";
                tischauswahl_comboBox2.DataSource = table;
            }
        }

        // --- EVENT: ARTIKEL AUS WARENKORB ENTFERNEN ---
        private void loschen_Button2_Click(object sender, EventArgs e)
        {
            if (Bestellkorb_listBox1.SelectedItem is WarenkorbItem item)
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

        // --- EVENT: BESTELLUNG ABSCHLIEẞEN (Wichtigster Teil!) ---
        private void an_kuche_Button3_Click_aa(object sender, EventArgs e)
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

            if (tischauswahl_comboBox2.SelectedValue == null)
            {
                MessageBox.Show("Bitte einen Mitarbeiter auswählen!");
                return;
            }

            int zuletztBestellterTischId = tisch.TischId;

            // Transaktion: Entweder alles speichern oder gar nichts (Sicherheit!)
            using (var conn = Database.GetConnection())
            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    // 1. Check: Ist der Tisch reserviert? Dann Gast-ID holen.
                    int gastId;
                    string reservierungsCheck = @"
                SELECT gastid_fk 
                FROM reservierungen 
                WHERE tisch_id_fk = @tid 
                AND DATE(datum) = @datum 
                AND slot = @slot 
                AND zustand = 'aktiv' 
                LIMIT 1";

                    using (var checkCmd = new MySqlCommand(reservierungsCheck, conn, transaction))
                    {
                        checkCmd.Parameters.AddWithValue("@tid", tisch.TischId);
                        checkCmd.Parameters.AddWithValue("@datum", dateTimePicker1.Value.Date);
                        checkCmd.Parameters.AddWithValue("@slot", HoleSlot());
                        object resGast = checkCmd.ExecuteScalar();

                        if (resGast != null && resGast != DBNull.Value)
                            gastId = Convert.ToInt32(resGast);
                        else
                            gastId = GetLaufgastId(conn, transaction); // Sonst Standard-Laufgast
                    }

                    // 2. Kopfdaten der Bestellung speichern
                    string bestellQuery = @"
                INSERT INTO bestellungen 
                (datum, gast_id_fk, tisch_id_fk, personalnr_fk, status, slot)
                VALUES 
                (@datum, @gast, @tisch, @mitarbeiter, 'offen', @slot);
                SELECT LAST_INSERT_ID();";

                    int bestellNr;
                    using (var cmd = new MySqlCommand(bestellQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@datum", dateTimePicker1.Value.Date);
                        cmd.Parameters.AddWithValue("@gast", gastId);
                        cmd.Parameters.AddWithValue("@tisch", tisch.TischId);
                        cmd.Parameters.AddWithValue("@mitarbeiter", tischauswahl_comboBox2.SelectedValue);
                        cmd.Parameters.AddWithValue("@slot", HoleSlot());
                        bestellNr = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // 3. Jede einzelne Position (Pizza etc.) speichern
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

                    transaction.Commit(); // Erst jetzt wird alles fest in die DB geschrieben

                    AktualisiereTische(); // Tisch-Farben auffrischen
                    MessageBox.Show("Bestellung gespeichert 🍕");
                    warenkorb.Clear();
                    WarenkorbAktualisieren();

                    // GUI: Den Tisch wieder selektieren
                    for (int i = 0; i < tischauswahl.Items.Count; i++)
                    {
                        if (tischauswahl.Items[i] is TischItem item && item.TischId == zuletztBestellterTischId)
                        {
                            tischauswahl.SelectedIndex = i;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Bei Fehler: Alle Änderungen rückgängig machen
                    MessageBox.Show("Fehler beim Speichern: " + ex.Message);
                }
            }
        }

        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            dataGridView1.Rows[e.RowIndex].Selected = true;
            WarenkorbAktualisieren();
        }

        // Logik um ein Item zum Warenkorb hinzuzufügen oder die Menge zu erhöhen
        private void WarenkorbAdd(int quelle)
        {
            if (quelle < 0) return;
            DataGridViewRow row = dataGridView1.Rows[quelle];
            int speiseId = Convert.ToInt32(row.Cells["speise_id"].Value);
            string name = row.Cells["speisename"].Value.ToString();
            decimal preis = Convert.ToDecimal(row.Cells["preis"].Value);

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

        private void slot_comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AktualisiereTische();
        }

        private void summe_TextBox1_TextChanged(object sender, EventArgs e) { }

        private void Mitarbeiter_combobox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            AktualisiereTische();
        }

        // Lädt Tische basierend auf dem Bereich und zeigt deren Status (Frei/Besetzt/Reserviert)
        private void LadeTische(string bereich)
        {
            tischauswahl.Items.Clear();
            DateTime datum = dateTimePicker1.Value.Date;

            // SQL-Logik: Prüft in Bestellungen und Reservierungen, was mit dem Tisch los ist
            string query = @"
SELECT  
    t.tisch_id,
    t.bereich,
    CASE
        WHEN EXISTS (
            SELECT 1 FROM bestellungen b
            WHERE b.tisch_id_fk = t.tisch_id AND b.slot = @slot AND b.status = 'offen'
        ) THEN 'Besetzt'
        WHEN EXISTS (
            SELECT 1 FROM reservierungen r
            WHERE r.tisch_id_fk = t.tisch_id AND DATE(r.datum) = @datum AND r.slot = @slot AND r.zustand = 'aktiv'
        ) THEN 'Besetzt'
        WHEN EXISTS (
            SELECT 1 FROM reservierungen r
            WHERE r.tisch_id_fk = t.tisch_id AND DATE(r.datum) = @datum AND r.slot = @slot AND r.zustand = 'offen'
        ) THEN 'Reserviert'
        ELSE 'Frei'
    END AS status
FROM tische t
WHERE t.aktiv = true AND t.bereich = @bereich
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

        // Wandelt den ComboBox-Index in die Slot-Nummer der DB um
        private int HoleSlot()
        {
            if (slot_comboBox1_.SelectedIndex == -1) return 0;
            return slot_comboBox1_.SelectedIndex + 1;
        }

        private void AktualisiereTischeAuto() { }

        // Steuert, welcher Bereich (z.B. Terrasse/Saal) für den Mitarbeiter geladen wird
        private void AktualisiereTische()
        {
            int? aktuellGewaehlterTischId = null;
            if (tischauswahl.SelectedItem is TischItem alterTisch)
                aktuellGewaehlterTischId = alterTisch.TischId;

            if (tischauswahl_comboBox2.SelectedValue == null || HoleSlot() == 0)
            {
                tischauswahl.Items.Clear();
                return;
            }

            int personalNr = Convert.ToInt32(tischauswahl_comboBox2.SelectedValue);
            string query = "SELECT bereich FROM mitarbeiter WHERE personalnr = @pnr";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@pnr", personalNr);
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    LadeTische(result.ToString());

                    if (aktuellGewaehlterTischId.HasValue)
                    {
                        for (int i = 0; i < tischauswahl.Items.Count; i++)
                        {
                            if (tischauswahl.Items[i] is TischItem t && t.TischId == aktuellGewaehlterTischId.Value)
                            {
                                tischauswahl.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private int GetLaufgastId(MySqlConnection conn, MySqlTransaction transaction)
        {
            string query = "SELECT gastid FROM gast WHERE laufgast = true LIMIT 1";
            using (var cmd = new MySqlCommand(query, conn, transaction))
            {
                object result = cmd.ExecuteScalar();
                if (result == null) throw new Exception("Kein Laufgast gefunden!");
                return Convert.ToInt32(result);
            }
        }

        // Zeichnet die Tisch-Items in der ComboBox farbig (Rot = Besetzt, Grün = Frei)
        private void tischauswahl_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            TischItem tisch = (TischItem)tischauswahl.Items[e.Index];
            e.DrawBackground();

            Color farbe = Color.Green;
            switch (tisch.Status.ToLower())
            {
                case "besetzt": farbe = Color.Red; break;
                case "reserviert": farbe = Color.Orange; break;
                case "frei": farbe = Color.Green; break;
            }

            using (Brush brush = new SolidBrush(farbe))
            {
                e.Graphics.DrawString(tisch.ToString(), e.Font, brush, e.Bounds.Left, e.Bounds.Top);
            }
            e.DrawFocusRectangle();
        }

        // Logik: Wenn ein reservierter Tisch ausgewählt wird, fragen ob die Gäste da sind
        private void tischauswahl_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (tischauswahl.SelectedItem == null) return;
            TischItem tisch = (TischItem)tischauswahl.SelectedItem;

            if (tisch.Status.ToLower() == "reserviert")
            {
                var result = MessageBox.Show("Gäste da? Tisch jetzt öffnen?", "Reservierung", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    tischauswahl.SelectedIndex = -1;
                    return;
                }

                using (var conn = Database.GetConnection())
                {
                    string update = "UPDATE reservierungen SET zustand = 'aktiv' WHERE tisch_id_fk = @tid AND DATE(datum) = @datum AND slot = @slot AND zustand = 'offen'";
                    using (var cmd = new MySqlCommand(update, conn))
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

        private void Bestellkorb_listBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void tischauswahl_comboBox2_SelectedIndexChanged(object sender, EventArgs e) { AktualisiereTischeAuto(); }
        private void slot_comboBox1_SelectedIndexChanged_1(object sender, EventArgs e) { AktualisiereTische(); }
        private void dateTimePicker1_ValueChanged_1(object sender, EventArgs e) { AktualisiereTischeAuto(); }
        private void label7_Click(object sender, EventArgs e) { }

        // Tick-Event für die Uhrzeit
        private void Timer1_Tick(object sender, EventArgs e)
        {
            timenow();
        }

        private void Bestellung_dateTimePicker1_ValueChanged_2(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}