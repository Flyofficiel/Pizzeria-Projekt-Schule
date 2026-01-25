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

namespace Pizzeria_Projekt_Schule
{
    public partial class Loginform : Form
    {
        public Loginform()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connString = "server=localhost;uid=root;pwd=root;database=pizzaprojekt";
            string inputUsername = usernameinput.Text;
            string inputPassword = passwortinput.Text;

            const string query = "SELECT personalnr, passwort FROM mitarbeiter WHERE personalnr = @username AND passwort = @passwort";

            using (var conn = new MySqlConnection(connString))
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@username", inputUsername);
                cmd.Parameters.AddWithValue("@passwort", inputPassword);

                try
                {
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            MessageBox.Show("Login Erfolgreich");

                            // Show main page without terminating the application 
                            Hauptmenu mainpage = new Hauptmenu();
                            mainpage.Show();
                            this.Hide();
                        }
                        else
                        {
                            // Login failed und macht dann eine MessageBox auf wo dann geht das der Login fehlgeschlagen ist weil das Passwort oder der username falsch ist
                            MessageBox.Show("Username or password incorrect.", "Login failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // gibt den error an wenn es ein fehler gibt
                    MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        
    }
}

