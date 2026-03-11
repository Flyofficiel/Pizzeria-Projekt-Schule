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
    // Diese Seite ist das Herzstück: Hier werden Bestellungen aufgenommen, 
    // Tische zugewiesen und alles an die Datenbank übertragen.
    public partial class Bestellungsseite : Form
    {
        private Timer timer1; // Timer für die Live-Uhr oben rechts

        public Bestellungsseite()
        {
            InitializeComponent();

            // Wir verknüpfen das Auswahl-Event für die Tische
            tischauswahl.SelectionChangeCommitted += Tischauswahl_SelectionChangeCommitted;

            // Der Timer aktualisiert jede Sekunde die Uhrzeit
            timer1 = new Timer();
            timer1.Interval = 1000;
            timer1.Tick += Timer1_Tick;
            timer1.Start();

            Timenow(); // Uhrzeit direkt beim Öffnen anzeigen
        }

        // Hilfsfunktion für das deutsche Zeitformat
        private void Timenow()
        {
            Uhrzeitlabel.Text = DateTime.Now.ToString("HH:mm:ss dddd, dd.MM.yyyy",
            System.Globalization.CultureInfo.GetCultureInfo("de-DE"));
        }

        // Beim Laden der Seite bereiten wir alle Listen und Dropdowns vor
        private void Bestellungspagerichtig_Load(object sender, EventArgs e)
        {
            Bestellkorb_listBox1.Font = new Font("Segoe UI", 12, FontStyle.Regular);

            // 1. Zeit-Slots (Uhrzeiten) festlegen
            slot_comboBox1_.Items.Clear();
            slot_comboBox1_.Items.AddRange(new string[] { "12-15", "15-18", "18-21", "21-24" });
            slot_comboBox1_.SelectedIndex = 0;
            slot_comboBox1_.DropDownStyle = ComboBoxStyle.DropDownList; // Verhindert Tippen in der Box

            // 2. Tischauswahl: Wir nutzen 'OwnerDraw', um die Tische später farbig zu markieren
            tischauswahl.DrawMode = DrawMode.OwnerDrawFixed;
            tischauswahl.DrawItem += Tischauswahl_DrawItem;
            tischauswahl.DropDownStyle = ComboBoxStyle.DropDownList;

            // 3. Service-Personal aus der Datenbank laden
            MitarbeiterLaden();
            tischauswahl_comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;

            if (tischauswahl_comboBox2.Items.Count > 0)
            {
                tischauswahl_comboBox2.SelectedIndex = 0;
            }

            // 4. Kalender-Event verknüpfen und Tische laden
            dateTimePicker1.ValueChanged += DateTimePicker1_ValueChanged;
            AktualisiereTische();

            // 5. Speisekarte (Produkte) aus der DB laden
            string query = "SELECT speise_id, speisename, preis FROM speisen WHERE aktiv = 1";
            using (var conn = Database.GetConnection())
            {
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
                BestellGridDesign(); // Optik anpassen

                dataGridView1.ClearSelection();
                dataGridView1.CurrentCell = null;
            }

            // Grid-Sicherheit: Nur ganze Zeilen wählen, kein Bearbeiten möglich
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;

            // Preisspalte als Euro formatieren
            if (dataGridView1.Columns.Contains("preis"))
            {
                dataGridView1.Columns["preis"].DefaultCellStyle.Format = "C2";
                dataGridView1.Columns["preis"].DefaultCellStyle.FormatProvider =
                    System.Globalization.CultureInfo.GetCultureInfo("de-DE");
            }
        }

        // Zurück zum Hauptmenü
        private void Abbrechen_button5_Click(object sender, EventArgs e)
        {
            Hauptmenu hauptmenu = new Hauptmenu();
            hauptmenu.Show();
            this.Close();
        }

        // Eine kleine Hilfsklasse für die Liste im Warenkorb
        public class WarenkorbItem
        {
            public int SpeiseId { get; set; }
            public string Name { get; set; }
            public decimal Preis { get; set; }
            public int Menge { get; set; }

            // Bestimmt, wie der Text in der Warenkorb-Liste links aussieht
            public override string ToString()
            {
                return $"{Name} x{Menge}  ({Preis * Menge:0.00} €)";
            }
        }

        // Hier speichern wir die aktuell gewählten Pizzen/Getränke zwischen
        List<WarenkorbItem> warenkorb = new List<WarenkorbItem>();

        // Aktualisiert die ListBox und berechnet die Summe neu
        private void WarenkorbAktualisieren()
        {
            Bestellkorb_listBox1.Items.Clear();
            decimal summe = 0;

            foreach (var item in warenkorb)
            {
                Bestellkorb_listBox1.Items.Add(item);
                summe += item.Preis * item.Menge;
            }

            // Gesamtsumme im Textfeld anzeigen (Euro-Format)
            summe_TextBox1.Text = summe.ToString("C2",
            System.Globalization.CultureInfo.GetCultureInfo("de-DE"));
        }

        // --- ARTIKEL HINZUFÜGEN ---
        private void Hinzufugen_Button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Bitte zuerst ein Gericht aus der Liste wählen!");
                return;
            }

            int rowIndex = dataGridView1.SelectedRows[0].Index;
            WarenkorbAdd(rowIndex); // Logik zum Hinzufügen/Erhöhen
            WarenkorbAktualisieren();
        }

        // Lädt alle Mitarbeiter, die im 'Service' arbeiten, für die Zuweisung
        private void MitarbeiterLaden()
        {
            string query = @"
                SELECT personalnr, CONCAT(vorname,' ',nachname) AS name
                FROM mitarbeiter
                WHERE rolle = 'service' AND aktiv = true";

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

        // --- ARTIKEL ENTFERNEN ---
        private void Loeschen_Button2_Click(object sender, EventArgs e)
        {
            if (Bestellkorb_listBox1.SelectedItem is WarenkorbItem item)
            {
                item.Menge--; // Menge um 1 reduzieren
                if (item.Menge <= 0)
                    warenkorb.Remove(item); // Ganz löschen, wenn 0 erreicht

                WarenkorbAktualisieren();
            }
            else
            {
                MessageBox.Show("Bitte wähle den Artikel im Warenkorb aus, den du entfernen willst.");
            }
        }

        // --- BESTELLUNG ABSCHLIESSEN (TRANSAKTION) ---
        private void An_kueche_Button3_Click_aa(object sender, EventArgs e)
        {
            // Pflichtfelder prüfen
            if (warenkorb.Count == 0) { MessageBox.Show("Der Warenkorb ist noch leer!"); return; }
            if (!(tischauswahl.SelectedItem is TischItem tisch)) { MessageBox.Show("Bitte einen Tisch wählen!"); return; }
            if (tischauswahl_comboBox2.SelectedValue == null) { MessageBox.Show("Wer bedient diesen Tisch? (Mitarbeiter wählen)"); return; }

            int zuletztBestellterTischId = tisch.TischId;

            // Wir nutzen eine Transaktion: Wenn beim Speichern der 10. Pizza ein Fehler auftritt, 
            // wird auch die Bestellung an sich nicht gespeichert (ganz oder gar nicht!).
            using (var conn = Database.GetConnection())
            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    // 1. Prüfen, ob für diesen Tisch gerade eine Reservierung läuft
                    int gastId;
                    string reservierungsCheck = @"
                        SELECT gastid_fk FROM reservierungen 
                        WHERE tisch_id_fk = @tid AND DATE(datum) = @datum 
                        AND slot = @slot AND zustand = 'aktiv' LIMIT 1";

                    using (var checkCmd = new MySqlCommand(reservierungsCheck, conn, transaction))
                    {
                        checkCmd.Parameters.AddWithValue("@tid", tisch.TischId);
                        checkCmd.Parameters.AddWithValue("@datum", dateTimePicker1.Value.Date);
                        checkCmd.Parameters.AddWithValue("@slot", HoleSlot());
                        object resGast = checkCmd.ExecuteScalar();

                        if (resGast != null && resGast != DBNull.Value)
                            gastId = Convert.ToInt32(resGast);
                        else
                            gastId = GetLaufgastId(conn, transaction); // Standard-Gast (Laufkundschaft)
                    }

                    // 2. Die Haupt-Bestellung anlegen
                    string bestellQuery = @"
                        INSERT INTO bestellungen (gast_id_fk, tisch_id_fk, personalnr_fk, status, slot)
                        VALUES (@gast, @tisch, @mitarbeiter, 'offen', @slot);
                        SELECT LAST_INSERT_ID();";

                    int bestellNr;
                    using (var cmd = new MySqlCommand(bestellQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@gast", gastId);
                        cmd.Parameters.AddWithValue("@tisch", tisch.TischId);
                        cmd.Parameters.AddWithValue("@mitarbeiter", tischauswahl_comboBox2.SelectedValue);
                        cmd.Parameters.AddWithValue("@slot", HoleSlot());
                        bestellNr = Convert.ToInt32(cmd.ExecuteScalar()); // Wir brauchen die neue ID für die Positionen
                    }

                    // 3. Alle Pizzen/Getränke aus dem Warenkorb einzeln in 'bestellposition' speichern
                    foreach (var item in warenkorb)
                    {
                        string posQuery = @"
                            INSERT INTO bestellposition (bestellnr_fk, speise_id_fk, menge, preis_beim_kauf)
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

                    transaction.Commit(); // Alles fertig? Dann ab in die Datenbank!

                    AktualisiereTische(); // Tisch-Farben updaten
                    MessageBox.Show("Bestellung wurde an die Küche übermittelt! 🍕");
                    warenkorb.Clear();
                    WarenkorbAktualisieren();
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Bei Fehlern: Alles rückgängig machen!
                    MessageBox.Show("Fehler beim Speichern: " + ex.Message);
                }
            }
        }

        private void DataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            dataGridView1.Rows[e.RowIndex].Selected = true;
        }

        // Interne Logik: Speise zum Warenkorb-Objekt hinzufügen
        private void WarenkorbAdd(int quelle)
        {
            if (quelle < 0) return;
            DataGridViewRow row = dataGridView1.Rows[quelle];
            int speiseId = Convert.ToInt32(row.Cells["speise_id"].Value);
            string name = row.Cells["speisename"].Value.ToString();
            decimal preis = Convert.ToDecimal(row.Cells["preis"].Value);

            // Prüfen, ob die Pizza schon im Korb ist
            var item = warenkorb.FirstOrDefault(x => x.SpeiseId == speiseId);

            if (item != null)
                item.Menge++; // Nur Menge erhöhen
            else
                warenkorb.Add(new WarenkorbItem { SpeiseId = speiseId, Name = name, Preis = preis, Menge = 1 });
        }

        private void Slot_comboBox1_SelectedIndexChanged(object sender, EventArgs e) { AktualisiereTische(); }

        // Lädt die Tische und prüft den Status (Besetzt/Frei/Reserviert) per SQL
        private void LadeTische(string bereich)
        {
            tischauswahl.Items.Clear();
            DateTime datum = dateTimePicker1.Value.Date;

            // Die SQL-Abfrage schaut in drei Tabellen gleichzeitig nach dem Status
            string query = @"
                SELECT t.tisch_id, t.bereich,
                CASE
                    WHEN EXISTS (SELECT 1 FROM bestellungen b WHERE b.tisch_id_fk = t.tisch_id AND b.slot = @slot AND b.status = 'offen') THEN 'Besetzt'
                    WHEN EXISTS (SELECT 1 FROM reservierungen r WHERE r.tisch_id_fk = t.tisch_id AND DATE(r.datum) = @datum AND r.slot = @slot AND r.zustand = 'aktiv') THEN 'Besetzt'
                    WHEN EXISTS (SELECT 1 FROM reservierungen r WHERE r.tisch_id_fk = t.tisch_id AND DATE(r.datum) = @datum AND r.slot = @slot AND r.zustand = 'offen') THEN 'Reserviert'
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

        private void DateTimePicker1_ValueChanged(object sender, EventArgs e) { AktualisiereTische(); }

        private int HoleSlot()
        {
            return slot_comboBox1_.SelectedIndex + 1;
        }

        // Aktualisiert die Tischliste basierend auf dem Bereich des Mitarbeiters
        private void AktualisiereTische()
        {
            if (tischauswahl_comboBox2.SelectedValue == null || HoleSlot() == 0) return;

            int personalNr = Convert.ToInt32(tischauswahl_comboBox2.SelectedValue);
            string query = "SELECT bereich FROM mitarbeiter WHERE personalnr = @pnr";

            using (var conn = Database.GetConnection())
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@pnr", personalNr);
                object result = cmd.ExecuteScalar();
                if (result != null) LadeTische(result.ToString());
            }
        }

        // Hilfsfunktion: Holt die Standard-ID für Gäste ohne Reservierung
        private int GetLaufgastId(MySqlConnection conn, MySqlTransaction transaction)
        {
            string query = "SELECT gastid FROM gast WHERE laufgast = true LIMIT 1";
            using (var cmd = new MySqlCommand(query, conn, transaction))
            {
                object result = cmd.ExecuteScalar();
                if (result == null) throw new Exception("Fehler: Kein 'Laufgast' in der Datenbank angelegt!");
                return Convert.ToInt32(result);
            }
        }

        // Das hier macht die Tisch-Liste farbig!
        private void Tischauswahl_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            TischItem tisch = (TischItem)tischauswahl.Items[e.Index];
            e.DrawBackground();

            Color farbe = Color.Green;
            if (tisch.Status == "Besetzt") farbe = Color.Red;
            else if (tisch.Status == "Reserviert") farbe = Color.Orange;

            using (Brush brush = new SolidBrush(farbe))
            {
                e.Graphics.DrawString(tisch.ToString(), e.Font, brush, e.Bounds.Left, e.Bounds.Top);
            }
            e.DrawFocusRectangle();
        }

        // Wenn ein reservierter Tisch gewählt wird, fragen wir ob die Gäste da sind
        private void Tischauswahl_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (tischauswahl.SelectedItem is TischItem tisch && tisch.Status == "Reserviert")
            {
                var result = MessageBox.Show("Sind die Gäste für die Reservierung da? Tisch jetzt öffnen?", "Check-In", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
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
                else { tischauswahl.SelectedIndex = -1; }
            }
        }

        private void Timer1_Tick(object sender, EventArgs e) { Timenow(); }

        // Design für die Speisekarten-Tabelle (DataGridView)
        private void BestellGridDesign()
        {
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.EnableHeadersVisualStyles = false;

            // Header-Style in Dunkelrot (passend zum Pizzeria-Thema)
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(139, 34, 34);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            dataGridView1.ColumnHeadersHeight = 40;

            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (dataGridView1.Columns.Contains("speisename")) dataGridView1.Columns["speisename"].HeaderText = "Gericht";
            if (dataGridView1.Columns.Contains("preis")) dataGridView1.Columns["preis"].HeaderText = "Preis (€)";
        }

        // Unbenutzte Event-Methoden (müssen bleiben, damit der Designer nicht meckert)
        private void Summe_TextBox1_TextChanged(object sender, EventArgs e) { }
        private void Mitarbeiter_combobox2_SelectedIndexChanged(object sender, EventArgs e) { }
        private void Bestellkorb_listBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void Slot_comboBox1_SelectedIndexChanged_1(object sender, EventArgs e) { }
    }
}