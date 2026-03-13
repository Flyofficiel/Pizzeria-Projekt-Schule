namespace Pizzeria_Projekt_Schule
{
    partial class tischauswahl
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
            this.button41 = new System.Windows.Forms.Button();
            this.Tischauswahl_dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tischauswahl_dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1025, 85);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(424, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(200, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tisch auswahl";
            // 
            // button41
            // 
            this.button41.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(43)))), ((int)(((byte)(43)))));
            this.button41.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button41.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button41.ForeColor = System.Drawing.SystemColors.Control;
            this.button41.Location = new System.Drawing.Point(712, 473);
            this.button41.Name = "button41";
            this.button41.Size = new System.Drawing.Size(174, 42);
            this.button41.TabIndex = 12;
            this.button41.Text = "Zurück";
            this.button41.UseVisualStyleBackColor = false;
            this.button41.Click += new System.EventHandler(this.Zuruck_button41_Click);
            // 
            // Tischauswahl_dataGridView1
            // 
            this.Tischauswahl_dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Tischauswahl_dataGridView1.Location = new System.Drawing.Point(182, 125);
            this.Tischauswahl_dataGridView1.Name = "Tischauswahl_dataGridView1";
            this.Tischauswahl_dataGridView1.Size = new System.Drawing.Size(704, 314);
            this.Tischauswahl_dataGridView1.TabIndex = 13;
           
            // 
            // tischauswahl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1049, 543);
            this.Controls.Add(this.Tischauswahl_dataGridView1);
            this.Controls.Add(this.button41);
            this.Controls.Add(this.panel1);
            this.Name = "tischauswahl";
            this.Text = "Tischauwahl";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Tischauswahl_dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button41;
        private System.Windows.Forms.DataGridView Tischauswahl_dataGridView1;
    }
}