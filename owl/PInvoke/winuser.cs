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
            long left;
            long top;
            long right;
            long bottom;
        };

        // https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-windowinfo
        public struct WINDOWINFO
        {
            UInt32 cbSize;
            RECT rcWindow;
            RECT rcClient;
            UInt32 dwStyle;
            UInt32 dwExStyle;
            UInt32 dwWindowStatus;
            uint cxWindowBorders;
            uint cyWindowBorders;
            UInt16 atomWindowType; // https://stackoverflow.com/questions/10525511/what-is-the-atom-data-type
            UInt16 wCreatorVersion;
        };


        // https://www.pinvoke.net/default.aspx/user32/getwindowinfo.html
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

        // https://www.pinvoke.net/default.aspx/user32.FindWindow
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindow(IntPtr ZeroOnly, string lpWindowName);
    }
}
