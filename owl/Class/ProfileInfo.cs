using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace owl.Class
{
    public class ProfileInfo
    {

        public ProfileInfo(Dictionary<string, Class.Account> _Profiles = null)
        {
            Profiles = _Profiles ?? new Dictionary<string, Class.Account> { };
        }

        public DateTime LastSaved = DateTime.Now;
        public int vProfileFormat = 1;
        public string LastAccountLoggedIn = null;

        public Dictionary<string, Class.Account> Profiles = null; //new Dictionary<string, Class.Account>();

    }
}

namespace owl.Utils
{
    public static class ProfileInfo
    {
        // Returns a dictionary of all the accounts selected in lvData
        public static Dictionary<string, Class.Account> GetSelectedItems()
        {
            Dictionary<string, Class.Account> _ret_list = new Dictionary<string, Class.Account> { };

            foreach (KeyValuePair<string, Class.Account> _acc in Globals.CurrentProfile.Profiles)
            {
                if (_acc.Value.LVI.Selected)
                    _ret_list.Add(_acc.Key, _acc.Value);
            }

            return _ret_list;
        }

    }
}