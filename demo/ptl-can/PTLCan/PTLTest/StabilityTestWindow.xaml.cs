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
    /// StabilityTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StabilityTestWindow : Window
    {
        private int i;
        private SerialPort sp;
        private int j=1;
        private int count=100;
        private bool SendFlag = true;
        private bool StabilityPass = false;

        public StabilityTestWindow()
        {
            
            InitializeComponent();
            this.Closing += Window_Closing;
           
        }

        private void Stabilitybutton_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(IdtextBox.Text))
            {
                MessageBox.Show("请输入编号");
            }
            int LampId = int.Parse(IdtextBox.Text);
            while (StabilityPass==false)
            {
                try
                {
                    if (this.sp == null)
                    {
                        this.sp = new SerialPort(Settings.Default.com, 9600, Parity.None);
                        this.sp.Open();
                       
                        LogUtil.Logger.Info("Open Success.....");
                    }
                    else
                    {
                        if (sp.IsOpen)
                        {

                            while (SendFlag)
                            {
                                for (i = 1; i <= count; i++)
                                {
                                    this.Send(LampId, i, "FF0000");
                                   
                                    this.Receive();

                                }
                                SendFlag = false;



                                if (SendFlag==false && StabilityPass == true)
                                {
                                    MessageBox.Show("稳定性测试通过");
                                }
                                else if(SendFlag==false && StabilityPass==false)
                                {
                                    MessageBox.Show("稳定性测试未通过");
                                }
                                

                            }
                        }
                        else
                        {
                            this.sp.Open();
                            //this.sp.DataReceived += new SerialDataReceivedEventHandler(Sp_DataReceived);
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


      
        private void Receive()
        {

            Thread.Sleep(400);
            byte[] rdata = new byte[sp.BytesToRead];
            sp.Read(rdata, 0, rdata.Length);            
            LogUtil.Logger.Info("接收数据：" + ScaleHelper.HexBytesToString(rdata));
            if(rdata.Length == 12)
            {
                j++;
            }
           
            if (j == count)
            {
                StabilityPass = true;
            }

        }

        public void Send(int lightId, int number, string rgbColor)
        {
            
            // 00 00 01 01, 01, C0, 01, 00 01, 01 00 00
            // 给1号，发显示为 00 01，RGB为010000
            string cmd = string.Format("{0}01C001{1}{2}",
                ScaleHelper.DecimalToHexString(lightId + 256, true, 8),
                ScaleHelper.DecimalToHexString(number, true, 4),
                rgbColor.ToUpper());

            LogUtil.Logger.Info("发送 命令：" + cmd);
            byte[] bcmd = ScaleHelper.HexStringToHexByte(cmd);
            
            if (!sp.IsOpen)
            {
                this.sp.Open();
              
            }
            sp.Write(bcmd, 0, bcmd.Length);
        }

        private void IdtextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
