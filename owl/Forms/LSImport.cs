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

namespace owl.Forms
{
    public partial class LSImport : Form
    {
        // SAG Account class template for de/serialization
        public class SAGAccount
        {
            public string login, password, email, steamid;
        };

        // Dictates the import source
        // 0 = Firefox
        // 1 = Google Chrome
        private int    import_mode    = -1;
        private string  import_ff_dir = null;

        // Cached list of existing username for efficiency
        private List<string> CachedExistingUser = new List<string> { };

        public LSImport(string sender_name)
        {
            switch (sender_name)
            {
                case "ddAccountImportSAGFF": import_mode = 0; break;
                case "ddAccountImportSAGGC": import_mode = 1; break;

                default:
                    MessageBox.Show("Unknown import source!", "SAG Local Storage Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
            }

            // Initialize the list of existing user
            foreach (KeyValuePair<string, Class.Account> _value in Globals.CurrentProfile)
                CachedExistingUser.Add(_value.Value.Username);

            InitializeComponent();
        }

        private void LSImport_Load(object sender, EventArgs e)
        {
            // ==============
            // Initialization
            // ==============

            switch (import_mode)
            {
                case 0: // Firefox
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
                            _reader.Read();

                            // Add it to the table
                            foreach (SAGAccount _account in JsonConvert.DeserializeObject<List<SAGAccount>>(_reader["value"].ToString()))
                            {
                                // Check if we already imported that account | LINQ is slow but who cares? not like this software requires fast performance
                                if (Globals.CurrentProfile.Values.Where(x => x.Username == _account.login).Count() != 0) continue;

                                ListViewItem _lvi = new ListViewItem("profile/" + _account.steamid);
                                _lvi.SubItems.AddRange(new string[] { _account.login, _account.password });
                                lvImports.Items.Add(_lvi);
                            }

                            

                            _sql_conn.Close();
                        }

                        break;
                    }

                case 1: // Google Chrome
                    break;
            }
        }
    }
}
