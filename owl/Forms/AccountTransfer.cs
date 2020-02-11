using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace owl.Forms
{
    public partial class AccountTransfer : Form
    {

        public AccountTransfer(ref string[] _profiles)
        {
            InitializeComponent();
            cbDest.Items.AddRange(_profiles);
            cbDest.SelectedIndex = 0;
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            // TODO: use a cached transfered account for cases of failure
            // TODO: decide if source profile should be auto saved or not

            string            profilePath  = Path.Combine(Globals.Info.profilesPath, cbDest.Items[cbDest.SelectedIndex].ToString()) + ".json";
            Class.ProfileInfo _destProfile = Utils.Account.Load(profilePath);

            if (_destProfile == null)
            {
                MessageBox.Show("Failed to load destination profile!", "Transfer account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (KeyValuePair<string, Class.Account> _account in Utils.ProfileInfo.GetSelectedItems())
            {
                _destProfile.Profiles[_account.Key] = _account.Value;

                if (cbRemove.Checked)
                {
                    // Remove the account first before removing the lvi since other threads might access a dead lvi from a disposing account instance
                    ref ListViewItem _lvi_tmp = ref _account.Value.LVI;
                    Globals.CurrentProfile.Profiles.Remove(_account.Key);
                    _lvi_tmp.Remove();
                }
            }

            // Save destination profile
            if (!Utils.Account.Save(ref _destProfile, profilePath))
            {
                MessageBox.Show("Failed to save destination profile!", "Transfer account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            this.Close();
        }
    }
}
