using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace owl.PInvoke
{
    public static class winuser
    {
        // https://docs.microsoft.com/en-us/windows/win32/api/windef/ns-windef-rect
        public struct RECT
        {
            public UInt32 left;
            public UInt32 top;
            public UInt32 right;
            public UInt32 bottom;
        };

        // https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-windowinfo
        public struct WINDOWINFO
        {
            public UInt32 cbSize;
            public RECT rcWindow;
            public RECT rcClient;
            public UInt32 dwStyle;
            public UInt32 dwExStyle;
            public UInt32 dwWindowStatus;
            public uint cxWindowBorders;
            public uint cyWindowBorders;
            public UInt16 atomWindowType; // https://stackoverflow.com/questions/10525511/what-is-the-atom-data-type
            public UInt16 wCreatorVersion;
        };


        // https://www.pinvoke.net/default.aspx/user32/getwindowinfo.html
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

        // https://www.pinvoke.net/default.aspx/user32.FindWindow
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // https://www.pinvoke.net/default.aspx/user32.getforegroundwindow
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
    }
}
