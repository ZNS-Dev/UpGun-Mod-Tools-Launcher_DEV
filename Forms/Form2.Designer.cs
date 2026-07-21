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
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.BtnCloseWindowPublish = new System.Windows.Forms.Button();
            this.BtnPublishMod = new System.Windows.Forms.Button();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.PathPak = new System.Windows.Forms.TextBox();
            this.BtnSelectPak = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.ImageIconPreview = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.BtnSelectIcon = new System.Windows.Forms.Button();
            this.TxtBoxPourcent = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ImageIconPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox2
            // 
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox2.Location = new System.Drawing.Point(12, 111);
            this.textBox2.MaxLength = 255;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(360, 20);
            this.textBox2.TabIndex = 3;
            // 
            // textBox3
            // 
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox3.Location = new System.Drawing.Point(16, 158);
            this.textBox3.MaxLength = 30000;
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(356, 70);
            this.textBox3.TabIndex = 5;
            // 
            // textBox5
            // 
            this.textBox5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox5.Location = new System.Drawing.Point(50, 269);
            this.textBox5.Name = "textBox5";
            this.textBox5.ReadOnly = true;
            this.textBox5.Size = new System.Drawing.Size(322, 20);
            this.textBox5.TabIndex = 7;
            // 
            // BtnCloseWindowPublish
            // 
            this.BtnCloseWindowPublish.Location = new System.Drawing.Point(258, 370);
            this.BtnCloseWindowPublish.Name = "BtnCloseWindowPublish";
            this.BtnCloseWindowPublish.Size = new System.Drawing.Size(75, 23);
            this.BtnCloseWindowPublish.TabIndex = 9;
            this.BtnCloseWindowPublish.Text = "Cancel";
            this.BtnCloseWindowPublish.UseVisualStyleBackColor = true;
            this.BtnCloseWindowPublish.Click += new System.EventHandler(this.BtnCloseWindowPublish_Click);
            // 
            // BtnPublishMod
            // 
            this.BtnPublishMod.Location = new System.Drawing.Point(258, 341);
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
            this.checkedListBox1.IntegralHeight = false;
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
            this.checkedListBox1.Location = new System.Drawing.Point(53, 295);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(156, 139);
            this.checkedListBox1.Sorted = true;
            this.checkedListBox1.TabIndex = 0;
            this.checkedListBox1.TabStop = false;
            // 
            // PathPak
            // 
            this.PathPak.AllowDrop = true;
            this.PathPak.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PathPak.Location = new System.Drawing.Point(12, 62);
            this.PathPak.Name = "PathPak";
            this.PathPak.ReadOnly = true;
            this.PathPak.Size = new System.Drawing.Size(360, 20);
            this.PathPak.TabIndex = 13;
            // 
            // BtnSelectPak
            // 
            this.BtnSelectPak.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSelectPak.Location = new System.Drawing.Point(12, 33);
            this.BtnSelectPak.Name = "BtnSelectPak";
            this.BtnSelectPak.Size = new System.Drawing.Size(360, 23);
            this.BtnSelectPak.TabIndex = 14;
            this.BtnSelectPak.Text = "Drag your file .pak here, or click to browse";
            this.BtnSelectPak.UseVisualStyleBackColor = true;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 439);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(314, 10);
            this.progressBar1.TabIndex = 0;
            // 
            // ImageIconPreview
            // 
            this.ImageIconPreview.Location = new System.Drawing.Point(12, 263);
            this.ImageIconPreview.Name = "ImageIconPreview";
            this.ImageIconPreview.Size = new System.Drawing.Size(32, 32);
            this.ImageIconPreview.TabIndex = 17;
            this.ImageIconPreview.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Myanmar Text", 9F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.SystemColors.Window;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 21);
            this.label1.TabIndex = 20;
            this.label1.Text = "File .pak";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Myanmar Text", 9F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.SystemColors.Window;
            this.label2.Location = new System.Drawing.Point(12, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 21);
            this.label2.TabIndex = 21;
            this.label2.Text = "Title";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Myanmar Text", 9F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.SystemColors.Window;
            this.label3.Location = new System.Drawing.Point(12, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 21);
            this.label3.TabIndex = 22;
            this.label3.Text = "Description";
            // 
            // BtnSelectIcon
            // 
            this.BtnSelectIcon.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnSelectIcon.Location = new System.Drawing.Point(12, 234);
            this.BtnSelectIcon.Name = "BtnSelectIcon";
            this.BtnSelectIcon.Size = new System.Drawing.Size(360, 23);
            this.BtnSelectIcon.TabIndex = 23;
            this.BtnSelectIcon.Text = "Drag your file .png here, or click to browse";
            this.BtnSelectIcon.UseVisualStyleBackColor = true;
            // 
            // TxtBoxPourcent
            // 
            this.TxtBoxPourcent.AutoSize = true;
            this.TxtBoxPourcent.Font = new System.Drawing.Font("Myanmar Text", 9F, System.Drawing.FontStyle.Bold);
            this.TxtBoxPourcent.ForeColor = System.Drawing.SystemColors.Window;
            this.TxtBoxPourcent.Location = new System.Drawing.Point(332, 437);
            this.TxtBoxPourcent.Name = "TxtBoxPourcent";
            this.TxtBoxPourcent.Size = new System.Drawing.Size(41, 21);
            this.TxtBoxPourcent.TabIndex = 24;
            this.TxtBoxPourcent.Text = "100%";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Myanmar Text", 9F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.SystemColors.Window;
            this.label4.Location = new System.Drawing.Point(12, 298);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 21);
            this.label4.TabIndex = 25;
            this.label4.Text = "Tags";
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(384, 461);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.TxtBoxPourcent);
            this.Controls.Add(this.BtnSelectIcon);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ImageIconPreview);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.BtnSelectPak);
            this.Controls.Add(this.PathPak);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.BtnPublishMod);
            this.Controls.Add(this.BtnCloseWindowPublish);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Publish Mod";
            ((System.ComponentModel.ISupportInitialize)(this.ImageIconPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Button BtnCloseWindowPublish;
        private System.Windows.Forms.Button BtnPublishMod;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.TextBox PathPak;
        private System.Windows.Forms.Button BtnSelectPak;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.PictureBox ImageIconPreview;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button BtnSelectIcon;
        private System.Windows.Forms.Label TxtBoxPourcent;
        private System.Windows.Forms.Label label4;
    }
}