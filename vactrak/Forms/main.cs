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

        }

        private void CbProfile_LoadProfiles()
        {
            cbProfile.Items.Clear();
            foreach(string _profile in Directory.GetFiles(Globals.Info.profilesPath, "*.json"))
                cbProfile.Items.Add(Path.GetFileNameWithoutExtension(_profile));

            if (cbProfile.Items.Count != 0) cbProfile.SelectedIndex = 0;
            // Create a default profile if there's no profile
            else
            {
                Globals.CurrentProfile = new Dictionary<string, Class.VTAccount> { };
                if (!Utils.VTAccount.Save(ref Globals.CurrentProfile, Globals.Info.profilesPath + "/default.json")) Application.Exit();
                CbProfile_LoadProfiles(); // just do recursion cause lazyyy
            }
        }
    }
}
