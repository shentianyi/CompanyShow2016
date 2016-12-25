using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 智能仓库控制系统
{
    /// <summary>
    /// 读取写入条目类
    /// </summary>
    public class WRItem
    {
        /// <summary>
        /// 条目读取时间
        /// </summary>
        public DateTime ReadTime;
        /// <summary>
        /// 条目写入修改时间
        /// </summary>
        public DateTime WriteTime;
        /// <summary>
        /// 条目内容 读写时自动变更相关时间
        /// </summary>
        public object ItemValue {
            get {
                ReadTime = DateTime.Now;
                IsModify = false;
                return item;
            }
            set {
                item = value;
                WriteTime = DateTime.Now;
                IsModify = true;
            }
        }
        /// <summary>
        /// 条目内容 不变更相关时间
        /// </summary>
        public object NoTimeItemValue {
            get {
                return item;
            }
            set {
                item = value;
            }
        }
        /// <summary>
        /// 至上次读取后是否修改过
        /// </summary>
        public bool IsModify = false;
        object item;
        public WRItem(object item) { 
            ItemValue=item;
        }
    }
    public class Log {
        /// <summary>
        /// 日志条目信息
        /// </summary>
        List<WRItem> items = new List<WRItem>();
        /// <summary>
        /// 最后写入日期，超过一定时间后自动将日志永久存储
        /// </summary>
        DateTime LastWriteTime = new DateTime();
        /// <summary>
        /// 日志线程
        /// </summary>
        System.Threading.Thread logThread = null;
        /// <summary>
        /// 日志文件
        /// </summary>
        System.IO.StreamWriter file = null;
        /// <summary>
        /// 上次写入的日志编号
        /// </summary>
        int LastWriteIndex = 0;
		/// <summary>
		/// 是否立即写入日志
		/// </summary>
		bool Flush = false;

		public Log() {
            logThread = new System.Threading.Thread(LogWriteFunc);
			logThread.Name = "日志线程";
            System.IO.FileStream file2 = new System.IO.FileStream("log.log", System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.Read);
            if (file2.Length > 100 * 1024*1024 ) {     
                System.Windows.Forms.MessageBox.Show("日志文件已大于100MB\r\n建议检查、备份、删除日志");
            }
            file = new System.IO.StreamWriter(file2);
			logThread.Start();
        }
		~Log() {
            try
            {
                if (file != null)
                    file.Close();
            }
            catch { }
			logThread.Abort();
		}
        /// <summary>
        /// 接收数据委托
        /// </summary>
        /// <param name="TcpData"></param>
        public delegate void LogAddEventDel(WRItem Log_item);
        /// <summary>
        /// 接收数据事件
        /// </summary>
        public event LogAddEventDel LogAddEvent;
		/// <summary>
		/// 添加一条日志
		/// </summary>
		/// <param name="item">日志内容</param>
		/// <param name="WriteTime">写入日志时是否自动附加时间在最前边</param>
		/// <param name="flush">是否立即刷新写入日志</param>
		public void AddLog(object item, bool WriteTime = false,bool flush=false)
        {
            string msg = "";
            if(WriteTime)
                msg=DateTime.Now.ToString()+":"+item.ToString();
            else
                msg=item.ToString();
            var wr=new WRItem(msg);
            items.Add(wr);
            if (LogAddEvent != null)
                LogAddEvent(wr);
            if (items.Count > 1000)
            {
                items.RemoveAt(0);
                if(LastWriteIndex>0)
                    LastWriteIndex--;
            }
			Flush = flush;

		}
        /// <summary>
        /// 日志条目
        /// </summary>
        /// <returns></returns>
        public List<WRItem> LogItems() {
            return items;
        }
        /// <summary>
        /// 写入日志线程，当有日志变更时，每隔5秒写入一次本地日志
        /// </summary>
        void LogWriteFunc() {
            bool w = false; //是否写入了日志
			int c = 0;
            while(true){

				do
				{	//延时5秒，如果有立即写入标志则退出延时
					System.Threading.Thread.Sleep(10);
					c++;
					if (Flush)
					{
						Flush = false;
						break;
					}
				} while (c < 500);
				c = 0;

				w = false;
                for (int i = LastWriteIndex; i < items.Count; i++) {
                    file.WriteLine(items[i].NoTimeItemValue.ToString());
                    LastWriteIndex = i+1;
                    w = true;
                }
                if(w)
                    file.Flush();
            }
        }
    }

}
