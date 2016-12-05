using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Brilliantech.Framwork.Utils.LogUtil;
using System.Threading;
using PTLTest.Properties;

namespace PTLTest
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

        private void Testbutton_Click(object sender, RoutedEventArgs e)
        {
            OffTestWindow OffTestWin = new OffTestWindow();
            OffTestWin.ShowDialog(); 
           


        }

        private void SetIdbutton_Click(object sender, RoutedEventArgs e)
        {
            SetIdWindow SetWin = new SetIdWindow();
            SetWin.ShowDialog();
        }

        private void Checkbutton_Click(object sender, RoutedEventArgs e)
        {
            CheckIdWindow CheckWin = new CheckIdWindow();
            CheckWin.ShowDialog();
        }

        private void NumShowbutton_Click(object sender, RoutedEventArgs e)
        {
            NumTestWindow NumWin = new NumTestWindow();
            NumWin.ShowDialog();
        }

        private void ColorShowbutton_Click(object sender, RoutedEventArgs e)
        {
            ColorTestWindow ColorWin = new ColorTestWindow();
            ColorWin.ShowDialog();
        }

        private void Stabilitybutton_Click(object sender, RoutedEventArgs e)
        {
            StabilityTestWindow StabilityWin = new StabilityTestWindow();
            StabilityWin.ShowDialog();
        }
    }
}
