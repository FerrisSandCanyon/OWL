using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace owl.Class
{
    public class AppUpdate
    {
        public int    vMajor = 0,
                      vMinor = 0,
                      vPatch = 0;
        public string dlLink = null,
                      desc   = null;

        public bool IsNewer()
        {
            if (this.vMajor > Globals.Info.vMajor || this.vMinor > Globals.Info.vMinor || this.vPatch > Globals.Info.vPatch)
                return true;

            return false;
        }
    }
}

namespace owl.Utils
{
    public static class AppUpdate
    {
        public static void Check(ref Class.AppUpdate _au, string updateInfoLink = null)
        {
            using (WebClient _wc = new WebClient())
            {
                _au = JsonConvert.DeserializeObject<Class.AppUpdate>(_wc.DownloadString(updateInfoLink));
            }
        }

        public static void CheckUpdateWrapper(bool silentUpdate = false)
        {
            /*
            Thread _thread = new Thread(new ThreadStart(() =>
            {
                Utils.AppUpdate.Check(ref Globals.AppUpdateInfo, Globals.Info.updateInfoLink);

                if (Globals.AppUpdateInfo.IsCurrent())
                {
                    MessageBox.Show("You're currently running the newest released version!", "Update Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (Globals.AppUpdateInfo.IsNewer()
                &&  MessageBox.Show($"A newer version of OWL is available! version {Globals.AppUpdateInfo.vMajor}.{Globals.AppUpdateInfo.vMinor}.{Globals.AppUpdateInfo.vMinor}\n\n{Globals.AppUpdateInfo.desc}\n\nWould you like to open the download link: {Globals.AppUpdateInfo.dlLink}?", "Update available", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    Process.Start(Globals.AppUpdateInfo.dlLink);
                }
            }));

            if (!_thread.IsAlive)
                _thread.Start();
           */

            Utils.AppUpdate.Check(ref Globals.AppUpdateInfo, Globals.Info.updateInfoLink);

            if (Globals.AppUpdateInfo.IsNewer())
            {
                if (MessageBox.Show($"A newer version of OWL is available! version {Globals.AppUpdateInfo.vMajor}.{Globals.AppUpdateInfo.vMinor}.{Globals.AppUpdateInfo.vMinor}\n\n{Globals.AppUpdateInfo.desc}\n\nWould you like to open the download link: {Globals.AppUpdateInfo.dlLink}?", "Update available", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    Process.Start(Globals.AppUpdateInfo.dlLink);
            }
            else
            {
                if (!silentUpdate)
                    MessageBox.Show("You're currently running the newest released version!", "Update Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }
    }
}