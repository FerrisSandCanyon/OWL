using System;
using System.Windows.Forms;

namespace owl.Forms
{
    public partial class AccountInfo : Form
    {
        // References to our data list view for adding new accounts
        ListView        lvData = null;

        // References to our account for editing accounts
        Class.Account account    = null;

        // Mode / usage of this form instance
        // false = Add account
        // true  = Edit account
        bool         mode   = false;

        public static string cache_note           = "";    // Cached note text to be automatically loaded for the next usage
        public static bool   cache_onTop          = false, // Add account form is always on top of every other window
                             cache_addAnother     = false, // Automatically re-open the add account menu (Ignored on edit mode)
                             cache_addAnotherFlag = false; // Set to true when the user selected "Add" account, otherwise false. This is to prevent re-opening the UI if the user closed it

        public AccountInfo(ref ListView _lvData)
        {
            lvData = _lvData;
            mode   = false;
            cache_addAnotherFlag = false;
            InitializeComponent();
        }

        public AccountInfo(ref Class.Account _account)
        {
            account = _account;
            mode    = true;
            InitializeComponent();
        }

        private void FormAddAccount_Load(object sender, EventArgs e)
        {
            this.btnApply.BackgroundImage = mode ? global::owl.Properties.Resources.edit : global::owl.Properties.Resources.plus;
            this.Text                     = mode ? "Edit Account" : "Add Account";
            this.TopMost                  = cbTop.Checked = cache_onTop;
            cbAdd.Enabled                 = !mode;
            cbAdd.Checked                 = cache_addAnother;
            tbNote.Text                   = mode ? account.Note : cache_note;

            if (Globals.Config.maskPassword)
                tbPass.PasswordChar = '●';

            if (mode)
            {
                tbURL.Text  = account.SteamURL;
                tbUser.Text = account.Username;
                tbPass.Text = account.Password;
            }
        }

        private void FormAddAccount_Closing(object sender, EventArgs e)
        {
            cache_note = tbNote.Text;
        }

        private void CbTop_CheckedChanged(object sender, EventArgs e)
        {
            cache_onTop = this.TopMost = cbTop.Checked;
        }

        private void CbAdd_CheckedChanged(object sender, EventArgs e)
        {
            cache_addAnother = cbAdd.Checked;
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

            if (Sanitize("https", false) || Sanitize("http", false))
                Sanitize("://steamcommunity.com/", false);

            Sanitize("/", true);
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            tbURL_Sanitize(); // Sanitize the url

            // Validate user input
            if (!string.IsNullOrWhiteSpace(tbURL.Text)
            || (!string.IsNullOrWhiteSpace(tbUser.Text) && !string.IsNullOrWhiteSpace(tbPass.Text)) )
            {
                // Validates the steam URL
                if (!string.IsNullOrWhiteSpace(tbURL.Text) && !tbURL.Text.StartsWith("id/") && !tbURL.Text.StartsWith("profiles/"))
                {
                    MessageBox.Show("Invalid Steam URL!", "Account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Edit account
                if (mode)
                {
                    account.LVI.SubItems[1].Text = account.SteamURL = tbURL.Text;
                    account.LVI.SubItems[3].Text = account.Username = tbUser.Text;
                                                   account.Password = tbPass.Text;
                    account.LVI.SubItems[6].Text = account.Note     = tbNote.Text;

                    if (Globals.hFormMain.SelectedAccount == account)
                        Globals.hFormMain.tbNote.Text = account.Note; // This might trigger the event rendering the upper assignment useless but, eh.
                }

                // Add account
                else
                {
                    string uniqueId = Utils.Account.MakeUniqueKey();

                    // Create the new account
                    Class.Account _account = new Class.Account(DateTime.MinValue, uniqueId, tbURL.Text, "", tbUser.Text, tbPass.Text, tbNote.Text, false);

                    // Add it
                    Globals.CurrentProfile.Profiles.Add(uniqueId, _account);
                    Utils.Account.AddToTable(ref lvData, uniqueId, ref _account);

                    // Finish
                    cache_addAnotherFlag = true;
                }

                this.Close();
            }
            else
            {
                MessageBox.Show("You must atleast provide a steam url (that begins with either \"id/\" or \"profile/\") or a username and password to make a valid entry!", "Add account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        // Auto Paste
        private void Event_AutoPaste(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsText())
                return;
            ((System.Windows.Forms.TextBox)sender).Text = Clipboard.GetText() ?? "";
        }
    }
}
