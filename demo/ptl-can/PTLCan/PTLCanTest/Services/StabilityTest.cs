﻿using System;
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

   public class StabilityTest
    {
        private int i;
        private SerialPort sp;
        private int j = 1;
        private int count = 100;
        private bool SendFlag = true;
        private bool StabilityPass = false;


        public void Stability(int LampId)
        {

            MessageBox.Show("测试即将开始，请耐心等待结果");
           
            while (StabilityPass == false)
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

        private void Receive()
        {

            Thread.Sleep(400);
            byte[] rdata = new byte[sp.BytesToRead];
            sp.Read(rdata, 0, rdata.Length);
            LogUtil.Logger.Info("接收数据：" + ScaleConvertor.HexBytesToString(rdata));
            if (rdata.Length == 12)
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

    }

}
