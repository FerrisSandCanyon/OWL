using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace owl.Class
{
    public class AppUpdate
    {
        public AppUpdate(string _updateInfoLink = "", bool checkNow = false)
        {
            this.updateInfoLink = _updateInfoLink;

            if (checkNow)
                this.Check(this.updateInfoLink);

        }

        public int    vMajor = 0,
                      vMinor = 0,
                      vPatch = 0;
        public string dlLink = null,
                      desc   = null;

        [JsonIgnore]
        private string updateInfoLink = "";

        public void Check(string _updateInfoLink = "")
        {

        }

        public bool IsNewer()
        {
            if (this.vMajor > Globals.Info.vMajor || this.vMinor > Globals.Info.vMinor || this.vPatch > Globals.Info.vPatch)
                return true;

            return false;
        }
    }
}
