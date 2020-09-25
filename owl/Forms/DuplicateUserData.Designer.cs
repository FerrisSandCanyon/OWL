namespace owl.Forms
{
    partial class DuplicateUserData
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DuplicateUserData));
            this.lvFolders = new System.Windows.Forms.ListView();
            this.chFolder = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnAll = new System.Windows.Forms.Button();
            this.lbDestination = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lvFolders
            // 
            this.lvFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFolders.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.lvFolders.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvFolders.CheckBoxes = true;
            this.lvFolders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFolder});
            this.lvFolders.ForeColor = System.Drawing.Color.White;
            this.lvFolders.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvFolders.HideSelection = false;
            this.lvFolders.Location = new System.Drawing.Point(-1, -1);
            this.lvFolders.Name = "lvFolders";
            this.lvFolders.Size = new System.Drawing.Size(410, 425);
            this.lvFolders.TabIndex = 0;
            this.lvFolders.UseCompatibleStateImageBehavior = false;
            this.lvFolders.View = System.Windows.Forms.View.Details;
            // 
            // chFolder
            // 
            this.chFolder.Text = "Folder";
            this.chFolder.Width = 365;
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopy.Location = new System.Drawing.Point(514, 431);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 23);
            this.btnCopy.TabIndex = 1;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnAll
            // 
            this.btnAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAll.Location = new System.Drawing.Point(433, 431);
            this.btnAll.Name = "btnAll";
            this.btnAll.Size = new System.Drawing.Size(75, 23);
            this.btnAll.TabIndex = 2;
            this.btnAll.Text = "Select All";
            this.btnAll.UseVisualStyleBackColor = true;
            this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // lbDestination
            // 
            this.lbDestination.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbDestination.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.lbDestination.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbDestination.ForeColor = System.Drawing.Color.White;
            this.lbDestination.FormattingEnabled = true;
            this.lbDestination.Location = new System.Drawing.Point(415, 32);
            this.lbDestination.Name = "lbDestination";
            this.lbDestination.Size = new System.Drawing.Size(174, 392);
            this.lbDestination.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(415, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Destination:";
            // 
            // DuplicateUserData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(601, 462);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbDestination);
            this.Controls.Add(this.btnAll);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.lvFolders);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DuplicateUserData";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Duplicating user data from ";
            this.Load += new System.EventHandler(this.DuplicateUserData_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvFolders;
        private System.Windows.Forms.ColumnHeader chFolder;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnAll;
        private System.Windows.Forms.ListBox lbDestination;
        private System.Windows.Forms.Label label1;
    }
}