using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brilliantech.Framwork.Utils.ConvertUtil;
using Brilliantech.Framwork.Utils.LogUtil;

namespace ReadMessage
{
    public class Parser
    {

        public static string readMessage(byte[] MessageBytes)
        {


            int MessageCount = MessageBytes.Count();
            string mean = "";
            if (MessageCount == 13)
            {
                byte[] LampBytes = new byte[4] { MessageBytes[1], MessageBytes[2], MessageBytes[3], MessageBytes[4] };
               
                int LampId = ScaleConvertor.HexBytesToDecimal(LampBytes);

                int SN = MessageBytes[5];
                int STU = MessageBytes[7];
                int Num = (MessageBytes[8] << 8 | MessageBytes[9]);


                switch (MessageBytes[6])
                {
                    case (byte)176:
                        {
                            mean = "编号为" + LampId + ",STU为" + STU + "的第" + SN + "条肯定回答指令";
                            break;
                        }
                    case (byte)192:
                        {
                            mean = "编号为" + LampId + ",数码管数字为" + Num + "的第" + SN + "条设置指令";
                            break;
                        }

                    case (byte)209:
                        {

                            mean = "编号为"+LampId+",数码管数字为" + Num + "的第"+SN+"条确认指令";
                            break;

                        }
                    case (byte)210:
                        {
                            mean = mean = "编号为" + LampId + ",数码管数字为" + Num + "的第" + SN + "条取消指令";

                            break;
                        }
                }
            }
            return mean;
        }
    }
}
