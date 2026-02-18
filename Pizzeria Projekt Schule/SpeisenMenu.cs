using MySqlConnector;
using System;
using System.Collections;
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
    // Dieses Formular ist für die Verwaltung der Speisen zuständig
    public partial class SpeisenMenu : Form
    {
        public SpeisenMenu()
        {
            // Initialisiert alle grafischen Elemente des Formulars
            InitializeComponent();
        }

        // Diese Methode löscht eine Speise NICHT komplett,
        // sondern setzt sie auf inaktiv (Soft Delete)
        private void SpeiseLoeschen()
        {
            // SQL-Befehl: Speise auf inaktiv setzen
            string query = @"
        UPDATE speisen
        SET aktiv = 0
        WHERE speise_id = @speise_id
    ";

            // Verbindung zur Datenbank holen
            MySqlConnection conn = Database.GetConnection();

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                // Die ID der aktuell ausgewählten Speise wird übergeben
                cmd.Parameters.AddWithValue(
                    "@speise_id",
                    dataGridView1.CurrentRow.Cells["speise_id"].Value
                );

                // SQL-Befehl ausführen
                cmd.ExecuteNonQuery();
            }

            // Bestätigung für den Benutzer
            MessageBox.Show("Speise gelöscht ✔");

            // DataGridView neu laden damit Änderung sichtbar wird
            SpeisenLaden();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void label2_Click(object sender, EventArgs e)
        {
            
        }

        // Zurück-Button zum Hauptmenü
        private void button4_Click(object sender, EventArgs e)
        {
            Hauptmenu mainmenupage = new Hauptmenu();
            mainmenupage.Show();
            this.Close();
        }

        // Beim Laden des Formulars werden die Speisen automatisch geladen
        private void Speissen_Load(object sender, EventArgs e)
        {
            SpeisenLaden();
        }

        // Öffnet das Formular zum Hinzufügen einer neuen Speise
        private void button2_Click(object sender, EventArgs e)
        {
            Speisehinzufügen speisenhinzufügen = new Speisehinzufügen();
            speisenhinzufügen.Show();
        }

        // Button zum Löschen einer Speise
        private void button3_Click(object sender, EventArgs e)
        {
            // Prüfen ob überhaupt eine Zeile ausgewählt ist
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Bitte zuerst eine Speise auswählen.");
                return;
            }

            // Sicherheitsabfrage damit nichts aus Versehen gelöscht wird
            DialogResult result = MessageBox.Show(
                "Möchten Sie diese Speise wirklich löschen?",
                "Bestätigung",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            // Nur wenn "Ja" geklickt wird, wird gelöscht
            if (result == DialogResult.Yes)
            {
                SpeiseLoeschen();
            }
        }

        // Diese Methode lädt alle aktiven Speisen aus der Datenbank
        private void SpeisenLaden()
        {
            // Es werden nur Speisen geladen, die aktiv = 1 sind
            string query = "SELECT speise_id, speisename, speisentyp, preis, zutaten FROM speisen WHERE aktiv = 1";

            MySqlConnection conn = Database.GetConnection();

            // Datenadapter holt die Daten aus der Datenbank
            MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
            DataTable table = new DataTable();
            adapter.Fill(table);

            // Daten im DataGridView anzeigen
            dataGridView1.DataSource = table;

            // Preis als Euro formatieren (deutsches Format)
            dataGridView1.Columns["preis"].DefaultCellStyle.Format = "C2";
            dataGridView1.Columns["preis"].DefaultCellStyle.FormatProvider =
                System.Globalization.CultureInfo.GetCultureInfo("de-DE");

            // Spaltenüberschriften benutzerfreundlicher machen
            dataGridView1.Columns["speise_id"].HeaderText = "ID";
            dataGridView1.Columns["speisename"].HeaderText = "Name";
            dataGridView1.Columns["speisentyp"].HeaderText = "Typ";
            dataGridView1.Columns["preis"].HeaderText = "Preis";
            dataGridView1.Columns["zutaten"].HeaderText = "Zutaten";
        }

    }
}
