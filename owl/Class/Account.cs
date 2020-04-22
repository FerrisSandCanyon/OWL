using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace owl.Class
{
    public class Account
    {

        // ==============
        // Initialization
        // ==============

        public Account() { }
        public Account(DateTime _CooldownDelta, string _ClassObjectID = null, string _SteamURL = null, string _Name = null, string _Username = null, string _Password = null, string _Note = null, bool _Banned = false)
        {
            ClassObjectID = _ClassObjectID;
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

        public string    SteamURL      = null,
                         Name          = null,
                         Username      = null,
                         Password      = null,
                         Note          = null,
                         ClassObjectID = null;

        public bool      Banned        = false;

        public DateTime  CooldownDelta  = DateTime.MinValue,
                         LastInfoUpdate = DateTime.MinValue;

        // =================
        // Instance handling
        // =================

        [JsonIgnore]
        public ListViewItem LVI = null; // Reference to the list view item of the current account instance

        [JsonIgnore]
        public static int RunningParserThreads = 0; // Total count of running parser threads

        // =======
        // Methods
        // =======

        // Initializes the parser
        public bool Parse()
        {
            if (this.LVI == null
            ||  this.hThread != null && this.hThread.IsAlive )
                return false;

            // Check if this instance has a URL
            if (string.IsNullOrWhiteSpace(this.SteamURL))
            {
                this.SetText("No URL!");
                return false;
            }

            // Check if its already banned, if it is skip it unless the config is set to force the account status
            if (this.Banned && !Globals.Config.forceStatus)
            {
                this.SetText("Skipped!");
                return false;
            }

            this.SetText("Init parser...");

            // Initialize the parser
            try
            {
                this.hThread = new Thread(new ThreadStart(this.ParserThread));
                this.hThread.Name = "parserThread";
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
                --RunningParserThreads;
            }
        }

        // Login the account
        public bool Login(LOGINMETHOD mode = 0)
        {
            if (string.IsNullOrWhiteSpace(this.Username) || string.IsNullOrWhiteSpace(this.Password))
            {
                MessageBox.Show("This account doesn't contain any credentials to login to", "Login Account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (Globals.IsLoggingIn)
            {
                MessageBox.Show("The application is already actively logging in an account!", "Login Account", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Make sure steam is closed. Only check if logging in with normal or force
            if (mode < LOGINMETHOD.NORMAL_NO_CHECK)
            {
                Process[] _temp_proc_list;
                while ((_temp_proc_list = Process.GetProcessesByName("Steam")).Count() > 0)
                {
                    if (mode == LOGINMETHOD.FORCE || MessageBox.Show("OWL has detected that the Steam client is still running. Would you like to close it forcibly? This is not recommended since it can cause instability. Only choose this option if you know what you're doing.", "Login Account", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                        foreach (Process steam in _temp_proc_list)
                            steam.Kill();
                    else
                        return false;

                    Thread.Sleep(800); // lazy
                }
            }

            Globals.IsLoggingIn = true;
            Globals.hFormMain.FormMain_UpdateTitle();

            if (Globals.LastAccountLogin != null)
                Globals.LastAccountLogin.LVI.BackColor = System.Drawing.Color.FromArgb(255, 42, 42, 42);

            Globals.LastAccountLogin = this;
            this.LVI.BackColor = System.Drawing.Color.FromArgb(255, 20, 20, 20);

            Globals.CurrentProfile.LastAccountLoggedIn = this.LVI.SubItems[0].Text;

            new Thread(new ThreadStart(() =>
            {
                if (mode == LOGINMETHOD.CLICK_CREDENTIALS)
                {
                    Class.Account _acc = this;
                    new Forms.ClickLogin(ref _acc).ShowDialog();
                }
                else 
                {
                    Process.Start(Globals.Config.steamPath + "/Steam.exe", $"-login \"{this.Username}\" \"{this.Password}\" {Globals.Config.steamParam}");
                }
                
                Globals.hFormMain.Invoke(new Action(() =>
                {
                    Globals.IsLoggingIn = false;
                    Globals.hFormMain.FormMain_UpdateTitle();
                }));
            }
            )).Start();

            return true;
        }

        // Obtains the cooldown
        public bool DisplayCooldown()
        {
            if (this.LVI == null)
                return false;

            if (this.CooldownDelta == DateTime.MinValue)
                this.SetText("No Cooldown!", 5);
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
            ++RunningParserThreads;
            NSoup.Nodes.Document _steamPage = null;
            
            try
            {
                _steamPage = NSoup.NSoupClient.Connect("https://steamcommunity.com/" + this.SteamURL).Get();
            }
            catch
            {
                --RunningParserThreads;
                this.SetText("Failed! (HttpClient)");
                return;
            }

            if (_steamPage == null)
            {
                --RunningParserThreads;
                this.SetText("Failed! (Null)");
                return;
            }

            this.SetText(this.Name = _steamPage.Select(".actual_persona_name").Text ?? "", 2);

            if (String.IsNullOrWhiteSpace(this.Name))
            {
                --RunningParserThreads;
                this.SetText("Invalid URL!");
                return;
            }

            this.SetText((this.Banned = !(_steamPage.Select(".profile_ban")).IsEmpty).ToString(), 4);
            this.LastInfoUpdate = DateTime.Now;
            this.SetText("Finished!");

            --RunningParserThreads;
        }

        // =========
        // Utilities
        // =========

        // Defaults to 7 to set status text
        public bool SetText(string status, int index = 7)
        {
            try
            {
                if (this.LVI == null) return false;

                if (this.LVI.ListView.InvokeRequired)
                {
                    if (Globals.hMainThread == null && !Globals.hMainThread.IsAlive) return false;
                    this.LVI.ListView.Invoke(new Action(() => { this.LVI.SubItems[index].Text = status; }));
                    return true;
                }
                else
                {
                    this.LVI.SubItems[index].Text = status;
                    return true;
                }
            }
            catch (Exception ex)
            {
                #if DEBUG
                    Debug.WriteLine("STI Exception: " + ex + "\nMain Thread alive? (Obv not): " + Globals.hMainThread.IsAlive.ToString());
                #endif
                return false;
            }
        }

        // =====================
        // HTTP Client Utilities
        // =====================

        public bool HttpLogin()
        {
            return false;
        }

        public bool HttpDestroy()
        {
            return false;
        }

        public int HttpEdit(string name, string realName, string summary, string id, string[] groups, 
                            int myProfile, int gameDetails, int friendList, int inventory, int profileComments,
                            BufferedStream profile = null)
        {
            return 0;
        }
    }
}

namespace owl.Utils
{
    public class Account
    {
        public static bool AccountExists(string username)
        {
            return Globals.CurrentProfile.Profiles.Values.FirstOrDefault(x => x.Username == username) != null;
        }

        public static bool Save(ref Class.ProfileInfo _profileInfo, string _profilePath)
        {
            try
            {
                _profileInfo.LastSaved = DateTime.Now; // Update last saved info
                _profileInfo.vProfileFormat = Globals.Info.vProfileFormat; // Update the saved profile JSON format version
                using (StreamWriter _sw = new StreamWriter(_profilePath, false, System.Text.Encoding.UTF8))
                {
                    _sw.WriteLine(JsonConvert.SerializeObject(_profileInfo));
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

        public static Class.ProfileInfo Load(string _profilePath)
        {
            try
            {
                string content = "";
                using (StreamReader _sr = new StreamReader(_profilePath, System.Text.Encoding.UTF8))
                {
                    content = _sr.ReadToEnd();
                    _sr.Close();
                }
                return JsonConvert.DeserializeObject<Class.ProfileInfo>((content));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Profile: {_profilePath}\nException: {ex}", "Failed to load profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static void AddToTable(ref ListView _lv, string _uniqueID, ref Class.Account _account)
        {  
            _account.LVI = (new ListViewItem(_uniqueID));
            _account.LVI.SubItems.AddRange(new String[] { _account.SteamURL, _account.Name, _account.Username, _account.Banned.ToString(), "", _account.Note, "" });
            _lv.Items.Add(_account.LVI);

            if (_uniqueID == Globals.CurrentProfile.LastAccountLoggedIn)
            {
                Globals.LastAccountLogin = _account;
                _account.LVI.BackColor = System.Drawing.Color.FromArgb(255, 20, 20, 20);
            }
        }

        public static string MakeUniqueKey(uint length = 10)
        {
            // Generate a unique id for the json
            Random _rnd = new Random();
            string uniqueId = "";

            do
            {
                uniqueId = "";

                for (int x = 1; x < length; x++)
                    uniqueId += Globals.Charset[_rnd.Next(0, Globals.Charset.Length - 1)];

            } while (Globals.CurrentProfile.Profiles.ContainsKey(uniqueId));

            return uniqueId;
        }

        public static bool HttpGenerate()
        {
            return false;
        }

    }
}