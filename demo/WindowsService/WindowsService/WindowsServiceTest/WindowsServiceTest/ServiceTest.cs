using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using Brilliantech.Framwork.Utils.LogUtil;
using Brilliantech.Framwork.Utils.ConvertUtil;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using ScmWcfService;


namespace WindowsServiceTest
{
    public partial class ServiceTest : ServiceBase
    {
        public ServiceTest()
        {
            InitializeComponent();
        }

        Socket tcpServer;
        Thread ClientRecieveThread;
      
        bool runflag = true;

        Dictionary<string, Socket> clients = new Dictionary<string, Socket>();
        Dictionary<string, Thread> clientThreads = new Dictionary<string, Thread>();
        private string PTLIp = string.Empty;
        private string WMSIp = string.Empty;
        private string PTLKey = string.Empty;
        private string WMSKey = Settings1.Default.WMSKey;
        private bool IsReceived = false;
        private bool IsReSent = false;
        private int ReSentCount = 0;
        private bool IsSent = false;
        private bool IsWMSReceived = false;
        private bool IsWMSReSent = false;
        private int WMSReSentCount = 0;
        private bool IsWMSSent = false;




        protected override void OnStart(string[] args)
        {

            try
            {

                LogUtil.Logger.Info("【开启服务】" + DateTime.Now);
                IPAddress ip = IPAddress.Parse(Settings1.Default.ServerIp);
                int port = int.Parse(Settings1.Default.ServerPort);
                tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                tcpServer.Bind(new IPEndPoint(ip, port));  //绑定IP地址：端口  
                tcpServer.Listen(10);    //设定最多10个排队连接请求  
                LogUtil.Logger.Info(string.Format("启动监听{0}成功", tcpServer.LocalEndPoint.ToString()));
                ClientRecieveThread = new Thread(ListenConnnect);
                ClientRecieveThread.IsBackground = true;
                ClientRecieveThread.Start();


            }
            catch (Exception ee)
            {
                LogUtil.Logger.Error(ee.Message);
            }
        }



        public void OnStart()
        {

           
            LogUtil.Logger.Info("【开启服务】" + DateTime.Now);
            IPAddress ip = IPAddress.Parse(Settings1.Default.ServerIp);
            int port = int.Parse(Settings1.Default.ServerPort);
            tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpServer.Bind(new IPEndPoint(ip, port));  //绑定IP地址：端口  
            tcpServer.Listen(10);    //设定最多10个排队连接请求  
            LogUtil.Logger.Info(string.Format("启动监听{0}成功", tcpServer.LocalEndPoint.ToString()));
            ClientRecieveThread = new Thread(ListenConnnect);
            ClientRecieveThread.IsBackground = true;
            ClientRecieveThread.Start();

            while (true)
            {
                string a = "Starting";
            }


        }

        //public void OnStop()
        //{
        //    runflag = false;
        //    Thread.Sleep(5000);
        //    LogUtil.Logger.Info("【关闭服务】" + DateTime.Now);

        //    foreach (var n in clients)
        //    {
        //        if (n.Value != null)
        //        {
        //            n.Value.Close();
        //            //clients.Remove(n.Key);

        //        }
        //    }




        //    //foreach (var t in clientThreads)
        //    //{
        //    //    if (t.Value != null)
        //    //    {

        //    //        clientThreads.Remove(t.Key);


        //    //    }
        //    //}


        //    /// 关闭server
        //    //if (tcpServer != null)
        //    //{
        //    //    tcpServer.Close();
        //    //}


        //}


        protected override void OnStop()
        {
            runflag = false;
            Thread.Sleep(5000);
            LogUtil.Logger.Info("【关闭服务】" + DateTime.Now);

            foreach (var n in clients)
            {
                if (n.Value != null)
                {
                    n.Value.Close();


                }
            }


        }




