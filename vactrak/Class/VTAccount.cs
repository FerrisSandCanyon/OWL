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

        public string SteamURL = null, Name = null, Username = null, Password = null, Note = null;
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

        public static void AddToTable(ref ListView _lv, string _uniqueID, ref Class.VTAccount _vta)
        {
            _lv.Items.Add(_uniqueID).SubItems.AddRange(new String[] { _vta.SteamURL, _vta.Name, _vta.Username, _vta.Banned.ToString(), "", _vta.Note });
        }

    }
}