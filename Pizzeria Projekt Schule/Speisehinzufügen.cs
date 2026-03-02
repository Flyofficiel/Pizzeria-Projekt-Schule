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
    public partial class Speisehinzufügen : Form
    {
        public Speisehinzufügen()
        {
            InitializeComponent();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // --- 1. DATEN VORBEREITEN ---
            string name = textBox2.Text.Trim();
            string zutaten = textBox4.Text.Trim();

            // --- NEU: VORBEREITUNG FÜR ERRORPROVIDER ---                
            bool hatFehler = false; // Wird auf true gesetzt, wenn ein Feld falsch ist
            errorProvider1.Clear(); // Alle alten Fehler-Icons löschen
                                    // -------------------------------------------

            // --- 2. PLAUSIBILITÄTS-CHECKS ---

            // Check: Name leer oder zu kurz?
            if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
            {
                // Fehler-Icon anzeigen
                errorProvider1.SetError(textBox2, "Der Name muss mindestens 3 Zeichen lang sein!");
                hatFehler = true;
            }

            // Check: Besteht der Name nur aus Zahlen/Sonderzeichen?
            if (!name.Any(char.IsLetter))
            {
                // Fehler-Icon anzeigen
                errorProvider1.SetError(textBox2, "Der Name muss echte Buchstaben enthalten.");
                hatFehler = true;
            }

            // Check: Preis-Format (Deutsch: 12,50)
            if (!decimal.TryParse(textBox3.Text, System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.GetCultureInfo("de-DE"), out decimal preis))
            {
                // Fehler-Icon anzeigen
                errorProvider1.SetError(textBox3, "Bitte einen gültigen Preis eingeben (z.B. 8,50).");
                hatFehler = true;
            }
            // Check: Preis realistisch? (Falls Preis schon ein Fehler ist, nicht nochmal checken)
            else if (preis <= 0 || preis > 999.99m)
            {
                errorProvider1.SetError(textBox3, "Der Preis muss zwischen 0,01 € und 999,99 € liegen.");
                hatFehler = true;
            }

            // Check: Kategorie ausgewählt?
            if (comboBox1.SelectedItem == null)
            {
                // Fehler-Icon anzeigen
                errorProvider1.SetError(comboBox1, "Bitte eine Kategorie auswählen!");
                hatFehler = true;
            }

            // --- NEU: ZURÜCKSPRINGEN WENN FEHLER ---
            if (hatFehler)
            {
                return; // Methode beenden, nichts speichern
            }
            // ---------------------------------------

            // --- 3. DATENBANK-LOGIK ---
            try
            {
                using (MySqlConnection conn = Database.GetConnection())
                {
                    // Doppelte Speise verhindern (Case-Insensitive durch die DB)
                    string checkQuery = "SELECT COUNT(*) FROM speisen WHERE speisename = @name AND aktiv = true";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@name", name);
                        if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                        {
                            // Fehler anzeigen
                            errorProvider1.SetError(textBox2, "Diese Speise existiert bereits!");
                            return;
                        }
                    }

                    // Speichern
                    string query = "INSERT INTO speisen (speisename, speisentyp, preis, zutaten, aktiv) VALUES (@name, @typ, @preis, @zutaten, true)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@typ", comboBox1.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@preis", preis);
                        cmd.Parameters.AddWithValue("@zutaten", zutaten);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Speise erfolgreich hinzugefügt! ✔");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern: " + ex.Message);
            }
        }


        private void speisenhin_Load(object sender, EventArgs e)
        {

        }
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !char.IsDigit(e.KeyChar) &&
                e.KeyChar != ',' &&
                e.KeyChar != '.')
            {
                e.Handled = true;
            }

        }
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !char.IsLetter(e.KeyChar) &&
                e.KeyChar != ' ' &&
                e.KeyChar != ',')
            {
                e.Handled = true;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(textBox2, "");
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox2.Text))
            {
                // Trim entfernt Leerzeichen am Anfang/Ende
                string input = textBox2.Text.Trim();

                if (input.Length > 0)
                {
                    // Erster Buchstabe groß, Rest bleibt wie er ist
                    textBox2.Text = char.ToUpper(input[0]) + input.Substring(1);
                }
            }
        }
    }
}
