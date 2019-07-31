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

namespace owl.Class
{
    public class Account
    {

        // ==============
        // Initialization
        // ==============

        public Account() { }
        public Account(DateTime _CooldownDelta, string _SteamURL = null, string _Name = null, string _Username = null, string _Password = null, string _Note = null, bool _Banned = false)
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

        public string    SteamURL = null, Name = null, Username = null, Password = null, Note = null;
        public bool      Banned = false;
        public DateTime  CooldownDelta = DateTime.MinValue;

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
                this.hThread.Name = "vt_parser";
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

        // Safely abort a parser thread
        public void SafeAbort()
        {
            if (this.hThread != null && this.hThread.IsAlive)
            {
                this.hThread.Abort();
                Globals.RunningThreads--;
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
            Process[] _temp_proc_list;
            while ((_temp_proc_list = Process.GetProcessesByName("Steam")).Count() > 0)
            {
                if (forceKill || MessageBox.Show("OWL has detected that the Steam client is still running. Would you like to close it forcibly? This is not recommended since it can cause instability. Only choose this option if you know what you're doing.", "Login Account", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                    foreach (Process steam in _temp_proc_list) steam.Kill();
                else
                    return false;

                Thread.Sleep(800); // lazy
            }

            new Thread(new ThreadStart(() =>
            {
                // TODO: Add user option to add custom steam parameters
                Process.Start(Globals.Config.steamPath + "/Steam.exe", $"-login \"{this.Username}\" \"{this.Password}\" {Globals.Config.steamParam}");
                Globals.hFormMain.Invoke(new Action(() => { Globals.hFormMain.FormMain_SetTitle(); }));
            }
            )).Start();

            return true;
        }

        // Obtains the cooldown
        public bool DisplayCooldown()
        {
            if (this.LVI == null) return false;

            if (this.CooldownDelta == DateTime.MinValue) this.SetText("No Cooldown!", 5);
            else
            {
                TimeSpan _time = this.CooldownDelta - DateTime.Now;
                if (_time.TotalSeconds < 1)
                {
                    this.CooldownDelta = DateTime.MinValue;
                    return true;
                }

                this.SetText(String.Format("{0:00}d:{1:00}h:{2:00}m:{3:00}s", _time.Days, _time.Hours, _time.Minutes, _time.Seconds), 5);
            }

            return true;
        }

        // =============
        // Parser Thread
        // =============

        [JsonIgnore]
        public Thread hThread = null; // Reference to the parser thread that is running

        private void ParserThread()
        {
            Globals.RunningThreads++;
            NSoup.Nodes.Document _steamPage = null;
            
            try
            {
                _steamPage = NSoup.NSoupClient.Connect("https://steamcommunity.com/" + this.SteamURL).Get();
            }
            catch
            {
                Globals.RunningThreads--;
                this.SetText("Failed! (HttpClient)");
                return;
            }

            if (_steamPage == null)
            {
                Globals.RunningThreads--;
                this.SetText("Failed! (Null)");
                return;
            }

            this.SetText(this.Name = _steamPage.Select(".actual_persona_name").Text ?? "", 2);

            if (String.IsNullOrWhiteSpace(this.Name))
            {
                Globals.RunningThreads--;
                this.SetText("Invalid URL!");
                return;
            }

            bool _temp_isbanned = this.Banned = !(_steamPage.Select(".profile_ban")).IsEmpty;
            this.SetText(_temp_isbanned.ToString(), 4);

            Globals.RunningThreads--;
            this.SetText("Finished!");
        }

        // =========
        // Utilities
        // =========

        // Defaults to 7 to set status text
        public bool SetText(string status, int index = 7)
        {
            if (this.LVI == null) return false;

            if (this.LVI.ListView.InvokeRequired)
            {
                if (Globals.hMainThread == null && !Globals.hMainThread.IsAlive) return false;

                try
                {
                    this.LVI.ListView.Invoke(new Action(() => { this.LVI.SubItems[index].Text = status; }));
                    return true;
                }
                catch(Exception ex)
                {
                    #if DEBUG
                        Debug.WriteLine("STI Exception: " + ex + "\nMain Thread alive? (Obv not): " + Globals.hMainThread.IsAlive.ToString());
                    #endif

                    return false;
                }
            }
            else
            {
                this.LVI.SubItems[index].Text = status;
                return true;
            }
        }
    }
}

namespace owl.Utils
{
    public class Account
    {
        public static bool Save(ref Dictionary<string, Class.Account> _accountList, string _profilePath)
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
                MessageBox.Show($"Profile: {_profilePath}\nException: {ex}", "Failed to save profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static Dictionary<string, Class.Account> Load(string _profilePath)
        {
            try
            {
                string content = "";
                using (StreamReader _sr = new StreamReader(_profilePath, System.Text.Encoding.UTF8))
                {
                    content = _sr.ReadToEnd();
                    _sr.Close();
                }
                return JsonConvert.DeserializeObject<Dictionary<string, Class.Account>>((content));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Profile: {_profilePath}\nException: {ex}", "Failed to load profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static void AddToTable(ref ListView _lv, string _uniqueID, ref Class.Account _vta)
        {
            _vta.LVI = (new ListViewItem(_uniqueID));
            _vta.LVI.SubItems.AddRange(new String[] { _vta.SteamURL, _vta.Name, _vta.Username, _vta.Banned.ToString(), "", _vta.Note, "" });
            _lv.Items.Add(_vta.LVI);
        }

    }
}