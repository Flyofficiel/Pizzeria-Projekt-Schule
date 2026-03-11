using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pizzeria_Projekt_Schule
{
    // In diesem Fenster legen wir neue Pizzen, Nudeln oder Getränke in der Datenbank an
    public partial class Speisehinzufügen : Form
    {
        public Speisehinzufügen()
        {
            InitializeComponent();
        }

        private void Label4_Click(object sender, EventArgs e) { }

        // Falls man doch keine Lust hat was einzutragen, bricht das Fenster hier einfach ab
        private void Abbrechen_button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Das ist das Herzstück: Hier werden die Eingaben geprüft und gespeichert
        private void Hinzufugen_button1_Click(object sender, EventArgs e)
        {
            // Wir nehmen den Text und entfernen Leerzeichen am Rand (Trim), damit die DB sauber bleibt
            string name = Name_textBox2.Text.Trim();
            string zutaten = zutaten_textBox4.Text.Trim();

            // Variable um zu merken, ob wir einen Fehler gefunden haben
            bool hatFehler = false;
            errorProvider1.Clear(); // Alle alten Fehlermeldungen einmal wegwischen

            // Check: Der Name darf nicht leer sein und sollte mindestens 3 Zeichen haben
            if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
            {
                errorProvider1.SetError(Name_textBox2, "Der Name ist ein bisschen zu kurz!");
                hatFehler = true;
            }

            // Check: Eine Pizza braucht im Namen mindestens ein paar Buchstaben
            if (!name.Any(char.IsLetter))
            {
                errorProvider1.SetError(Name_textBox2, "Der Name muss auch Buchstaben haben.");
                hatFehler = true;
            }

            // Check: Ist der Preis eine richtige Zahl? Wir nutzen TryParse, damit das Programm nicht abstürzt
            if (!decimal.TryParse(Preis_textBox3.Text, System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.GetCultureInfo("de-DE"), out decimal preis))
            {
                errorProvider1.SetError(Preis_textBox3, "Bitte einen Preis wie 8,50 eingeben.");
                hatFehler = true;
            }
            // Check: Der Preis sollte nicht negativ oder völlig übertrieben sein
            else if (preis <= 0 || preis > 999.99m)
            {
                errorProvider1.SetError(Preis_textBox3, "Der Preis muss zwischen 0 und 1000 Euro liegen.");
                hatFehler = true;
            }

            // Check: Man muss eine Kategorie in der Liste auswählen
            if (Speissen_typ_comboBox1.SelectedItem == null)
            {
                errorProvider1.SetError(Speissen_typ_comboBox1, "Bitte wähle aus, was es für ein Typ ist.");
                hatFehler = true;
            }

            // Wenn irgendwo oben ein Fehler war, hören wir hier auf und speichern nichts
            if (hatFehler)
            {
                return;
            }

            try
            {
                using (MySqlConnection conn = Database.GetConnection())
                {
                    // Wir schauen erst mal nach, ob es eine aktive Speise mit dem Namen schon gibt
                    string checkQuery = "SELECT COUNT(*) FROM speisen WHERE speisename = @name AND aktiv = true";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@name", name);
                        if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                        {
                            errorProvider1.SetError(Name_textBox2, "Dieses Gericht gibt es schon auf der Karte!");
                            return;
                        }
                    }

                    // Wenn alles passt, schießen wir die Daten mit INSERT in die Tabelle
                    string query = "INSERT INTO speisen (speisename, speisentyp, preis, zutaten, aktiv) VALUES (@name, @typ, @preis, @zutaten, true)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Parameter schützen uns davor, dass jemand böse Befehle in die Textboxen schreibt (SQL Injection)
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@typ", Speissen_typ_comboBox1.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@preis", preis);
                        cmd.Parameters.AddWithValue("@zutaten", zutaten);

                        cmd.ExecuteNonQuery(); // Den Befehl abschicken
                    }
                }
                MessageBox.Show("Die Speise wurde erfolgreich gespeichert! ✔");
                this.Close(); // Fenster zu, wenn alles geklappt hat
            }
            catch (Exception ex)
            {
                // Falls die Datenbank mal nicht will
                MessageBox.Show("Fehler beim Speichern in der Datenbank: " + ex.Message);
            }
        }

        private void Speisenhin_Load(object sender, EventArgs e) { }

        // Hier wird beim Tippen kontrolliert, dass im Preis nur Zahlen und Komma/Punkt landen
        private void TextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                e.KeyChar != ',' && e.KeyChar != '.')
            {
                e.Handled = true; // Taste wird einfach verschluckt
            }
        }

        // Bei den Zutaten erlauben wir nur Text, Kommas und Leerzeichen
        private void TextBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) &&
                e.KeyChar != ' ' && e.KeyChar != ',')
            {
                e.Handled = true;
            }
        }

        // Wenn der Nutzer wieder tippt, löschen wir das rote Fehler-Icon
        private void Name_textBox2_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(Name_textBox2, "");
        }

        // Sobald man aus der Namensbox klickt, machen wir den ersten Buchstaben automatisch groß
        private void TextBox2_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Name_textBox2.Text))
            {
                string input = Name_textBox2.Text.Trim();
                if (input.Length > 0)
                {
                    Name_textBox2.Text = char.ToUpper(input[0]) + input.Substring(1);
                }
            }
        }

        // Diese Methoden lassen wir leer, falls wir später noch was beim Klicken ändern wollen
        private void Speissen_typ_comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void Preis_textBox3_TextChanged(object sender, EventArgs e) { }
        private void Zutaten_textBox4_TextChanged(object sender, EventArgs e) { }
    }
}