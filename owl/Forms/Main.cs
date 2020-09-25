using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;
using System.Linq;

namespace owl
{
    public partial class FormMain : Form
    {

        public Class.Account SelectedAccount = null; // Reference to the first selected account in lvData

        // Form title tracking
        public string title_status = null,
                      title_fallback = null;

        public FormMain()
        {
            InitializeComponent();
        }

        public int lvDataSelectedCount
        {
            get
            {
                return lvData.SelectedItems.Count;
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.Focus();

            // ==============
            // Initialization
            // ==============

#if DEBUG
            this.Text += " [DEBUG MODE]";
#else // Release
                ddUtils.Visible = false;

                ddManageObtainFullStart.Visible = false;
                ddManageObtainFullAbort.Visible = false;
                toolStripSeparator8.Visible = false;

                ddManageCooldownCustom.Visible = false;

                ddAccountImportSAGGC.Visible = false;
#endif

            this.title_fallback = this.Text;
            Globals.hMainThread = Thread.CurrentThread;
            CbProfile_LoadProfileDirectory();

            lblBuildInfo.Text = "v" + Globals.Info.verStr;

            // Cooldown tracker thread
            new Thread(new ThreadStart(() =>
            {
                while (Globals.hMainThread != null && Globals.hMainThread.IsAlive)
                {
                    Thread.Sleep(Globals.Config.cooldownRefresh);

                    try
                    {
                        if (Globals.CurrentProfile == null || Globals.CurrentProfile.Profiles == null || Globals.CurrentProfile.Profiles.Count == 0)
                            continue;

                        foreach (KeyValuePair<string, Class.Account> _account in Globals.CurrentProfile.Profiles)
                        {
                            if (!Globals.hMainThread.IsAlive)
                                break;
                            _account.Value.DisplayCooldown();
                        }
                    }
                    catch { }
                }
            }
            )).Start();

            FormMain_UpdateTitle();

#if !DEBUG
            if (Globals.Config.startupUpdateChk)
                Utils.AppUpdate.CheckUpdateWrapper(true);
#endif
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Globals.IsLoggingIn || Globals.ParserQueue != null && Globals.ParserQueue.IsAlive)
            {
                MessageBox.Show("Please make sure all tasks are finished before terminating the program.", "Close Aborted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Cancel = true;
            }
        }

        // Legacy 
        public void FormMain_SetTitle(string _status = null)
        {
            this.title_status = _status;
            FormMain_UpdateTitle();
        }

        public void FormMain_UpdateTitle()
        {
            this.Text = this.title_fallback +
                       (Globals.CurrentProfile != null && Globals.CurrentProfile.LastSaved != DateTime.MinValue ? $" | Last Save: {Globals.CurrentProfile.LastSaved}" : "") +
                       (Globals.IsLoggingIn ? " | Logging in..." : "") +
                       (string.IsNullOrWhiteSpace(this.title_status) ? "" : $" | {this.title_status}");
        }

        private void LvData_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvData.SelectedItems.Count == 0)
            {
                tbNote.Text = "";
                SelectedAccount = null;
            }
            else
            {
                if (lvData.SelectedItems.Count > 1 || !Globals.CurrentProfile.Profiles.TryGetValue(lvData.SelectedItems[0].SubItems[0].Text, out SelectedAccount))
                    return;

                tbNote.Text = SelectedAccount.Note;
            }
        }

        private void TbNote_TextChanged(object sender, EventArgs e)
        {
            if (lvData.SelectedItems.Count == 0 || SelectedAccount == null || SelectedAccount.LVI == null)
                return;

            SelectedAccount.LVI.SubItems[6].Text = SelectedAccount.Note = tbNote.Text;

            if (lvData.SelectedItems.Count > 1)
            {
                foreach (KeyValuePair<string, Class.Account> _acc in Utils.ProfileInfo.GetSelectedItems())
                {
                    if (_acc.Value == SelectedAccount)
                        continue;

                    _acc.Value.LVI.SubItems[6].Text = _acc.Value.Note = tbNote.Text;
                }
            }
        }

