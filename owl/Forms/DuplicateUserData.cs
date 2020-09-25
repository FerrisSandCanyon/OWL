using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace owl.Forms
{
    public partial class DuplicateUserData : Form
    {
        /// <summary>
        /// The source of the data
        /// </summary>
        Class.Account ParentAccount = null;

        /// <summary>
        /// Directory path to the parent account user data
        /// </summary>
        string ParentAccountDir = null;

        /// <summary>
        /// List of Destination accounts
        /// </summary>
        List<Class.Account> ChildAccount = null;

        public DuplicateUserData(Class.Account parentaccount, List<Class.Account> childaccount)
        {
            InitializeComponent();
            ParentAccount = parentaccount;
            ChildAccount = childaccount;
            ParentAccountDir = Path.Combine(Globals.Config.steamPath, $"userdata\\{parentaccount.ResolveSteamID3()}");
        }

        private void DuplicateUserData_Load(object sender, EventArgs e)
        {
            this.Text += ParentAccount.Username;

            foreach (string folder in Directory.EnumerateDirectories(ParentAccountDir))
                lvFolders.Items.Add(new ListViewItem(Path.GetFileName(folder)));

            foreach (Class.Account account in ChildAccount)
                lbDestination.Items.Add(account.Username);

        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lvFolders.Items)
            {
                lvi.Checked = true;
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvi in lvFolders.Items)
            {
                if (!lvi.Checked)
                    continue;

                string parent_path = Path.Combine(ParentAccountDir, lvi.SubItems[0].Text);

                foreach (Class.Account account in ChildAccount)
                {
                    string id3 = account.ResolveSteamID3();

                    if (id3 == null)
                        continue;

                    string account_path = Path.Combine(Globals.Config.steamPath, $"userdata\\{id3}\\{lvi.SubItems[0].Text}");

                    foreach (string dirpath in Directory.GetDirectories(parent_path, "*", SearchOption.AllDirectories))
                        Directory.CreateDirectory(dirpath.Replace(parent_path, account_path));

                    foreach (string filepath in Directory.GetFiles(parent_path, "*.*", SearchOption.AllDirectories))
                        File.Copy(filepath, filepath.Replace(parent_path, account_path), true);
                }
            }

            MessageBox.Show("User data has been copied!", "Duplicate user data", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
