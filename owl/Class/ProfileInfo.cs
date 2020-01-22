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
        public Dictionary<string, Class.Account> Profiles = null; //new Dictionary<string, Class.Account>();
        public string LastAccountLoggedIn = null;
    }
}
