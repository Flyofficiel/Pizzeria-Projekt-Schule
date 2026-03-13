using MySqlConnector;
using System;
using System.Windows.Forms;

namespace Pizzeria_Projekt_Schule
{
    // Diese Klasse regelt die Verbindung zwischen unserem C#-Programm und der MySQL-Datenbank.
    // Ich habe sie 'static' gemacht, damit ich sie in jedem Fenster (z.B. Mitarbeiterverwaltung)
    // direkt aufrufen kann, ohne ein neues Objekt zu erstellen.
    public static class Database
    {
        // Der ConnectionString ist quasi die 'Anschrift' der Datenbank.
        // localhost = der eigene PC, uid/pwd = Benutzerdaten, database = Pizzeria Vesuv
        private static string connectionString = "server=localhost;uid=root;pwd=root;database=pizzaprojekt";

        // Diese Methode gibt uns eine offene Verbindung zurück, mit der wir SQL-Befehle schicken können.
        public static MySqlConnection GetConnection()
        {
            try
            {
                // Wir erstellen ein neues Verbindungsobjekt mit unseren Zugangsdaten
                MySqlConnection conn = new MySqlConnection(connectionString);

                // Hier versuchen wir die Verbindung zu öffnen
                conn.Open();

                return conn;
            }
            catch (Exception ex)
            {
                // Falls die Datenbank nicht erreichbar ist
                // zeigen wir eine Fehlermeldung an, statt das Programm abstürzen zu lassen.
                MessageBox.Show("Fehler: Verbindung zur Datenbank fehlgeschlagen!\n" + ex.Message,
                                "Datenbank-Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}