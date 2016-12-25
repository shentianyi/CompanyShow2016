using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 智能仓库控制系统
{
	/// <summary>
	/// 项目主静态类，静态变量
	/// </summary>
	public class App
	{
		/// <summary>
		/// 日志信息 线程
		/// </summary>
		public static Log Logs = new Log();
		/// <summary>
		/// 程序配置
		/// </summary>
		public static Config Config = new Config();
        /// <summary>
        /// AGV小车网络通讯
        /// </summary>
		public static TCPServer TCP = null;
		/// <summary>
		/// AGV所有小车实时信息
		/// </summary>
		public static List<AGVInfo> AGVsInfo = null;
        
		/// <summary>
		/// OPC通讯
		/// </summary>
		public static OPC OPC = null;
		/// <summary>
		/// 主窗体引用
		/// </summary>
		public static frmMain FrmMain = null;
		/// <summary>
		/// 自动运行模式/手动运行模式
		/// </summary>
		public static bool AutoRunMode = false;
		/// <summary>
		/// 开始运行标志 
		/// </summary>
		public static bool RunFlag = false;
		/// <summary>
		/// 系统初始化完毕标志
		/// </summary>
		public static bool initOK = false;

	}
}
