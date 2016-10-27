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
        /// <param name="numHight">高位显示数, 0~99</param>
        /// <param name="numLow">低位显示数, 0~99</param>
        /// <param name="rgbColor">灯颜色, RGB, 如白色：FFFFFF </param>
        public void Send(int lightId,int numHight, int numLow, string rgbColor)
        {
            // 00 00 01 01 01 C0 01 00 01 01 00 00
            // 给1号，发显示为 00 01，RGB为010000
            
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
        private bool OpenCom()
        {
            try
            {
                if (this.sp == null)
                {
                    this.sp = new SerialPort(this.com, this.baudRate);
                    this.sp.ReadTimeout = this.timeout;
                    this.sp.DataReceived += Sp_DataReceived;
                }
                this.sp.Open();
                return true;
            }
            catch (Exception ex)
            {
                LogUtil.Logger.Error(ex.Message);
                throw ex;
            }
        }

    }
}
