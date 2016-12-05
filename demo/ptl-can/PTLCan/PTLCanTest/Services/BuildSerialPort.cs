using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTLCanTest.Properties;
using System.IO.Ports;
using System.Windows;
using Brilliantech.Framwork.Utils.ConvertUtil;
using Brilliantech.Framwork.Utils.LogUtil;

namespace PTLCanTest.Services
{
    public class BuildSerialPort
    {


        public void SP_Build(SerialPort sp)
        {
            if (sp != null)
            {
                if (sp.IsOpen == false)
                {
                    try
                    {

                        sp = new SerialPort(Settings.Default.com, 9600, Parity.None);
                        sp.Open();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }



        public void SP_Close(SerialPort sp)
        {
            if (sp != null)
            {
                if (sp.IsOpen)
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


        public void SP_Send(SerialPort sp, int lightId, int number, string rgbColor)
        {

            // 00 00 01 01, 01, C0, 01, 00 01, 01 00 00
            // 给1号，发显示为 00 01，RGB为010000
                string cmd = string.Format("{0}01C001{1}{2}",
                ScaleConvertor.DecimalToHexString(lightId + 256, true, 8),
                ScaleConvertor.DecimalToHexString(number, true, 4),
                rgbColor.ToUpper());

            LogUtil.Logger.Info("发送 命令：" + cmd);
            byte[] bcmd = ScaleConvertor.HexStringToHexByte(cmd);

            if (sp.IsOpen==false)
            {
                sp.Open();

            }
            sp.Write(bcmd, 0, bcmd.Length);
        }


    }
}