        private void refreshListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CbProfile_LoadProfileDirectory();
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
            ddManageCooldown.HideDropDown();
        }

        private void importConvertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new Forms.ProfileJSONImport()).ShowDialog();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            Utils.AppUpdate.CheckUpdateWrapper();
        }

        private void ddManageLoginAutoSteamClear_Click(object sender, EventArgs e)
        {
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "AutoLoginUser", "", RegistryValueKind.String);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "RememberPassword", 0, RegistryValueKind.DWord);
        }

        private void ddSteamUserDataDelete_Click(object sender, EventArgs e)
        {
            if (lvData.SelectedItems.Count == 0)
            {
                MessageBox.Show("No items selected.", "Delete Userdata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string log = "";
            foreach (KeyValuePair<string, Class.Account> account in Utils.ProfileInfo.GetSelectedItems())
            {
                string id3 = account.Value.ResolveSteamID3();

                if (id3 == null)
                    continue;

                string finalpath = Path.Combine(Globals.Config.steamPath, $"userdata/{id3}");

                if (!Directory.Exists(finalpath))
                    continue;

                Directory.Delete(finalpath, true);
                log += $"\n* {account.Value.Username} ({finalpath})";
            }

            MessageBox.Show(log, "Deletion log", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ddSteamUserDataDuplicate_Click(object sender, EventArgs e)
        {
            if (lvData.SelectedItems.Count < 2)
            {
                MessageBox.Show("Select atleast 2 items to duplicate user data to and from.", "Duplicate Userdata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var selectedAccs = Utils.ProfileInfo.GetSelectedItems();
            var parentacc = selectedAccs.ElementAt(0);
            selectedAccs.Remove(parentacc.Key);
            new Forms.DuplicateUserData(parentacc.Value, selectedAccs.Values.ToList()).ShowDialog();
        }

        private void swapAccountPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Globals.CurrentProfile == null)
            {
                MessageBox.Show("No profile loaded.", "Swap Accounts Position", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ListView.SelectedListViewItemCollection selitems = lvData.SelectedItems;

            if (selitems.Count != 2)
            {
                MessageBox.Show("Please select 2 items to swap positions.", "Swap Accounts Position", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            // for backwards compability, we store the class object id. in later versions the account class itself has this information.
            string swapA_ClassObjectID  = lvData.SelectedItems[0].SubItems[0].Text;
            string swapB_ClassObjectID = lvData.SelectedItems[1].SubItems[0].Text;

            // Store the first account temporarily
            Class.Account swap_Account = Globals.CurrentProfile.Profiles[swapA_ClassObjectID];

            // Swap the two accounts and Re-assign the class object id
            (Globals.CurrentProfile.Profiles[swapA_ClassObjectID] = Globals.CurrentProfile.Profiles[swapB_ClassObjectID]).ClassObjectID = swapA_ClassObjectID;
            (Globals.CurrentProfile.Profiles[swapB_ClassObjectID] = swap_Account).ClassObjectID = swapB_ClassObjectID;

            // Assign the new last login properly if applicable
            bool isSwapA = swapA_ClassObjectID == Globals.CurrentProfile.LastAccountLoggedIn;
            if (isSwapA || swapB_ClassObjectID == Globals.CurrentProfile.LastAccountLoggedIn)
            {
                Globals.CurrentProfile.LastAccountLoggedIn = isSwapA ? swapB_ClassObjectID : swapA_ClassObjectID;
            }

            // Reload the table
            CbProfile_LoadToTable();
        }

        #region Profile

        // Loads the selected profile.
        private void CbProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbProfile.Items.Count < 1)
            {
                FormMain_UpdateTitle();
                return;
            }

            string _profilePath = Globals.Info.profilesPath + "/" + cbProfile.Items[cbProfile.SelectedIndex].ToString() + ".json";

            // Make sure it exists
            if (!File.Exists(_profilePath))
            {
                MessageBox.Show("This profile does not exist", "Load profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CbProfile_LoadProfileDirectory();
                return;
            }

            // Load the profile
            Globals.CurrentProfile = Utils.Account.Load(_profilePath);
            if (Globals.CurrentProfile == null)
                Application.Exit();

            CbProfile_LoadToTable();

            FormMain_UpdateTitle();
        }

        // Load the current profile into the table
        private void CbProfile_LoadToTable()
        {
            lvData.Items.Clear();

            if (Globals.CurrentProfile.vProfileFormat < Globals.Info.vProfileFormat)
            {
                MessageBox.Show($"Invalid/Incompatible Profile JSON format version!\nLoaded JSON format version: {Globals.CurrentProfile.vProfileFormat}\nCurrent JSON format version: {Globals.Info.vProfileFormat}", "Load JSON", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Load all the accounts to our table
            if (Globals.CurrentProfile.Profiles != null)
                foreach (KeyValuePair<string, Class.Account> _data in Globals.CurrentProfile.Profiles)
                {
                    Class.Account _account = _data.Value;
                    Utils.Account.AddToTable(ref lvData, _data.Key, ref _account);
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
                Globals.CurrentProfile = new Class.ProfileInfo();
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
            Globals.CurrentProfile = new Class.ProfileInfo();
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
            if (cbProfile.Items.Count < 1)
                return;

            if (!Utils.Account.Save(ref Globals.CurrentProfile, Globals.Info.profilesPath + "/" + cbProfile.Items[cbProfile.SelectedIndex].ToString() + ".json"))
                Application.Exit();

            FormMain_UpdateTitle();
        }

        // Set as default profile
        private void SetAsDefaultProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals.Config.defaultProfile = cbProfile.Items[cbProfile.SelectedIndex].ToString();
            if (!Utils.Config.Save(ref Globals.Config, Globals.Info.cfgPath)) Application.Exit();
            MessageBox.Show("Current profile has been set as the default profile!", "Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void transferAccountsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cbProfile.Items.Count < 2)
            {
                MessageBox.Show("No profile to transfer to!", "Transfer Account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (lvData.SelectedItems.Count < 1)
            {
                MessageBox.Show("No account(s) selected to transfer!", "Transfer Account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idx = 0;
            string[] profiles       = new string[cbProfile.Items.Count - 1]; // Stores the profiles except the currently selected
            string   currentProfile = cbProfile.Items[cbProfile.SelectedIndex].ToString();
           
            // Parses the profile list obtained at startup from Globals.Info.profilesPath
            foreach (string profileName in cbProfile.Items)
            {
                if (profileName == currentProfile)
                    continue;

                profiles[idx++] = profileName;
            }

            new Forms.AccountTransfer(ref profiles).ShowDialog();
        }

#endregion

#region Account

        // Add an account
        private void DdAccountAdd_Click(object sender, EventArgs e)
        {
            do
            {
                using (Forms.AccountInfo _fad = new Forms.AccountInfo(ref lvData))
                {
                    _fad.ShowDialog();
                    _fad.Dispose();
                }
            } while (Forms.AccountInfo.cache_addAnother && Forms.AccountInfo.cache_addAnotherFlag);
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
                Class.Account _account;
                if (!Globals.CurrentProfile.Profiles.TryGetValue(_lvi.SubItems[0].Text, out _account))
                {
                    MessageBox.Show("Failed to obtain account data for steam url: " + _lvi.SubItems[1].Text + " with a unique id: " + _lvi.SubItems[0].Text + ". \n\nThis account has been skipped.", "Edit account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }

                if (_account.hThread != null && _account.hThread.IsAlive)
                {
                    MessageBox.Show("You cannot edit an account that is actively parsing!", "Edit account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }

                using (Forms.AccountInfo _fad = new Forms.AccountInfo(ref _account))
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

            foreach (KeyValuePair<string, Class.Account> _account in Utils.ProfileInfo.GetSelectedItems())
            {

                if (_account.Value.hThread != null && _account.Value.hThread.IsAlive)
                {
                    MessageBox.Show("You cannot remove an account that is actively parsing!", "Remove account", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }

                Globals.CurrentProfile.Profiles.Remove(_account.Key);
                _account.Value.LVI.Remove();
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
                Dictionary<string, Class.Account> _accounts = lvData.SelectedItems.Count > 0 ? Utils.ProfileInfo.GetSelectedItems() : Globals.CurrentProfile.Profiles;
                // Thread handling
                Globals.ParserQueue = new Thread(new ThreadStart(() =>
                {
                    foreach (KeyValuePair<string, Class.Account> _account in _accounts)
                    {
                        // Hold the thread if we exceed max thread limit defined by the config
                        while (Class.Account.RunningParserThreads >= Globals.Config.maxThreads)
                            Thread.Sleep(500);

                        _account.Value.Parse();
                    }
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
                Class.Account _account;
                if (!Globals.CurrentProfile.Profiles.TryGetValue(_lvi.SubItems[0].Text, out _account))
                {
                    _lvi.SubItems[7].Text = "Reference error!";
                    return false;
                }

                // Abort it if it the instance has a running thread
                if (_account.hThread != null && _account.hThread.IsAlive)
                {
                    _account.SafeAbort();
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
            if ( MessageBox.Show(
                "OWL version " + Globals.Info.verStr + "\n" +
                "A multipurpose tool for managing Steam accounts.\n\n" +
                "https://github.com/FerrisSandCanyon/OWL\n\n" +
                "* You can paste contents to the textbox in the accounts form using double click.\n" +
                "* Steam URL's are automatically sanitized.\n" +
                "* Clicking on the Cooldown dropdown will by default add 21 hours to the selected account." +
                "\n\nWould you like to open the project's Github?"

               , "OWL", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                Process.Start("https://github.com/FerrisSandCanyon/OWL");
            }
        }

#endregion

#region Account Login

        private void DdManageLoginNormal_Click(object sender, EventArgs e)
        {
            AccountLogin(LOGINMETHOD.NORMAL);
        }

        private void DdManageLoginForce_Click(object sender, EventArgs e)
        {
            AccountLogin(LOGINMETHOD.FORCE);
        }

        private void ddManageLoginNormalNoCheck_Click(object sender, EventArgs e)
        {
            AccountLogin(LOGINMETHOD.NORMAL_NO_CHECK);
        }

        private void ddManageLoginClickCreds_Click(object sender, EventArgs e)
        {
            AccountLogin(LOGINMETHOD.CLICK_CREDENTIALS);
        }

        private void AccountLogin(LOGINMETHOD mode = LOGINMETHOD.NORMAL)
        {
            if (lvData.SelectedItems.Count != 1)
            {
                MessageBox.Show("Please select a single account to login to!", "Login account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Class.Account _account;
            if (!Globals.CurrentProfile.Profiles.TryGetValue(lvData.SelectedItems[0].SubItems[0].Text, out _account))
            {
                MessageBox.Show("Reference error!", "Login Account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _account.Login(mode);
        }

#endregion

#region Events

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        {
            switch (keyData)
            {
                // Save Profile
                case Keys.Control | Keys.S:
                    BtnProfileSave_Click(null, null);
                    break;

                // Delete Account
                case Keys.Delete:
                    DdAccountRemove_Click(null, null);
                    break;

                // Add Cooldown to account
                case Keys.Control | Keys.D:
                    Event_AddCooldown(ddManageCooldown21hours, null);
                    break;

                // Login
                case Keys.Control | Keys.F:
                    AccountLogin(Globals.Config.loginMethod);
                    break;

                // Add New Account
                case Keys.Control | Keys.N:
                    DdAccountAdd_Click(null, null);
                    break;

                // Edit Account
                case Keys.Control | Keys.E:
                    DdAccountEdit_Click(null, null);
                    break;

                // Copy all info to clipboard
                case Keys.Control | Keys.C:
                    Event_CopyToClipboard(ddManageClipboardAll, null);
                    break;

                // Obtain Account Info
                case Keys.Control | Keys.A:
                    DdManageObtainStart_Click(null, null);
                    break;

                // Open URL
                case Keys.Control | Keys.O:
                {
                    foreach (KeyValuePair<string, Class.Account> _acc in Utils.ProfileInfo.GetSelectedItems())
                    {
                        if (string.IsNullOrWhiteSpace(_acc.Value.SteamURL))
                            continue;

                        new Thread(() =>
                        {
                            Process.Start("https://steamcommunity.com/" + _acc.Value.SteamURL);
                        }).Start();
                    }

                    break;
                }

                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }

            return true;
        }

        private void Event_AddCooldown(object sender, EventArgs e)
        {
            if (lvData.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an account to apply the cooldown to!", "Add Cooldown", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (KeyValuePair<string, Class.Account> _account in Utils.ProfileInfo.GetSelectedItems())
            {
                switch (((ToolStripMenuItem)sender).Name)
                {
                    case "ddManageCooldown7days":   _account.Value.CooldownDelta = DateTime.Now.AddDays(7)           ; break;
                    case "ddManageCooldown1day":    _account.Value.CooldownDelta = DateTime.Now.AddDays(1)           ; break;
                    case "ddManageCooldown22hours": _account.Value.CooldownDelta = DateTime.Now.AddHours(22)         ; break;
                    case "ddManageCooldown21hours": _account.Value.CooldownDelta = DateTime.Now.AddHours(21)         ; break;
                    case "ddManageCooldown2hours":  _account.Value.CooldownDelta = DateTime.Now.AddHours(2)          ; break;
                    case "ddManageCooldown1hour":   _account.Value.CooldownDelta = DateTime.Now.AddHours(1)          ; break;
                    case "ddManageCooldown30min":   _account.Value.CooldownDelta = DateTime.Now.AddMinutes(30)       ; break;
                    case "ddManageCooldown15min":   _account.Value.CooldownDelta = DateTime.Now.AddMinutes(15)       ; break;

                    case "ddManageCooldownRemove":
                    default:
                        _account.Value.CooldownDelta = DateTime.MinValue;
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

            Class.Account _account;
            if (!Globals.CurrentProfile.Profiles.TryGetValue(lvData.SelectedItems[0].SubItems[0].Text, out _account))
            {
                MessageBox.Show("Reference error!", "Copy to clipboard", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string relay = "";

            switch(((ToolStripMenuItem)sender).Name)
            {
                case "ddManageClipboardUserPass":
                    Clipboard.SetText(relay = ClipFormat(
                        new string[]{ _account.Username, _account.Password },
                        new string[]{         "Username",        "Password"    }
                        ) ?? "");
                    break;

                case "ddManageClipboardURL":
                    //Clipboard.SetText(relay = ((Globals.Config.clipboardDetail ? "Steam URL: " : "") + "https://steamcommunity.com/" + _account.SteamURL ?? "null"));
                    Clipboard.SetText(relay = "https://steamcommunity.com/" + _account.SteamURL ?? "null");
                    break;

                case "ddManageClipboardNotes":
                    Clipboard.SetText(relay = ((Globals.Config.clipboardDetail ? "Notes: " : "") + _account.Note ?? "null"));
                    break;

                case "ddManageClipboardAll":
                    Clipboard.SetText(relay = ClipFormat(
                        new string[] { "https://steamcommunity.com/" + _account.SteamURL, _account.Username, _account.Password, _account.Name, _account.Banned.ToString(), _account.Note },
                        new string[] {                                         "Steam URL",       "Username",        "Password",        "Name",        "Banned",                   "Note"}
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
    }
#endregion
}
