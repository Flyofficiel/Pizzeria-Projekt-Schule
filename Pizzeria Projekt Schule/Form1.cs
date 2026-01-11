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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connString = "server=localhost;uid=root;pwd=root;database=pizzaprojekt";
            string inputUsername = usernameinput.Text.Trim();
            string inputPassword = passwortinput.Text;

            const string query = "SELECT username, passwort FROM User WHERE username = @username AND passwort = @passwort";

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
                            string UsernameDB = reader["username"].ToString();
                            string PasswortDB = reader["passwort"].ToString();
                            MessageBox.Show("Login Erfolgreich");

                            // Show main page without terminating the application by closing the startup form
                            var m1 = new mainpage();
                            // When mainpage closes, close the hidden login form so the app exits
                            m1.FormClosed += (s, args) => this.Close();
                            this.Hide();
                            m1.Show();
                        }
                        else
                        {
                            MessageBox.Show("Username or password incorrect.", "Login failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
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

