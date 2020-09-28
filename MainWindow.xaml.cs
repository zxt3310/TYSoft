using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Newtonsoft.Json;
using System.Dynamic;

namespace TYManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr child;

        static int a, b = 0;
        public MainWindow()
        {
            InitializeComponent();
            this.SizeChanged += new SizeChangedEventHandler(OnResize);
        }
        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            this.LoadExe();
        }

        private async void requestBtn_Click(object sender, RoutedEventArgs e)
        {
           
                BaiduNetClient baiduNetClient = new BaiduNetClient();
                //baiduNetClient.BaseUrl = @"www.ranknowcn.com";
                baiduNetClient.sucNotifer += reqSuc;
                baiduNetClient.failNotifer += reqFail;
                baiduNetClient.HttpGetReq(@"webservices/gofish/get.php?a=" + a.ToString() + @"&b=" + b.ToString(), null);
               // baiduNetClient.HttpGetReq(@"updates/files/RankAllSetup.exe", null);
                a++; b++;
           
        }

        private void reqSuc(dynamic res)
        {
            
            Console.WriteLine(res);
        }

        private void reqFail(string res)
        {

        }

        public const uint WS_CHILD = 0x40000000, WS_POPUP = 0x80000000;
        public const int WS_MAX = 0x01000000;
        public const int GWL_STYLE = (-16);
        
        public const uint WS_CAPTION = 0x00C00000, WS_OVERLAPPED = 0x00000000, WS_SYSMENU = 0x00080000, WS_THICKFRAME = 0x00040000, WS_MAXIMIZEBOX = 0x00010000, WS_MINIMIZEBOX = 0x00020000;
        public const uint WS_CHILDWINDOW = 0x40000000;
        public const uint WS_CLIPCHILDREN = 0x02000000;

        public const uint WS_EX_MDICHILD = 0x00000040;

        public const uint SWP_SHOWWINDOW = 0x0040, SWP_NOSIZE = 0x0001, SWP_NOZORDER = 0x0004, SWP_FRAMECHANGED = 0x0020, SWP_NOMOVE = 0x0002;

        /*        private static int GWL_EXSTYLE = -20;

                private static UInt32 WS_EX_DLGMODALFRAME = 0x00000001;
                private static UInt32 WS_EX_WINDOWEDGE = 0x00000100;
                private static UInt32 WS_EX_CLIENTEDGE = 0x00000200;
                private static UInt32 WS_EX_STATICEDGE = 0x00020000;*/

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
        private void LoadExe()
        {

            string exePath = @"..\..\apps\app001\bin\Debug\app001.exe";//exe存放地址;
            //string exePath = @"C:\Program Files (x86)\Tencent\WeChat\WeChat.exe";
            if (!System.IO.File.Exists(exePath))
            {
                return;
            }
            string AppName = exePath.Substring(exePath.LastIndexOf("\\")).Replace("\\", "").Replace(".exe", "");
            if (System.Diagnostics.Process.GetProcessesByName(AppName).Count() > 0)
            {
                System.Diagnostics.Process.GetProcessesByName(AppName)[0].Kill();//结束正在运行的exe
            }
            System.Diagnostics.Process process = new System.Diagnostics.Process(); //从新开启exe
            process.StartInfo = new System.Diagnostics.ProcessStartInfo(exePath)
            {
                /*CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden*/
            };
            process.Start();

            while(FindWindow(null,"子程序") == IntPtr.Zero)
            {
                Thread.Sleep(10);
            }

            child = FindWindow(null, "子程序");

            IntPtr main = new WindowInteropHelper(Window.GetWindow(this.testPanel)).Handle;
            
            uint myStyle = GetWindowLong(child, GWL_STYLE);
            //uint myExSyle = GetWindowLong(child, GWL_EXSTYLE);
            myStyle &= ~(WS_POPUP | WS_CAPTION | WS_THICKFRAME | WS_SYSMENU | WS_MINIMIZEBOX | WS_MAXIMIZEBOX | WS_CLIPCHILDREN);

            
            //myExSyle &= ~(WS_EX_DLGMODALFRAME | WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE | WS_EX_STATICEDGE);

            SetWindowLong(child, GWL_STYLE, myStyle | WS_CHILD);//修改启动程序的窗口样式为无边框
            // SetWindowLong(child, GWL_EXSTYLE, myExSyle);
            SetParent(child, main);//设置与主程序关联
            MoveWindow(child, 0, 0, (int)testPanel.ActualWidth, (int)testPanel.ActualHeight, true);//嵌入到主程序，并设置窗体位置和大小
        }

        public void OnResize(object s, SizeChangedEventArgs e)
        {
            MoveWindow(child, 0, 0, (int)testPanel.ActualWidth *2, (int)testPanel.ActualHeight*2, true);//嵌入到主程序，并设置窗体位置和大小
        }
    }

    public class JsonObj
    {
        public int a { get; set; }
        public int b { get; set; }
    }
}
