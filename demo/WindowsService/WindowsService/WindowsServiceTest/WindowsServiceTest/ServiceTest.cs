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
        //Thread CheckConnectedThread;
        bool runflag = true;

        Dictionary<string, Socket> clients = new Dictionary<string, Socket>();
        Dictionary<string, Thread> clientThreads = new Dictionary<string, Thread>();
        private string PTLIp = string.Empty;
        private string WMSIp = string.Empty;
        private string PTLKey = string.Empty;
        private string WMSKey = Settings1.Default.WMSKey;
        //private bool IsReceived = false;
        //private bool IsReSent = false;
        //private int ReSentCount = 0;
        //private bool IsSent = false;
        //private bool IsResp = false;
        //private bool IsReResp = false;
        //private bool IsReceResp = false;
        //private int RespCount = 0;



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
                ////CheckConnectedThread = new Thread(CheckConnected);
                ////CheckConnectedThread.IsBackground = true;
                ////CheckConnectedThread.Start();


            }
            catch (Exception ee)
            {
                LogUtil.Logger.Error(ee.Message);
            }
        }



        //public void OnStart()
        //{


        //    LogUtil.Logger.Info("【开启服务】" + DateTime.Now);
        //    IPAddress ip = IPAddress.Parse(Settings1.Default.ServerIp);
        //    int port = int.Parse(Settings1.Default.ServerPort);
        //    tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    tcpServer.Bind(new IPEndPoint(ip, port));  //绑定IP地址：端口  
        //    tcpServer.Listen(10);    //设定最多10个排队连接请求  
        //    LogUtil.Logger.Info(string.Format("启动监听{0}成功", tcpServer.LocalEndPoint.ToString()));
        //    ClientRecieveThread = new Thread(ListenConnnect);
        //    ClientRecieveThread.IsBackground = true;
        //    ClientRecieveThread.Start();
        //    //CheckConnectedThread = new Thread(CheckConnected);
        //    //CheckConnectedThread.IsBackground = true;
        //    //CheckConnectedThread.Start();

        //    while (true)
        //    {
        //        string a = "Starting";
        //    }


        //}

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
                    //clients.Remove(n.Key);

                }
            }




            //foreach (var t in clientThreads)
            //{
            //    if (t.Value != null)
            //    {

            //        clientThreads.Remove(t.Key);


            //    }
            //}


            /// 关闭server
            //if (tcpServer != null)
            //{
            //    tcpServer.Close();
            //}

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
                    //string[] justIp = client.RemoteEndPoint.ToString().Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    //string realWMSIp = justIp[0];
                    //WMSIp = Settings1.Default.WMSIp;

                    //    if (realWMSIp.Equals(WMSIp))
                    //{
                    //        string key = WMSKey;
                    //        WMSIp = client.RemoteEndPoint.ToString();
                    //        if (clients.ContainsKey(key) == false)
                    //        {
                    //            clients.Add(key, client);
                    //        }
                    //        LogUtil.Logger.Info("WMS编号：" + key + ",灯ip" + client.RemoteEndPoint.ToString() + "已连接服务器");
                    //        Thread clientThread = new Thread(ListenClientMsg);
                    //        clientThread.IsBackground = true;
                    //        clientThread.Start(client);
                    //        if (clientThreads.ContainsKey(key)==false)
                    //        {
                    //            clientThreads.Add(key, clientThread);
                    //        }
                    //    }
                    //else
                    //{



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

                    //IsReceived = false;

                    byte[] result = new byte[13];
                    int dataLength = client.Receive(result);

                    //if (IsSent && (dataLength > 0) && (result[6] != 0))
                    //{
                    //    IsReceived = true;
                    //}
                    //if (IsResp && (result[6] == 254))
                    //{
                    //    IsReceResp = true;
                    //}

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
                                byte[] ResponeBytes = Transmit(MessageBytes);//指令内容
                                WMSIp = client.RemoteEndPoint.ToString();

                                PTLKey = MessageBytes[0].ToString();//目标灯Ip
                                sendMsgToClient(PTLKey, ResponeBytes);

                                //while (IsReceived == false && IsReSent == true)
                                //{
                                //    sendMsgToClient(PTLKey, ResponeBytes);

                                //}
                                //IsReSent = false;
                            }

                        }
                        else
                        {

                            switch (result[6])
                            {
                                case (byte)176:
                                    {
                                        if (string.IsNullOrEmpty(PTLKey))
                                        {
                                            break;
                                        }
                                        MessageBytes[0] = Convert.ToByte(PTLKey);
                                        sendMsgToWMS(WMSKey, MessageBytes);
                                        PTLKey = string.Empty;
                                        break;
                                    }
                                ///缺货报警
                                case (byte)209:
                                    {
                                        byte[] ResponeBytes = Responese(true, MessageBytes);
                                        sendMsgToClient(ReadMessage.Parser.GetValueByIp(client.RemoteEndPoint.ToString()), ResponeBytes);
                                        MessageBytes[0] = Convert.ToByte(ReadMessage.Parser.GetValueByIp(client.RemoteEndPoint.ToString()));

                                        //sendMsgToWMS(WMSKey, MessageBytes);
                                        if (((MessageBytes[8] << 8 | MessageBytes[9]) != 0)&&MessageBytes[7]==0 )
                                        {
                                            
                                            OrderService os = new OrderService();
                                            var q = os.CreateOrderByLed(client.RemoteEndPoint.ToString(), MessageBytes[4].ToString(), (MessageBytes[8] << 8 | MessageBytes[9]));
                                            if (q.Success)
                                            {
                                                string order = "创建要货单成功，订单编号:" + q.data.id + "\n";
                                                LogUtil.Logger.Info(order);
                                                byte[] OrderresponseMessage = OrderResponese(MessageBytes);
                                                sendMsgToClient(MessageBytes[0].ToString(), OrderresponseMessage);

                                            }
                                            else
                                            {

                                                string order = "创建要货单失败\n";
                                                LogUtil.Logger.Info(order);
                                                //LogUtil.Logger.Error(q.Message.ToString());
                                                byte[] OrderresponseMessage = OrderResponese(MessageBytes);
                                                sendMsgToClient(MessageBytes[0].ToString(), OrderresponseMessage);
                                                //byte[] OrderresponseMessage = DeleteOrderResponese(MessageBytes);
                                                //sendMsgToClient(MessageBytes[0].ToString(), OrderresponseMessage);

                                            }
                                        }
                                        else if (((MessageBytes[8] << 8 | MessageBytes[9]) != 0) && MessageBytes[7] != 0)
                                        {

                                            byte[] OrderresponseMessage = OrderResponese(MessageBytes);
                                            sendMsgToClient(MessageBytes[0].ToString(), OrderresponseMessage);

                                        }
                                        else
                                        {
                                            byte[] OrderresponseMessage = DeleteOrderResponese(MessageBytes);
                                            sendMsgToClient(MessageBytes[0].ToString(), OrderresponseMessage);
                                        }


                                        //while (IsReceResp == false && IsReResp == true)
                                        //{
                                        //    sendMsgToWMS(WMSKey, MessageBytes);
                                        //}


                                        break;
                                    }
                                case (byte)210:
                                    {
                                        byte[] ResponeBytes = Responese(true, MessageBytes);
                                        sendMsgToClient(ReadMessage.Parser.GetValueByIp(client.RemoteEndPoint.ToString()), ResponeBytes);
                                        MessageBytes[0] = Convert.ToByte(ReadMessage.Parser.GetValueByIp(client.RemoteEndPoint.ToString()));
                                        //OrderService os = new OrderService();
                                        //var q = os.CreateOrderByLed(client.RemoteEndPoint.ToString(), MessageBytes[4].ToString(), (MessageBytes[8] << 8 | MessageBytes[9]));
                                        //if (q.Success == true)
                                        //{
                                        //    string order = "取消要货单成功，订单编号:" + q.data.id + "\n";
                                        //    LogUtil.Logger.Info(order);
                                        //    byte[] OrderresponseMessage = DeleteOrderResponese(MessageBytes);
                                        //    sendMsgToClient(MessageBytes[0].ToString(), OrderresponseMessage);

                                        //}
                                        //else
                                        //{
                                            string order = "取消要货单失败\n";
                                            LogUtil.Logger.Info(order);
                                            byte[] OrderresponseMessage = DeleteOrderResponese(MessageBytes);
                                            sendMsgToClient(MessageBytes[0].ToString(), OrderresponseMessage);
                                        //}
                                        ////sendMsgToWMS(WMSKey, MessageBytes);
                                        ////while (IsReceResp == false && IsReResp == true)
                                        ////{
                                        ////    sendMsgToWMS(WMSKey, MessageBytes);
                                        ////}
                                        break;
                                    }
                                default: break;

                            }
                        }


                    }
                    //else
                    //{
                    //    if (IsReSent)
                    //    {

                    //    }
                    //}
                    //IsSent = false;
                    //IsReSent = false;
                    //IsReceived = false;
                    //ReSentCount = 0;
                    //IsReResp = false;
                    //IsResp = false;
                    //IsReceResp = false;
                    //RespCount = 0;

                }
                catch (SocketException ee)
                {

                    //RemoveClient(client.RemoteEndPoint.ToString());
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
                    //    //if (runflag)
                    //    //{
                    //    //    //RemoveClient(client.RemoteEndPoint.ToString());

                    //    //}


                }
            }
        }


        /// <summary>
        /// 定时器 发送给PTL后开启  等待接收
        /// </summary>
        //private void SetTimer()
        //{
        //    if (IsSent)
        //    {
        //        IsReSent = false;
        //        Thread.Sleep(600);

        //        if (IsReceived)
        //        {
        //            IsReSent = false;
        //        }
        //        else
        //        {
        //            IsReSent = true;

        //        }
        //    }

        //}


        //private void SetReTimer()
        //{
        //    if (IsResp)
        //    {
        //        IsReResp = false;
        //        Thread.Sleep(600);

        //        if (IsReceResp)
        //        {
        //            IsReResp = false;
        //        }
        //        else
        //        {
        //            IsReResp = true;

        //        }
        //    }

        //}


        /// <summary>
        /// 向客户端发送消息
        /// </summary>
        /// <param name="clientIP"></param>
        /// <param name="msgBody"></param>
        private void sendMsgToClient(string clientIP, byte[] msg)
        {


            //if (IsReSent)
            //{
            //    ReSentCount++;
            //}
            //else
            //{
            //    ReSentCount = 0;
            //}
            //if (ReSentCount > 2)
            //{
            //    LogUtil.Logger.Error("超时重发超过最大次数");
            //    IsReceived = false;
            //    IsSent = false;
            //    IsReSent = false;
            //    byte[] ErrorMsg = TransmitToErrorMsg(WMSKey, msg,1);                
            //    sendMsgToWMS(WMSKey, ErrorMsg);
            //    IsReResp = false;
            //    IsResp = false;
            //    IsReceResp = false;
            //    RespCount = 0;

            //}
            //else
            //{
            if (!clients.Keys.Contains(clientIP))
            {
                LogUtil.Logger.Info("目标地址" + ReadMessage.Parser.GetIpByValue(clientIP) + "未连接");
                return;
            }
            if (clients[clientIP].Connected)
            {
                clients[clientIP].Send(msg, msg.Length, SocketFlags.None);
                string SendMeans = "发送给" + ReadMessage.Parser.GetIpByValue(clientIP) + "," + ReadMessage.Parser.readMessage(msg);
                if (string.IsNullOrEmpty(ReadMessage.Parser.readMessage(msg)) == false)
                {
                    LogUtil.Logger.Info("【发送解析】" + SendMeans);
                }
            }
            else
            {
                LogUtil.Logger.Error(clients[clientIP].RemoteEndPoint.ToString() + "已断开");
            }
            //    IsSent = true;
            //    SetTimer();

            //}


        }

        /// <summary>
        /// 将从PTL接受到的消息转发给WMS
        /// </summary>
        /// <param name="IPKey"></param>
        /// <param name="msg"></param>
        private void sendMsgToWMS(string IPKey, byte[] msg)
        {
            //if (IsReResp)
            //{
            //    RespCount++;
            //}
            //else
            //{
            //    RespCount = 0;
            //}
            //if (RespCount > 2)
            //{
            //    LogUtil.Logger.Error("超时重发给神秘小车超过最大次数");
            //    byte[] ErrorMsg = TransmitToErrorMsg(msg[0].ToString(), msg,2);
            //    LogUtil.Logger.Info(ReadMessage.Parser.readMessage(ErrorMsg));
            //    IsReceResp = false;
            //    IsReResp = false;
            //    IsResp = false;
            //    RespCount = 0;
            //}
            //else
            //{

            if (clients.ContainsKey(IPKey))
            {
                string SendMeans = "发送给" + clients[IPKey].RemoteEndPoint.ToString() + "," + ReadMessage.Parser.readMessage(msg);

                if (clients[IPKey].Connected)
                {
                    clients[IPKey].Send(msg, msg.Length, SocketFlags.None);
                    if (string.IsNullOrEmpty(ReadMessage.Parser.readMessage(msg)) == false)
                    {
                        LogUtil.Logger.Info("【发送解析】" + SendMeans);
                    }
                    //    IsResp = true;
                    //    SetReTimer();

                }

                else
                {
                    LogUtil.Logger.Info(clients[IPKey].RemoteEndPoint.ToString() + "已断开链接。\n" +
                        "指令" + ScaleConvertor.HexBytesToString(msg) + "未发送。\n" +
                   "指令解析" + SendMeans);
                }


            }

            else
            {
                LogUtil.Logger.Info("【错误解析】" + "WMS未连接");
            }

            //}


        }

        /// <summary>
        /// 移除Client
        /// </summary>
        /// <param name="client"></param>
        private void RemoveClient(string IPKey)
        {
            string key = GetKeyByValue(IPKey);
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
                //thisThread.Abort();


            }
        }





        /// <summary>
        /// 通过IP查询key
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetKeyByValue(string value)
        {
            if (string.IsNullOrWhiteSpace(ReadMessage.Parser.GetValueByIp(value)))
            {
                return Settings1.Default.WMSKey;
            }
            else
            {
                return ReadMessage.Parser.GetValueByIp(value);
            }

        }






        /// <summary>
        /// 主动汇报或取消时 发送回复
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public byte[] Responese(bool pre, byte[] msg)
        {
            if (msg.Count() == 13)
            {
                string mean = string.Empty;

                int LampId = msg[4];


                string back = string.Format("88{0}{1}B0{2}{3}{4}000000",
                   ScaleConvertor.DecimalToHexString(LampId + 256, true, 8),
                   ScaleConvertor.DecimalToHexString(msg[5], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[7], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[8], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[9], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[10], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[11], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[12], true, 2));

                byte[] bback = ScaleConvertor.HexStringToHexByte(back);

                return bback;
            }


            else
            {
                return null;
            }
        }


        /// <summary>
        /// WMS发送给PTL
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public byte[] TransmitToErrorMsg(string Ipkey, byte[] msg, int ErrorNr)
        {
            if (msg.Count() == 13)
            {
                string mean = string.Empty;

                int LampId = msg[4];

                string back = string.Format("{0}{1}{2}EE{3}{4}00000000",
                  ScaleConvertor.DecimalToHexString(int.Parse(Ipkey), true, 2),
                  ScaleConvertor.DecimalToHexString(LampId + 256, true, 8),
                   ScaleConvertor.DecimalToHexString(msg[5], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[7], true, 2),
                   ScaleConvertor.DecimalToHexString(ErrorNr, true, 2));


                byte[] bback = ScaleConvertor.HexStringToHexByte(back);

                return bback;
            }


            else
            {
                return null;
            }
        }

        public byte[] Transmit(byte[] msg)
        {
            if (msg.Count() == 13)
            {
                string mean = string.Empty;

                int LampId = msg[4];

                string back = string.Format("88{0}{1}C0{2}{3}{4}{5}{6}{7}",
                   ScaleConvertor.DecimalToHexString(LampId + 256, true, 8),
                   ScaleConvertor.DecimalToHexString(msg[5], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[7], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[8], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[9], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[10], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[11], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[12], true, 2));

                byte[] bback = ScaleConvertor.HexStringToHexByte(back);

                return bback;
            }


            else
            {
                return null;
            }
        }

        public byte[] OrderResponese(byte[] msg)
        {
            if (msg.Count() == 13)
            {
                string mean = string.Empty;

                int LampId = msg[4];


                string back = string.Format("88{0}{1}C001{2}{3}0000FF",
                   ScaleConvertor.DecimalToHexString(LampId + 256, true, 8),                                  
                   ScaleConvertor.DecimalToHexString(msg[5], true, 2),
                    ScaleConvertor.DecimalToHexString(msg[8], true, 2),
                   ScaleConvertor.DecimalToHexString(msg[9], true, 2)
                  );

                byte[] bback = ScaleConvertor.HexStringToHexByte(back);

                return bback;
            }


            else
            {
                return null;
            }
        }


        public byte[] DeleteOrderResponese(byte[] msg)
        {
            if (msg.Count() == 13)
            {
                string mean = string.Empty;

                int LampId = msg[4];


                string back = string.Format("88{0}{1}C0000000000000",
                   ScaleConvertor.DecimalToHexString(LampId + 256, true, 8),
                   ScaleConvertor.DecimalToHexString(msg[5], true, 2)                

                  );

                byte[] bback = ScaleConvertor.HexStringToHexByte(back);

                return bback;
            }


            else
            {
                return null;
            }
        }


    }

}
