using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;

namespace owl
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        [STAThread]
        static void Main()
        {
            if (!Initialize())
            {
                MessageBox.Show("Initialization failed!", "OWL", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(Globals.hFormMain = new FormMain());
        }

        static bool Initialize()
        {
            bool shouldResave = false;

            // Check for the profile folder if it doesnt exist already
            Directory.CreateDirectory(Globals.Info.profilesPath);

            // Initialize config
            if (File.Exists(Globals.Info.cfgPath))
            {
                Globals.Config = Utils.Config.Load(Globals.Info.cfgPath);
            }
            else
            {
                MessageBox.Show("OWL couldn't find any existing config. default config will be generated and used in this session", "Config initialization", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Globals.Config = new Class.Config();
                shouldResave = true;
            }

            if (Globals.Config == null)
            {
                MessageBox.Show("Error: Config was unsuccessfully loaded.", "Config initialization", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

#if DEBUG
            Debug.WriteLine($"Config Content on load ({Globals.Info.cfgPath}): {JsonConvert.SerializeObject(Globals.Config)}");
#endif
            
            // ===================
            // Check config values
            // ===================

            // Default profile
            if (String.IsNullOrWhiteSpace(Globals.Config.defaultProfile))
            {
                MessageBox.Show("Invalid defaultprofile parameter. this parameter value has been set back to default", "Config warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Globals.Config.defaultProfile = "default";
                shouldResave = true;
            }
            
            // Steam Path
            if (!Directory.Exists(Globals.Config.steamPath))
            {
                MessageBox.Show($"Steam folder: \"{Globals.Config.steamPath}\" doesnt exist.\n\nPlease select a correct Steam directory.", "Config warning", MessageBoxButtons.OK, MessageBoxIcon.Error);

                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    while (!File.Exists(Path.Combine(fbd.SelectedPath, "Steam.exe")))
                    {
                        if (fbd.ShowDialog() == DialogResult.Cancel)
                            return false;
                    }

                    Globals.Config.steamPath = fbd.SelectedPath.Replace('\\', '/');
                    shouldResave = true;
                }
            }

            // Resave the config if it was automatically modified due to errors
            if (shouldResave)
            {
                if (!Utils.Config.Save(ref Globals.Config, Globals.Info.cfgPath))
                    return false;
            }
            
            #if DEBUG
                Debug.WriteLine(String.Format("Config Content after check ({0}): {1}\nShould Resave: ", Globals.Info.cfgPath, JsonConvert.SerializeObject(Globals.Config), shouldResave ? "yes" : "no"));
            #endif
            
            Debug.WriteLine("The window was " + (PInvoke.winuser.FindWindow("vguiPopupWindow", "Steam Login") == IntPtr.Zero ? "not" : "") + " found.");

            return true;
        }
    }
}
