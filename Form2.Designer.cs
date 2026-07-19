namespace UpGun_Mod_Tools_Launcher
{
    partial class Form2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.BtnSelectIcon = new System.Windows.Forms.Button();
            this.BtnCloseWindowPublish = new System.Windows.Forms.Button();
            this.BtnPublishMod = new System.Windows.Forms.Button();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.PathPak = new System.Windows.Forms.TextBox();
            this.BtnSelectPak = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(72, 195);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(35, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "Title:";
            // 
            // textBox2
            // 
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox2.Location = new System.Drawing.Point(141, 195);
            this.textBox2.MaxLength = 255;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(216, 20);
            this.textBox2.TabIndex = 3;
            // 
            // textBox3
            // 
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox3.Location = new System.Drawing.Point(141, 221);
            this.textBox3.MaxLength = 30000;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(216, 20);
            this.textBox3.TabIndex = 5;
            // 
            // textBox4
            // 
            this.textBox4.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox4.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.textBox4.ForeColor = System.Drawing.Color.White;
            this.textBox4.Location = new System.Drawing.Point(72, 221);
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(63, 20);
            this.textBox4.TabIndex = 4;
            this.textBox4.Text = "Description:";
            // 
            // textBox5
            // 
            this.textBox5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox5.Location = new System.Drawing.Point(110, 247);
            this.textBox5.Name = "textBox5";
            this.textBox5.ReadOnly = true;
            this.textBox5.Size = new System.Drawing.Size(210, 20);
            this.textBox5.TabIndex = 7;
            // 
            // textBox6
            // 
            this.textBox6.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.textBox6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox6.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBox6.ForeColor = System.Drawing.Color.White;
            this.textBox6.Location = new System.Drawing.Point(72, 247);
            this.textBox6.Name = "textBox6";
            this.textBox6.ReadOnly = true;
            this.textBox6.Size = new System.Drawing.Size(32, 20);
            this.textBox6.TabIndex = 6;
            this.textBox6.Text = "Icon:";
            this.textBox6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // BtnSelectIcon
            // 
            this.BtnSelectIcon.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSelectIcon.Location = new System.Drawing.Point(326, 247);
            this.BtnSelectIcon.Name = "BtnSelectIcon";
            this.BtnSelectIcon.Size = new System.Drawing.Size(31, 20);
            this.BtnSelectIcon.TabIndex = 8;
            this.BtnSelectIcon.Text = "...";
            this.BtnSelectIcon.UseVisualStyleBackColor = true;
            // 
            // BtnCloseWindowPublish
            // 
            this.BtnCloseWindowPublish.Location = new System.Drawing.Point(190, 400);
            this.BtnCloseWindowPublish.Name = "BtnCloseWindowPublish";
            this.BtnCloseWindowPublish.Size = new System.Drawing.Size(75, 23);
            this.BtnCloseWindowPublish.TabIndex = 9;
            this.BtnCloseWindowPublish.Text = "Cancel";
            this.BtnCloseWindowPublish.UseVisualStyleBackColor = true;
            this.BtnCloseWindowPublish.Click += new System.EventHandler(this.BtnCloseWindowPublish_Click);
            // 
            // BtnPublishMod
            // 
            this.BtnPublishMod.Location = new System.Drawing.Point(110, 400);
            this.BtnPublishMod.Name = "BtnPublishMod";
            this.BtnPublishMod.Size = new System.Drawing.Size(75, 23);
            this.BtnPublishMod.TabIndex = 10;
            this.BtnPublishMod.Text = "Publish Mod";
            this.BtnPublishMod.UseVisualStyleBackColor = true;
            this.BtnPublishMod.Click += new System.EventHandler(this.BtnPublishMod_Click);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.CheckOnClick = true;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Items.AddRange(new object[] {
            "AK-Thodik",
            "Animation",
            "Armor",
            "Audio",
            "Character",
            "Face",
            "Gamemode",
            "Knife",
            "Map",
            "Model",
            "Settings",
            "UI",
            "Upgrade",
            "Weapon",
            "WIP"});
            this.checkedListBox1.Location = new System.Drawing.Point(110, 273);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(155, 124);
            this.checkedListBox1.Sorted = true;
            this.checkedListBox1.TabIndex = 0;
            this.checkedListBox1.TabStop = false;
            // 
            // PathPak
            // 
            this.PathPak.AllowDrop = true;
            this.PathPak.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PathPak.Location = new System.Drawing.Point(113, 131);
            this.PathPak.Name = "PathPak";
            this.PathPak.ReadOnly = true;
            this.PathPak.Size = new System.Drawing.Size(216, 20);
            this.PathPak.TabIndex = 13;
            // 
            // BtnSelectPak
            // 
            this.BtnSelectPak.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSelectPak.Location = new System.Drawing.Point(335, 131);
            this.BtnSelectPak.Name = "BtnSelectPak";
            this.BtnSelectPak.Size = new System.Drawing.Size(31, 20);
            this.BtnSelectPak.TabIndex = 14;
            this.BtnSelectPak.Text = "...";
            this.BtnSelectPak.UseVisualStyleBackColor = true;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(384, 461);
            this.Controls.Add(this.BtnSelectPak);
            this.Controls.Add(this.PathPak);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.BtnPublishMod);
            this.Controls.Add(this.BtnCloseWindowPublish);
            this.Controls.Add(this.BtnSelectIcon);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.textBox6);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Publish Mod";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Button BtnSelectIcon;
        private System.Windows.Forms.Button BtnCloseWindowPublish;
        private System.Windows.Forms.Button BtnPublishMod;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.TextBox PathPak;
        private System.Windows.Forms.Button BtnSelectPak;
    }
}