using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace owl.Forms
{
    public partial class ClickLogin : Form
    {
        private Class.Account account = null;

        IntPtr hSteamWnd      = IntPtr.Zero;

        private string status
        {
            set
            {
                lblStatus.Text = value;
            }
        }

        private bool isSteamRunning
        {
            get
            {
                return Process.GetProcessesByName("Steam").Count() > 0;
            }
        }

        public ClickLogin(ref Class.Account _account)
        {
            account = _account;
            InitializeComponent();
        }

        private void ClickLogin_Load(object sender, EventArgs e)
        {
            lblAcc.Text += account.Username; // Setup account label

            while (true)
            {
                int chkTries = 0;
                status = "Initializing...";

                // Wait for steam
                while (!isSteamRunning)
                {
                    status = $"Checking steam process... {chkTries++.ToString()}";
                    Thread.Sleep(500);
                }

                chkTries = 0; // Reset back to 0

                // Get window handle
                while (hSteamWnd == IntPtr.Zero)
                {
                    status = $"Finding window handle... {chkTries++.ToString()}";
                    hSteamWnd = PInvoke.winuser.FindWindow("vguiPopupWindow", "Steam Login");

                    if (!isSteamRunning)
                        continue;
                }    
            }

        }
    }
}
