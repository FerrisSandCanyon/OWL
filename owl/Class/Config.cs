using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;

namespace owl.Class
{
    public class Config
    {
        public string steamPath        = "C:/Program Files (x86)/Steam",   // Path to steam directory
                      defaultProfile   = "default",                        // Default profile to be loaded
                      hashedKey        = "",                               // Hashed key / password
                      steamParam       = "";                               // Custom steam startup parameters

        public int    cooldownRefresh  = 800,                              // Cooldown refresh rate measured in milliseconds
                      maxThreads       = 4,                                // Maximum threads that can run at the same time
                      loginMethod      = 0;                                // Login Method used by the shortcut key

        public bool   forceStatus      = false,                            // Forces status check on accounts with Bans
                      maskPassword     = true,                             // Display the password in plain text or mask it with • on the accounts form
                      clipboardDetail  = false,                            // Dictates clipboard mode whether to use a single line (false) or detailed (true) format
                      startupUpdateChk = true;                             // Checks for updates at startup
    }
}

namespace owl.Utils
{
    public class Config
    {
        public static bool Save(ref Class.Config _configClass, string _configPath)
        {
            try
            {
                using (StreamWriter _sw = new StreamWriter(_configPath, false, System.Text.Encoding.UTF8))
                {
                    _sw.WriteLine(JsonConvert.SerializeObject(_configClass));
                    _sw.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Profile: {_configPath}\nException: {ex}", "Failed to save config", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static Class.Config Load(string _configPath)
        {
            try
            {
                string content = "";
                using (StreamReader _sr = new StreamReader(_configPath, System.Text.Encoding.UTF8))
                {
                    content = _sr.ReadToEnd();
                    _sr.Close();
                }
                return JsonConvert.DeserializeObject<Class.Config>((content));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Profile: {_configPath}\nException: {ex}", "Failed to load config", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

    }
}
