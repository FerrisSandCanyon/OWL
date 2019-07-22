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
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.Focus();

            // ==============
            // Initialization
            // ==============

            #if DEBUG
                this.Text += " - DEBUG MODE";
            #endif

            Globals.titleFallback = this.Text;
            CbProfile_LoadProfileDirectory();

        }

        // Loads the selected profile. Note: add check if that json still exists before attempting to load
        private void CbProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbProfile.Items.Count < 1) return;
            Globals.CurrentProfile = Utils.VTAccount.Load(Globals.Info.profilesPath + "/" + cbProfile.Items[cbProfile.SelectedIndex].ToString() + ".json");
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

                if (defaultIndex == -1)
                    cbProfile.SelectedIndex = 0;
                else 
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
            if (profileIndex == -1)
                cbProfile.SelectedIndex = 0;
            else
                cbProfile.SelectedIndex = profileIndex;
        }

    }
}
