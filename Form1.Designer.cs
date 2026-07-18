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
            this.BtnUpload = new System.Windows.Forms.Button();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // BtnStartUpGun
            // 
            this.BtnStartUpGun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnStartUpGun.Image = ((System.Drawing.Image)(resources.GetObject("BtnStartUpGun.Image")));
            this.BtnStartUpGun.Location = new System.Drawing.Point(12, 12);
            this.BtnStartUpGun.Name = "BtnStartUpGun";
            this.BtnStartUpGun.Size = new System.Drawing.Size(32, 32);
            this.BtnStartUpGun.TabIndex = 0;
            this.BtnStartUpGun.UseVisualStyleBackColor = true;
            this.BtnStartUpGun.Click += new System.EventHandler(this.BtnStartUpGun_Click);
            // 
            // BtnUpload
            // 
            this.BtnUpload.Location = new System.Drawing.Point(435, 269);
            this.BtnUpload.Name = "BtnUpload";
            this.BtnUpload.Size = new System.Drawing.Size(137, 25);
            this.BtnUpload.TabIndex = 3;
            this.BtnUpload.Text = "Upload";
            this.BtnUpload.UseVisualStyleBackColor = true;
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(12, 50);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(417, 244);
            this.checkedListBox1.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(584, 461);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.BtnUpload);
            this.Controls.Add(this.BtnStartUpGun);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "UpGun Mod Tools Launcher";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnStartUpGun;
        private System.Windows.Forms.Button BtnUpload;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
    }
}