        /// <summary>
        /// 获取client的IP并且加入到clients池
        /// </summary>
        private void ListenConnnect()
        {
            while (runflag)
            {

                Socket client = tcpServer.Accept();
                try
                {

                    string key = GetKeyByValue(client.RemoteEndPoint.ToString());

                    if (string.IsNullOrEmpty(key) || key.Equals(WMSKey))
                    {
                        key = Settings1.Default.WMSKey;
                        LogUtil.Logger.Info("客户端IP地址:" + client.RemoteEndPoint.ToString() + "，编号" + key + "已连接服务器");
                        if (clients.ContainsKey(key))
                        {
                            RemoveClient(clients[key].RemoteEndPoint.ToString());
                            clients.Add(key, client);

                        }


                    }

                    else
                    {
                        if (clients.ContainsKey(key))
                        {
                            RemoveClient(clients[key].RemoteEndPoint.ToString());
                            clients.Add(key, client);

                        }
                        LogUtil.Logger.Info("CAN总线编号：" + key + ",灯ip" + client.RemoteEndPoint.ToString() + "已连接服务器");
                    }
                    if (clients.ContainsKey(key) == false)
                    {
                        clients.Add(key, client);
                    }

                    Thread clientThread = new Thread(ListenClientMsg);
                    clientThread.IsBackground = true;
                    clientThread.Start(client);
                    if (clientThreads.ContainsKey(key) == false)
                    {
                        clientThreads.Add(key, clientThread);
                    }


                }


                //}
                catch (Exception ee)
                {
                    LogUtil.Logger.Info(ee.Message);
                    break;
                }

            }

        }
        /// <summary>
        /// 数据接收 
        /// </summary>
        /// <param name="cliento"></param>

        private void ListenClientMsg(object cliento)
        {
            object new_cliento = new object();
            new_cliento = cliento;
            Socket client = new_cliento as Socket;
            while (true)
            {
                try
                {

                    byte[] result = new byte[13];
                    int dataLength = client.Receive(result);

                   
                   


                    if (dataLength > 0 && result[6] != 0)
                    {

                        byte[] MessageBytes = result.Take(dataLength).ToArray();
                        //接收数据解析
                        string ClientIp = client.RemoteEndPoint.ToString();
                        string keys = GetKeyByValue(ClientIp);
                        string Receivemeans = "来自:" + ClientIp + "," + ReadMessage.Parser.readMessage(MessageBytes);
                        LogUtil.Logger.Info("【接收数据】" + ScaleConvertor.HexBytesToString(MessageBytes));
                        if (string.IsNullOrEmpty(ReadMessage.Parser.readMessage(MessageBytes)) == false)
                        {

                            LogUtil.Logger.Info("【接收解析】" + Receivemeans);
                        }


                        ///数据转发
                        if (MessageBytes[0] != 136)
                        {
                            if (MessageBytes[6] != 176)
                            {
                                PTLKey = MessageBytes[0].ToString();//目标灯Ip编号

                                byte[] ResponeBytes = ReadMessage.MessageByteProcessing.Transmit(MessageBytes);//指令内容
                                WMSIp = client.RemoteEndPoint.ToString();

                                SendToClient(PTLKey, ResponeBytes, true);



                            }
                            else
                            {
                               
                                if(IsWMSSent)
                                {                                                                 
                                IsWMSReceived = true;
                                }
                                else
                                {
                                    IsWMSReceived = false;
                                }
                            }

                        }
                        else
                        {

                            switch (result[6])
                            {
                                case (byte)176:
                                    {
                                        if (IsSent)//指令已经发送 并且收到了B0的回应
                                        {
                                            IsReceived = true;
                                          

                                        }
                                        else
                                        {
                                            IsReceived = false;
                                        }
                                        if (string.IsNullOrEmpty(PTLKey))
                                        {
                                            break;
                                        }
                                        MessageBytes[0] = Convert.ToByte(PTLKey);
                                        SendToClient(WMSKey, MessageBytes,true);
                                        PTLKey = string.Empty;
                                        break;
                                    }
                                ///缺货报警
                                case (byte)209:
                                    {
                                        byte[] ResponeBytes = ReadMessage.MessageByteProcessing.Responese(true, MessageBytes);
                                        SendToClient(ReadMessage.Parser.GetValueByIp(client.RemoteEndPoint.ToString()), ResponeBytes, false);
                                        MessageBytes[0] = Convert.ToByte(ReadMessage.Parser.GetValueByIp(client.RemoteEndPoint.ToString()));

                                       
                                        if (((MessageBytes[8] << 8 | MessageBytes[9]) != 0) && MessageBytes[7] == 0)
                                        {

                                            OrderService os = new OrderService();
                                            var q = os.CreateOrderByLed(client.RemoteEndPoint.ToString(), MessageBytes[4].ToString(), (MessageBytes[8] << 8 | MessageBytes[9]));
                                            if (q.Success)
                                            {
                                                string order = "创建要货单成功，订单编号:" + q.data.id + "\n";
                                                LogUtil.Logger.Info(order);
                                                byte[] OrderresponseMessage = ReadMessage.MessageByteProcessing.OrderResponese(MessageBytes);
                                                SendToClient(MessageBytes[0].ToString(), OrderresponseMessage, false);

                                            }
                                            else
                                            {

                                                string order = "创建要货单失败\n";
                                                LogUtil.Logger.Info(order);
                                               

                                                byte[] OrderresponseMessage = ReadMessage.MessageByteProcessing.OrderResponese(MessageBytes);
                                                SendToClient(MessageBytes[0].ToString(), OrderresponseMessage, false);


                                            }
                                        }
                                        else if (((MessageBytes[8] << 8 | MessageBytes[9]) != 0) && MessageBytes[7] != 0)
                                        {

                                            byte[] OrderresponseMessage = ReadMessage.MessageByteProcessing.OrderResponese(MessageBytes);
                                            SendToClient(MessageBytes[0].ToString(), OrderresponseMessage, false);

                                        }
                                        else
                                        {
                                            byte[] OrderresponseMessage = ReadMessage.MessageByteProcessing.DeleteOrderResponese(MessageBytes);
                                            SendToClient(MessageBytes[0].ToString(), OrderresponseMessage, false);
                                        }


                                      


                                        break;
                                    }
                                case (byte)210:
                                    {
                                        byte[] ResponeBytes = ReadMessage.MessageByteProcessing.Responese(true, MessageBytes);
                                        SendToClient(ReadMessage.Parser.GetValueByIp(client.RemoteEndPoint.ToString()), ResponeBytes, false);
                                        MessageBytes[0] = Convert.ToByte(ReadMessage.Parser.GetValueByIp(client.RemoteEndPoint.ToString()));
                                        //OrderService os = new OrderService();
                                        //var q = os.CreateOrderByLed(client.RemoteEndPoint.ToString(), MessageBytes[4].ToString(), (MessageBytes[8] << 8 | MessageBytes[9]));
                                        //if (q.Success == true)
                                        //{
                                        //    string order = "取消要货单成功，订单编号:" + q.data.id + "\n";
                                        //    LogUtil.Logger.Info(order);
                                        //    byte[] OrderresponseMessage = DeleteOrderResponese(MessageBytes);
                                        //    sendToPTL(MessageBytes[0].ToString(), OrderresponseMessage,false);

                                        //}
                                        //else
                                        //{
                                        string order = "取消要货单失败\n";
                                        LogUtil.Logger.Info(order);
                                        byte[] OrderresponseMessage = ReadMessage.MessageByteProcessing.DeleteOrderResponese(MessageBytes);
                                        SendToClient(MessageBytes[0].ToString(), OrderresponseMessage, false);
                                        //}
                                      
                                        break;
                                    }
                                default: break;

                            }
                        }


                    }
                 

                }
                catch (SocketException ee)
                {

                  
                    LogUtil.Logger.Error(ee.Message);
                    break;


                }

                catch (System.ObjectDisposedException)
                {
                    //LogUtil.Logger.Info("已释放连接");
                    break;
                }
                catch (Exception ee)
                {
                    LogUtil.Logger.Info(ee.Message);
                    break;
                  


                }
            }
        }


