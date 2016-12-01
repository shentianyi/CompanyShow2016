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
using System.IO.Ports;
using PTLCanController;

namespace PTLTest
{
    /// <summary>
    /// CheckIdWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CheckIdWindow : Window
    {

        private SerialPort sp;

        public CheckIdWindow()
        {
            InitializeComponent();
            this.Closing += Window_Closing;

        }



        /// <summary>
        /// 接收处理数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string mean = string.Empty;
            Thread.Sleep(15);
            SerialPort sp = (SerialPort)sender;
            byte[] rdata = new byte[sp.BytesToRead];
            sp.Read(rdata, 0, rdata.Length);

            LogUtil.Logger.Info("接收数据：" + ScaleHelper.HexBytesToString(rdata));
            switch (rdata[5])
            {
                case (byte)209:
                    {
                        int LampId = rdata[3];
                        string back = string.Format("{0}{1}B0010000000000",
                       ScaleHelper.DecimalToHexString(LampId + 256, true, 8),
                       ScaleHelper.DecimalToHexString(rdata[4], true, 2));

                        byte[] bback = ScaleHelper.HexStringToHexByte(back);

                        sp.Write(bback, 0, bback.Length);
                        LogUtil.Logger.Info(string.Format("发送回复: " + ScaleHelper.HexBytesToString(bback)));
                        int ShowNum = (rdata[7] << 8 | rdata[8]);
                        MessageBoxResult MSBOXRE = MessageBox.Show("是否发送了数码管数字为" + ShowNum + "的确认指令?", "按键测试结果", MessageBoxButton.YesNo);

                        if (MSBOXRE == MessageBoxResult.Yes)
                        {
                            mean = "确认键测试通过";
                        }


                        else
                        {
                            mean = "确认键测试未通过";
                            
                            

                        }
                        LogUtil.Logger.Info(mean);
                        MessageBox.Show(mean);
                        break;

                    }
                case (byte)210:
                    {

                        int LampId = rdata[3];
                        string back = string.Format("{0}{1}B0010000000000",
                       ScaleHelper.DecimalToHexString(LampId + 256, true, 8),
                       ScaleHelper.DecimalToHexString(rdata[4], true, 2));

                        byte[] bback = ScaleHelper.HexStringToHexByte(back);

                        sp.Write(bback, 0, bback.Length);
                        LogUtil.Logger.Info(string.Format("发送回复: " + ScaleHelper.HexBytesToString(bback)));
                        int ShowNum = (rdata[7] << 8 | rdata[8]);
                        MessageBoxResult MSBOXRE = MessageBox.Show("是否发送了数码管数字为" + ShowNum + "的取消指令?", "按键测试结果", MessageBoxButton.YesNo);

                        if (MSBOXRE == MessageBoxResult.Yes)
                        {
                                mean = "取消键测试通过";
                            }

                       
                        else
                        {
                            mean = "取消键测试未通过";
                           
                        }
                        LogUtil.Logger.Info(mean);
                        MessageBox.Show(mean);
                        break;
                    }



            }
           


        }

     

        private void CheckIdbutton_Click(object sender, RoutedEventArgs e)
        {
          
            while (true)
            {
                try
                {
                    if (this.sp == null)
                    {
                        this.sp = new SerialPort(Settings.Default.com, 9600, Parity.None);
                        this.sp.Open();
                        this.sp.DataReceived += new SerialDataReceivedEventHandler(Sp_DataReceived);

                        LogUtil.Logger.Info("Open Success.....");
                    }
                    else
                    {
                        if (sp.IsOpen)
                        {
                            return;
                        }
                        else
                        {
                            this.sp.Open();
                            this.sp.DataReceived += new SerialDataReceivedEventHandler(Sp_DataReceived);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.Logger.Error("COM 打开错误", ex);
                    throw ex;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            if (sp != null)
            {
                try
                {
                    sp.Close();
                }

                catch (Exception ex)
                {
                    LogUtil.Logger.Error(ex.Message, ex);

                    MessageBox.Show(ex.Message);
                }
            }
        }


    }
}
