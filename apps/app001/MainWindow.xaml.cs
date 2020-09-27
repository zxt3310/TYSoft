using System;
using System.Collections.Generic;
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

namespace app001
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            float R = (float) random.NextDouble(); 
            float G = (float)random.NextDouble();
            float B = (float)random.NextDouble();

            testPanel.Background = new SolidColorBrush(Color.FromScRgb(1,R,G,B));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            testPanel.Background = new SolidColorBrush(Colors.Red);
        }
    }
}
