using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brilliantech.Framwork.Utils.ConvertUtil;
using Brilliantech.Framwork.Utils.LogUtil;
using System.Reflection;
using System.ComponentModel;
using ReadMessage.ENUM;


namespace ReadMessage
{
    public class Parser
    {
        public static string GetIpByValue(int EnumValue)
        {
            string name = typeof(NumForIp).GetEnumName(EnumValue);
            FieldInfo fieldinfo = typeof(NumForIp).GetField(name);
            Object obj = fieldinfo.GetCustomAttribute(typeof(DescriptionAttribute), false);
            DescriptionAttribute da = (DescriptionAttribute)obj;
            string ds = da.Description;
            return ds;
        }


        public static string GetColor(int R,int G,int B)
        {
            string Color = "";
            int Count = 0;
            int RCount = 0;
            int GCount = 0;
            int BCount = 0;
            if(R!=0)
            {
                RCount = 1;
            }
            if(G!=0)
            {
                GCount = 1;
            }
            if(B!=0)
            {
                BCount = 1;
            }
            Count = RCount * 4 + GCount * 2 + BCount * 1;
            switch(Count)
            {
                case 0: Color = "灯灭"; break;
                case 1: Color = "蓝色"; break;
                case 2: Color = "绿色"; break;
                case 3: Color = "青色"; break;//蓝绿
                case 4: Color = "红色"; break;
                case 5: Color = "紫色"; break;//红蓝
                case 6: Color = "黄色"; break;//红绿
                case 7: Color = "白色"; break;

            }
            return Color;
        }

        public static string readMessage(byte[] MessageBytes)
        {


            int MessageCount = MessageBytes.Count();
            string mean = "";
            if (MessageCount == 13)
            {


                int LampId = MessageBytes[4];

                int SN = MessageBytes[5];
                int STU = MessageBytes[7];
                int Num = (MessageBytes[8] << 8 | MessageBytes[9]);
                string Color = GetColor(MessageBytes[10], MessageBytes[11], MessageBytes[12]);
               


                if (MessageBytes[0] != 136 && MessageBytes[6] == 255)
                {
                    string AimedIp = GetIpByValue(MessageBytes[0]);


                    mean = "转发给" + AimedIp + ",编号为" + LampId + ",STU为" + STU + "的第" + SN + "条指令";


                }

                else
                {

                    switch (MessageBytes[6])
                    {
                        case (byte)176:
                            {
                                mean = "编号为" + LampId + ",STU为" + STU + "的第" + SN + "条肯定回答指令";
                                break;
                            }
                        case (byte)192:
                            {
                                mean = "编号为" + LampId + ",数码管数字为" + Num + ",颜色为"+Color+",的第" + SN + "条设置指令";
                                break;
                            }

                        case (byte)209:
                            {

                                mean = "编号为" + LampId + ",数码管数字为" + Num + ",颜色为" + Color + ",的第" + SN + "条确认指令";
                                break;

                            }
                        case (byte)210:
                            {
                                mean = mean = "编号为" + LampId + ",数码管数字为" + ",颜色为" + Color + ",的第" + SN + "条取消指令";

                                break;
                            }
                    }
                }
                
            }

            return mean;
        }
    }
}
