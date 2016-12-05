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
using PTLCanTest.Services;

namespace PTLCanTest.Services
{
   public class OffConnTest
    {
        
       
        public void OffConnection(SerialPort sp)
        {
            BuildSerialPort bs = new BuildSerialPort();
            bs.SP_Build(sp);
            
            string mean = string.Empty;
            bs.SP_Send(sp,0, 2, "0000FF");

            MessageBoxResult MSBOXRE = MessageBox.Show("是否显示数字为0002,颜色为红色", "测试结果", MessageBoxButton.YesNo);
            if (MSBOXRE == MessageBoxResult.Yes)
            {
                mean = "连通测试通过";

            }
            else
            {
                mean = "连通测试未通过";
            }

            MessageBox.Show(mean);


        }

    }
}
