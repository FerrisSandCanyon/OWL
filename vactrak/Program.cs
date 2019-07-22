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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Check for the profile folder
            if (!Directory.Exists(Globals.Info.profilesPath)) Directory.CreateDirectory(Globals.Info.profilesPath);

            // Initialize config
            if (File.Exists(Globals.Info.cfgPath))
                Globals.Config = Utils.VTConfig.Load(Globals.Info.cfgPath);
            else
            {
                MessageBox.Show("VACTrak couldn't load a config because there's none. VACTrak will now generate a new config file containing the default settings.", "Config initialization", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Globals.Config = new Class.VTConfig();
                if (!Utils.VTConfig.Save(ref Globals.Config, Globals.Info.cfgPath)) return;
            }

            if (Globals.Config == null)
            {
                MessageBox.Show("Config was unsuccessfuly initialized.", "Config initialization", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            #if DEBUG
                Debug.WriteLine(String.Format("Config Content on load ({0}): {1}", Globals.Info.cfgPath, JsonConvert.SerializeObject(Globals.Config)));
            #endif

            // ===================
            // Check config values
            // ===================
            bool shouldResave = false;

            // Default profile
            if (String.IsNullOrWhiteSpace(Globals.Config.defaultProfile))
            {
                MessageBox.Show("A blank default profile is an invalid parameter. This parameter has been set back to default", "Config error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Globals.Config.defaultProfile = "default";
                shouldResave = true;
            }

            // Steam Path
            if (!Directory.Exists(Globals.Config.steamPath))
            {
                MessageBox.Show("The path: \"" + Globals.Config.steamPath + "\" is an invalid steam path.\n\nPlease point VACTrak to the correct steam directory.", "Config error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                using (FolderBrowserDialog _fbd = new FolderBrowserDialog())
                {
                    while (!File.Exists(_fbd.SelectedPath + "\\Steam.exe"))
                        if (_fbd.ShowDialog() == DialogResult.Cancel) return;

                    Globals.Config.steamPath = _fbd.SelectedPath.Replace('\\', '/');
                    shouldResave = true;
                }
            }

            // Resave the config if it was automatically modified due to user errors
            if (shouldResave) if (!Utils.VTConfig.Save(ref Globals.Config, Globals.Info.cfgPath)) return;

            #if DEBUG
                Debug.WriteLine(String.Format("Config Content after check ({0}): {1}\nShould Resave: ", Globals.Info.cfgPath, JsonConvert.SerializeObject(Globals.Config), shouldResave ? "yes" : "no"));
            #endif

            Application.Run(new FormMain());
        }
    }
}
