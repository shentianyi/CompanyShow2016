using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//使用到TcpListen类
using System.Net.Sockets;
//使用到StreamWriter类
using System.IO;
//使用IPAddress类、IPHostEntry类等
using System.Net;
using System.Threading;

namespace 智能仓库控制系统
{
	public class TCPServer
	{

		/// <summary>
		/// TCP主监听对象
		/// </summary>
		TcpListener TCP = null;
        /// <summary>
        /// TCP客户端线程列表
        /// </summary>
        public List<Thread> tcpThreads = new List<Thread>();
		/// <summary>
		/// TCP客户端列表 整个线程出错时移除
		/// </summary>
        public List<Socket> tcpClients = new List<Socket>();
		#region 委托，不用
		///// <summary>
		///// 接收数据委托
		///// </summary>
		///// <param name="TcpData"></param>
		//public delegate void GetDataEvent(byte[] TcpData);
		///// <summary>
		///// 接收数据事件
		///// </summary>
		//public event GetDataEvent GetData;
		#endregion
		/// <summary>
		/// 实例化
		/// </summary>
		public TCPServer() {
			var port = App.Config["/Config/agv/port"];
			var ip= App.Config["/Config/agv/ip"];
			var byte_ip = ip.Split('.').Select<string, byte>(n => Convert.ToByte(n)).ToArray();
			TCP = new TcpListener(
				new IPEndPoint(
					new IPAddress(byte_ip), 
					int.Parse(port)	)
				);
			App.Logs.AddLog("准备监听TCP端口"+ip+":"+port, true);
		}

