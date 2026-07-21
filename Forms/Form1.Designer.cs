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
            this.BtnDiscordUG = new System.Windows.Forms.ToolStripMenuItem();
            this.BtnDiscordUGModding = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TxtBoxMessage = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripTextBox2 = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnUpload
            // 
            this.BtnUpload.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.BtnUpload.Location = new System.Drawing.Point(421, 50);
            this.BtnUpload.Name = "BtnUpload";
            this.BtnUpload.Size = new System.Drawing.Size(125, 30);
            this.BtnUpload.TabIndex = 3;
            this.BtnUpload.Text = "Upload";
            this.BtnUpload.UseVisualStyleBackColor = true;
            // 
            // BtnRefreshList
            // 
            this.BtnRefreshList.Location = new System.Drawing.Point(421, 86);
            this.BtnRefreshList.Name = "BtnRefreshList";
            this.BtnRefreshList.Size = new System.Drawing.Size(125, 30);
            this.BtnRefreshList.TabIndex = 5;
            this.BtnRefreshList.Text = "Refresh List";
            this.BtnRefreshList.UseVisualStyleBackColor = true;
            // 
            // ListBoxWorkshopItem
            // 
            this.ListBoxWorkshopItem.BackColor = System.Drawing.Color.Gray;
            this.ListBoxWorkshopItem.Font = new System.Drawing.Font("Franklin Gothic Medium", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListBoxWorkshopItem.ForeColor = System.Drawing.SystemColors.Window;
            this.ListBoxWorkshopItem.FormattingEnabled = true;
            this.ListBoxWorkshopItem.ItemHeight = 21;
            this.ListBoxWorkshopItem.Items.AddRange(new object[] {
            "NO TITLE! (000.00MB)",
            "NO TITLE! (000.00MB)",
            "NO TITLE! (000.00MB)",
            "NO TITLE! (000.00MB)",
            "NO TITLE! (000.00MB)",
            "NO TITLE! (000.00MB)",
            "NO TITLE! (000.00MB)",
            "NO TITLE! (000.00MB)",
            "NO TITLE! (000.00MB)",
            "NO TITLE! (000.00MB)",
            "NO TITLE! (000.00MB)",
            "NO TITLE! (000.00MB)",
            "NO TITLE! (000.00MB)",
            "NO TITLE! (000.00MB)",
            "NO TITLE! (000.00MB)",
            "NO TITLE! (000.00MB)",
            "NO TITLE! (000.00MB)",
            "NO TITLE! (000.00MB)",
            "NO TITLE! (000.00MB)",
            "NO TITLE! (000.00MB)",
            "NO TITLE! (000.00MB)",
            "NO TITLE! (000.00MB)"});
            this.ListBoxWorkshopItem.Location = new System.Drawing.Point(12, 50);
            this.ListBoxWorkshopItem.Name = "ListBoxWorkshopItem";
            this.ListBoxWorkshopItem.Size = new System.Drawing.Size(403, 382);
            this.ListBoxWorkshopItem.Sorted = true;
            this.ListBoxWorkshopItem.TabIndex = 0;
            this.ListBoxWorkshopItem.TabStop = false;
            this.ListBoxWorkshopItem.UseTabStops = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BtnDiscordModding,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(558, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // BtnDiscordModding
            // 
            this.BtnDiscordModding.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BtnDiscordUG,
            this.BtnDiscordUGModding});
            this.BtnDiscordModding.Name = "BtnDiscordModding";
            this.BtnDiscordModding.Size = new System.Drawing.Size(59, 20);
            this.BtnDiscordModding.Text = "Discord";
            // 
            // BtnDiscordUG
            // 
            this.BtnDiscordUG.Name = "BtnDiscordUG";
            this.BtnDiscordUG.Size = new System.Drawing.Size(163, 22);
            this.BtnDiscordUG.Text = "UpGun";
            this.BtnDiscordUG.Click += new System.EventHandler(this.BtnDiscordUG_Click);
            // 
            // BtnDiscordUGModding
            // 
            this.BtnDiscordUGModding.Name = "BtnDiscordUGModding";
            this.BtnDiscordUGModding.Size = new System.Drawing.Size(163, 22);
            this.BtnDiscordUGModding.Text = "UpGun Modding";
            this.BtnDiscordUGModding.Click += new System.EventHandler(this.BtnDiscordUGModding_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.helpToolStripMenuItem.Text = "About";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // TxtBoxMessage
            // 
            this.TxtBoxMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.TxtBoxMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TxtBoxMessage.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.TxtBoxMessage.ForeColor = System.Drawing.SystemColors.Window;
            this.TxtBoxMessage.Location = new System.Drawing.Point(0, 442);
            this.TxtBoxMessage.Multiline = true;
            this.TxtBoxMessage.Name = "TxtBoxMessage";
            this.TxtBoxMessage.ReadOnly = true;
            this.TxtBoxMessage.ShortcutsEnabled = false;
            this.TxtBoxMessage.Size = new System.Drawing.Size(558, 31);
            this.TxtBoxMessage.TabIndex = 7;
            this.TxtBoxMessage.TabStop = false;
            this.TxtBoxMessage.Text = resources.GetString("TxtBoxMessage.Text");
            this.TxtBoxMessage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox2,
            this.toolStripTextBox1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 476);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(558, 25);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripTextBox2
            // 
            this.toolStripTextBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.toolStripTextBox2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStripTextBox2.ForeColor = System.Drawing.SystemColors.Window;
            this.toolStripTextBox2.Name = "toolStripTextBox2";
            this.toolStripTextBox2.ReadOnly = true;
            this.toolStripTextBox2.ShortcutsEnabled = false;
            this.toolStripTextBox2.Size = new System.Drawing.Size(40, 25);
            this.toolStripTextBox2.Text = "v1.0.0";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.toolStripTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.toolStripTextBox1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStripTextBox1.ForeColor = System.Drawing.Color.Lime;
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.ReadOnly = true;
            this.toolStripTextBox1.ShortcutsEnabled = false;
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 25);
            this.toolStripTextBox1.Text = "Last version";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(558, 501);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.TxtBoxMessage);
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
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button BtnUpload;
        private System.Windows.Forms.Button BtnRefreshList;
        private System.Windows.Forms.ListBox ListBoxWorkshopItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem BtnDiscordModding;
        private System.Windows.Forms.ToolStripMenuItem BtnDiscordUG;
        private System.Windows.Forms.ToolStripMenuItem BtnDiscordUGModding;
        private System.Windows.Forms.TextBox TxtBoxMessage;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox2;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    }
}