namespace Pizzeria_Projekt_Schule
{
    partial class Reservierungsseite
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.mySqlCommand1 = new MySqlConnector.MySqlCommand();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Telefon_textBox2 = new System.Windows.Forms.TextBox();
            this.Name_textBox1 = new System.Windows.Forms.TextBox();
            this.Tischauswahl_comboBox2 = new System.Windows.Forms.ComboBox();
            this.nureservierung_personenzahl_numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.Uhrzeit_comboBox1 = new System.Windows.Forms.ComboBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.guestuebernehmen = new System.Windows.Forms.Button();
            this.stammgaste_dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nureservierung_personenzahl_numericUpDown1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.stammgaste_dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.ForeColor = System.Drawing.SystemColors.Control;
            this.panel1.Location = new System.Drawing.Point(13, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(958, 77);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(341, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(261, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "Reservierung buchen";
            // 
            // mySqlCommand1
            // 
            this.mySqlCommand1.CommandTimeout = 0;
            this.mySqlCommand1.Connection = null;
            this.mySqlCommand1.Transaction = null;
            this.mySqlCommand1.UpdatedRowSource = System.Data.UpdateRowSource.None;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel2.Controls.Add(this.Telefon_textBox2);
            this.panel2.Controls.Add(this.Name_textBox1);
            this.panel2.Controls.Add(this.Tischauswahl_comboBox2);
            this.panel2.Controls.Add(this.nureservierung_personenzahl_numericUpDown1);
            this.panel2.Controls.Add(this.Uhrzeit_comboBox1);
            this.panel2.Controls.Add(this.dateTimePicker1);
            this.panel2.Location = new System.Drawing.Point(119, 10);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(260, 318);
            this.panel2.TabIndex = 1;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // textBox2
            // 
            this.Telefon_textBox2.Location = new System.Drawing.Point(27, 270);
            this.Telefon_textBox2.Name = "textBox2";
            this.Telefon_textBox2.Size = new System.Drawing.Size(200, 20);
            this.Telefon_textBox2.TabIndex = 5;
            this.Telefon_textBox2.TextChanged += new System.EventHandler(this.Telefon_textBox2_TextChanged);
            this.Telefon_textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox2_KeyPress);
            // 
            // textBox1
            // 
            this.Name_textBox1.Location = new System.Drawing.Point(27, 226);
            this.Name_textBox1.Name = "textBox1";
            this.Name_textBox1.Size = new System.Drawing.Size(200, 20);
            this.Name_textBox1.TabIndex = 4;
            this.Name_textBox1.TextChanged += new System.EventHandler(this.Name_textBox1_TextChanged);
            this.Name_textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Name_textBox1_KeyPress);
            // 
            // Tischauswahl_comboBox2
            // 
            this.Tischauswahl_comboBox2.FormattingEnabled = true;
            this.Tischauswahl_comboBox2.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40"});
            this.Tischauswahl_comboBox2.Location = new System.Drawing.Point(27, 178);
            this.Tischauswahl_comboBox2.Name = "Tischauswahl_comboBox2";
            this.Tischauswahl_comboBox2.Size = new System.Drawing.Size(200, 21);
            this.Tischauswahl_comboBox2.TabIndex = 3;
            this.Tischauswahl_comboBox2.SelectedIndexChanged += new System.EventHandler(this.Tischauswahl_comboBox2_SelectedIndexChanged);
            // 
            // nureservierung_personenzahl_numericUpDown1
            // 
            this.nureservierung_personenzahl_numericUpDown1.Location = new System.Drawing.Point(27, 127);
            this.nureservierung_personenzahl_numericUpDown1.Name = "nureservierung_personenzahl_numericUpDown1";
            this.nureservierung_personenzahl_numericUpDown1.Size = new System.Drawing.Size(200, 20);
            this.nureservierung_personenzahl_numericUpDown1.TabIndex = 2;
            this.nureservierung_personenzahl_numericUpDown1.ValueChanged += new System.EventHandler(this.reservierung_personenzahl_numericUpDown1_ValueChanged);
            // 
            // Uhrzeit_comboBox1
            // 
            this.Uhrzeit_comboBox1.FormattingEnabled = true;
            this.Uhrzeit_comboBox1.Items.AddRange(new object[] {
            "12-15",
            "15-18",
            "18-21",
            "21-24"});
            this.Uhrzeit_comboBox1.Location = new System.Drawing.Point(27, 77);
            this.Uhrzeit_comboBox1.Name = "Uhrzeit_comboBox1";
            this.Uhrzeit_comboBox1.Size = new System.Drawing.Size(200, 21);
            this.Uhrzeit_comboBox1.TabIndex = 1;
            this.Uhrzeit_comboBox1.SelectedIndexChanged += new System.EventHandler(this.Uhrzeit_comboBox1_SelectedIndexChanged);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CustomFormat = "dd.MM.yyyy";
            this.dateTimePicker1.Location = new System.Drawing.Point(27, 36);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 0;
            this.dateTimePicker1.Value = new System.DateTime(2026, 1, 28, 0, 0, 0, 0);
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.reservierung_dateTimePicker1_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Datum:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(9, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 15);
            this.label3.TabIndex = 3;
            this.label3.Text = "Uhrzeit:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(9, 137);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "Personen:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(9, 188);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 15);
            this.label5.TabIndex = 5;
            this.label5.Text = "Tisch:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(9, 236);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 15);
            this.label6.TabIndex = 6;
            this.label6.Text = "Name:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(9, 281);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 15);
            this.label7.TabIndex = 7;
            this.label7.Text = "Telefon:";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.SystemColors.Control;
            this.button1.Location = new System.Drawing.Point(655, 187);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(206, 61);
            this.button1.TabIndex = 8;
            this.button1.Text = "Reservierung speichern";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.Reservierungspeichern_Button);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.SystemColors.Control;
            this.button2.Location = new System.Drawing.Point(655, 358);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(206, 61);
            this.button2.TabIndex = 9;
            this.button2.Text = "Abbrechen";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.abbrechen_button2_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(13, 120);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(565, 393);
            this.tabControl1.TabIndex = 10;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(557, 367);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Reservierung";
            this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.guestuebernehmen);
            this.tabPage2.Controls.Add(this.stammgaste_dataGridView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(557, 367);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Stammgäste";
            this.tabPage2.Click += new System.EventHandler(this.tabPage2_Click);
            // 
            // guestuebernehmen
            // 
            this.guestuebernehmen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.guestuebernehmen.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.guestuebernehmen.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guestuebernehmen.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.guestuebernehmen.Location = new System.Drawing.Point(419, 139);
            this.guestuebernehmen.Name = "guestuebernehmen";
            this.guestuebernehmen.Size = new System.Drawing.Size(114, 48);
            this.guestuebernehmen.TabIndex = 1;
            this.guestuebernehmen.Text = "Gast Übernehmen";
            this.guestuebernehmen.UseVisualStyleBackColor = false;
            this.guestuebernehmen.Click += new System.EventHandler(this.guestuebernehmen_Click);
            // 
            // dataGridView1
            // 
            this.stammgaste_dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            this.stammgaste_dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.stammgaste_dataGridView1.Location = new System.Drawing.Point(25, 21);
            this.stammgaste_dataGridView1.Name = "dataGridView1";
            this.stammgaste_dataGridView1.Size = new System.Drawing.Size(368, 328);
            this.stammgaste_dataGridView1.TabIndex = 0;
            // 
            // Reservierungsseite
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(983, 547);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.Name = "Reservierungsseite";
            this.Text = "Reservierung";
            this.Load += new System.EventHandler(this.reservierung_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nureservierung_personenzahl_numericUpDown1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.stammgaste_dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private MySqlConnector.MySqlCommand mySqlCommand1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox Telefon_textBox2;
        private System.Windows.Forms.TextBox Name_textBox1;
        private System.Windows.Forms.ComboBox Tischauswahl_comboBox2;
        private System.Windows.Forms.NumericUpDown nureservierung_personenzahl_numericUpDown1;
        private System.Windows.Forms.ComboBox Uhrzeit_comboBox1;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button guestuebernehmen;
        private System.Windows.Forms.DataGridView stammgaste_dataGridView1;
    }
}