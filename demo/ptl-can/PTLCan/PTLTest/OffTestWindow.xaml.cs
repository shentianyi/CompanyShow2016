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
using System.Windows.Shapes;
using Brilliantech.Framwork.Utils.LogUtil;
using System.Threading;
using PTLTest.Properties;

namespace PTLTest
{
    /// <summary>
    /// OffTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OffTestWindow : Window
    {
        PTLCanController.Controller controller;
        public OffTestWindow()
        {
            InitializeComponent();
            this.Closing += Window_Closing;
            // 初始化com，出错需提示，防止崩溃和处理问题
            try
            {
                controller = new PTLCanController.Controller(Settings.Default.com);
                controller.Open();
                // MessageBox.Show("打开端口成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OffConnTestbutton_Click(object sender, RoutedEventArgs e)
        {
            string mean = string.Empty;
            controller.Send(0, 2, "0000FF");
            if (Settings.Default.sendSleep > 0)
            {
                Thread.Sleep(Settings.Default.sendSleep);
            }

            MessageBoxResult MSBOXRE = MessageBox.Show("是否显示数字为0002,颜色为红色", "测试结果", MessageBoxButton.YesNo);
            if (MSBOXRE == MessageBoxResult.Yes)
            {
                mean = "连通测试通过";
                
            }
            else
            {
                mean = "连通测试未通过";
            }

            MessageBox.Show(mean);


        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.controller.Close();

            }
            catch (Exception ex)
            {
                LogUtil.Logger.Error(ex.Message, ex);

                MessageBox.Show(ex.Message);
            }
        }
    }
}
