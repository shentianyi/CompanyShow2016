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
    /// SetIdWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetIdWindow : Window
    {

        PTLCanController.Controller controller;
        public SetIdWindow()
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

        private void SetIdtextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SetIdbutton_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(OldtextBox.Text))
            {
                MessageBox.Show("请输入灯目前编号");
                return;
            }
            if (string.IsNullOrEmpty(SetIdtextBox.Text))
            {
                MessageBox.Show("请输入目标灯编号");
                return;
            }
            int OldLampId = int.Parse(OldtextBox.Text);
            int LampId = int.Parse(SetIdtextBox.Text);
            controller.Set(OldLampId,LampId);
            if (Settings.Default.sendSleep > 0)
            {
                Thread.Sleep(Settings.Default.sendSleep);
            }
            controller.Send(LampId, 9876, "FFFFFF");

            //string tempa = string.Empty;          
            //MessageBoxResult MSBOXRE = MessageBox.Show("是否显示数字为9876,颜色为白色", "测试结果", MessageBoxButton.YesNo);
            //if (MSBOXRE == MessageBoxResult.Yes)
            //{
            //    tempa = "设置成功";

            //}
            //else
            //{
            //    tempa = "设置失败";
            //}

            //MessageBox.Show(tempa);

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

        private void OldtextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
