using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace 智能仓库控制系统
{
	static class Program
	{
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main()
		{
            try{
                var t=System.Threading.Mutex.OpenExisting("agv_run_flag");
                MessageBox.Show("【AGV小车控制系统】已运行，不可多次运行本系统，请检查。");
                Environment.Exit(10);
            }
            catch{
            }
            System.Threading.Mutex run_flag = new System.Threading.Mutex(true, "agv_run_flag");
            
			//程序当前工作目录=程序所在EXE目录
			Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new frmMain());

		}
	}
}
