namespace Pizzeria_Projekt_Schule
{
    partial class Speisehinzufügen
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
            this.components = new System.ComponentModel.Container();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Name_textBox2 = new System.Windows.Forms.TextBox();
            this.Speissen_typ_comboBox1 = new System.Windows.Forms.ComboBox();
            this.Preis_textBox3 = new System.Windows.Forms.TextBox();
            this.zutaten_textBox4 = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Location = new System.Drawing.Point(836, 504);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(200, 100);
            this.flowLayoutPanel2.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(36, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(9, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Speisen typ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.Control;
            this.label4.Location = new System.Drawing.Point(50, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 15);
            this.label4.TabIndex = 5;
            this.label4.Text = "Preis";
            this.label4.Click += new System.EventHandler(this.Label4_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.Control;
            this.label5.Location = new System.Drawing.Point(36, 126);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 15);
            this.label5.TabIndex = 6;
            this.label5.Text = "Zutaten";
            // 
            // Name_textBox2
            // 
            this.Name_textBox2.Location = new System.Drawing.Point(162, 66);
            this.Name_textBox2.Name = "Name_textBox2";
            this.Name_textBox2.Size = new System.Drawing.Size(147, 20);
            this.Name_textBox2.TabIndex = 8;
            this.Name_textBox2.TextChanged += new System.EventHandler(this.Name_textBox2_TextChanged);
            this.Name_textBox2.Leave += new System.EventHandler(this.TextBox2_Leave);
            // 
            // Speissen_typ_comboBox1
            // 
            this.Speissen_typ_comboBox1.FormattingEnabled = true;
            this.Speissen_typ_comboBox1.Items.AddRange(new object[] {
            "🥤 GETRÄNKE",
            "🍕 PIZZA",
            "🥗 SALATE",
            "🍝 PASTA",
            "🍰 DESSERT"});
            this.Speissen_typ_comboBox1.Location = new System.Drawing.Point(162, 102);
            this.Speissen_typ_comboBox1.Name = "Speissen_typ_comboBox1";
            this.Speissen_typ_comboBox1.Size = new System.Drawing.Size(147, 21);
            this.Speissen_typ_comboBox1.TabIndex = 9;
            this.Speissen_typ_comboBox1.SelectedIndexChanged += new System.EventHandler(this.Speissen_typ_comboBox1_SelectedIndexChanged);
            // 
            // Preis_textBox3
            // 
            this.Preis_textBox3.Location = new System.Drawing.Point(162, 137);
            this.Preis_textBox3.Name = "Preis_textBox3";
            this.Preis_textBox3.Size = new System.Drawing.Size(147, 20);
            this.Preis_textBox3.TabIndex = 10;
            this.Preis_textBox3.TextChanged += new System.EventHandler(this.Preis_textBox3_TextChanged);
            this.Preis_textBox3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox3_KeyPress);
            // 
            // zutaten_textBox4
            // 
            this.zutaten_textBox4.Location = new System.Drawing.Point(162, 173);
            this.zutaten_textBox4.Name = "zutaten_textBox4";
            this.zutaten_textBox4.Size = new System.Drawing.Size(147, 20);
            this.zutaten_textBox4.TabIndex = 11;
            this.zutaten_textBox4.TextChanged += new System.EventHandler(this.Zutaten_textBox4_TextChanged);
            this.zutaten_textBox4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox4_KeyPress);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Location = new System.Drawing.Point(37, 47);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(102, 162);
            this.panel1.TabIndex = 12;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.SystemColors.Control;
            this.button1.Location = new System.Drawing.Point(37, 236);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(156, 45);
            this.button1.TabIndex = 13;
            this.button1.Text = "Hinzufügen";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.Hinzufugen_button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.SystemColors.Control;
            this.button2.Location = new System.Drawing.Point(221, 236);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(110, 45);
            this.button2.TabIndex = 14;
            this.button2.Text = "Zurück";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.Abbrechen_button2_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // Speisehinzufügen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 328);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.zutaten_textBox4);
            this.Controls.Add(this.Preis_textBox3);
            this.Controls.Add(this.Speissen_typ_comboBox1);
            this.Controls.Add(this.Name_textBox2);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Name = "Speisehinzufügen";
            this.Text = "Hinzufügen";
            this.Load += new System.EventHandler(this.Speisenhin_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox Name_textBox2;
        private System.Windows.Forms.ComboBox Speissen_typ_comboBox1;
        private System.Windows.Forms.TextBox Preis_textBox3;
        private System.Windows.Forms.TextBox zutaten_textBox4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}