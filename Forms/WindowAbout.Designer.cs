namespace UpGun_Mod_Tools_Launcher.Forms
{
    partial class WindowAbout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WindowAbout));
            this.TxtBoxMessageAbout = new System.Windows.Forms.TextBox();
            this.BtnAboutOk = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // TxtBoxMessageAbout
            // 
            this.TxtBoxMessageAbout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.TxtBoxMessageAbout.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TxtBoxMessageAbout.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.TxtBoxMessageAbout.Font = new System.Drawing.Font("Myanmar Text", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtBoxMessageAbout.ForeColor = System.Drawing.SystemColors.Window;
            this.TxtBoxMessageAbout.Location = new System.Drawing.Point(61, 39);
            this.TxtBoxMessageAbout.Multiline = true;
            this.TxtBoxMessageAbout.Name = "TxtBoxMessageAbout";
            this.TxtBoxMessageAbout.ReadOnly = true;
            this.TxtBoxMessageAbout.ShortcutsEnabled = false;
            this.TxtBoxMessageAbout.Size = new System.Drawing.Size(201, 50);
            this.TxtBoxMessageAbout.TabIndex = 8;
            this.TxtBoxMessageAbout.TabStop = false;
            this.TxtBoxMessageAbout.Text = "UpGun Mod Tools Launcher\r\nCopyright 2026 ZNS - Paulem";
            this.TxtBoxMessageAbout.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BtnAboutOk
            // 
            this.BtnAboutOk.BackColor = System.Drawing.Color.White;
            this.BtnAboutOk.Location = new System.Drawing.Point(120, 90);
            this.BtnAboutOk.Name = "BtnAboutOk";
            this.BtnAboutOk.Size = new System.Drawing.Size(32, 32);
            this.BtnAboutOk.TabIndex = 9;
            this.BtnAboutOk.Text = "OK";
            this.BtnAboutOk.UseVisualStyleBackColor = false;
            this.BtnAboutOk.Click += new System.EventHandler(this.BtnAboutOk_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(7, 39);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // WindowAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(274, 136);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.BtnAboutOk);
            this.Controls.Add(this.TxtBoxMessageAbout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WindowAbout";
            this.Text = "About Mod Tools Launcher";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TxtBoxMessageAbout;
        private System.Windows.Forms.Button BtnAboutOk;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}