using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
namespace 智能仓库控制系统
{
    public class WindowsAutoStart
    {
        /// <summary>
        /// 让程序自动在WINDOWS开机时启动
        /// </summary>
        /// <returns>是否成功</returns>
        public static bool Enable() {
            try
            {
                var reg = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
                var key = reg.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                key.SetValue("agv", Environment.CommandLine);
                key.Flush();
            }
            catch {
                return false;
            }
            return true;
        }
    }
}
