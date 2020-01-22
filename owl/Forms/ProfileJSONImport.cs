using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace owl.Forms
{
    public partial class ProfileJSONImport : Form
    {
        private List<ImportedJSON> Imports = new List<ImportedJSON> { };

        public ProfileJSONImport() { InitializeComponent(); }

        public class DeserializeTemplate
        {
            public int vProfileFormat = -1;
        }

        public class ImportedJSON
        {
            public ImportedJSON(ref ListViewItem _LVI, string _FilePath = "", string _RawJSON = "", int _vProfile = 0)
            {
                LVI      = _LVI;
                FilePath = _FilePath;
                RawJSON  = _RawJSON;
                vProfile = _vProfile;
            }

            public ListViewItem LVI       = null;
            public string       FilePath  = null,
                                RawJSON   = null;
            public int          vProfile  = 0;

            public Class.ProfileInfo Result = null;

            public void SetStatus(string status)
            {
                this.LVI.SubItems[2].Text = status;
            }
        }

        private void ProfileJSONImport_Load(object sender, EventArgs e)
        {
            this.Text += " | Current Profile Version: " + Globals.Info.vProfileFormat;
        }

        private void ConvertImports()
        {
            foreach (ImportedJSON _ijson in Imports)
            {
                if (_ijson.vProfile >= Globals.Info.vProfileFormat)
                {
                    _ijson.SetStatus("Invalid version. Skipped!");
                    continue;
                }

                _ijson.SetStatus("Converting...");

                try
                {
                    switch (_ijson.vProfile)
                    {
                        case -1: // OWL v2.1 and below
                        {
                            _ijson.Result = new Class.ProfileInfo(JsonConvert.DeserializeObject<Dictionary<string, Class.Account>>(_ijson.RawJSON));
                            _ijson.SetStatus("Converted!");
                            break;
                        }

                        default:
                        {
                            _ijson.SetStatus("Unhandled!");
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _ijson.SetStatus("Convert failed! " + ex.ToString());
                    continue;
                }
            }
        }

        private void WriteImports(bool WriteInProfiles = false)
        {
            foreach (ImportedJSON _ijson in Imports)
            {
                if (_ijson.Result == null)
                    continue;

                string writePath = WriteInProfiles ? Path.Combine(Globals.Info.profilesPath, Path.GetFileName(_ijson.FilePath)) : _ijson.FilePath;

                // If writing to profile check for existing profiles that might be overwritten
                if (WriteInProfiles
                && File.Exists(writePath)
                && MessageBox.Show($"This profile already exists in the profiles directory. Would you like to overwrite?\n\n Profile: {Path.GetFileName(writePath)}", "Overwrite file?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    continue;
                }

                try
                {
                    using (StreamWriter _sw = new StreamWriter(_ijson.FilePath, false, Encoding.UTF8))
                    {
                        _sw.Write(JsonConvert.SerializeObject(_ijson.Result));
                        _sw.Close(); //
                    }

                    _ijson.SetStatus("Converted! " + (WriteInProfiles ? "(+Import)" : "(+Write)"));
                }
                catch (Exception ex)
                {
                    _ijson.SetStatus(ex.ToString());
                }
            }
        }

        private void btnAddFiles_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog _ofd = new OpenFileDialog())
            {
                _ofd.Filter           = "OWL JSON Profile (*.json) | *.json";
                _ofd.Multiselect      = true;
                _ofd.Title            = "Import OWL JSON Profiles...";
                _ofd.InitialDirectory = Globals.Info.profilesPath;
                _ofd.ShowDialog();

                string       _content;
                ListViewItem _lvi;
                ImportedJSON _ijson;

                foreach (string _jsonFile in _ofd.FileNames)
                {
                    using (StreamReader _sr = new StreamReader(_jsonFile, System.Text.Encoding.UTF8))
                    {
                        _content = _sr.ReadToEnd();
                    }
           
                    _lvi   = new ListViewItem(_jsonFile);
                    _ijson = new ImportedJSON(ref _lvi, _jsonFile, _content, JsonConvert.DeserializeObject<DeserializeTemplate>(_content).vProfileFormat);
                   
                    Imports.Add(_ijson);
                    _lvi.SubItems.AddRange(new string[] { _ijson.vProfile.ToString(), "Imported!" });
                    lvProfiles.Items.Add(_lvi);
                }

            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            ConvertImports();
            WriteImports(false);
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            ConvertImports();
            WriteImports(true);
        }
    }
}
