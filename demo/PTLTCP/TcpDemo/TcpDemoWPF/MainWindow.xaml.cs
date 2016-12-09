using Brilliantech.Framwork.Utils.LogUtil;
using Brilliantech.Framwork.Utils.ConvertUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;
using System.Threading;



namespace TcpDemoWPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        //    private static int myProt = 8080;   //端口  
        Socket tcpServer;
        Thread ClientRecieveThread;
        bool runflag = true;

        Dictionary<string, Socket> clients = new Dictionary<string, Socket>();
        Dictionary<string, Thread> clientThreads = new Dictionary<string, Thread>();


        private void MakeConnection_Click(object sender, RoutedEventArgs e)
        {
            IPAddress ip = IPAddress.Parse(serverIPTB.Text);
            tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpServer.Bind(new IPEndPoint(ip, int.Parse(serverPortTB.Text)));  //绑定IP地址：端口  
            tcpServer.Listen(10);    //设定最多10个排队连接请求  

            MessageBox.Show(string.Format("启动监听{0}成功", tcpServer.LocalEndPoint.ToString()));
            MakeConnection.Content = "【服务器已启动】";
            // SocketTcpAccept.Send(serversay);
            ClientRecieveThread = new Thread(ListenConnnect);
            ClientRecieveThread.IsBackground = true;
            ClientRecieveThread.Start();
        }

        private void ListenConnnect()
        {
            while (runflag)
            {
                Socket client = tcpServer.Accept();

                try
                {
                    this.Dispatcher.Invoke(new Action(() => { clientLB.Items.Add(client.RemoteEndPoint.ToString()); }));
                    string key = GetClientKey(client);

                    clients.Add(key, client);
                    Thread clientThread = new Thread(ListenClientMsg);
                    clientThread.IsBackground = true;
                    clientThread.Start(client);
                    clientThreads.Add(key, clientThread);

                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message);
                    LogUtil.Logger.Info(ee.Message);
                    RemoveClient(client);
                }

            }

        }

        private void ListenClientMsg(object cliento)
        {
            Socket client = cliento as Socket;
            try
            {
                while (true)
                {
                    byte[] result = new byte[13];
                    int dataLength = client.Receive(result);
                    if (dataLength > 0)
                    {
                        byte[] MessageBytes = result.Take(dataLength).ToArray();
                        //发送回复
                        switch (result[6])
                        {
                            case (byte)209:
                                {
                                    byte[] ResponeBytes = Responese(MessageBytes);
                                    sendMsgToClient(client.RemoteEndPoint.ToString(), ResponeBytes);
                                    LogUtil.Logger.Info(string.Format("发送提交指令的回复: " + ScaleConvertor.HexBytesToString(ResponeBytes)));
                                    break;
                                }
                            case (byte)210:
                                {
                                    byte[] ResponeBytes = Responese(MessageBytes);
                                    sendMsgToClient(client.RemoteEndPoint.ToString(), ResponeBytes);
                                    LogUtil.Logger.Info(string.Format("发送取消指令的回复: " + ScaleConvertor.HexBytesToString(ResponeBytes)));
                                    break;
                                }
                            default: break;

                        }



                        //接收数据解析
                        string Receivemeans = ReadMessage.Parser.readMessage(MessageBytes);
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            ReceiveText.AppendText(Receivemeans + "\n");
                        }));
                        LogUtil.Logger.Info("【数据】" + ScaleConvertor.HexBytesToString(MessageBytes));
                        LogUtil.Logger.Info("【解析】" + Receivemeans);




                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                LogUtil.Logger.Info(ee.Message);
                RemoveClient(client);
            }
        }

        /// <summary>
        /// 向客户端发送消息
        /// </summary>
        /// <param name="clientIP"></param>
        /// <param name="msgBody"></param>
        private void sendMsgToClient(string clientIP, byte[] msg)
        {

            clients[clientIP].Send(msg, msg.Length, SocketFlags.None);
            string SendMeans = ReadMessage.Parser.readMessage(msg);
            LogUtil.Logger.Info("【解析】" + SendMeans);

            this.Dispatcher.Invoke(new Action(() => { SendText.AppendText(SendMeans + "\n"); }));
        }

        /// <summary>
        /// 移除Client
        /// </summary>
        /// <param name="client"></param>
        private void RemoveClient(Socket client)
        {
            string key = GetClientKey(client);
            if (this.clients.Keys.Contains(key))
            {
                if (this.clients[key].Connected)
                {
                    this.clients[key].Close();
                }
                this.clients.Remove(key);
                this.Dispatcher.Invoke(new Action(() =>
                {
                    clientLB.Items.Remove(key);
                }));

            }

            if (this.clientThreads.Keys.Contains(key))
            {
                if (this.clientThreads[key].IsAlive)
                {
                    this.clientThreads[key].Abort();
                }
                this.clientThreads.Remove(key);
            }
        }

        /// <summary>
        /// 窗口关闭时停止线程等
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            /// 停止Client监听
            foreach (var t in clientThreads)
            {
                if (t.Value.IsAlive)
                {
                    t.Value.Abort();
                }
            }

            /// 停止线程
            if (ClientRecieveThread != null && ClientRecieveThread.IsAlive)
            {
                ClientRecieveThread.Abort();
            }

            /// 关闭server
            if (tcpServer != null)
            {
                tcpServer.Close();
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            new ClientWindow().Show();
        }

        private string GetClientKey(Socket client)
        {
            //获得Client的IP和端口
            return client.RemoteEndPoint.ToString();
        }



        /// <summary>
        /// 主动汇报或取消时 发送回复
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public byte[] Responese(byte[] msg)
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

        private void serverIPTB_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}