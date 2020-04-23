using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace owl.Forms
{
    public partial class ClickLogin : Form
    {
        private Class.Account      account    = null;
        PInvoke.winuser.WINDOWINFO windowinfo = new PInvoke.winuser.WINDOWINFO();
        Point                      curpos;
        IntPtr                     hSteamWnd_ = IntPtr.Zero;
        Thread                     thWorker;

#if DEBUG
        Thread dbgThread; // Handle to the debug display thread
#endif

        private string status
        {
            set
            {
                if (lblStatus.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        lblStatus.Text = value;
                    }));
                }
                else
                {
                    lblStatus.Text = value;
                }
            }
        }

        private bool isSteamRunning
        {
            get
            {
                return Process.GetProcessesByName("Steam").Count() > 0;
            }
        }

        private IntPtr hSteamWnd
        {
            get 
            {
                return PInvoke.winuser.FindWindow("vguiPopupWindow", "Steam Login");
            }
        }

        public ClickLogin(ref Class.Account _account)
        {
            account = _account;
            InitializeComponent();
            windowinfo.cbSize = (uint)Marshal.SizeOf(windowinfo);

#if !DEBUG
            lbldbg.Visible = false;
            this.Height = 96;
#endif
        }

        private void WorkerThread()
        {
            status = "Init Thread...";
            int count = 0;

            // Wait for the steam process
            while (!isSteamRunning)
            {
                status = $"Waiting for steam process... {count++}";
                Thread.Sleep(500);
            }

            count = 0; // Reset counter

            // Wait for the login form
            while (hSteamWnd == IntPtr.Zero)
            {
                status = $"Waiting for steam login window... {count++}";
                Thread.Sleep(500);
            }

            status = "Ready...";

            // Track user input and window while making sure steam and the login form is still present
            while (isSteamRunning && (hSteamWnd_ = hSteamWnd) != IntPtr.Zero)
            {
                Thread.Sleep(1);

                if (PInvoke.winuser.GetForegroundWindow() != hSteamWnd_)
                    continue;
                
                if (!PInvoke.winuser.GetWindowInfo(hSteamWnd_, ref windowinfo))
                    continue;
                
                curpos = Cursor.Position;

                this.Invoke(new Action(() =>
                {
                    this.Location = new Point(
                        (int)(windowinfo.rcWindow.left - windowinfo.cxWindowBorders + 1),
                        (int)(windowinfo.rcWindow.top + (windowinfo.rcWindow.bottom - windowinfo.rcWindow.top) + 6)
                        );
                }));
            }

            status = "Sending close...";

            this.Invoke(new Action(() =>
            {
                this.Close();
            }));
        }

        private void ClickLogin_Load(object sender, EventArgs e)
        {
            status = "Initializing...";
            
            lblAcc.Text += account.Username; // Setup account label
            thWorker = new Thread(WorkerThread);
            thWorker.Start();

#if DEBUG
            dbgThread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(500);
                    this.Invoke(new Action(() =>
                    {
                        lbldbg.Text = String.Format(
                        "steam hwnd:{0}\nsteam pos: {1}, {2} cursor pos: {3}, {4}\ndebug info: {5}",
                        this.hSteamWnd_.ToString(), windowinfo.rcWindow.left.ToString(), windowinfo.rcWindow.top.ToString(), curpos.X.ToString(), curpos.Y.ToString(), "");
                    }));
                }
            });

            dbgThread.Start();
#endif
        }

        private void ClickLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            status = "Closing...";

            thWorker.Abort();

#if DEBUG
            dbgThread.Abort();
#endif
        }
    }
}
