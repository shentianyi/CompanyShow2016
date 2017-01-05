using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace WindowsServiceTest
{
   public static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
       public static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ServiceTest()
            };
            ServiceBase.Run(ServicesToRun);


            //test code
            //ServiceTest st = new ServiceTest();
            //st.OnStart();
            //Thread.Sleep(10000);
            //st.OnStop();

        }
    }
}
