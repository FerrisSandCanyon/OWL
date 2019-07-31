namespace vactrak.Forms
{
    partial class Settings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.label1 = new System.Windows.Forms.Label();
            this.tbPath = new System.Windows.Forms.TextBox();
            this.tbParam = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbCD = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbThread = new System.Windows.Forms.TextBox();
            this.cbForce = new System.Windows.Forms.CheckBox();
            this.cbMask = new System.Windows.Forms.CheckBox();
            this.btnBrowse = new System.Windows.Forms.LinkLabel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClip = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(69, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Steam Path:";
            // 
            // tbPath
            // 
            this.tbPath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.tbPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbPath.ForeColor = System.Drawing.Color.White;
            this.tbPath.Location = new System.Drawing.Point(142, 12);
            this.tbPath.Name = "tbPath";
            this.tbPath.ReadOnly = true;
            this.tbPath.Size = new System.Drawing.Size(212, 22);
            this.tbPath.TabIndex = 12;
            this.tbPath.Text = "C:/Program Files (x86)/Steam";
            // 
            // tbParam
            // 
            this.tbParam.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.tbParam.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbParam.ForeColor = System.Drawing.Color.White;
            this.tbParam.Location = new System.Drawing.Point(142, 40);
            this.tbParam.Name = "tbParam";
            this.tbParam.Size = new System.Drawing.Size(212, 22);
            this.tbParam.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Steam Parameters:";
            // 
            // tbCD
            // 
            this.tbCD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.tbCD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCD.ForeColor = System.Drawing.Color.White;
            this.tbCD.Location = new System.Drawing.Point(142, 68);
            this.tbCD.Name = "tbCD";
            this.tbCD.Size = new System.Drawing.Size(46, 22);
            this.tbCD.TabIndex = 16;
            this.tbCD.Text = "800";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Cooldown refresh rate:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(194, 70);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(21, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "ms";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(221, 70);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Max Threads:";
            // 
            // tbThread
            // 
            this.tbThread.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.tbThread.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbThread.ForeColor = System.Drawing.Color.White;
            this.tbThread.Location = new System.Drawing.Point(301, 68);
            this.tbThread.Name = "tbThread";
            this.tbThread.Size = new System.Drawing.Size(53, 22);
            this.tbThread.TabIndex = 19;
            this.tbThread.Text = "4";
            // 
            // cbForce
            // 
            this.cbForce.AutoSize = true;
            this.cbForce.Location = new System.Drawing.Point(142, 125);
            this.cbForce.Name = "cbForce";
            this.cbForce.Size = new System.Drawing.Size(89, 17);
            this.cbForce.TabIndex = 20;
            this.cbForce.Text = "Force Status";
            this.cbForce.UseVisualStyleBackColor = true;
            // 
            // cbMask
            // 
            this.cbMask.AutoSize = true;
            this.cbMask.Checked = true;
            this.cbMask.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMask.Location = new System.Drawing.Point(249, 125);
            this.cbMask.Name = "cbMask";
            this.cbMask.Size = new System.Drawing.Size(105, 17);
            this.cbMask.TabIndex = 21;
            this.cbMask.Text = "Mask Password";
            this.cbMask.UseVisualStyleBackColor = true;
            // 
            // btnBrowse
            // 
            this.btnBrowse.AutoSize = true;
            this.btnBrowse.LinkColor = System.Drawing.Color.White;
            this.btnBrowse.Location = new System.Drawing.Point(360, 14);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(22, 13);
            this.btnBrowse.TabIndex = 22;
            this.btnBrowse.TabStop = true;
            this.btnBrowse.Text = "[...]";
            this.btnBrowse.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.BtnBrowse_LinkClicked);
            // 
            // btnSave
            // 
            this.btnSave.BackgroundImage = global::vactrak.Properties.Resources.save;
            this.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(382, 60);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(32, 32);
            this.btnSave.TabIndex = 23;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnClip
            // 
            this.btnClip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.btnClip.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btnClip.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClip.Location = new System.Drawing.Point(142, 96);
            this.btnClip.Name = "btnClip";
            this.btnClip.Size = new System.Drawing.Size(212, 23);
            this.btnClip.TabIndex = 24;
            this.btnClip.Text = "Clipboard Mode: Single Line";
            this.btnClip.UseVisualStyleBackColor = false;
            this.btnClip.Click += new System.EventHandler(this.BtnClip_Click);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.ClientSize = new System.Drawing.Size(429, 147);
            this.Controls.Add(this.btnClip);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.cbMask);
            this.Controls.Add(this.cbForce);
            this.Controls.Add(this.tbThread);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbCD);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbParam);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbPath);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.FormSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox tbPath;
        public System.Windows.Forms.TextBox tbParam;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox tbCD;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox tbThread;
        private System.Windows.Forms.CheckBox cbForce;
        private System.Windows.Forms.CheckBox cbMask;
        private System.Windows.Forms.LinkLabel btnBrowse;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClip;
    }
}