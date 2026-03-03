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
    public partial class speiupd : Form
    {
        // Der Konstruktor: Hier werden die Daten der Speise aus der Liste empfangen
        public speiupd(int id, string name, string typ, decimal preis, string zutaten)
        {
            InitializeComponent();

            // Wir speichern die ID der Speise in einer Variable, damit wir wissen, 
            // welche Pizza wir später in der Datenbank ändern müssen.
            speiseId = id;

            // Die Textboxen werden mit den aktuellen Daten gefüllt, damit man sie bearbeiten kann
            textBox2.Text = name;
            comboBox1.Text = typ;
            textBox3.Text = preis.ToString();
            textBox4.Text = zutaten;
        }

        // Variable für die ID
        private int speiseId;

        // --- BUTTON: AKTUALISIEREN ---
        private void button1_Click(object sender, EventArgs e)
        {
            // 1. Check: Der Name darf nicht leer sein
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Speisename darf nicht leer sein!");
                return;
            }

            // 2. Check: Der Name darf nicht nur aus Zahlen bestehen
            if (textBox2.Text.All(char.IsDigit))
            {
                MessageBox.Show("Speisename darf nicht nur Zahlen enthalten!");
                return;
            }

            // Der SQL-Befehl zum Ändern (Update) der Daten.
            // Wir nutzen wieder @-Parameter für die Sicherheit.
            string query = @"
                UPDATE speisen
                SET speisename = @name,
                    speisentyp = @typ,
                    preis = @preis,
                    zutaten = @zutaten
                WHERE speise_id = @id";

            // Verbindung zur Datenbank holen
            MySqlConnection conn = Database.GetConnection();

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                // Die Werte aus den Eingabefeldern an den SQL-Befehl übergeben
                cmd.Parameters.AddWithValue("@name", textBox2.Text);
                cmd.Parameters.AddWithValue("@typ", comboBox1.Text);

                // Den Preis sicher umwandeln (ersetzt Komma durch Punkt für die Datenbank)
                if (!decimal.TryParse(
                    textBox3.Text.Replace(",", "."),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal preis))
                {
                    MessageBox.Show("Ungültiger Preis!");
                    return;
                }

                cmd.Parameters.AddWithValue("@preis", preis);
                cmd.Parameters.AddWithValue("@zutaten", textBox4.Text);
                cmd.Parameters.AddWithValue("@id", speiseId);

                // Den Befehl ausführen
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Speise erfolgreich aktualisiert ✔");
            this.Close(); // Fenster schließen nach dem Speichern
        }

        // --- BUTTON: ABBRECHEN ---
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close(); // Schließt das Fenster ohne zu speichern
        }

        // --- EINGABE-KONTROLLE: PREIS ---
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Nur Zahlen, Backspace, Komma und Punkt erlauben
            if (!char.IsControl(e.KeyChar) &&
                !char.IsDigit(e.KeyChar) &&
                e.KeyChar != ',' &&
                e.KeyChar != '.')
            {
                e.Handled = true; // Ungültige Zeichen blockieren
            }

            // Komfort: Wenn der User einen Punkt tippt, machen wir automatisch ein Komma daraus
            if (e.KeyChar == '.')
            {
                e.KeyChar = ',';
            }
        }

        // --- EINGABE-KONTROLLE: ZUTATEN ---
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Nur Buchstaben, Leerzeichen, Kommas und Löschtaste erlauben
            if (!char.IsControl(e.KeyChar) &&
                    !char.IsLetter(e.KeyChar) &&
                    e.KeyChar != ' ' &&
                    e.KeyChar != ',')
            {
                e.Handled = true;
            }
        }

        private void speiupd_Load(object sender, EventArgs e)
        {
            // Hier könnte noch Code beim Laden stehen
        }
    }
}