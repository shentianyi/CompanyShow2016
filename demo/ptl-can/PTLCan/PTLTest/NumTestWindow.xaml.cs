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
    /// NumTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NumTestWindow : Window
    {
        PTLCanController.Controller controller;
        public NumTestWindow()
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

        private void IdtextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void NumtextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Numbutton_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(IdtextBox.Text))
            {
                MessageBox.Show("请输入灯编号");
                return;
            }

            int LampId = int.Parse(IdtextBox.Text);
            if (string.IsNullOrEmpty(NumtextBox.Text))
            {
                string mean = string.Empty;
                controller.Send(LampId, 1111, "0000FF");
                MessageBoxResult MSBOXRE = MessageBox.Show("是否显示数字为 1111", "测试结果", MessageBoxButton.YesNo);
                if (MSBOXRE == MessageBoxResult.Yes)
                {
                   
                    controller.Send(LampId, 3333, "0000FF");
                    MessageBoxResult NumRe1 = MessageBox.Show("是否显示数字为 3333", "测试结果", MessageBoxButton.YesNo);
                    if(NumRe1==MessageBoxResult.Yes)
                    {
                        mean = "数字测试通过";
                    }
                    else
                    {
                        mean = "数字测试未通过";
                    }

                }
                else
                {
                    mean = "数字测试未通过";
                }
                LogUtil.Logger.Info(mean);
                MessageBox.Show(mean);
            }
            else
            {
                int Num = int.Parse(NumtextBox.Text);
                string tempa = string.Empty;
                controller.Send(LampId, Num, "0000FF");
                MessageBoxResult MSBOXRE = MessageBox.Show("是否显示数字为"+Num, "测试结果", MessageBoxButton.YesNo);
                if (MSBOXRE == MessageBoxResult.Yes)
                {
                    tempa = "数字测试通过";
                   

                }
                else
                {
                    tempa = "数字测试未通过";
                }

                MessageBox.Show(tempa);
            }
            
            
            
            
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