		/// <summary>
		/// 开始监听TCP服务
		/// </summary>
		/// <returns>是否开始成功</returns>
		public bool Start() {
            try
            {
                TCP.Start();
            }
            catch ( Exception e2){ 
                var msg="监听AGV小车失败，可能端口已被其它程序占用。\r\n程序即将退出。";
                App.Logs.AddLog(msg,true,true);
                System.Windows.Forms.MessageBox.Show(msg,"严重错误!");
#if ! DEBUG
                Environment.Exit(2);
#endif
            }
            Thread tcpMainThread = new Thread(TCPmainFunc);
			tcpMainThread.Name = "TCP主监听线程";
            App.Logs.AddLog("已开始TCP监听服务线程", true);
			tcpMainThread.Start();
			return true;
		}
        /// <summary>
        /// 主处理线程，接收到请求后开启一个新线程
        /// </summary>
        public void TCPmainFunc() {
            int TimeCount = 5;
            while (true)
            {
                //有数据请求，如果线程运行1秒后仍有请求则创建新线程

                if (TCP.Pending() && TimeCount >=10)//有客户端连接
                {
                    //System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + " 有新的小车请求连接");

                    Thread tcpClient = new Thread(TcpClientThreadFunc);
                    tcpThreads.Add(tcpClient);
					tcpClient.Name = "AGV小车通讯线程 序号"+tcpClients.Count;
					tcpClient.Start();
                    App.Logs.AddLog("创建新的小车通讯线程", true);
                    TimeCount = 1;
                }
                
                Thread.Sleep(100);
                if (TimeCount > 0)
                    TimeCount++;

                //循环检查线程是否已异常退出，删除
                for(int i=0;i<tcpThreads.Count;i++){
                    if(tcpThreads[i].IsAlive==false){
                        tcpThreads[i] = null;
                        App.Logs.AddLog("关闭小车通讯线程", true);
                        tcpThreads.RemoveAt(i);
                    }
                }

                //异常，通常这里不会执行到
                if (tcpThreads.Count > 100) {
                    System.Windows.Forms.MessageBox.Show("系统异常\r\nAGV小车通讯线程数量已超过100", "警告");
                    App.Logs.AddLog("AGV小车通讯线程数量已超过100", true,true);
                }
            }
        }
		/// <summary>
		/// TCP单个客户端处理线程函数
		/// </summary>
		public void TcpClientThreadFunc()
		{
            Socket client = null;

            bool SendFlag = false;//发送请求获取信息标志，接收后清空
            decimal SendTimeCount = 0;//发送时间计数，每隔0.5秒获取一次小车信息
            bool ReceiveOKFlag = true;//接收成功标志
			System.Threading.Timer tmr = null;//定时检测是否掉线 1秒以上掉一
			//是否有请求
			if (TCP.Pending())
            {
                //agvTheadRunFlag = false;
                client = TCP.AcceptSocket();
                tcpClients.Add(client);
                App.Logs.AddLog("收到AGV小车连接请求:" + client.RemoteEndPoint.ToString(), true);
                string ip = client.RemoteEndPoint.ToString().Split(':').First();
                string id = App.Config.GetValueFind("/Config/agv/agv_list/ip", ip, "id");
                var id2 = Convert.ToInt16(id);
                if (id == null)
                    return;
                byte[] buffer = new byte[client.ReceiveBufferSize];
                try
                {
                    while (true)
                    {
                        //如果接收数据成功且间隔大于0.5秒 或 接收失败，间隔大于3秒则 发送 请求状态 数据
                        if ((ReceiveOKFlag && SendTimeCount >= 0.3m) || (ReceiveOKFlag == false && SendTimeCount >= 0.7m))
                        {
                            SendTimeCount = 0;
                            var dat = AGV.SendGetAGVinfo(id2);
                            client.Send(dat);//, 0, dat.Length);
                            App.AGVsInfo.SingleOrDefault(n => n.ID == id2).SendTime = DateTime.Now;
                            SendFlag = true;
                            ReceiveOKFlag = false;  
                        }
                        else
                            SendTimeCount += 0.05m;
                        
                        if (SendFlag && client.Available >= 19)//是否有可用数据供接收
                        {
                            SendFlag = false;
                            ReceiveOKFlag = true;

                            client.Receive(buffer);
                            for (int i = 0; i < buffer.Length; i++)
                            {
                                if (buffer[i] == 0xfd)
                                {
                                    if (i + 19 <= buffer.Length)
                                    { //有效数据
                                        var agvDat = AGV.ConvAGVData(buffer.Skip(i).Take(19).ToArray());
                                        if (agvDat == null)
                                            break;
                                        Int16 agvID = agvDat.SAddress;
                                        
                                        int find_id = -1;
                                        try
                                        {
                                            find_id = App.AGVsInfo.FindIndex(n => n.ID == agvID);
                                            if (find_id >= 0)//如果集合中没有，新增加的，否则先移除
                                            {
                                                var info = AGV.ConvSrcDatToInfo(agvDat,App.AGVsInfo[find_id]);
                                                info.OnLine = true;
                                                info.NotRequest = false;
                                                //==============
                                                if (info.SetLineNumber > 0) { //有发送目标路线,检查是否到目标站点，且停止，如果是清SetLineNumber
                                                    if (info.LineCard == AGV.GetAGVLineStationCard(info.SetLineNumber)
                                                        && info.RunMode == AGVinfoRunEnum.停止) {
                                                            info.SetLineNumber = 0;
                                                        }
                                                }
                                                //==============
                                                if (tmr != null)	//1.5秒内又有新数据，清空定时器
                                                    tmr.Dispose();
                                                tmr = new Timer(func =>
                                                {
                                                    info.OnLine = false;
                                                }, null, 2500, 0);
											}
										}
                                        catch {
                                        
                                        }
                                        break;
                                    }
                                    else
                                        break;
                                }
                            }
                        }
                        Thread.Sleep(50);

                    }
                }
                catch(Exception e2) {
                    App.Logs.AddLog("AGV小车通讯线程发生异常:" + e2.ToString(), true);
					client.Dispose();
                    tcpClients.Remove(client);
                    return;

                }

            }
            else {
                return;
            }




		}

        /// <summary>
        /// 给AGV小车发送消息
        /// </summary>
        /// <param name="id">小车ID</param>
        /// <param name="tcp_dat">TCP消息内容</param>
        /// <returns>是否发送成功</returns>
        public bool SendAGVmsg(Int16 id, byte[] tcp_dat) {
            try
            {
				string ip = App.Config.GetxxFind("/Config/agv/agv_list/ip", "id", id.ToString());
                var client = App.TCP.tcpClients.Single(n => n.RemoteEndPoint.ToString().Split(':').First() == ip);
                if (client == null)
                    return false;
                string dat = "";
                for (int i = 0; i < tcp_dat.Count(); i++) {
                    dat += Convert.ToString(tcp_dat[i], 16) + ",";
                }
                var info = App.AGVsInfo.SingleOrDefault(n => n.ID == id);
                info.NotRequest = true;
                info.LastCmdTime = DateTime.Now;
                System.Diagnostics.Debug.WriteLine("TCP发送消息:"+dat);
                client.Send(tcp_dat);
                return true;
            }
            catch(Exception e)
            {
                App.Logs.AddLog("发送AGV小车消息时出错:" + e.ToString(),true);
                return false;
            }
        }

	}
}
