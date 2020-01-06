using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace owl
{
    public partial class FormMain : Form
    {

        public Class.Account VTASelectedAccount = null; // Reference to the first selected account in lvData

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

            Globals.Cache.TitleFallback = this.Text;
            Globals.hMainThread = Thread.CurrentThread;
            CbProfile_LoadProfileDirectory();

            lblBuildInfo.Text = "v" + Globals.Info.verStr;

            // Cooldown tracker thread
            new Thread(new ThreadStart(() =>
            {
                bool IsAlive()
                {
                    return Globals.hMainThread != null && Globals.hMainThread.IsAlive;
                }

                while (IsAlive())
                {
                    Thread.Sleep(Globals.Config.cooldownRefresh);

                    try
                    {
                        if (Globals.CurrentProfile == null || Globals.CurrentProfile.Count == 0)
                            continue;

                        foreach (KeyValuePair<string, Class.Account> _account in Globals.CurrentProfile)
                        {
                            if (!IsAlive()) break;
                            _account.Value.DisplayCooldown();
                        }
                    }
                    catch { }
                }
            }
            )).Start();
        }

        public void FormMain_SetTitle(string _status = null)
        {
            this.Text = _status == null ? Globals.Cache.TitleFallback : Globals.Cache.TitleFallback + " - " + _status;
        }

        private void LvData_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvData.SelectedItems.Count == 0)
            {
                tbNote.Text = "";
                VTASelectedAccount = null;
            }
            else
            {
                if (!Globals.CurrentProfile.TryGetValue(lvData.SelectedItems[0].SubItems[0].Text, out VTASelectedAccount) || lvData.SelectedItems.Count > 1)
                    return;

                tbNote.Text = VTASelectedAccount.Note;
            }
        }

        private void TbNote_TextChanged(object sender, EventArgs e)
        {
            if (lvData.SelectedItems.Count == 0 || VTASelectedAccount == null || VTASelectedAccount.LVI == null)
                return;

            VTASelectedAccount.LVI.SubItems[6].Text = VTASelectedAccount.Note = tbNote.Text;

            if (lvData.SelectedItems.Count > 1)
            {
                Class.Account _currAccount = null;
                for (int x = 1; x < lvData.SelectedItems.Count; x++)
                {
                    if (!Globals.CurrentProfile.TryGetValue(lvData.SelectedItems[x].SubItems[0].Text, out _currAccount))
                        continue;

                    _currAccount.LVI.SubItems[6].Text = _currAccount.Note = tbNote.Text;
                }
            }
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            using (Forms.Settings _fs = new Forms.Settings())
            {
                _fs.ShowDialog();
                _fs.Dispose();
            }
        }

        private void DdAccountImportSAC_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog _ofd = new OpenFileDialog())
            {
                _ofd.FileName = "account.txt";
                _ofd.Filter = "Text Files (*.txt) | *.txt";
                _ofd.ShowDialog();

                if (File.Exists(_ofd.FileName))
                    new Forms.Import(_ofd.FileName, 1, ref lvData).ShowDialog();

                _ofd.Dispose();
            }
        }

        private void ddManageCooldown_Click(object sender, EventArgs e)
        {
            // By default, the parent drop down will add 21 hours by sending the button for adding 21 hours cooldown to the event listener
            Event_AddCooldown(ddManageCooldown21hours, null);
            ddManage.HideDropDown();
        }

        #region Profile

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

            // Load the profile
            Globals.CurrentProfile = Utils.Account.Load(_profilePath);
            if (Globals.CurrentProfile == null) Application.Exit();

            lvData.Items.Clear();

            // Load all the accounts to our table
            foreach (KeyValuePair<string, Class.Account> _data in Globals.CurrentProfile)
            {
                Class.Account _vta = _data.Value;
                Utils.Account.AddToTable(ref lvData, _data.Key, ref _vta);
            }
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
                Globals.CurrentProfile = new Dictionary<string, Class.Account> { };
                if (!Utils.Account.Save(ref Globals.CurrentProfile, Globals.Info.profilesPath + "/" + Globals.Config.defaultProfile + ".json")) Application.Exit();
                CbProfile_LoadProfileDirectory(); // just do recursion cause lazyyy
            }
        }

        // Create a new profile
        private void AddProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string profileName = Microsoft.VisualBasic.Interaction.InputBox("Enter the profile's name (leave blank to cancel)", "Create new profile", null); // yes, lazy. very

            if (String.IsNullOrWhiteSpace(profileName)) return; // If canceled by the user

            // Sanitize the profile name
            foreach (char _invalid in Path.GetInvalidFileNameChars())
                profileName.Replace(_invalid, '_');

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
            Globals.CurrentProfile = new Dictionary<string, Class.Account> { };
            if (!Utils.Account.Save(ref Globals.CurrentProfile, profilePath)) Application.Exit();

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

        // Save profile
        private void BtnProfileSave_Click(object sender, EventArgs e)
        {
            if (cbProfile.Items.Count < 1) return;
            if (!Utils.Account.Save(ref Globals.CurrentProfile, Globals.Info.profilesPath + "/" + cbProfile.Items[cbProfile.SelectedIndex].ToString() + ".json")) Application.Exit();
            MessageBox.Show("Profile has been saved!", "Save profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Set as default profile
        private void SetAsDefaultProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals.Config.defaultProfile = cbProfile.Items[cbProfile.SelectedIndex].ToString();
            if (!Utils.Config.Save(ref Globals.Config, Globals.Info.cfgPath)) Application.Exit();
            MessageBox.Show("Current profile has been set as the default profile!", "Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region Account

        // Add an account
        private void DdAccountAdd_Click(object sender, EventArgs e)
        {
            do
            {
                using (Forms.Account _fad = new Forms.Account(ref lvData))
                {
                    _fad.ShowDialog();
                    _fad.Dispose();
                }
            } while (Globals.Cache.AddAnother && Globals.Cache.AddAnotherFlag);
        }

        // Edit an account
        private void DdAccountEdit_Click(object sender, EventArgs e)
        {
            if (lvData.SelectedItems.Count == 0)
            {
                MessageBox.Show("No account selected to edit.", "Edit Account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (ListViewItem _lvi in lvData.SelectedItems)
            {
                // Find the account in our profile dictionary
                Class.Account _vta;
                if (!Globals.CurrentProfile.TryGetValue(_lvi.SubItems[0].Text, out _vta))
                {
                    MessageBox.Show("Failed to obtain account data for steam url: " + _lvi.SubItems[1].Text + " with a unique id: " + _lvi.SubItems[0].Text + ". \n\nThis account has been skipped.", "Edit account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }

                if (_vta.hThread != null && _vta.hThread.IsAlive)
                {
                    MessageBox.Show("You cannot edit an account that is actively parsing!", "Edit account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }

                using (Forms.Account _fad = new Forms.Account(ref _vta))
                {
                    _fad.ShowDialog();
                    _fad.Dispose();
                }
            }
        }

        // Remove an account
        private void DdAccountRemove_Click(object sender, EventArgs e)
        {
            if (lvData.SelectedItems.Count == 0)
            {
                MessageBox.Show("No account selected to remove.", "Remove Account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (ListViewItem _lvi in lvData.SelectedItems)
            {
                #if DEBUG
                    Debug.WriteLine("Deleting account id: " + _lvi.SubItems[0].Text);
                #endif

                Class.Account _vta;
                if (!Globals.CurrentProfile.TryGetValue(_lvi.SubItems[0].Text, out _vta))
                {
                    MessageBox.Show("Failed to obtain account data for unique id: " + _lvi.SubItems[0].Text + ". \n\nThis account has been skipped.", "Remove account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }

                if (_vta.hThread != null && _vta.hThread.IsAlive)
                {
                    MessageBox.Show("You cannot remove an account that is actively parsing!", "Remove account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }

                Globals.CurrentProfile.Remove(_lvi.SubItems[0].Text);
                _lvi.Remove();
            }
        }

        private void DdManageObtainStart_Click(object sender, EventArgs e)
        {
            if (lvData.Items.Count == 0)
            {
                MessageBox.Show("There are no accounts to parse!", "Parse Queue", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Globals.ParserQueue != null && Globals.ParserQueue.IsAlive)
            {
                MessageBox.Show("A parsing queue is already active!", "Parse Queue", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // Better than invoking 3 delegates that doesn't even work
                List<ListViewItem> _tmp_acc_list = new List<ListViewItem> { };
                if (lvData.SelectedItems.Count == 0) foreach (ListViewItem _lvi in lvData.Items)         _tmp_acc_list.Add(_lvi); // Parses all of the items when there's non selected
                else                                 foreach (ListViewItem _lvi in lvData.SelectedItems) _tmp_acc_list.Add(_lvi); // Parses the selected item

                // Thread handling
                Globals.ParserQueue = new Thread(new ThreadStart(() =>
                {
                    void RunParserThread(ListViewItem _lvi)
                    {
                        // Hold the thread if we exceed max thread limit defined by the config
                        while (Globals.RunningThreads >= Globals.Config.maxThreads) Thread.Sleep(500);

                        Class.Account _vta;
                        if (!Globals.CurrentProfile.TryGetValue(_lvi.SubItems[0].Text, out _vta))
                        {
                           _vta.SetText("Reference error!");
                            return;
                        }

                        _vta.Parse();
                        return;
                    }

                    foreach (ListViewItem _lvi in _tmp_acc_list)
                        RunParserThread(_lvi);
                }
                ));

                Globals.ParserQueue.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to create parser queue!\n\nException: " + ex, "Parse Account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DdManageObtainAbort_Click(object sender, EventArgs e)
        {
            if (lvData.Items.Count == 0)
            {
                MessageBox.Show("There are no accounts to be selected!", "Parse account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Func<ListViewItem, bool> AbortParserThread = (ListViewItem _lvi) =>
            {
                Class.Account _vta;
                if (!Globals.CurrentProfile.TryGetValue(_lvi.SubItems[0].Text, out _vta))
                {
                    _lvi.SubItems[7].Text = "Reference error!";
                    return false;
                }

                // Abort it if it the instance has a running thread
                if (_vta.hThread != null && _vta.hThread.IsAlive)
                {
                    _vta.SafeAbort();
                    _lvi.SubItems[7].Text = "Parse Aborted!";
                    return true;
                }

                return false;
            };

            if (lvData.SelectedItems.Count == 0) foreach (ListViewItem _lvi in lvData.Items)         AbortParserThread(_lvi); // Abort all of the items when there's non selected
            else                                 foreach (ListViewItem _lvi in lvData.SelectedItems) AbortParserThread(_lvi); // Abort the selected item
        }

        private void DdManageObtainQueueAbort_Click(object sender, EventArgs e)
        {
            if (Globals.ParserQueue == null || !Globals.ParserQueue.IsAlive)
            {
                MessageBox.Show("There's current no running parser queue to terminate.", "Abort queue", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Globals.ParserQueue.Abort();
        }

        #endregion

        #region Info Pop-Up

        private void DdManageObtainInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Normal Parsing uses simple HttpClient to obtain the HTML of the account's steam page then parses for the information it needs, This is done because its faster since the information that is needed is already public.\n\n" +
                "Full Parsing on the other hand utilizes steam api and sometimes an actual login since some information aren't displayed publicly, therefore slower."
               , "OWL Account Parsing", MessageBoxButtons.OK, MessageBoxIcon.Information
            );
        }

        private void BtnAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(

                "OWL version " + Globals.Info.verStr + "\n" +
                "A multipurpose tool for managing Steam accounts.\n\n" +
                "https://github.com/FerrisSandCanyon/OWL\n\n" +
                "* You can paste contents to the textbox in the accounts form using double click.\n" +
                "* Steam URL's are automatically sanitized.\n" +
                "* Clicking on the Cooldown dropdown will by default add 21 hours to the selected account."

               , "OWL (Overwatch Whitelist)", MessageBoxButtons.OK, MessageBoxIcon.Information
            );
        }

        #endregion

        #region Account Login

        private void DdManageLoginNormal_Click(object sender, EventArgs e)
        {
            AccountLogin();
        }

        private void DdManageLoginForce_Click(object sender, EventArgs e)
        {
            AccountLogin(true);
        }

        private void AccountLogin(bool force = false)
        {
            if (lvData.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select a single account to login to!", "Login account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Class.Account _vta;
            if (!Globals.CurrentProfile.TryGetValue(lvData.SelectedItems[0].SubItems[0].Text, out _vta))
            {
                MessageBox.Show("Reference error!", "Login Account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FormMain_SetTitle("Logging in...");
            if (!_vta.Login(force)) FormMain_SetTitle();
        }

        #endregion

        #region Events

        private void Event_AddCooldown(object sender, EventArgs e)
        {
            if (lvData.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an account to apply the cooldown to!", "Add Cooldown", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (ListViewItem _lvi in lvData.SelectedItems)
            {
                Class.Account _vta;
                if (!Globals.CurrentProfile.TryGetValue(_lvi.SubItems[0].Text, out _vta))
                {
                    MessageBox.Show("Reference error!", "Add Cooldown", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }

                switch (((ToolStripMenuItem)sender).Name)
                {
                    case "ddManageCooldown7days":   _vta.CooldownDelta = DateTime.Now.AddDays(7)           ; break;
                    case "ddManageCooldown1day":    _vta.CooldownDelta = DateTime.Now.AddDays(1)           ; break;
                    case "ddManageCooldown22hours": _vta.CooldownDelta = DateTime.Now.AddHours(22)         ; break;
                    case "ddManageCooldown21hours": _vta.CooldownDelta = DateTime.Now.AddHours(21)         ; break;
                    case "ddManageCooldown2hours":  _vta.CooldownDelta = DateTime.Now.AddHours(2)          ; break;
                    case "ddManageCooldown1hour":   _vta.CooldownDelta = DateTime.Now.AddHours(1)          ; break;
                    case "ddManageCooldown30min":   _vta.CooldownDelta = DateTime.Now.AddMinutes(30)       ; break;
                    case "ddManageCooldown15min":   _vta.CooldownDelta = DateTime.Now.AddMinutes(15)       ; break;

                    case "remove":
                    default:
                        _vta.CooldownDelta = DateTime.MinValue;
                        break;
                }

            }
        }

        private void Event_CopyToClipboard(object sender, EventArgs e)
        {
            if (lvData.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select an account to obtain information from", "Copy to clipboard", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Func<string[], string[], string> ClipFormat = (string[] data, string[] rep) =>
            {
                string result = "";

                if (data.Length != rep.Length) return null;

                if (Globals.Config.clipboardDetail)
                {
                    for (int idx = 0; idx < data.Length; idx++)
                    {
                        result += $"{rep[idx]}: {data[idx]}\r\n";
                    }
                }
                else
                {
                    foreach (string _info in data) result += _info + ":";
                }

                return result;
            };

            Class.Account _vta;
            if (!Globals.CurrentProfile.TryGetValue(lvData.SelectedItems[0].SubItems[0].Text, out _vta))
            {
                MessageBox.Show("Reference error!", "Copy to clipboard", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string relay = "";

            switch(((ToolStripMenuItem)sender).Name)
            {
                case "ddManageClipboardUserPass":
                    Clipboard.SetText(relay = ClipFormat(
                        new string[]{ _vta.Username, _vta.Password },
                        new string[]{ "Username",    "Password"    }
                        ) ?? "");
                    break;

                case "ddManageClipboardURL":
                    //Clipboard.SetText(relay = ((Globals.Config.clipboardDetail ? "Steam URL: " : "") + "https://steamcommunity.com/" + _vta.SteamURL ?? "null"));
                    Clipboard.SetText(relay = "https://steamcommunity.com/" + _vta.SteamURL ?? "null");
                    break;

                case "ddManageClipboardNotes":
                    Clipboard.SetText(relay = ((Globals.Config.clipboardDetail ? "Notes: " : "") + _vta.Note ?? "null"));
                    break;

                case "ddManageClipboardAll":
                    Clipboard.SetText(relay = ClipFormat(
                        new string[] { "https://steamcommunity.com/" + _vta.SteamURL, _vta.Username, _vta.Password, _vta.Name, _vta.Banned.ToString(), _vta.Note },
                        new string[] {                                     "Steam URL",   "Username",    "Password",    "Name",    "Banned",               "Note"}
                        ) ?? "");
                    break;

                default:
                    return;
            }

            MessageBox.Show(relay, "Copied to clipboard!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Event_LSImport(object sender, EventArgs e)
        {
            using (Forms.Import _lsi = new Forms.Import(((ToolStripMenuItem)sender).Name, 0, ref lvData))
            {
                _lsi.ShowDialog();
                _lsi.Dispose();
            }
        }

        #endregion
    }
}
