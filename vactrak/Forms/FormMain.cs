using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace vactrak
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.Focus();

            // ==============
            // Initialization
            // ==============

            #if DEBUG
                this.Text += " (DEBUG MODE)";
            #endif

            Globals.titleFallback = this.Text;
            CbProfile_LoadProfileDirectory();

        }

        private void FormMain_SetTitle(string _title = null)
        {
            if (_title == null) this.Text = Globals.titleFallback;
            this.Text = Globals.titleFallback + " - " + _title;
        }

        // Loads the selected profile.
        private void CbProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbProfile.Items.Count < 1) return;
            string _profilePath    = Globals.Info.profilesPath + "/" + cbProfile.Items[cbProfile.SelectedIndex].ToString() + ".json";

            // Make sure it exists
            if (!File.Exists(_profilePath))
            {
                MessageBox.Show("This profile does not exist", "Load profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CbProfile_LoadProfileDirectory();
                return;
            }

            Globals.CurrentProfile = Utils.VTAccount.Load(_profilePath);
            if (Globals.CurrentProfile == null) Application.Exit();
        }

        // Searches the profile directory for profile jsons
        private void CbProfile_LoadProfileDirectory()
        {
            cbProfile.Items.Clear();

            #if DEBUG
                foreach(string _profile in Directory.GetFiles(Globals.Info.profilesPath, "*.json"))
                {
                    string _profilename = Path.GetFileNameWithoutExtension(_profile);
                    cbProfile.Items.Add(_profilename);
                    Debug.WriteLine(String.Format("Profile: {0}\nProfile Name:[{1}]", _profile, _profilename));
                } 
            #else
                foreach(string _profile in Directory.GetFiles(Globals.Info.profilesPath, "*.json"))
                    cbProfile.Items.Add(Path.GetFileNameWithoutExtension(_profile));
            #endif

            if (cbProfile.Items.Count != 0)
            {
                int defaultIndex = cbProfile.FindStringExact(Globals.Config.defaultProfile);
                if (defaultIndex == -1) defaultIndex = 0;
                cbProfile.SelectedIndex = defaultIndex;
            }
            // Create a default profile if there's no profile
            else
            {
                Globals.CurrentProfile = new Dictionary<string, Class.VTAccount> { };
                if (!Utils.VTAccount.Save(ref Globals.CurrentProfile, Globals.Info.profilesPath + "/" + Globals.Config.defaultProfile + ".json")) Application.Exit();
                CbProfile_LoadProfileDirectory(); // just do recursion cause lazyyy
            }
        }

        // Create a new profile
        private void AddProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string profileName = Microsoft.VisualBasic.Interaction.InputBox("Enter the profile's name (leave blank to cancel)", "Create new profile", null); // yes, lazy. very

            // Sanitize the profile name
            foreach (char _invalid in Path.GetInvalidFileNameChars())
                profileName.Replace(_invalid, '_');

            if (String.IsNullOrWhiteSpace(profileName)) return;                           // If canceled by the user
            string profilePath = Globals.Info.profilesPath + "/" + profileName + ".json"; // Just for convenience, concat the string altogether.

            // Check if it already exists
            if (File.Exists(profilePath))
            {
                MessageBox.Show("This profile already exists!", "Create profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Try to create the profile json
            try
            {
                File.CreateText(profilePath).Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to create profile.\n\nException: " + ex, "Create profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CbProfile_LoadProfileDirectory();

            // Create the actual profile
            Globals.CurrentProfile = new Dictionary<string, Class.VTAccount> { };
            if (!Utils.VTAccount.Save(ref Globals.CurrentProfile, profilePath)) Application.Exit();

            // Load and set the newly created profile to our current one
            int profileIndex = cbProfile.FindStringExact(profileName);
            if (profileIndex == -1) profileIndex = 0;
            cbProfile.SelectedIndex = profileIndex;
        }

        // Removes a profile
        private void RemoveProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove this profile?", "Remove profile", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No) return;

            if (cbProfile.Items.Count < 1) return;
            string _profilePath = Globals.Info.profilesPath + "/" + cbProfile.Items[cbProfile.SelectedIndex].ToString() + ".json";

            // Make sure it exists
            if (!File.Exists(_profilePath))
            {
                MessageBox.Show("This profile does not exist", "Delete profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CbProfile_LoadProfileDirectory();
                return;
            }

            try
            {
                File.Delete(_profilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Failed to delete profile.\n\nFile: {0}\nException: {1}", _profilePath, ex), "Delete profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Reload the profile drop down
            CbProfile_LoadProfileDirectory();
        }

        // Set as default profile
        private void SetAsDefaultProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals.Config.defaultProfile = cbProfile.Items[cbProfile.SelectedIndex].ToString();
            if (!Utils.VTAccount.Save(ref Globals.CurrentProfile, Globals.Info.profilesPath + "/" + Globals.Config.defaultProfile + ".json")) Application.Exit();
            MessageBox.Show("Current profile has been set as the default profile!", "Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(

                "VACTrak# version " + Globals.Info.verStr + "\n" +
                "A multipurpose tool for managing Steam accounts.\n\n" +
                "Developed by: FerrisSandCanyon\n\n" +
                "https://github.com/FerrisSandCanyon/vactrak"

               ,"VACTrak#", MessageBoxButtons.OK, MessageBoxIcon.Information
            );
        }

        private void DdAccountAdd_Click(object sender, EventArgs e)
        {
            do
            {
                using (Forms.FormAddAccount _fad = new Forms.FormAddAccount(ref lvData))
                {
                    _fad.ShowDialog();
                    _fad.Dispose();
                }
            } while (Globals.Cache.AddAnother && Globals.Cache.AddAnotherFlag);
        }

        private void DdAccountEdit_Click(object sender, EventArgs e)
        {
            if (lvData.SelectedItems.Count == 0)
            {
                MessageBox.Show("No account selected to edit.", "Edit Account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (ListViewItem _lvi in lvData.SelectedItems)
                using (Forms.FormAddAccount _fad = new Forms.FormAddAccount(_lvi))
                {
                    _fad.ShowDialog();
                    _fad.Dispose();
                }
        }

        // Save profile
        private void BtnProfileSave_Click(object sender, EventArgs e)
        {
            if (cbProfile.Items.Count < 1) return;
            string _profilePath = Globals.Info.profilesPath + "/" + cbProfile.Items[cbProfile.SelectedIndex].ToString() + ".json";
            if (!Utils.VTAccount.Save(ref Globals.CurrentProfile, _profilePath)) Application.Exit();
            MessageBox.Show("Profile has been saved!", "Save profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
