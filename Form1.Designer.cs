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
            this.BtnRefreshList = new System.Windows.Forms.Button();
            this.ListBoxWorkshopItem = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // BtnUpload
            // 
            this.BtnUpload.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.BtnUpload.Location = new System.Drawing.Point(12, 300);
            this.BtnUpload.Name = "BtnUpload";
            this.BtnUpload.Size = new System.Drawing.Size(210, 25);
            this.BtnUpload.TabIndex = 3;
            this.BtnUpload.Text = "Upload";
            this.BtnUpload.UseVisualStyleBackColor = true;
            // 
            // BtnRefreshList
            // 
            this.BtnRefreshList.Location = new System.Drawing.Point(227, 300);
            this.BtnRefreshList.Name = "BtnRefreshList";
            this.BtnRefreshList.Size = new System.Drawing.Size(210, 25);
            this.BtnRefreshList.TabIndex = 5;
            this.BtnRefreshList.Text = "Refresh List";
            this.BtnRefreshList.UseVisualStyleBackColor = true;
            this.BtnRefreshList.Click += new System.EventHandler(this.button1_Click);
            // 
            // ListBoxWorkshopItem
            // 
            this.ListBoxWorkshopItem.BackColor = System.Drawing.Color.Gray;
            this.ListBoxWorkshopItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListBoxWorkshopItem.ForeColor = System.Drawing.SystemColors.Window;
            this.ListBoxWorkshopItem.FormattingEnabled = true;
            this.ListBoxWorkshopItem.ItemHeight = 16;
            this.ListBoxWorkshopItem.Location = new System.Drawing.Point(12, 17);
            this.ListBoxWorkshopItem.Name = "ListBoxWorkshopItem";
            this.ListBoxWorkshopItem.Size = new System.Drawing.Size(560, 276);
            this.ListBoxWorkshopItem.Sorted = true;
            this.ListBoxWorkshopItem.TabIndex = 0;
            this.ListBoxWorkshopItem.TabStop = false;
            this.ListBoxWorkshopItem.UseTabStops = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(584, 461);
            this.Controls.Add(this.ListBoxWorkshopItem);
            this.Controls.Add(this.BtnRefreshList);
            this.Controls.Add(this.BtnUpload);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "UpGun Mod Tools Launcher";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button BtnUpload;
        private System.Windows.Forms.Button BtnRefreshList;
        private System.Windows.Forms.ListBox ListBoxWorkshopItem;
    }
}