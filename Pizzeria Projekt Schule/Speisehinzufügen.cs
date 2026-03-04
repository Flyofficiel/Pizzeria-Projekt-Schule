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
    // Dieses Fenster dient dazu, eine neue Speise (z.B. eine Pizza) in die Datenbank einzutragen.
    public partial class Speisehinzufügen : Form
    {
        public Speisehinzufügen()
        {
            InitializeComponent();
        }

        private void Label4_Click(object sender, EventArgs e) { }

        // Schließt das Fenster, falls man es sich anders überlegt hat
        private void Abbrechen_button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // --- BUTTON: SPEICHERN ---
        private void Hinzufugen_button1_Click(object sender, EventArgs e)
        {
            // --- 1. DATEN VORBEREITEN ---
            // Trim entfernt unnötige Leerzeichen am Anfang und Ende
            string name = Name_textBox2.Text.Trim();
            string zutaten = zutaten_textBox4.Text.Trim();

            // Vorbereitung für die Fehlermeldungen (ErrorProvider)
            bool hatFehler = false;
            errorProvider1.Clear(); // Zuerst alle alten Warn-Symbole löschen

            // --- 2. PLAUSIBILITÄTS-CHECKS (Prüfen ob die Eingaben Sinn machen) ---

            // Check: Ist der Name leer oder zu kurz?
            if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
            {
                errorProvider1.SetError(Name_textBox2, "Der Name muss mindestens 3 Zeichen lang sein!");
                hatFehler = true;
            }

            // Check: Besteht der Name nur aus Zahlen? (Wir brauchen echte Buchstaben)
            if (!name.Any(char.IsLetter))
            {
                errorProvider1.SetError(Name_textBox2, "Der Name muss echte Buchstaben enthalten.");
                hatFehler = true;
            }

            // Check: Ist der Preis im richtigen Format (z.B. 8,50)?
            if (!decimal.TryParse(Preis_textBox3.Text, System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.GetCultureInfo("de-DE"), out decimal preis))
            {
                errorProvider1.SetError(Preis_textBox3, "Bitte einen gültigen Preis eingeben (z.B. 8,50).");
                hatFehler = true;
            }
            // Check: Ist der Preis realistisch (nicht 0 € und nicht über 1000 €)?
            else if (preis <= 0 || preis > 999.99m)
            {
                errorProvider1.SetError(Preis_textBox3, "Der Preis muss zwischen 0,01 € und 999,99 € liegen.");
                hatFehler = true;
            }

            // Check: Wurde eine Kategorie (Pizza, Pasta, Getränk) ausgewählt?
            if (Speissen_typ_comboBox1.SelectedItem == null)
            {
                errorProvider1.SetError(Speissen_typ_comboBox1, "Bitte eine Kategorie auswählen!");
                hatFehler = true;
            }

            // Wenn irgendeiner der Checks oben fehlgeschlagen ist, speichern wir NICHT
            if (hatFehler)
            {
                return;
            }

            // --- 3. DATENBANK-LOGIK ---
            try
            {
                using (MySqlConnection conn = Database.GetConnection())
                {
                    // Sicherheit: Wir prüfen erst, ob es diese Speise schon gibt
                    string checkQuery = "SELECT COUNT(*) FROM speisen WHERE speisename = @name AND aktiv = true";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@name", name);
                        if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                        {
                            errorProvider1.SetError(Name_textBox2, "Diese Speise existiert bereits!");
                            return;
                        }
                    }

                    // Wenn alles okay ist, legen wir den neuen Datensatz an (INSERT)
                    string query = "INSERT INTO speisen (speisename, speisentyp, preis, zutaten, aktiv) VALUES (@name, @typ, @preis, @zutaten, true)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@typ", Speissen_typ_comboBox1.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@preis", preis);
                        cmd.Parameters.AddWithValue("@zutaten", zutaten);

                        cmd.ExecuteNonQuery(); // Befehl ausführen
                    }
                }
                MessageBox.Show("Speise erfolgreich hinzugefügt! ✔");
                this.Close(); // Fenster schließen nach Erfolg
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern: " + ex.Message);
            }
        }

        private void Speisenhin_Load(object sender, EventArgs e) { }

        // Kontrolliert beim Tippen, dass im Preis-Feld nur Zahlen, Komma oder Punkt landen
        private void TextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                e.KeyChar != ',' && e.KeyChar != '.')
            {
                e.Handled = true; // Ungültige Zeichen blockieren
            }
        }

        // Kontrolliert beim Tippen, dass bei den Zutaten nur Text und Kommas landen
        private void TextBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) &&
                e.KeyChar != ' ' && e.KeyChar != ',')
            {
                e.Handled = true;
            }
        }

        // Löscht das Fehler-Symbol, sobald der User wieder anfängt zu tippen
        private void Name_textBox2_TextChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(Name_textBox2, "");
        }

        // Wenn der User das Namens-Feld verlässt, wird der erste Buchstabe automatisch groß gemacht
        private void TextBox2_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Name_textBox2.Text))
            {
                string input = Name_textBox2.Text.Trim();
                if (input.Length > 0)
                {
                    // Erster Buchstabe Groß + der Rest vom Text
                    Name_textBox2.Text = char.ToUpper(input[0]) + input.Substring(1);
                }
            }
        }

        private void Speissen_typ_comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Preis_textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void Zutaten_textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}