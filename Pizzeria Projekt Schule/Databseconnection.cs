using MySqlConnector;

namespace Pizzeria_Projekt_Schule
{
    public static class Database
    {
        private static string connectionString =
            "server=localhost;uid=root;pwd=root;database=pizzaprojekt";

        public static MySqlConnection GetConnection()
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();
            return conn;
        }
    }
}
