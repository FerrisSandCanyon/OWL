namespace vactrak.Forms
{
    partial class FormAddAccount
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAddAccount));
            this.tbURL = new System.Windows.Forms.TextBox();
            this.lblURL = new System.Windows.Forms.Label();
            this.tbUser = new System.Windows.Forms.TextBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.tbPass = new System.Windows.Forms.TextBox();
            this.lblPass = new System.Windows.Forms.Label();
            this.tbNote = new System.Windows.Forms.TextBox();
            this.lblNotes = new System.Windows.Forms.Label();
            this.cbTop = new System.Windows.Forms.CheckBox();
            this.cbAdd = new System.Windows.Forms.CheckBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbURL
            // 
            this.tbURL.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.tbURL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbURL.ForeColor = System.Drawing.Color.White;
            this.tbURL.Location = new System.Drawing.Point(79, 12);
            this.tbURL.Name = "tbURL";
            this.tbURL.Size = new System.Drawing.Size(209, 22);
            this.tbURL.TabIndex = 13;
            // 
            // lblURL
            // 
            this.lblURL.AutoSize = true;
            this.lblURL.Location = new System.Drawing.Point(9, 15);
            this.lblURL.Name = "lblURL";
            this.lblURL.Size = new System.Drawing.Size(64, 13);
            this.lblURL.TabIndex = 12;
            this.lblURL.Text = "Steam URL:";
            // 
            // tbUser
            // 
            this.tbUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.tbUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbUser.ForeColor = System.Drawing.Color.White;
            this.tbUser.Location = new System.Drawing.Point(79, 40);
            this.tbUser.Name = "tbUser";
            this.tbUser.Size = new System.Drawing.Size(209, 22);
            this.tbUser.TabIndex = 15;
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(12, 42);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(61, 13);
            this.lblUser.TabIndex = 14;
            this.lblUser.Text = "Username:";
            // 
            // tbPass
            // 
            this.tbPass.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.tbPass.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbPass.ForeColor = System.Drawing.Color.White;
            this.tbPass.Location = new System.Drawing.Point(79, 68);
            this.tbPass.Name = "tbPass";
            this.tbPass.Size = new System.Drawing.Size(209, 22);
            this.tbPass.TabIndex = 17;
            // 
            // lblPass
            // 
            this.lblPass.AutoSize = true;
            this.lblPass.Location = new System.Drawing.Point(14, 70);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(59, 13);
            this.lblPass.TabIndex = 16;
            this.lblPass.Text = "Password:";
            // 
            // tbNote
            // 
            this.tbNote.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.tbNote.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbNote.ForeColor = System.Drawing.Color.White;
            this.tbNote.Location = new System.Drawing.Point(79, 96);
            this.tbNote.Name = "tbNote";
            this.tbNote.Size = new System.Drawing.Size(209, 22);
            this.tbNote.TabIndex = 19;
            // 
            // lblNotes
            // 
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(33, 98);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(40, 13);
            this.lblNotes.TabIndex = 18;
            this.lblNotes.Text = "Notes:";
            // 
            // cbTop
            // 
            this.cbTop.AutoSize = true;
            this.cbTop.Location = new System.Drawing.Point(79, 124);
            this.cbTop.Name = "cbTop";
            this.cbTop.Size = new System.Drawing.Size(99, 17);
            this.cbTop.TabIndex = 20;
            this.cbTop.Text = "Always on Top";
            this.cbTop.UseVisualStyleBackColor = true;
            this.cbTop.CheckedChanged += new System.EventHandler(this.CbTop_CheckedChanged);
            // 
            // cbAdd
            // 
            this.cbAdd.AutoSize = true;
            this.cbAdd.Location = new System.Drawing.Point(184, 124);
            this.cbAdd.Name = "cbAdd";
            this.cbAdd.Size = new System.Drawing.Size(91, 17);
            this.cbAdd.TabIndex = 21;
            this.cbAdd.Text = "Add another";
            this.cbAdd.UseVisualStyleBackColor = true;
            this.cbAdd.CheckedChanged += new System.EventHandler(this.CbAdd_CheckedChanged);
            // 
            // btnApply
            // 
            this.btnApply.BackgroundImage = global::vactrak.Properties.Resources.plus;
            this.btnApply.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnApply.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnApply.FlatAppearance.BorderSize = 0;
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Location = new System.Drawing.Point(302, 51);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(32, 32);
            this.btnApply.TabIndex = 22;
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.BtnApply_Click);
            // 
            // FormAddAccount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.ClientSize = new System.Drawing.Size(350, 155);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.cbAdd);
            this.Controls.Add(this.cbTop);
            this.Controls.Add(this.tbNote);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.tbPass);
            this.Controls.Add(this.lblPass);
            this.Controls.Add(this.tbUser);
            this.Controls.Add(this.lblUser);
            this.Controls.Add(this.tbURL);
            this.Controls.Add(this.lblURL);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormAddAccount";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Account/Edit Account";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormAddAccount_Closing);
            this.Load += new System.EventHandler(this.FormAddAccount_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbURL;
        private System.Windows.Forms.Label lblURL;
        private System.Windows.Forms.TextBox tbUser;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.TextBox tbPass;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.TextBox tbNote;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.CheckBox cbTop;
        private System.Windows.Forms.CheckBox cbAdd;
        private System.Windows.Forms.Button btnApply;
    }
}