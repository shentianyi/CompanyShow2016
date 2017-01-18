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
        public static string GetIpByValue(string EnumValue)
        {
            int EnumValueNum = int.Parse(EnumValue);
            string name = typeof(NumForIp).GetEnumName(EnumValueNum);
            FieldInfo fieldinfo = typeof(NumForIp).GetField(name);
            Object obj = fieldinfo.GetCustomAttribute(typeof(DescriptionAttribute), false);
            DescriptionAttribute ds = (DescriptionAttribute)obj;

            return ds.Description;


        }

        /// <summary>
        /// 通过Name获取Description
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetIpByName(string name)
        {

            FieldInfo fieldinfo = typeof(NumForIp).GetField(name);
            Object obj = fieldinfo.GetCustomAttribute(typeof(DescriptionAttribute), false);
            DescriptionAttribute ds = (DescriptionAttribute)obj;


            return ds.Description;




        }


        /// <summary>
        /// 通过Description得到Value
        /// </summary>
        /// <param name = "Ip" ></ param >
        /// < returns ></ returns >
        public static string GetValueByIp(string Ip)
        {
            string value = string.Empty;
            var AllNames = typeof(NumForIp).GetEnumNames();
            foreach (string SingleName in AllNames)
            {
                if (GetIpByName(SingleName).Equals(Ip))
                {
                    int ValueNum = (int)Enum.Parse(typeof(NumForIp), SingleName);
                    value = ValueNum.ToString();
                    return value;

                }

            }
            return value;
        }



        public static string GetColor(int R, int G, int B)
        {
            string Color = "";
            int Count = 0;
            int RCount = 0;
            int GCount = 0;
            int BCount = 0;
            if (R != 0)
            {
                RCount = 1;
            }
            if (G != 0)
            {
                GCount = 1;
            }
            if (B != 0)
            {
                BCount = 1;
            }
            Count = RCount * 4 + GCount * 2 + BCount * 1;
            switch (Count)
            {
                case 0: Color = "灯灭"; break;
                case 1: Color = "红色"; break;
                case 2: Color = "绿色"; break;
                case 3: Color = "黄色"; break;//红绿
                case 4: Color = "蓝色"; break;
                case 5: Color = "紫色"; break;//蓝红
                case 6: Color = "青色"; break;//蓝绿
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



                //if (MessageBytes[0] != 136)
                //{



                //    mean += "转发给:" + GetIpByValue(MessageBytes[0].ToString()) + ",";


                //}


                switch (MessageBytes[6])
                {
                    //case (byte)176:
                    //    {
                    //        mean += "编号为" + LampId + ",STU为" + STU + "的第" + SN + "条肯定回答指令";
                    //        break;
                    //    }
                    case (byte)192:
                        {
                            mean += "编号为" + LampId + ",数码管数字为" + Num + ",颜色为" + Color + ",的第" + SN + "条设置指令";
                            break;
                        }

                    case (byte)209:
                        {

                            mean += "编号为" + LampId + ",数码管数字为" + Num + ",颜色为" + Color + ",的第" + SN + "条确认指令";
                            break;

                        }
                    case (byte)210:
                        {
                            mean += mean = "编号为" + LampId + ",数码管数字为" + ",颜色为" + Color + ",的第" + SN + "条取消指令";

                            break;
                        }
                    case (byte)238:
                        {
                            mean += "编号为" + LampId + ",STU为" + STU + "的第" + SN + "条错误指令,错误原因：";
                            switch (MessageBytes[8])
                            {
                                case (byte)1:
                                    mean += "PTL灯无反馈";
                                    break;
                                case (byte)2:
                                    mean += "WMS无反馈";
                                    break;
                                default: break;
                            }
                            break;
                        }
                }
            }



            return mean;
        }
    }
}
