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
    /// ColorTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ColorTestWindow : Window
    {
        PTLCanController.Controller controller;
        public ColorTestWindow()
        {
            InitializeComponent();
            this.Closing += Window_Closing;
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

        private void Colorbutton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(IdtextBox.Text))
            {
                MessageBox.Show("请输入灯编号");
                return;
            }
            int LampId = int.Parse(IdtextBox.Text);
            string tempa = string.Empty;
            controller.Send(LampId, 0, "0000FF");
            MessageBoxResult RRE = MessageBox.Show("是否显示颜色为红色", "颜色测试确认", MessageBoxButton.YesNo);
            if (RRE == MessageBoxResult.Yes)
            {
                controller.Send(LampId, 0, "00FF00");

                MessageBoxResult GRE = MessageBox.Show("是否显示颜色为绿色", "颜色测试确认", MessageBoxButton.YesNo);

                if (GRE == MessageBoxResult.Yes)
                {
                    controller.Send(LampId, 0, "FF0000");

                    MessageBoxResult BRE = MessageBox.Show("是否显示颜色为蓝色", "颜色测试确认", MessageBoxButton.YesNo);
                    if (BRE == MessageBoxResult.Yes)
                    {
                        tempa = "颜色测试通过";
                    }
                    else
                    {
                        tempa = "颜色测试未通过";
                    }
                }
                else
                {
                    tempa = "颜色测试未通过";
                }

            }

            else
            {
                tempa = "颜色测试未通过";
            }


            MessageBox.Show(tempa);

        }

        private void IdtextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

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
