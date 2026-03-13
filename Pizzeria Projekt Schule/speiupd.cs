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
    // Dieses Fenster öffnet sich, wenn wir eine Speise aus der Liste bearbeiten wollen
    public partial class speiupd : Form
    {
        // Der Konstruktor bekommt die aktuellen Daten der Speise direkt beim Öffnen mitgeliefert
        public speiupd(int id, string name, string typ, decimal preis, string zutaten)
        {
            InitializeComponent();

            // Wir merken uns die ID in der Variable speiseId, damit wir später wissen,
            // welchen Datensatz wir in der Datenbank mit dem UPDATE-Befehl treffen müssen.
            speiseId = id;

            // Die Textfelder werden mit den Werten gefüllt, die momentan in der Datenbank stehen
            Name_speisseupd_textBox2.Text = name;
            Speisentyp_speisseupd_comboBox1.Text = typ;
            Preis_speisseupd_textBox3.Text = preis.ToString();
            zutaten_speisseupd_textBox4.Text = zutaten;
        }

        // Hier speichern wir die ID zwischen, weil wir sie im Konstruktor bekommen, 
        // aber erst beim Klick auf "Speichern" wieder brauchen.
        private int speiseId;

        // Das passiert, wenn man auf den Speichern/Aktualisieren Button drückt
        private void Update_speisseupd_button1_Click(object sender, EventArgs e)
        {
            // Check: Das Feld darf nicht leer gelassen werden
            if (string.IsNullOrWhiteSpace(Name_speisseupd_textBox2.Text))
            {
                MessageBox.Show("Bitte gib einen Namen für die Speise ein!");
                return;
            }

            // Check: Ein Name der nur aus Zahlen besteht, ist wahrscheinlich ein Tippfehler
            if (Name_speisseupd_textBox2.Text.All(char.IsDigit))
            {
                MessageBox.Show("Der Name darf nicht nur aus Zahlen bestehen!");
                return;
            }

            // Wir bereiten den SQL-Befehl vor. UPDATE ändert bestehende Zeilen.
            // Die @-Platzhalter sind wichtig, damit niemand Schadcode einschleusen kann.
            string query = @"
                UPDATE speisen
                SET speisename = @name,
                    speisentyp = @typ,
                    preis = @preis,
                    zutaten = @zutaten
                WHERE speise_id = @id";

            // Wir holen uns die Verbindung zur Datenbank
            MySqlConnection conn = Database.GetConnection();

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                // Die Werte aus den Textboxen werden an die SQL-Parameter übergeben
                cmd.Parameters.AddWithValue("@name", Name_speisseupd_textBox2.Text);
                cmd.Parameters.AddWithValue("@typ", Speisentyp_speisseupd_comboBox1.Text);

                // Den Preis müssen wir vorsichtig umwandeln, damit Punkte und Kommas kein Chaos anrichten
                if (!decimal.TryParse(
                    Preis_speisseupd_textBox3.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal preis))
                {
                    MessageBox.Show("Der Preis hat ein falsches Format!");
                    return;
                }

                cmd.Parameters.AddWithValue("@preis", preis);
                cmd.Parameters.AddWithValue("@zutaten", zutaten_speisseupd_textBox4.Text);
                cmd.Parameters.AddWithValue("@id", speiseId); // Hier nutzen wir die ID von oben

                // Den Befehl an die Datenbank schicken
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Änderungen wurden übernommen ✔");
            this.Close(); // Fenster schließen, wir springen automatisch zurück zur Liste
        }

        // Einfach das Fenster zumachen, wenn man sich umentscheidet
        private void Abbrechen_button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Hier passen wir auf, dass der User im Preisfeld keinen Quatsch eintippt
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Erlaubt sind nur Zahlen, die Löschtaste und Trennzeichen
            if (!char.IsControl(e.KeyChar) &&
                !char.IsDigit(e.KeyChar) &&
                e.KeyChar != ',' &&
                e.KeyChar != '.')
            {
                e.Handled = true; // Blockiert die Taste
            }

            // Kleiner Trick: Wenn der User einen Punkt tippt, machen wir ein Komma daraus
            if (e.KeyChar == '.')
            {
                e.KeyChar = ',';
            }
        }

        // Bei den Zutaten erlauben wir nur Buchstaben und Kommas zur Trennung
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

        private void Zuruck_button2_Click(object sender, EventArgs e)
        {
            
            this.Close();
        }

      
    }
}