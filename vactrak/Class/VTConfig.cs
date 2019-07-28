using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows.Forms;

namespace vactrak.Class
{
    public class VTConfig
    {
        public string steamPath        = "C:/Program Files (x86)/Steam",   // Path to steam directory
                      defaultProfile   = "default",                        // Default profile to be loaded
                      hashedKey        = "",                               // Hashed key / password
                      steamParam       = "";                               // Custom steam startup parameters

        public int    cooldownRefresh  = 800,                              // Cooldown refresh rate measured in milliseconds
                      maxThreads       = 4;                                // Maximum threads that can run at the same time

        public bool   forceStatus      = false,                            // Forces status check on accounts with Bans
                      maskPassword     = true;                             // Display the password in plain text or mask it with • on the accounts form
    }
}

namespace vactrak.Utils
{
    public class VTConfig
    {
        public static bool Save(ref Class.VTConfig _configClass, string _configPath)
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
                MessageBox.Show(String.Format("Profile: {0}\nException: {1}", _configPath, ex), "Failed to save config", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static Class.VTConfig Load(string _configPath)
        {
            try
            {
                string content = "";
                using (StreamReader _sr = new StreamReader(_configPath, System.Text.Encoding.UTF8))
                {
                    content = _sr.ReadToEnd();
                    _sr.Close();
                }
                return JsonConvert.DeserializeObject<Class.VTConfig>((content));
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Profile: {0}\nException: {1}", _configPath, ex), "Failed to load config", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

    }
}
