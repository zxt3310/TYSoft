using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace TYManager
{
    class AppNest
    {
        public const int GWL_STYLE = (-16);
        public const int GWL_EXSTYLE = (-20);

        public const uint WS_POPUP = 0x80000000,WS_CHILDWINDOW = 0x40000000;
        public const uint WS_CAPTION = 0x00C00000, WS_OVERLAPPED = 0x00000000, WS_SYSMENU = 0x00080000, WS_THICKFRAME = 0x00040000, WS_MAXIMIZEBOX = 0x00010000, WS_MINIMIZEBOX = 0x00020000;
        public const uint WS_CLIPCHILDREN = 0x02000000;
        public const uint WS_EX_MDICHILD = 0x00000040;
        public const uint SWP_SHOWWINDOW = 0x0040, SWP_NOSIZE = 0x0001, SWP_NOZORDER = 0x0004, SWP_FRAMECHANGED = 0x0020, SWP_NOMOVE = 0x0002;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll")]
        public static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint newLong);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndlnsertAfter, int X, int Y, int cx, int cy, uint Flags);
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern System.IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        public static extern bool MoveWindow(System.IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        public string AppPath { get; set; }
        public string WindowName { get; set; }

        private IntPtr Host;
        public AppNest(string exePath,string windowName,IntPtr hwnd)
        {
            this.AppPath = exePath;
            this.WindowName = windowName;
            Host = hwnd;
        }

        public void Run()
        {
            if (this.AppPath.Length == 0)
            {
                return;
            }

            if (!System.IO.File.Exists(this.AppPath))
            {
                return;
            }
            string AppName = this.AppPath.Substring(this.AppPath.LastIndexOf("\\")).Replace("\\", "").Replace(".exe", "");
            if (Process.GetProcessesByName(AppName).Count() > 0)
            {
                Process.GetProcessesByName(AppName)[0].Kill();//结束正在运行的exe
            }
            ProcessStartInfo info = new ProcessStartInfo(AppName);
            Process.Start(info);

            while (FindWindow(null, this.WindowName) == IntPtr.Zero)
            {
                Thread.Sleep(10);
            }

            IntPtr child = FindWindow(null, this.WindowName);
            SetParent(child, Host);//设置与主程序关联

            uint style = GetWindowLong(child, GWL_STYLE);
            style &= ~(WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX | WS_CLIPCHILDREN) | WS_CHILDWINDOW;
            SetWindowLong(child, GWL_STYLE, style);//修改启动程序的窗口样式为无边框

            
        }
    }
}
