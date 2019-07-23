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
        // References to our data list view
        ListView     lvData = null;
        ListViewItem lvItem = null;

        // Mode / usage of this form instance
        // false = Add account
        // true = Edit account
        bool         mode   = false;

        public FormAddAccount(ref ListView _lvData)
        {
            lvData = _lvData;
            mode   = false;
            Globals.Cache.AddAnotherFlag = false;
            InitializeComponent();
        }

        public FormAddAccount(ListViewItem _lvItem)
        {
            lvItem = _lvItem;
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
            Globals.Cache.AddAnotherFlag = true;
            this.Close();
        }
    }
}
