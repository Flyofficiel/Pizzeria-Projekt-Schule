namespace Pizzeria_Projekt_Schule
{
    partial class Loginpizzeriavesus
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Loginpizzeriavesus));
            this.button1 = new System.Windows.Forms.Button();
            this.usernameinput = new System.Windows.Forms.TextBox();
            this.passwortinput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Passwordunhide = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.Menu;
            this.button1.Location = new System.Drawing.Point(421, 255);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Login";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.Einloggen_Button);
            // 
            // usernameinput
            // 
            this.usernameinput.Location = new System.Drawing.Point(385, 164);
            this.usernameinput.Name = "usernameinput";
            this.usernameinput.Size = new System.Drawing.Size(130, 20);
            this.usernameinput.TabIndex = 1;
            this.usernameinput.TextChanged += new System.EventHandler(this.Usernameinput_TextChanged);
            // 
            // passwortinput
            // 
            this.passwortinput.Location = new System.Drawing.Point(385, 208);
            this.passwortinput.Name = "passwortinput";
            this.passwortinput.PasswordChar = '●';
            this.passwortinput.Size = new System.Drawing.Size(130, 20);
            this.passwortinput.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(224, 164);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "Personalnummer";
            this.label1.Click += new System.EventHandler(this.Label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(224, 208);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 24);
            this.label2.TabIndex = 4;
            this.label2.Text = "Passwort";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(9, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(752, 29);
            this.label3.TabIndex = 5;
            this.label3.Text = "Willkommen zum Mangement Programm von der Pizzeria Vesuv";
            this.label3.Click += new System.EventHandler(this.Label3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(228, 255);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(93, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Exit";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Abbrechen_Button);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(24, 30);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(764, 77);
            this.panel1.TabIndex = 7;
            // 
            // Passwordunhide
            // 
            this.Passwordunhide.AutoSize = true;
            this.Passwordunhide.Location = new System.Drawing.Point(556, 211);
            this.Passwordunhide.Name = "Passwordunhide";
            this.Passwordunhide.Size = new System.Drawing.Size(115, 17);
            this.Passwordunhide.TabIndex = 8;
            this.Passwordunhide.Text = "Passwort anzeigen";
            this.Passwordunhide.UseVisualStyleBackColor = true;
            this.Passwordunhide.CheckedChanged += new System.EventHandler(this.Passwordunhide_CheckedChanged);
            // 
            // Loginpizzeriavesus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 449);
            this.Controls.Add(this.Passwordunhide);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.passwortinput);
            this.Controls.Add(this.usernameinput);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Loginpizzeriavesus";
            this.Text = "Pizzaria Vesuv";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox usernameinput;
        private System.Windows.Forms.TextBox passwortinput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox Passwordunhide;
    }
}

