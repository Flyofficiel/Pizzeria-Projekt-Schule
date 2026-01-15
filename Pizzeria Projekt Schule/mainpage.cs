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
    public partial class mainpage : Form
    {
        public mainpage()
        {
            InitializeComponent();
        }

        private void logout_Click(object sender, EventArgs e)
        {

            var loginForm = new Loginform();
            loginForm.FormClosed += (s, args) => this.Close();
            this.Hide();
            loginForm.Show();
        }

        

        private void bestellungenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bestellungspage bestellungspage = new Bestellungspage();
            bestellungspage.FormClosed += (s, args) => this.Close();
            this.Hide();
            bestellungspage.Show();
        }

        private void stammdatenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stammdatenpage stammdatenpage = new Stammdatenpage();
            stammdatenpage.FormClosed += (s, args) => this.Close();
            this.Hide();
            stammdatenpage.Show();
        }

        private void auswertungToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            auswertungspage auswertungspage = new auswertungspage();
            auswertungspage.FormClosed += (s, args) => this.Close();
            this.Hide();
            auswertungspage.Show();
        }

        
    }
}
