using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Brilliantech.Framwork.Utils.LogUtil;
using PTLCanClient.Properties;
using System.Threading;

namespace PTLCanClient
{
    // 亮灯模式
    public enum LightMode
    {
        Sigle,
        Multi
    }

    // 灯的类
    public class Light
    {
        public int id { get; set; }
        public int number { get; set; }
        public string rgbColor { get; set; }
    }
    /// <summary>
    /// LightWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LightWindow : Window
    {
        PTLCanController.Controller controller;
        public LightMode lightMode;

        public LightWindow()
        {
            InitializeComponent();
            // 设置默认值
            lightMode = LightMode.Sigle;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            initLightTB();

            // 初始化界面
            if (this.lightMode == LightMode.Sigle)
            {
                tipLabel.Content = "请扫描或输入物料SKU：";
            }
            else
            {
                tipLabel.Content = "请扫描或输入择货单号：";
            }

            // 初始化com，出错需提示，防止崩溃和处理问题
            try
            {
                controller = new PTLCanController.Controller(Settings.Default.com);
                controller.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 清空输入按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cleanBtn_Click(object sender, RoutedEventArgs e)
        {
            initLightTB();
        }


        /// <summary>
        /// 亮灯按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            this.LightUp();
        }

        /// <summary>
        /// 输入框按键释放，判断是否是回车
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scanText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                /// 这边先判断是否空不交给validate，
                /// 因为如果弹框提示了，用户敲击回车关闭提示框可能会不断触发这里的事件，不断弹窗
                /// ## 是否有个方式可以交个validate，同时避免弹框？
                if (!string.IsNullOrEmpty(this.scanText.Text))
                {
                    this.LightUp();
                }
            }
        }

        /// <summary>
        /// 亮灯
        /// </summary>
        private void LightUp()
        {
            try
            {
                if (this.validateInput())
                {
                    foreach (Light light in getLights())
                    {
                        controller.Send(light.id, light.number, light.rgbColor);

                        if (Settings.Default.sendSleep > 0)
                        {
                            Thread.Sleep(Settings.Default.sendSleep);
                        }
                    }

                    // 是否自动清空输入框
                    if (Settings.Default.autoCleanInput)
                    {
                        this.initLightTB();
                    }
                }
            }
            catch (Exception ex)
            {
                // 写入日志
                LogUtil.Logger.Error(ex.Message, ex);
            }
        }


        /// <summary>
        /// 获取需要亮的灯
        /// </summary>
        /// <returns></returns>
        private List<Light> getLights()
        {
            List<Light> lights = new List<Light>();
            try
            {
                if (this.lightMode == LightMode.Sigle)
                {
                    Light l = new Light()
                    {
                        number = new Random().Next(0, 9999),
                    };
                    if (scanText.Text.Contains(';'))
                    {
                        string[] arr = scanText.Text.Split(';');
                        l.id = int.Parse(arr[0]);
                        l.rgbColor = arr[1];
                    }
                    else
                    {
                        l.id = int.Parse(scanText.Text);
                        l.rgbColor = Settings.Default.singleLightColor;
                    }
                    lights.Add(l);
                }
                else
                {
                    if (scanText.Text.Contains(';'))
                    {
                        string[] arr = scanText.Text.Split(';');
                        foreach (var id in arr[0].Split(','))
                        {
                            lights.Add(new Light()
                            {
                                id = int.Parse(id),
                                number = new Random().Next(0, 9999),
                                rgbColor = arr[1]
                            });
                        }
                    }
                    else {
                        foreach (var id in Settings.Default.multiLights.Split(','))
                        {
                            lights.Add(new Light()
                            {
                                id = int.Parse(id),
                                number = new Random().Next(0, 9999),
                                rgbColor = Settings.Default.singleLightColor
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Logger.Info(ex.Message);
            }
            return lights;
        }

        
        /// <summary>
        ///  验证输入
        /// </summary>
        /// <returns></returns>
        public bool validateInput()
        {
            if (string.IsNullOrEmpty(this.scanText.Text))
            {
                MessageBox.Show("请扫描或输入!");
                initLightTB();
                return false;
            }
            else
            {
                if (this.lightMode == LightMode.Sigle && !this.scanText.Text.Contains(';'))
                {
                    int id = 0;
                    if(!int.TryParse(this.scanText.Text,out id))
                    {
                        MessageBox.Show("输入错误，请重新输入");
                        initLightTB();
                        return false;
                    }
                }
            }
            return true;
        }
        
        /// <summary>
        /// 设置输入框
        /// </summary>
        private void initLightTB()
        {
            this.scanText.Text = string.Empty;
            this.scanText.Focus();
        }

        /// <summary>
        /// 窗口关闭事件，关闭com
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        private void scanText_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
