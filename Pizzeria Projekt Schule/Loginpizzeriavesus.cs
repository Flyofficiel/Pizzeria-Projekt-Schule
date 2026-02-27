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
           
        {
            InitializeComponent();


            }

        }
        

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void Einloggen_Button(object sender, EventArgs e)
        {
            // Pflichtfelder prüfen
            if (string.IsNullOrWhiteSpace(usernameinput.Text) ||
                string.IsNullOrWhiteSpace(passwortinput.Text))
            {
                MessageBox.Show("Bitte alle Felder ausfüllen!");
                return;
            }

            // Personalnummer muss Zahl sein
            if (!int.TryParse(usernameinput.Text, out int personalNr))
            {
                MessageBox.Show("Personalnummer muss eine Zahl sein!");
                return;
            }

            string inputPassword = passwortinput.Text;

            const string query = @"
        SELECT personalnr, rolle, bereich 
        FROM mitarbeiter 
        WHERE personalnr = @username 
        AND passwort = @passwort
        AND aktiv = true";

            using (MySqlConnection conn = Database.GetConnection())
            {
               

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", personalNr);
                    cmd.Parameters.AddWithValue("@passwort", inputPassword);

                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                MessageBox.Show("Login erfolgreich!");

                                Hauptmenu mainpage = new Hauptmenu();
                                mainpage.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Personalnummer oder Passwort falsch!",
                                    "Login fehlgeschlagen",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Datenbankfehler: " + ex.Message);
                    }
                }
            }
        }

        private void Abbrechen_Button(object sender, EventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();

                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));

                return builder.ToString();
            }
        }
    }
}

