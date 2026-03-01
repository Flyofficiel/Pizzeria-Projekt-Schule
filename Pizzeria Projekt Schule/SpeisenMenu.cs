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
using System.Xml.Linq;

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
            string query = @"
        UPDATE speisen
        SET aktiv = 0
        WHERE speise_id = @speise_id";

            using (MySqlConnection conn = Database.GetConnection())
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@speise_id",
                    dataGridView1.CurrentRow.Cells["speise_id"].Value
                );

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Speise gelöscht ✔");
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
            if (dataGridView1.CurrentRow == null ||
                dataGridView1.CurrentRow.Cells["speise_id"].Value == null)
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
            try
            {
                // Wir nutzen COALESCE, um sicherzustellen, dass niemals ein 'NULL' (leer)
                // bei den Zahlenfeldern ankommt, sondern im Notfall eine 0.
                string query = @"
            SELECT speise_id, speisename, speisentyp, 
            COALESCE(preis, 0.00) as preis, zutaten ,aktiv
            FROM speisen
            WHERE aktiv = 1";

                using (MySqlConnection conn = Database.GetConnection())
                using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                {
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    dataGridView1.DataSource = table;
                }

                // Diese Zeilen sind gut, aber sie setzen voraus, dass 'preis' eine Zahl ist.
                if (dataGridView1.Columns.Contains("preis"))
                {
                    dataGridView1.Columns["preis"].DefaultCellStyle.Format = "C2";
                    dataGridView1.Columns["preis"].DefaultCellStyle.FormatProvider =
                        System.Globalization.CultureInfo.GetCultureInfo("de-DE");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler: " + ex.Message);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            string query;

            if (checkBox1.Checked)
            {
                // Nur aktive Speisen anzeigen
                query = @"SELECT speise_id, speisename, speisentyp, preis, zutaten 
                  FROM speisen 
                  WHERE aktiv = 1";
            }
            else
            {
                // Alle Speisen anzeigen
                query = @"SELECT speise_id, speisename, speisentyp, preis, zutaten ,aktiv
                  FROM speisen";
            }

            using (MySqlConnection conn = Database.GetConnection())
            using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
            }

            dataGridView1.Columns["preis"].DefaultCellStyle.Format = "C2";
            dataGridView1.Columns["preis"].DefaultCellStyle.FormatProvider =
                System.Globalization.CultureInfo.GetCultureInfo("de-DE");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // 🔥 Nach dem Schließen automatisch neu laden
            SpeisenLaden(); ;
        }



        private void button1_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Bitte zuerst eine Speise auswählen.");
                return;
            }

            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["speise_id"].Value);
            string name = dataGridView1.CurrentRow.Cells["speisename"].Value.ToString();
            string typ = dataGridView1.CurrentRow.Cells["speisentyp"].Value.ToString();
            // Statt: decimal preis = Convert.ToDecimal(dataGridView1.CurrentRow.Cells["preis"].Value);

            // Sicherer so:
            decimal preis = 0;
            if (dataGridView1.CurrentRow.Cells["preis"].Value != null)
            {
                decimal.TryParse(dataGridView1.CurrentRow.Cells["preis"].Value.ToString(),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.GetCultureInfo("de-DE"), out preis);
            }
            string zutaten = dataGridView1.CurrentRow.Cells["zutaten"].Value.ToString();

            speiupd updateForm = new speiupd(id, name, typ, preis, zutaten);
            updateForm.ShowDialog();

            SpeisenLaden(); // Nach Update neu laden
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
    }

