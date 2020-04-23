using System;
using System.IO;
using System.Windows.Forms;

namespace owl.Forms
{
    public partial class Settings : Form
    {

        private bool clipmode = false;
        private RadioButton[] LoginMethodRTBControls = new RadioButton[4] { null, null, null, null };

        private int LoginMethodControlRef
        {
            get
            {

                for (int x = 0; x < LoginMethodRTBControls.Length; x++)
                {
                    if (LoginMethodRTBControls[x].Checked)
                        return x;
                }

                return -1;
            }

            set
            {
                LoginMethodRTBControls[value].Checked = true;
            }
        }

        public Settings()
        {
            InitializeComponent();
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            LoginMethodRTBControls[0] = rtbLogMetNormal;
            LoginMethodRTBControls[1] = rtbLogMetForce;
            LoginMethodRTBControls[2] = rtbLogMetNormalNoCheck;
            LoginMethodRTBControls[3] = rtbLogMetClickCreds;

            tbPath.Text           = Globals.Config.steamPath ?? "";
            tbParam.Text          = Globals.Config.steamParam ?? "";
            tbCD.Text             = Globals.Config.cooldownRefresh.ToString() ?? "800";
            tbThread.Text         = Globals.Config.maxThreads.ToString() ?? "4";
            cbForce.Checked       = Globals.Config.forceStatus;
            cbMask.Checked        = Globals.Config.maskPassword;
            clipmode              = Globals.Config.clipboardDetail;
            cbUpdate.Checked      = Globals.Config.startupUpdateChk;
            LoginMethodControlRef = (int)Globals.Config.loginMethod;

            BtnClip_SetTxt();
        }

        private void BtnBrowse_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (FolderBrowserDialog _fbd = new FolderBrowserDialog())
            {
                while (!File.Exists(_fbd.SelectedPath + "\\Steam.exe"))
                    if (_fbd.ShowDialog() == DialogResult.Cancel) return;

                tbPath.Text = Globals.Config.steamPath = _fbd.SelectedPath.Replace('\\', '/');
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            int newcd, newmax;
            if (String.IsNullOrWhiteSpace(tbPath.Text) || String.IsNullOrWhiteSpace(tbCD.Text) || String.IsNullOrWhiteSpace(tbThread.Text) || !int.TryParse(tbCD.Text, out newcd) || !int.TryParse(tbThread.Text, out newmax))
            {
                MessageBox.Show("Please properly provide every required field.", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Globals.Config.steamPath        = tbPath.Text;
            Globals.Config.steamParam       = tbParam.Text;
            Globals.Config.cooldownRefresh  = newcd;
            Globals.Config.maxThreads       = newmax;
            Globals.Config.forceStatus      = cbForce.Checked;
            Globals.Config.maskPassword     = cbMask.Checked;
            Globals.Config.clipboardDetail  = clipmode;
            Globals.Config.startupUpdateChk = cbUpdate.Checked;
            Globals.Config.loginMethod      = (LOGINMETHOD)LoginMethodControlRef;

            if (!Utils.Config.Save(ref Globals.Config, Globals.Info.cfgPath))
            {
                MessageBox.Show("Failed to save settings!", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Settings saved!", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void BtnClip_Click(object sender, EventArgs e)
        {
            clipmode = !clipmode;
            BtnClip_SetTxt();
        }

        private void BtnClip_SetTxt()
        {
            btnClip.Text = "Clipboard Mode: " + (clipmode ? "Detailed" : "Single Line");
        }
    }
}
