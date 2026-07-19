namespace UpGun_Mod_Tools_Launcher
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.BtnDiscordModding = new System.Windows.Forms.ToolStripMenuItem();
            this.upGunToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.upGunModdingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BtnGithub = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnUpload
            // 
            this.BtnUpload.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.BtnUpload.Location = new System.Drawing.Point(12, 310);
            this.BtnUpload.Name = "BtnUpload";
            this.BtnUpload.Size = new System.Drawing.Size(210, 25);
            this.BtnUpload.TabIndex = 3;
            this.BtnUpload.Text = "Upload";
            this.BtnUpload.UseVisualStyleBackColor = true;
            // 
            // BtnRefreshList
            // 
            this.BtnRefreshList.Location = new System.Drawing.Point(227, 310);
            this.BtnRefreshList.Name = "BtnRefreshList";
            this.BtnRefreshList.Size = new System.Drawing.Size(210, 25);
            this.BtnRefreshList.TabIndex = 5;
            this.BtnRefreshList.Text = "Refresh List";
            this.BtnRefreshList.UseVisualStyleBackColor = true;
            this.BtnRefreshList.Click += new System.EventHandler(this.BtnRefreshList_Click);
            // 
            // ListBoxWorkshopItem
            // 
            this.ListBoxWorkshopItem.BackColor = System.Drawing.Color.Gray;
            this.ListBoxWorkshopItem.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListBoxWorkshopItem.ForeColor = System.Drawing.SystemColors.Window;
            this.ListBoxWorkshopItem.FormattingEnabled = true;
            this.ListBoxWorkshopItem.ItemHeight = 21;
            this.ListBoxWorkshopItem.Location = new System.Drawing.Point(12, 27);
            this.ListBoxWorkshopItem.Name = "ListBoxWorkshopItem";
            this.ListBoxWorkshopItem.Size = new System.Drawing.Size(425, 256);
            this.ListBoxWorkshopItem.Sorted = true;
            this.ListBoxWorkshopItem.TabIndex = 0;
            this.ListBoxWorkshopItem.TabStop = false;
            this.ListBoxWorkshopItem.UseTabStops = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BtnDiscordModding,
            this.BtnGithub});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(558, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // BtnDiscordModding
            // 
            this.BtnDiscordModding.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.upGunToolStripMenuItem,
            this.upGunModdingToolStripMenuItem});
            this.BtnDiscordModding.Name = "BtnDiscordModding";
            this.BtnDiscordModding.Size = new System.Drawing.Size(59, 20);
            this.BtnDiscordModding.Text = "Discord";
            // 
            // upGunToolStripMenuItem
            // 
            this.upGunToolStripMenuItem.Name = "upGunToolStripMenuItem";
            this.upGunToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.upGunToolStripMenuItem.Text = "UpGun";
            this.upGunToolStripMenuItem.Click += new System.EventHandler(this.upGunToolStripMenuItem_Click);
            // 
            // upGunModdingToolStripMenuItem
            // 
            this.upGunModdingToolStripMenuItem.Name = "upGunModdingToolStripMenuItem";
            this.upGunModdingToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.upGunModdingToolStripMenuItem.Text = "UpGun Modding";
            // 
            // BtnGithub
            // 
            this.BtnGithub.Name = "BtnGithub";
            this.BtnGithub.Size = new System.Drawing.Size(55, 20);
            this.BtnGithub.Text = "Github";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(558, 501);
            this.Controls.Add(this.ListBoxWorkshopItem);
            this.Controls.Add(this.BtnRefreshList);
            this.Controls.Add(this.BtnUpload);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "UpGun Mod Tools Launcher";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button BtnUpload;
        private System.Windows.Forms.Button BtnRefreshList;
        private System.Windows.Forms.ListBox ListBoxWorkshopItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem BtnDiscordModding;
        private System.Windows.Forms.ToolStripMenuItem BtnGithub;
        private System.Windows.Forms.ToolStripMenuItem upGunToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem upGunModdingToolStripMenuItem;
    }
}