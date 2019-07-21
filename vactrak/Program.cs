using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace vactrak
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            // Initialize config
            if (File.Exists(Globals.Info.cfgPath))
                Globals.Config = Utils.VTConfig.LoadConfig(Globals.Info.cfgPath);
            else
            {
                MessageBox.Show("VACTrak couldn't load a config because there's none. VACTrak will now generate a new config file containing the default settings.", "Config missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Globals.Config = new Class.VTConfig();
                Utils.VTConfig.SaveConfig(ref Globals.Config, Globals.Info.cfgPath);
            }

            // ===================
            // Check config values
            // ===================

            // Steam Path
            if (!Directory.Exists(Globals.Config.steamPath))
            {
                MessageBox.Show("The path: \"" + Globals.Config.steamPath + "\" is an invalid steam path.\n\nPlease point VACTrak to the correct steam directory.", "Config error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                using (FolderBrowserDialog _fbd = new FolderBrowserDialog())
                {
                    while (!File.Exists(_fbd.SelectedPath + "\\Steam.exe"))
                        if (_fbd.ShowDialog(null) == DialogResult.Cancel) return;

                    Globals.Config.steamPath = _fbd.SelectedPath.Replace('\\', '/'); // Turn C:\directory to C:/directory
                    Utils.VTConfig.SaveConfig(ref Globals.Config, Globals.Info.cfgPath);
                }
            }


#if DEBUG
            Debug.Print(String.Format("Config Content ({0}): {1}", Globals.Info.cfgPath, JsonConvert.SerializeObject(Globals.Config)));
#endif

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new main());
        }
    }
}
