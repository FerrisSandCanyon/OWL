using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vactrak.Forms
{
    public partial class FormAccount : Form
    {
        // References to our data list view for adding new accounts
        ListView        lvData = null;

        // References to our account for editing accounts
        Class.VTAccount vta    = null;

        // Mode / usage of this form instance
        // false = Add account
        // true  = Edit account
        bool         mode   = false;

        public FormAccount(ref ListView _lvData)
        {
            lvData = _lvData;
            mode   = false;
            Globals.Cache.AddAnotherFlag = false;
            InitializeComponent();
        }

        public FormAccount(ref Class.VTAccount _vta)
        {
            vta    = _vta;
            mode   = true;
            InitializeComponent();
        }

        private void FormAddAccount_Load(object sender, EventArgs e)
        {
            this.btnApply.BackgroundImage = mode ? global::vactrak.Properties.Resources.edit : global::vactrak.Properties.Resources.plus;
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
            // lambda gang
            Func<string, bool> Sanitize = (string s) =>
            {
                if (tbURL.Text.StartsWith(s))
                {
                    tbURL.Text = tbURL.Text.Remove(0, s.Length);
                    return true;
                }

                return false;
            };

            // pEfficiency
            if (Sanitize("https://steamcommunity.com/") ||
                Sanitize("http://steamcommunity.com/" ) ||
                Sanitize("/"                          )    ) return;
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            tbURL_Sanitize(); // Sanitize the url

            // Validate user input
            if ((String.IsNullOrWhiteSpace(tbURL.Text) || !(tbURL.Text.StartsWith("id/") || tbURL.Text.StartsWith("profile/"))) && (String.IsNullOrWhiteSpace(tbUser.Text) || String.IsNullOrWhiteSpace(tbPass.Text)))
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
            }
            // Add account
            else
            {
                // Generate a unique id for the json
                Random _rnd = new Random();
                string uniqueId = "";
                do
                {
                    uniqueId = "";
                    for (int x = 1; x < 11; x++) uniqueId += Globals.Charset[_rnd.Next(0, Globals.Charset.Length - 1)];
                } while (Globals.CurrentProfile.ContainsKey(uniqueId));

                // Create the new account
                Class.VTAccount _vta = new Class.VTAccount(tbURL.Text, "", tbUser.Text, tbPass.Text, tbNote.Text, false, 0);

                // Add it
                Globals.CurrentProfile.Add(uniqueId, _vta);
                Utils.VTAccount.AddToTable(ref lvData, uniqueId, ref _vta);

                // Finish
                Globals.Cache.AddAnotherFlag = true;
            }

            this.Close();
        }
    }
}
