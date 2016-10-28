using Brilliantech.Framwork.Utils.LogUtil;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLCanController
{
    public class Controller
    {
        /// <summary>
        /// 串口参数
        /// </summary>
        private SerialPort sp { get; set; }
        private string com { get; set; }
        private int baudRate;
        private Parity parity;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="com">串口号</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="parity">校验</param>
        public Controller(string com, int baudRate = 9600, Parity parity = Parity.None)
        {
            this.com = com;
            this.baudRate = baudRate;
            this.parity = parity;

        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="lightId">灯设备的编号, 0~255</param>
        /// <param name="number">显示数, 0~9999</param>
        /// <param name="rgbColor">灯颜色, RGB, 如白色：FFFFFF </param>
        public void Send(int lightId,int number, string rgbColor)
        {   
            // 00 00 01 01, 01, C0, 01, 00 01, 01 00 00
            // 给1号，发显示为 00 01，RGB为010000
            string cmd = string.Format("{0}01C001{1}{2}",
                ScaleHelper.DecimalToHexString(lightId + 256,true,8),
                ScaleHelper.DecimalToHexString(number, true, 4),
                rgbColor.ToUpper());

            LogUtil.Logger.Info("发送 命令：" +cmd);
            byte[] bcmd = ScaleHelper.HexStringToHexByte(cmd);

            LogUtil.Logger.Info(string.Format("发送 命令Byte：{0}",bcmd) );
            if (!IsOpen())
            {
                this.Open();
            }
            sp.Write(bcmd, 0, bcmd.Length);
        }

        /// <summary>
        /// 确认串口是否打开
        /// </summary>
        /// <returns></returns>
        private bool IsOpen()
        {
            return this.sp != null && this.sp.IsOpen;
        }

        /// <summary>
        /// 打卡串口
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            try
            {
                if (this.sp == null)
                {
                    this.sp = new SerialPort(this.com, this.baudRate);
                    this.sp.DataReceived += new SerialDataReceivedEventHandler(Sp_DataReceived);
                }
                this.sp.Open();
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.Logger.Error("COM 打开错误",ex);
                throw ex;
            }
        }

        /// <summary>
        /// 接收处理数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] rdata = new byte[sp.BytesToRead];
            sp.Read(rdata, 0, rdata.Length);

            LogUtil.Logger.Info("接收数据：" + ScaleHelper.HexBytesToString(rdata));
            LogUtil.Logger.Info(string.Format("接收数据Byte：{0}", rdata));

        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        /// <returns></returns>
        public bool Close(bool reclose = false)
        {
            if (this.sp != null)
            {
                try
                {
                    if (this.sp.IsOpen)
                    {
                        this.sp.Close();
                        LogUtil.Logger.Info("Close Success");
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    if (!reclose)
                    {
                        Close(true);
                    }
                    LogUtil.Logger.Error("Close Error",ex);
                    if (reclose)
                    {
                        throw ex;
                    }
                }
            }
            return false;
        }

    }
}
