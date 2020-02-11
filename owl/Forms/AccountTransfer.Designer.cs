namespace owl.Forms
{
    partial class AccountTransfer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountTransfer));
            this.label1 = new System.Windows.Forms.Label();
            this.cbDest = new System.Windows.Forms.ComboBox();
            this.cbRemove = new System.Windows.Forms.CheckBox();
            this.btnTransfer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Destination:";
            // 
            // cbDest
            // 
            this.cbDest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDest.FormattingEnabled = true;
            this.cbDest.Location = new System.Drawing.Point(84, 12);
            this.cbDest.Name = "cbDest";
            this.cbDest.Size = new System.Drawing.Size(194, 21);
            this.cbDest.TabIndex = 1;
            // 
            // cbRemove
            // 
            this.cbRemove.AutoSize = true;
            this.cbRemove.Checked = true;
            this.cbRemove.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRemove.Location = new System.Drawing.Point(84, 39);
            this.cbRemove.Name = "cbRemove";
            this.cbRemove.Size = new System.Drawing.Size(136, 17);
            this.cbRemove.TabIndex = 2;
            this.cbRemove.Text = "Remove after transfer";
            this.cbRemove.UseVisualStyleBackColor = true;
            // 
            // btnTransfer
            // 
            this.btnTransfer.BackgroundImage = global::owl.Properties.Resources.transfer;
            this.btnTransfer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnTransfer.FlatAppearance.BorderSize = 0;
            this.btnTransfer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTransfer.Location = new System.Drawing.Point(284, 12);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(45, 40);
            this.btnTransfer.TabIndex = 3;
            this.btnTransfer.UseVisualStyleBackColor = true;
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // AccountTransfer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.ClientSize = new System.Drawing.Size(331, 66);
            this.Controls.Add(this.btnTransfer);
            this.Controls.Add(this.cbRemove);
            this.Controls.Add(this.cbDest);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(311, 105);
            this.Name = "AccountTransfer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Transfer Account";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbDest;
        private System.Windows.Forms.CheckBox cbRemove;
        private System.Windows.Forms.Button btnTransfer;
    }
}