using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vactrak.Class
{
    public class VTAccount
    {
        public string SteamURL = null, Username = null, Password = null, Note = null;
        public bool   Banned = false;
        public ulong  CooldownDelta = 0;
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

    }
}