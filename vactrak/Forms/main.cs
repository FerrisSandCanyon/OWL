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

        // Parses the directory for profile jsons
        private void GetProfilesJson()
        {

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
        }
    }
}
