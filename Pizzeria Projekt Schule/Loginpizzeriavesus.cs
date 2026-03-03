using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySqlConnector;
using System.Security.Cryptography;
using static System.Collections.Specialized.BitVector32;

namespace Pizzeria_Projekt_Schule
{
    public partial class Loginpizzeriavesus : Form
    {
        public Loginpizzeriavesus()
        {
            InitializeComponent();

            // Hier stellen wir ein, dass man die Fenstergröße ziehen kann und das Fenster maximieren darf
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.WindowState = FormWindowState.Normal;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Momentan passiert nichts direkt beim Laden der Seite
        }

        private void Einloggen_Button(object sender, EventArgs e)
        {
            // Erstmal prüfen, ob der Benutzer überhaupt etwas in die Felder eingegeben hat
            if (string.IsNullOrWhiteSpace(usernameinput.Text) ||
                string.IsNullOrWhiteSpace(passwortinput.Text))
            {
                MessageBox.Show("Bitte alle Felder ausfüllen!");
                return; // Wenn was fehlt, wird hier abgebrochen
            }

            // Da die Personalnummer in der Datenbank eine Zahl ist, wandeln wir den Text hier um
            if (!int.TryParse(usernameinput.Text, out int personalNr))
            {
                MessageBox.Show("Personalnummer muss eine Zahl sein!");
                return;
            }

            string inputPassword = passwortinput.Text;

            // Das ist unsere SQL-Abfrage. Wir suchen den Mitarbeiter mit der passenden Nummer und Passwort.
            // Die @-Zeichen sind Platzhalter, damit der Login sicher ist (SQL-Injection Schutz).
            const string query = @"
        SELECT personalnr, rolle, bereich 
        FROM mitarbeiter 
        WHERE personalnr = @username 
        AND passwort = @passwort
        AND aktiv = true";

            // Wir holen uns die Verbindung zur Datenbank
            using (MySqlConnection conn = Database.GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Hier füllen wir die Platzhalter von oben mit den echten Eingaben aus den Textboxen
                    cmd.Parameters.AddWithValue("@username", personalNr);
                    cmd.Parameters.AddWithValue("@passwort", inputPassword);

                    try
                    {
                        // Wir führen den Befehl aus und schauen nach, ob ein Datensatz gefunden wurde
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Wenn ein Mitarbeiter gefunden wurde, geht es zum Hauptmenü
                                MessageBox.Show("Login erfolgreich!");

                                Hauptmenu mainpage = new Hauptmenu();
                                mainpage.Show();
                                this.Hide(); // Das Login-Fenster wird nur versteckt, nicht gelöscht
                            }
                            else
                            {
                                // Falls Nummer oder Passwort nicht in der Datenbank stehen
                                MessageBox.Show("Personalnummer oder Passwort falsch!",
                                    "Login fehlgeschlagen",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Falls es ein Problem mit dem Server oder der Datenbank gibt
                        MessageBox.Show("Datenbankfehler: " + ex.Message);
                    }
                }
            }
        }

        private void Abbrechen_Button(object sender, EventArgs e)
        {
            // Schließt das Fenster, wenn man auf Abbrechen klickt
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Noch nicht belegt
        }

        private void label3_Click(object sender, EventArgs e)
        {
            // Noch nicht belegt
        }

        // Diese Funktion kann Passwörter verschlüsseln (SHA256 Hash)
        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                // Passwort in Bytes umwandeln und Hashen
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();

                // Den Byte-Salat in einen lesbaren Text umwandeln
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));

                return builder.ToString();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Noch nicht belegt
        }

        private void usernameinput_TextChanged(object sender, EventArgs e)
        {
            // Noch nicht belegt
        }

        // Das hier steuert, ob man das Passwort lesen kann oder nur Punkte sieht
        private void Passwordunhide_CheckedChanged(object sender, EventArgs e)
        {
            if (Passwordunhide.Checked)
            {
                // Passwort wird als normaler Text angezeigt
                passwortinput.PasswordChar = '\0';
            }
            else
            {
                // Passwort wird hinter dicken Punkten versteckt
                passwortinput.PasswordChar = '●';
            }
        }
    }
}