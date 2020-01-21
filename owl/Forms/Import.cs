using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace owl.Forms
{
    public partial class Import : Form
    {
        // SAG Account class template for de/serialization
        public class SAGAccount
        {
            public string login, password, email, steamid;
        };

        // Dictates the import source
        // 0 = Firefox
        // 1 = Google Chrome
        // 2 = SAC Text File
        private int      import_mode    = -1;

        private string   import_ff_dir  = null,
                         import_sac_txt = null;

        // Reference to the main list view on the main form
        private ListView lvRef         = null;

        public Import(string _info, int mode, ref ListView _lvRef)
        {
            InitializeComponent();

            switch(mode)
            {
                // Browser Import mode
                case 0:
                {
                    switch (_info)
                    {
                        case "ddAccountImportSAGFF":
                            import_mode = 0;
                            this.Text += "Firefox";
                            break;
                        case "ddAccountImportSAGGC":
                            import_mode = 1;
                            this.Text += "Google Chrome";
                            break;

                        default:
                            MessageBox.Show("Unknown import source!", "SAG Local Storage Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Close();
                            return;
                    }
                    break;
                }

                // SAC Import mode
                case 1:
                {
                    import_mode = 2;
                    import_sac_txt = _info;
                    this.Text += $"SAC | {_info}";
                    break;
                }
            }

            lvRef = _lvRef;
        }

        private void BtnSelAll_Click(object sender, EventArgs e)
        {
            if (lvImports.Items.Count == 0)
                return;

            foreach (ListViewItem _lvi in lvImports.Items)
                _lvi.Checked = true;
        }

        private void Import_Load(object sender, EventArgs e)
        {
            Import_ProperCall();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            Import_ProperCall();
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            Class.Account _acc;
            foreach (ListViewItem _lvi in lvImports.CheckedItems)
            {
                string uniqueId = Utils.Account.MakeUniqueKey();
                _acc = new Class.Account(DateTime.MinValue, uniqueId, _lvi.SubItems[0].Text, "", _lvi.SubItems[1].Text, _lvi.SubItems[2].Text, "", false);
                Globals.CurrentProfile.Profiles.Add(uniqueId, _acc);
                Utils.Account.AddToTable(ref lvRef, uniqueId, ref _acc);
            }

            this.Close();
        }

        private void Import_ProperCall()
        {
            switch (import_mode)
            {
                case 0: // Firefox
                    Import_FireFox();
                    break;

                case 1: // Google Chrome
                    Import_GoogleChrome();
                    break;

                case 2: // SAC Text File
                    Import_SAC();
                    break;
            }
        }

        private void Import_FireFox()
        {
            import_ff_dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Mozilla\Firefox\Profiles");

            // Check if import_ff_dir is pointing to a valid directory
            if (!Directory.Exists(import_ff_dir))
            {
                MessageBox.Show($"Firefox profile folder not found!\nPath: {import_ff_dir}\n\nPlease browse for the correct Firefox profile directory.", "Local Storage Import", MessageBoxButtons.OK, MessageBoxIcon.Error);

                using (FolderBrowserDialog _fbd = new FolderBrowserDialog())
                {
                    _fbd.RootFolder = Environment.SpecialFolder.ApplicationData;
                    if (_fbd.ShowDialog() == DialogResult.Cancel) this.Close();
                    import_ff_dir = _fbd.SelectedPath;
                }
            }

            lvImports.Items.Clear();

            // Parse each profile directory and connect to the local storage database
            SQLiteConnection _sql_conn;
            foreach (string _ff_profile in Directory.GetDirectories(import_ff_dir))
            {
                // Verify if profile directory contains a firefox local storage
                string _db_file = Path.Combine(_ff_profile, "webappsstore.sqlite");
                if (!File.Exists(_db_file)) continue;

                // Create and open a SQL connection
                _sql_conn = new SQLiteConnection($"Data Source=\"{_db_file}\";Version=3;");
                _sql_conn.Open();

                // Read and parse
                SQLiteDataReader _reader = (new SQLiteCommand("SELECT * FROM 'webappsstore2' WHERE key = 'genned_account'", _sql_conn).ExecuteReader());

                if (!_reader.Read())
                    continue;

                // Add it to the table
                foreach (SAGAccount _account in JsonConvert.DeserializeObject<List<SAGAccount>>(_reader["value"].ToString()))
                {
                    if (Utils.Account.AccountExists(_account.login))
                        continue;

                    ListViewItem _lvi = new ListViewItem("profiles/" + _account.steamid);
                    _lvi.SubItems.AddRange(new string[] { _account.login, _account.password });
                    lvImports.Items.Add(_lvi);
                }

                _sql_conn.Close();
            }
        }

        private void Import_GoogleChrome()
        {

        }

        // Note: i dont have any original account.txt that was generated by SAC and i cant get SAC to generate one, the info used is from the old screenshot on the repo's read me. so this might be broken
        private void Import_SAC()
        {
            /*
            Alias:        aaaaaaaaa
            Pass:         bbbbbbbbb
            Creation:     ccccccccc
            URL:          ddddddddd
            ###########################(27#)
            */

            if (!File.Exists(import_sac_txt))
            {
                MessageBox.Show($"Can't find SAC's account log file!\n\nFile: {import_sac_txt}", "OWL Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] contentBuffer = { null, null, null }; // buffer to store the parsed information (alias, pass, url). This way we dont need to create unnecessary instances of the Class.Account
            int      idxType       = -1; // stores the current line's type referencing an index in parserKeyword

            using (StreamReader _sr = new StreamReader(import_sac_txt))
            {
                int blockContCount = 0;

                while(_sr.Peek() > -1)
                {
                    if (++blockContCount > 5)
                    {
                        MessageBox.Show("Parser counter exceeded its maximum value, this may be caused by an incorrect/tampered format", "OWL SAC Parser", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    idxType = -1;
                    string lineBuffer = _sr.ReadLine();

                    if (string.IsNullOrWhiteSpace(lineBuffer))
                        continue;

                    switch(lineBuffer[0]) // Obtains the first letter of the line
                    {
                        case 'A': // Alias:
                            idxType = 0;
                            break;

                        case 'P': // Pass:
                            idxType = 1;
                            break;

                        case 'U': // URL:
                            idxType = 2;
                            break;

                        case '#': // End of block (add to import table)
                        {
                            if (string.IsNullOrWhiteSpace(contentBuffer[0])
                             || string.IsNullOrWhiteSpace(contentBuffer[1])
                             || string.IsNullOrWhiteSpace(contentBuffer[2]))
                            {
                                MessageBox.Show("Content buffer contains null or an invalid value, this may be caused by an incorrect/tampered format", "OWL SAC Parser", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            if (!Utils.Account.AccountExists(contentBuffer[0]))
                            {
                                ListViewItem _lvi = new ListViewItem("profiles/" + contentBuffer[2]);
                                _lvi.SubItems.AddRange(new string[] { contentBuffer[0], contentBuffer[1] });
                                lvImports.Items.Add(_lvi);
                            }

                            contentBuffer[0] = contentBuffer[1] = contentBuffer[2] = null; // Set our buffer back to null
                            blockContCount = 0;                                            // Reset our counter to 0
                            continue;
                        }

                        default: // Skip it if it doesn't fit what the parser is looking for
                            continue;
                    }

                    // Find the first instance of a text with a whitespace before it, no need for multiple checks since it falls to the switch's default making the index return 0 almost impossible
                    contentBuffer[idxType] = lineBuffer.Substring(Regex.Match(lineBuffer, @"\s\w").Index + 1);
                }
            }

        }

    }
}
