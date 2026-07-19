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
            this.BtnUpload = new System.Windows.Forms.Button();
            this.ListBoxWorkshopItem = new System.Windows.Forms.CheckedListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtnUpload
            // 
            this.BtnUpload.Location = new System.Drawing.Point(12, 300);
            this.BtnUpload.Name = "BtnUpload";
            this.BtnUpload.Size = new System.Drawing.Size(210, 25);
            this.BtnUpload.TabIndex = 3;
            this.BtnUpload.Text = "Upload";
            this.BtnUpload.UseVisualStyleBackColor = true;
            // 
            // ListBoxWorkshopItem
            // 
            this.ListBoxWorkshopItem.FormattingEnabled = true;
            this.ListBoxWorkshopItem.Location = new System.Drawing.Point(12, 50);
            this.ListBoxWorkshopItem.Name = "ListBoxWorkshopItem";
            this.ListBoxWorkshopItem.Size = new System.Drawing.Size(560, 244);
            this.ListBoxWorkshopItem.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(227, 300);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(210, 25);
            this.button1.TabIndex = 5;
            this.button1.Text = "Refresh List";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(584, 461);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ListBoxWorkshopItem);
            this.Controls.Add(this.BtnUpload);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "UpGun Mod Tools Launcher";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button BtnUpload;
        private System.Windows.Forms.CheckedListBox ListBoxWorkshopItem;
        private System.Windows.Forms.Button button1;
    }
}