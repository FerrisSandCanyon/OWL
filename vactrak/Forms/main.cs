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
            CbProfile_LoadProfiles();

        }

        private void CbProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbProfile.Items.Count < 1) return;
            Globals.CurrentProfile = Utils.VTAccount.Load(Globals.Info.profilesPath + "/" + cbProfile.Items[cbProfile.SelectedIndex].ToString() + ".json");
            if (Globals.CurrentProfile == null) Application.Exit();
        }

        private void CbProfile_LoadProfiles()
        {
            cbProfile.Items.Clear();

#if DEBUG
            foreach(string _profile in Directory.GetFiles(Globals.Info.profilesPath, "*.json"))
            {
                string _profilename = Path.GetFileNameWithoutExtension(_profile);
                cbProfile.Items.Add(_profilename);
                Debug.Write(String.Format("\nProfile: {0}\nProfile Name:[{1}]", _profile, _profilename));
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
                CbProfile_LoadProfiles(); // just do recursion cause lazyyy
            }
        }
    }
}
