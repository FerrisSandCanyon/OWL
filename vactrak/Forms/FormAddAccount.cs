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
    public partial class FormAddAccount : Form
    {
        // References to our data list view for adding new accounts
        ListView        lvData = null;

        // References to our data list view items and account for editing accounts
        ListViewItem    lvItem = null;
        Class.VTAccount vta    = null;

        // Mode / usage of this form instance
        // false = Add account
        // true  = Edit account
        bool         mode   = false;

        public FormAddAccount(ref ListView _lvData)
        {
            lvData = _lvData;
            mode   = false;
            Globals.Cache.AddAnotherFlag = false;
            InitializeComponent();
        }

        public FormAddAccount(ListViewItem _lvItem, ref Class.VTAccount _vta)
        {
            lvItem = _lvItem;
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

        private void BtnApply_Click(object sender, EventArgs e)
        {
            // Edit account
            if (mode)
            {
                lvItem.SubItems[1].Text = vta.SteamURL = tbURL.Text;
                lvItem.SubItems[3].Text = vta.Username = tbUser.Text;
                                          vta.Password = tbPass.Text;
                lvItem.SubItems[6].Text = vta.Note     = tbNote.Text;
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