        /// <summary>
        /// 定时器 发送给PTL后开启  等待接收
        /// </summary>
        private void SetTimer()
        {
           
                if (IsSent)
                {
                    IsReSent = false;
                    Thread.Sleep(4000);

                    if (IsReceived)
                    {
                        IsReSent = false;
                    }
                    else
                    {
                        IsReSent = true;

                    }
                }
            
        }


        private void SetWMSTimer()
        {
            if (IsWMSSent)
            {
                IsWMSReSent = false;
                Thread.Sleep(4000);

                if (IsWMSReceived)
                {
                    IsWMSReSent = false;
                }
                else
                {
                    IsWMSReSent = true;

                }
            }

        }


        /// <summary>
        /// 消息发送 分配线程
        /// </summary>
        /// <param name="ClientIpKey"></param>
        /// <param name="msg"></param>
        /// <param name="ResentBtn"></param>
        private void SendToClient(string ClientIpKey, byte[] msg, bool ResentBtn)
        {
           
                IsSent = false;
                IsReceived = false;
                ReSentCount = 0;
                IsReSent = false;
                IsWMSSent = false;
                IsWMSReceived = false;
                WMSReSentCount = 0;
                IsWMSReSent = false;
                Thread SendToPtlThread = new Thread(() => SendMsgToClient(ClientIpKey, msg, ResentBtn));
                SendToPtlThread.Start();
            
        }

        /// <summary>
        /// 向客户端发送消息
        /// </summary>
        /// <param name="ClientIpKey"></param>
        /// <param name="msgBody"></param>


