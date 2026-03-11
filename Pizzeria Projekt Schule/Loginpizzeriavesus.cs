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
    // Das ist das Formular für den Login-Bereich unserer Pizzeria
    public partial class Loginpizzeriavesus : Form
    {
        public Loginpizzeriavesus()
        {
            InitializeComponent();

            // Fenster-Einstellungen: Wir erlauben dem User das Fenster groß zu ziehen 
            // oder zu maximieren, damit es auf jedem Bildschirm gut aussieht.
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.WindowState = FormWindowState.Normal;

            // Komfort-Funktion: Wenn man die Enter-Taste drückt, wird automatisch 
            // der Login-Button (button1) ausgelöst.
            this.AcceptButton = button1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Hier könnte man beim Starten der Seite noch Dinge vorbereiten
        }

        // --- LOGIK: EINLOGGEN ---
        private void Einloggen_Button(object sender, EventArgs e)
        {
            // 1. Validierung: Erstmal checken, ob überhaupt was in den Feldern steht.
            // Wenn eines der Felder leer ist, zeigen wir eine Warnung und stoppen hier.
            if (string.IsNullOrWhiteSpace(usernameinput.Text) ||
                string.IsNullOrWhiteSpace(passwortinput.Text))
            {
                MessageBox.Show("Bitte alle Felder ausfüllen!", "Eingabe fehlt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Datentyp prüfen: Die Personalnummer muss eine Zahl sein, damit die DB sie finden kann.
            // 'TryParse' versucht den Text in eine Zahl zu wandeln. Wenn's klappt, landet sie in 'personalNr'.
            if (!int.TryParse(usernameinput.Text, out int personalNr))
            {
                MessageBox.Show("Die Personalnummer muss eine gültige Zahl sein!", "Fehler");
                return;
            }

            string inputPassword = passwortinput.Text;

            // 3. SQL-Abfrage: Wir suchen in der Tabelle 'mitarbeiter' nach der Nummer und dem Passwort.
            // WICHTIG: Wir nutzen Parameter (@username, @passwort) gegen SQL-Injection Angriffe!
            // Nur aktive Mitarbeiter (aktiv = true) dürfen sich einloggen.
            const string query = @"
                SELECT personalnr, rolle, bereich 
                FROM mitarbeiter 
                WHERE personalnr = @username 
                AND passwort = @passwort
                AND aktiv = true";

            // Wir öffnen die Verbindung zur Datenbank (Database-Klasse wird vorausgesetzt)
            using (MySqlConnection conn = Database.GetConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Hier binden wir die echten Werte an die Platzhalter der SQL-Abfrage
                    cmd.Parameters.AddWithValue("@username", personalNr);
                    cmd.Parameters.AddWithValue("@passwort", inputPassword);

                    try
                    {
                        // Wir führen die Abfrage aus
                        using (var reader = cmd.ExecuteReader())
                        {
                            // Wenn der Reader eine Zeile findet, waren die Daten korrekt
                            if (reader.Read())
                            {
                                MessageBox.Show("Login erfolgreich! Willkommen zurück.", "Erfolg");

                                // Neues Fenster (Hauptmenü) erstellen und anzeigen
                                Hauptmenu mainpage = new Hauptmenu();
                                mainpage.Show();

                                // Dieses Fenster hier verstecken
                                this.Hide();
                            }
                            else
                            {
                                // Keine Übereinstimmung in der Datenbank gefunden
                                MessageBox.Show("Personalnummer oder Passwort falsch!",
                                    "Login fehlgeschlagen",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Falls die DB-Verbindung mal streikt, fangen wir den Fehler hier ab
                        MessageBox.Show("Fehler bei der Datenbankverbindung: " + ex.Message, "Systemfehler");
                    }
                }
            }
        }

        // --- LOGIK: ABBRECHEN ---
        private void Abbrechen_Button(object sender, EventArgs e)
        {
            // Programm beenden oder Fenster schließen
            Application.Exit();
        }

        // --- EXTRA: PASSWORT VERSCHLÜSSELN ---
        // Diese Methode nutzt SHA256, um ein Passwort in einen Hash-Code zu verwandeln.
        // Das ist sicherer, als Passwörter im Klartext in der DB zu speichern.
        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                // Text in Byte-Array umwandeln und hashen
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();

                // Jedes Byte in ein Hex-Format umwandeln (macht den Text lesbar)
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));

                return builder.ToString();
            }
        }

        // --- LOGIK: PASSWORT ANZEIGEN / VERSTECKEN ---
        // Wenn man die Checkbox anklickt, wird das Passwort lesbar oder durch Punkte verdeckt.
        private void Passwordunhide_CheckedChanged(object sender, EventArgs e)
        {
            if (Passwordunhide.Checked)
            {
                // '\0' bedeutet: Kein spezielles Zeichen, also normaler Text
                passwortinput.PasswordChar = '\0';
            }
            else
            {
                // '●' ist das klassische Zeichen zum Verstecken von Passwörtern
                passwortinput.PasswordChar = '●';
            }
        }

        // Leere Methoden für Events, die wir aktuell nicht brauchen (aber im Designer existieren)
        private void Button3_Click(object sender, EventArgs e) { }
        private void Label3_Click(object sender, EventArgs e) { }
        private void Label1_Click(object sender, EventArgs e) { }
        private void Usernameinput_TextChanged(object sender, EventArgs e) { }
    }
}