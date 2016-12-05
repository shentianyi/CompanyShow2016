using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brilliantech.Framwork.Utils.LogUtil;
using Brilliantech.Framwork.Utils.ConvertUtil;
using System.Threading;
using PTLCanTest.Properties;
using System.IO.Ports;

using System.Windows;

namespace PTLCanTest.Services
{
   public class PressAfterShow
    {
        private bool BtnTest = false;
      //  private int LampId = 0;
        private int i;
        private SerialPort sp;
        private int j = 1;
        private int count = 100;
        private bool SendFlag = true;
        private bool StabilityPass = false;
        private string[] colors = new string[3] { "FF0000", "FFFFFF", "000000" };



        public void Stability(List<int> LampIds,int times=100)
        {
            count = times == -1 ? int.MaxValue : times;

            MessageBox.Show("测试即将开始，请耐心等待结果");

            while (StabilityPass == false)
            {
                try
                {
                    if (this.sp == null)
                    {
                        this.sp = new SerialPort(Settings.Default.com, 9600, Parity.None);
                        this.sp.DataReceived += new SerialDataReceivedEventHandler(Sp_DataReceived);
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
                                    foreach (var LampId in LampIds)
                                    {
                                        this.Send(LampId, i, colors[i%3]);
                                        Thread.Sleep(300);
                                        //BtnTest = false;
                                        //CheckId();
                                    }

                                }


                                if (SendFlag == false && StabilityPass == true)
                                {
                                    MessageBox.Show("稳定性测试通过");
                                }
                                else if (SendFlag == false && StabilityPass == false)
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

       

        public void Send(int lightId, int number, string rgbColor)
        {

            // 00 00 01 01, 01, C0, 01, 00 01, 01 00 00
            // 给1号，发显示为 00 01，RGB为010000
            string cmd = string.Format("{0}01C001{1}{2}",
                ScaleConvertor.DecimalToHexString(lightId + 256, true, 8),
                ScaleConvertor.DecimalToHexString(number, true, 4),
                rgbColor.ToUpper());

            LogUtil.Logger.Info("发送 命令：" + cmd);
            byte[] bcmd = ScaleConvertor.HexStringToHexByte(cmd);

            if (!sp.IsOpen)
            {
                this.sp.Open();

            }
            sp.Write(bcmd, 0, bcmd.Length);
        }



        public void CheckId()
        {
            
            while (BtnTest == false)
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
                            continue;
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
            if (sp.BytesToRead == 12)
            {
                byte[] rdata = new byte[sp.BytesToRead];
                sp.Read(rdata, 0, rdata.Length);

                LogUtil.Logger.Info("接收数据：" + ScaleConvertor.HexBytesToString(rdata));
                switch (rdata[5])
                {
                    case (byte)209:
                        {
                            int LampId = rdata[3];
                            string back = string.Format("{0}{1}B0010000000000",
                           ScaleConvertor.DecimalToHexString(LampId + 256, true, 8),
                           ScaleConvertor.DecimalToHexString(rdata[4], true, 2));

                            byte[] bback = ScaleConvertor.HexStringToHexByte(back);

                            sp.Write(bback, 0, bback.Length);
                            LogUtil.Logger.Info(string.Format("发送回复: " + ScaleConvertor.HexBytesToString(bback)));
                            int ShowNum = (rdata[7] << 8 | rdata[8]);
                            MessageBoxResult MSBOXRE = MessageBox.Show("是否发送了数码管数字为" + ShowNum + "的确认指令?", "按键测试结果", MessageBoxButton.YesNo);

                            if (MSBOXRE == MessageBoxResult.Yes)
                            {

                                BtnTest = true;
                                j++;
                                if(j==count)
                                {
                                    StabilityPass = true;
                                }
                            }


                           

                            break;

                        }
                    case (byte)210:
                        {

                            int LampId = rdata[3];
                            string back = string.Format("{0}{1}B0010000000000",
                          ScaleConvertor.DecimalToHexString(LampId + 256, true, 8),
                           ScaleConvertor.DecimalToHexString(rdata[4], true, 2));

                            byte[] bback = ScaleConvertor.HexStringToHexByte(back);

                            sp.Write(bback, 0, bback.Length);
                            LogUtil.Logger.Info(string.Format("发送回复: " + ScaleConvertor.HexBytesToString(bback)));
                            int ShowNum = (rdata[7] << 8 | rdata[8]);
                            MessageBoxResult MSBOXRE = MessageBox.Show("是否发送了数码管数字为" + ShowNum + "的取消指令?", "按键测试结果", MessageBoxButton.YesNo);

                            if (MSBOXRE == MessageBoxResult.Yes)
                            {

                                BtnTest = true;
                                j++;
                                if (j == count)
                                {
                                    StabilityPass = true;
                                }


                            }




                            break;
                        }
                }


            }

        }

      

    }
}