        private void SendMsgToClient(string ClientIpKey, byte[] msg, bool ResentBtn)
        {
            byte[] tempmsg = new byte[13] ;
            tempmsg = msg;
            if (ResentBtn)
            {
                SendMsgToClient(ClientIpKey, msg, false);
                if (ClientIpKey != Settings1.Default.WMSKey)//PTL
                {
                    IsSent = true;
                    SetTimer();


                    if (IsReSent == true) //是否已发送
                    {
                        ReSentCount++;
                        msg[5]++;
                    }
                    else
                    {
                        ReSentCount = 0;
                    }
                    if (ReSentCount > 2)
                    {
                        LogUtil.Logger.Error("ptl超时重发超过最大次数");
                        
                        if (ClientIpKey != Settings1.Default.WMSKey)
                        {
                            byte[] ErrorMsg = ReadMessage.MessageByteProcessing.TransmitToErrorMsg(WMSKey, msg, 1);
                            SendMsgToClient(WMSKey, ErrorMsg, false);
                        }
                        IsReceived = false;
                        IsSent = false;
                        IsReSent = false;
                        ReSentCount = 0;


                    }
                    else
                    {
                        while (IsSent == true && IsReceived == false)
                        {
                            SendMsgToClient(ClientIpKey, msg, true);
                        }

                    }
                }
                else//WMS
                {
                    IsWMSSent = true;
                    SetWMSTimer();


                    if (IsWMSReSent == true) //是否已发送
                    {
                        
                            WMSReSentCount++;
                            msg[5]++;
                        
                    }
                    else
                    {
                        WMSReSentCount = 0;
                    }
                    if (WMSReSentCount > 2)
                    {
                        LogUtil.Logger.Error("wms超时重发超过最大次数");
                        IsWMSReceived = false;
                        IsWMSSent = false;
                        IsWMSReSent = false;
                        WMSReSentCount = 0;
                       
                    }
                    else
                    {
                        while (IsWMSSent == true && IsWMSReceived == false)
                        {
                            SendMsgToClient(ClientIpKey, msg, true);
                        }

                    }
                }

            }
            else//不开启重发机制
            {
                if (!clients.Keys.Contains(ClientIpKey))
                {
                    if (ClientIpKey == Settings1.Default.WMSKey)
                    {
                        LogUtil.Logger.Error("WMS未连接/n");
                        
                    }
                    else
                    {
                        LogUtil.Logger.Info("CAN总线地址" + ReadMessage.Parser.GetIpByValue(ClientIpKey) + "未连接/n" +
                       "指令" + ScaleConvertor.HexBytesToString(msg) + "未发送。\n");
                        byte[] ErrorMsg = ReadMessage.MessageByteProcessing.TransmitToErrorMsg(WMSKey, msg, 2);
                        SendMsgToClient(WMSKey, ErrorMsg, false);
                       
                    }
                    return;
                }

                if (clients[ClientIpKey].Connected)
                {
                    clients[ClientIpKey].Send(msg, msg.Length, SocketFlags.None);
                    string SendMeans = "发送给" + clients[ClientIpKey].RemoteEndPoint.ToString() + "," + ReadMessage.Parser.readMessage(msg);
                    if (string.IsNullOrEmpty(ReadMessage.Parser.readMessage(msg)) == false)
                    {
                        LogUtil.Logger.Info("【发送解析】" + SendMeans);
                    }
                }
                else
                {
                    byte[] ErrorMsg = ReadMessage.MessageByteProcessing.TransmitToErrorMsg(WMSKey, msg, 2);
                    SendMsgToClient(WMSKey, ErrorMsg, false);
                    LogUtil.Logger.Info(clients[ClientIpKey].RemoteEndPoint.ToString() + "已断开链接。\n" +
                       "指令" + ScaleConvertor.HexBytesToString(msg) + "未发送。\n"
                  );
                    
                }
            }


        }



      

        /// <summary>
        /// 移除Client
        /// </summary>
        /// <param name="client"></param>
        private void RemoveClient(string IP)
        {
            string key = GetKeyByValue(IP);

            if (this.clients.Keys.Contains(key))
            {
                if (this.clients[key].Connected)
                {
                    this.clients[key].Close();
                }
                this.clients.Remove(key);


            }

            if (this.clientThreads.Keys.Contains(key))
            {

                Thread thisThread = this.clientThreads[key];
                this.clientThreads.Remove(key);
              


            }
        }





        /// <summary>
        /// 通过IP查询key
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetKeyByValue(string value)
        {
            string key = ReadMessage.Parser.GetValueByIp(value);

            if (string.IsNullOrEmpty(key))
            {
                return Settings1.Default.WMSKey;
            }
            else
            {
                return key;
            }

        }


        

    }

}
