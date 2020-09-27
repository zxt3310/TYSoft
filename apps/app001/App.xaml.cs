using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace app001
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnLoadCompleted(NavigationEventArgs e)
        {
            base.OnLoadCompleted(e);
            this.MainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            this.MainWindow.Left = 0;
            this.MainWindow.Top = 0;
        }
        
    }
}
