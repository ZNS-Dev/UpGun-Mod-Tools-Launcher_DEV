namespace UpGun_Mods_Tool_Launcher
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.BtnStartUpGun = new System.Windows.Forms.Button();
            this.BtnSelectPak = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.BtnUpload = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtnStartUpGun
            // 
            this.BtnStartUpGun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnStartUpGun.Image = ((System.Drawing.Image)(resources.GetObject("BtnStartUpGun.Image")));
            this.BtnStartUpGun.Location = new System.Drawing.Point(228, 84);
            this.BtnStartUpGun.Name = "BtnStartUpGun";
            this.BtnStartUpGun.Size = new System.Drawing.Size(32, 32);
            this.BtnStartUpGun.TabIndex = 0;
            this.BtnStartUpGun.UseVisualStyleBackColor = true;
            this.BtnStartUpGun.Click += new System.EventHandler(this.BtnStartUpGun_Click);
            // 
            // BtnSelectPak
            // 
            this.BtnSelectPak.Location = new System.Drawing.Point(362, 180);
            this.BtnSelectPak.Name = "BtnSelectPak";
            this.BtnSelectPak.Size = new System.Drawing.Size(100, 25);
            this.BtnSelectPak.TabIndex = 1;
            this.BtnSelectPak.Text = "Select File Pak";
            this.BtnSelectPak.UseVisualStyleBackColor = true;
            this.BtnSelectPak.Click += new System.EventHandler(this.BtnUpload_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 273);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(557, 20);
            this.textBox1.TabIndex = 2;
            // 
            // BtnUpload
            // 
            this.BtnUpload.Location = new System.Drawing.Point(362, 211);
            this.BtnUpload.Name = "BtnUpload";
            this.BtnUpload.Size = new System.Drawing.Size(100, 25);
            this.BtnUpload.TabIndex = 3;
            this.BtnUpload.Text = "Upload";
            this.BtnUpload.UseVisualStyleBackColor = true;
            this.BtnUpload.Click += new System.EventHandler(this.BtnUpload_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.ClientSize = new System.Drawing.Size(584, 461);
            this.Controls.Add(this.BtnUpload);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.BtnSelectPak);
            this.Controls.Add(this.BtnStartUpGun);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "UpGun Mod Tools Launcher";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnStartUpGun;
        private System.Windows.Forms.Button BtnSelectPak;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button BtnUpload;
    }
}