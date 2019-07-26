using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vactrak.Class
{
    public class VTAccount
    {

        // ==============
        // Initialization
        // ==============

        public VTAccount() { }
        public VTAccount(string _SteamURL, string _Name = null, string _Username = null, string _Password = null, string _Note = null, bool _Banned = false, ulong _CooldownDelta = 0)
        {
            SteamURL      = _SteamURL;
            Name          = _Name;
            Username      = _Username;
            Password      = _Password;
            Note          = _Note;
            Banned        = _Banned;
            CooldownDelta = _CooldownDelta;
        }

        // ======
        // Values
        // ======

        public string SteamURL = null, Name = null, Username = null, Password = null, Note = null;
        public bool   Banned = false;
        public ulong  CooldownDelta = 0;

        // =================
        // Instance handling
        // =================

        [JsonIgnore]
        public ListViewItem LVI = null; // Reference to the list view item of the current account instance

        // =======
        // Methods
        // =======

        // Initializes the parser
        public bool Parse()
        {
            if (this.LVI == null) return false;
            if (this.hThread != null && this.hThread.IsAlive) return false;

            // Check if this instance has a URL
            if (string.IsNullOrWhiteSpace(this.SteamURL))
            {
                this.SetText("No URL!");
                return false;
            }

            // Check if its already banned, if it is skip it unless the config is set to force the account status
            if (this.LVI.SubItems[4].Text == "True" && !Globals.Config.forceStatus)
            {
                this.SetText("Skipped!");
                return false;
            }

            this.SetText("Init parser...");

            // Initialize the parser
            try
            {
                this.hThread = new Thread(new ThreadStart(this.ParserThread));
                this.hThread.Start();
                this.SetText("Parsing...");
                return true;
            }
            catch (Exception ex)
            {
                this.SetText("Failed! (Thread)");
                MessageBox.Show("Failed to start thread for account: \"" + this.Username + "\"\n\nException: " + ex, "Account parser", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Login the account
        public bool Login(bool forceKill = false)
        {
            if (string.IsNullOrWhiteSpace(this.Username) || string.IsNullOrWhiteSpace(this.Password))
            {
                MessageBox.Show("This account doesn't contain any credentials to login to", "Login Account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Make sure steam is closed
            while (Process.GetProcessesByName("Steam").Count() > 0)
            {
                if (forceKill || MessageBox.Show("VACTrak# has detected that the Steam client is still running. Would you like to close it forcibly? This is not recommended since it can cause instability. Only choose this option if you know what you're doing.", "Login Account", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                    foreach (Process steam in Process.GetProcessesByName("Steam")) steam.Kill();
                else
                    return false;

                Thread.Sleep(800); // lazy
            }

            new Thread(new ThreadStart(() =>
            {
                // TODO: Add user option to add custom steam parameters
                Process.Start(Globals.Config.steamPath + "/Steam.exe", String.Format("-login {0} {1}", this.Username, this.Password));
                Globals.hFormMain.Invoke(new Action(() => { Globals.hFormMain.FormMain_SetTitle(); }));
            }
            )).Start();

            return true;
        }

        // =============
        // Parser Thread
        // =============

        [JsonIgnore]
        public Thread hThread = null; // Reference to the parser thread that is running

        private void ParserThread()
        {
            NSoup.Nodes.Document _steamPage = null;

            try
            {
                _steamPage = NSoup.NSoupClient.Connect("https://steamcommunity.com/" + this.SteamURL).Get();
            }
            catch
            {
                this.SetTextInvoked("Failed! (HttpClient)");
                return;
            }

            if (_steamPage == null)
            {
                this.SetTextInvoked("Failed! (Null)");
                return;
            }

            this.SetTextInvoked(this.Name = _steamPage.Select(".actual_persona_name").Text ?? "", 2);

            if (String.IsNullOrWhiteSpace(this.Name))
            {
                this.SetTextInvoked("Invalid URL!");
                return;
            }

            bool _temp_isbanned = this.Banned = !(_steamPage.Select(".profile_ban")).IsEmpty;
            this.SetTextInvoked(_temp_isbanned.ToString(), 4);

            this.SetTextInvoked("Finished!");
        }

        // =========
        // Utilities
        // =========

        // Defaults to 7 to set status text
        public bool SetText(string status, int index = 7)
        {
            if (this.LVI == null) return false;
            this.LVI.SubItems[index].Text = status;
            return true;
        }

        private bool SetTextInvoked(string status, int index = 7)
        {
            if (this.LVI == null) return false;
            this.LVI.ListView.Invoke(new Action(() => { this.LVI.SubItems[index].Text = status; }));
            return true;
        }

    }
}

namespace vactrak.Utils
{
    public class VTAccount
    {
        public static bool Save(ref Dictionary<string, Class.VTAccount> _accountList, string _profilePath)
        {
            try
            {
                using (StreamWriter _sw = new StreamWriter(_profilePath, false, System.Text.Encoding.UTF8))
                {
                    _sw.WriteLine(JsonConvert.SerializeObject(_accountList));
                    _sw.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Profile: {0}\nException: {1}", _profilePath, ex), "Failed to save profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static Dictionary<string, Class.VTAccount> Load(string _profilePath)
        {
            try
            {
                string content = "";
                using (StreamReader _sr = new StreamReader(_profilePath, System.Text.Encoding.UTF8))
                {
                    content = _sr.ReadToEnd();
                    _sr.Close();
                }
                return JsonConvert.DeserializeObject<Dictionary<string, Class.VTAccount>>((content));
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Profile: {0}\nException: {1}", _profilePath, ex), "Failed to load profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static void AddToTable(ref ListView _lv, string _uniqueID, ref Class.VTAccount _vta)
        {
            _vta.LVI = (new ListViewItem(_uniqueID));
            _vta.LVI.SubItems.AddRange(new String[] { _vta.SteamURL, _vta.Name, _vta.Username, _vta.Banned.ToString(), "", _vta.Note, "" });
            _lv.Items.Add(_vta.LVI);
        }

    }
}