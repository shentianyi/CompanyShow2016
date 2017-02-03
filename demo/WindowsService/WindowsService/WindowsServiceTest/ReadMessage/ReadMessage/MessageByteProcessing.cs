using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brilliantech.Framwork.Utils.ConvertUtil;

namespace ReadMessage
{
  public  class MessageByteProcessing
    {
        /// <summary>
        /// 主动汇报或取消时 发送回复
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] Responese(bool pre, byte[] msg)
        {
            if (msg.Count() == 13)
            {
                string mean = string.Empty;

                int LampId = msg[4];


                string back = string.Format("88{0}{1}B0{2}{3}{4}000000",
                   ScaleConvertor.DecimalToHexString(LampId + 256, true, 8),
                   ScaleConvertor.DecimalToHexString(msg[5], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[7], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[8], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[9], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[10], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[11], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[12], true, 2));

                byte[] bback = ScaleConvertor.HexStringToHexByte(back);

                return bback;
            }


            else
            {
                return null;
            }
        }


        /// <summary>
        /// WMS发送给PTL
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] TransmitToErrorMsg(string Ipkey, byte[] msg, int ErrorNr)
        {
            if (msg.Count() == 13)
            {
                string mean = string.Empty;

                int LampId = msg[4];

                string back = string.Format("{0}{1}{2}EE{3}{4}00000000",
                  ScaleConvertor.DecimalToHexString(int.Parse(Ipkey), true, 2),
                  ScaleConvertor.DecimalToHexString(LampId + 256, true, 8),
                   ScaleConvertor.DecimalToHexString(msg[5], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[7], true, 2),
                   ScaleConvertor.DecimalToHexString(ErrorNr, true, 2));


                byte[] bback = ScaleConvertor.HexStringToHexByte(back);

                return bback;
            }


            else
            {
                return null;
            }
        }

        public static  byte[] Transmit(byte[] msg)
        {
            if (msg.Count() == 13)
            {
                string mean = string.Empty;

                int LampId = msg[4];

                string back = string.Format("88{0}{1}C0{2}{3}{4}{5}{6}{7}",
                   ScaleConvertor.DecimalToHexString(LampId + 256, true, 8),
                   ScaleConvertor.DecimalToHexString(msg[5], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[7], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[8], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[9], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[10], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[11], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[12], true, 2));

                byte[] bback = ScaleConvertor.HexStringToHexByte(back);

                return bback;
            }


            else
            {
                return null;
            }
        }

        public static  byte[] OrderResponese(byte[] msg)
        {
            if (msg.Count() == 13)
            {
                string mean = string.Empty;

                int LampId = msg[4];


                string back = string.Format("88{0}{1}C001{2}{3}0000FF",
                   ScaleConvertor.DecimalToHexString(LampId + 256, true, 8),
                   ScaleConvertor.DecimalToHexString(msg[5], true, 2),
                    ScaleConvertor.DecimalToHexString(msg[8], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[9], true, 2)
                  );

                byte[] bback = ScaleConvertor.HexStringToHexByte(back);

                return bback;
            }


            else
            {
                return null;
            }
        }


        public static  byte[] DeleteOrderResponese(byte[] msg)
        {
            if (msg.Count() == 13)
            {
                string mean = string.Empty;

                int LampId = msg[4];


                string back = string.Format("88{0}{1}C0000000000000",
                   ScaleConvertor.DecimalToHexString(LampId + 256, true, 8),
                   ScaleConvertor.DecimalToHexString(msg[5], true, 2)

                  );

                byte[] bback = ScaleConvertor.HexStringToHexByte(back);

                return bback;
            }


            else
            {
                return null;
            }
        }
    }
}
