using System;
using System.Windows.Forms;

namespace owl.Forms
{
    public partial class Account : Form
    {
        // References to our data list view for adding new accounts
        ListView        lvData = null;

        // References to our account for editing accounts
        Class.Account vta    = null;

        // Mode / usage of this form instance
        // false = Add account
        // true  = Edit account
        bool         mode   = false;

        public Account(ref ListView _lvData)
        {
            lvData = _lvData;
            mode   = false;
            Globals.Cache.AddAnotherFlag = false;
            InitializeComponent();
        }

        public Account(ref Class.Account _vta)
        {
            vta    = _vta;
            mode   = true;
            InitializeComponent();
        }

        private void FormAddAccount_Load(object sender, EventArgs e)
        {
            this.btnApply.BackgroundImage = mode ? global::owl.Properties.Resources.edit : global::owl.Properties.Resources.plus;
            this.Text                     = mode ? "Edit Account" : "Add Account";
            this.TopMost                  = cbTop.Checked = Globals.Cache.OnTop;
            cbAdd.Enabled                 = !mode;
            cbAdd.Checked                 = Globals.Cache.AddAnother;
            tbNote.Text                   = mode ? vta.Note : Globals.Cache.Notes;

            if (Globals.Config.maskPassword) tbPass.PasswordChar = '●';

            if (mode)
            {
                tbURL.Text  = vta.SteamURL;
                tbUser.Text = vta.Username;
                tbPass.Text = vta.Password;
            }
        }

        private void FormAddAccount_Closing(object sender, EventArgs e)
        {
            Globals.Cache.Notes = tbNote.Text;
        }

        private void CbTop_CheckedChanged(object sender, EventArgs e)
        {
            Globals.Cache.OnTop = this.TopMost = cbTop.Checked;
        }

        private void CbAdd_CheckedChanged(object sender, EventArgs e)
        {
            Globals.Cache.AddAnother = cbAdd.Checked;
        }

        private void tbURL_Sanitize()
        {
            Func<string, bool, bool> Sanitize = (string s, bool end) =>
            {
                if (tbURL.Text.StartsWith(s))
                {
                    tbURL.Text = tbURL.Text.Remove(0, s.Length);
                    if (!end) return true;
                }

                if (end && tbURL.Text.EndsWith(s))
                {
                    tbURL.Text = tbURL.Text.Remove(tbURL.Text.Length - s.Length, s.Length);
                    return true;
                }

                return false;
            };

            if (Sanitize("https", false) || Sanitize("http", false)) Sanitize("://steamcommunity.com/", false);

            Sanitize("/", true);
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            tbURL_Sanitize(); // Sanitize the url

            // Validate user input
            if ((String.IsNullOrWhiteSpace(tbURL.Text) || !(tbURL.Text.StartsWith("id/") || tbURL.Text.StartsWith("profiles/"))) && (String.IsNullOrWhiteSpace(tbUser.Text) || String.IsNullOrWhiteSpace(tbPass.Text)))
            {
                MessageBox.Show("You must atleast provide a steam url (that begins with either \"id/\" or \"profile/\") or a username and password to make a valid entry!", "Add account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Edit account
            if (mode)
            {
                vta.LVI.SubItems[1].Text = vta.SteamURL = tbURL.Text;
                vta.LVI.SubItems[3].Text = vta.Username = tbUser.Text;
                                           vta.Password = tbPass.Text;
                vta.LVI.SubItems[6].Text = vta.Note     = tbNote.Text;

                if (Globals.hFormMain.VTASelectedAccount == vta)
                    Globals.hFormMain.tbNote.Text = vta.Note; // This might trigger the event rendering the upper assignment useless but, eh.
            }

            // Add account
            else
            {
                string uniqueId = Utils.Account.MakeUniqueKey();

                // Create the new account
                Class.Account _vta = new Class.Account(DateTime.MinValue, tbURL.Text, "", tbUser.Text, tbPass.Text, tbNote.Text, false);

                // Add it
                Globals.CurrentProfile.Add(uniqueId, _vta);
                Utils.Account.AddToTable(ref lvData, uniqueId, ref _vta);

                // Finish
                Globals.Cache.AddAnotherFlag = true;
            }

            this.Close();
        }

        // Auto Paste
        private void Event_AutoPaste(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsText()) return;
            ((System.Windows.Forms.TextBox)sender).Text = Clipboard.GetText() ?? "";
        }
    }
}
