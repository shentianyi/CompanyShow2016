using Brilliantech.Framwork.Utils.ConvertUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Brilliantech.Framwork.Utils.LogUtil;
using System.Collections;

namespace TcpDemoWPF
{
    /// <summary>
    /// ClientWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ClientWindow : Window
    {
        public ClientWindow()
        {
            InitializeComponent();
        }
        public static bool runflag = true;
        //private static byte[] buf = new byte[1024];
        Socket tcpClient;
        Thread ClientRecieveThread;
        Queue CmdQueue = new Queue();
        Thread ClientSendThread;
        private int ReceCount=0;
        private int SendNr = 0;
        private int IdCount;
        private byte[] NumBytes;
        private int Count;

       


        private void MakeConnect_Click(object sender, RoutedEventArgs e)
        {
           
            if (!ConnectServer())
            {
                return;
            }

            ClientRecieveThread = new Thread( Listen);
            ClientRecieveThread.IsBackground = true;
            ClientRecieveThread.Start();

            ClientSendThread = new Thread(Send);
            ClientSendThread.IsBackground = true;
            ClientSendThread.Start();
        }

        private void Listen()
        {
            
            while (runflag)
            {
                try
                {
                    //固定字节数13
                    byte[] result = new byte[13];
                    int dataLength = tcpClient.Receive(result);
                    
                       


                    
                    tcpClient.ReceiveTimeout = -1;
                    byte[] MessageBytes = result.Take(dataLength).ToArray();
                    
                    string Receivemeans = ReadMessage.Parser.readMessage(MessageBytes);
                   
                    this.Dispatcher.Invoke(new Action(() => {
                        ReceiveMessageText.AppendText(Receivemeans + "\n");
                        ReceiveMessageText.ScrollToEnd();
                        LogUtil.Logger.Info("【Receive数据】" + ScaleConvertor.HexBytesToString(MessageBytes));
                        ReceCount = ReceCount + 1;

                    }));

                    if (SendNr == Count)
                    {
                        if (ReceCount == Count)
                        {


                            LogUtil.Logger.Info(MessageBytes[4] + "稳定性测试通过");
                            this.Dispatcher.Invoke(new Action(() => { MessageBox.Show("稳定性测试通过"); runflag = false;ReceiveMessageText.Clear();SendMessageText.Clear(); }));
                        }
                        else
                        {
                            LogUtil.Logger.Info("【稳定性测试接收:】" + ReceCount+"条");
                        }
                    }
                 
                  




                }
                catch (SocketException ex)
                {
                    LogUtil.Logger.Error(ex.Message, ex);
                    if (ex.SocketErrorCode == SocketError.TimedOut)
                    {
                        sendMsg(currentCmd, true);
                    }
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message);
                    runflag = false;
                }

            }
        }


        private void Send()
        {
            while (true)
            {
                if (CmdQueue.Count > 0)
                {
                    //System_CAPS_pubmethod	Dequeue()	 移除并返回位于 Queue 开始处的对象 保证先进先出
                    sendMsg(CmdQueue.Dequeue() as byte[]);
                    SendNr = SendNr + 1;
                   Thread.Sleep(Properties.Settings.Default.SleepTime);
                }
            }
        }
        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <returns></returns>
        private bool ConnectServer()
        {
            try
            {
                if (tcpClient == null)
                {
                    tcpClient=  new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                if (tcpClient.Connected)
                {
                    return true;
                }
                //   Socket SocketTcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                tcpClient.Connect(new IPEndPoint(IPAddress.Parse(serverIPTB.Text), int.Parse(serverPortTB.Text)));
                
                MessageBox.Show("连接服务器成功");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }
        



       

        byte[] currentCmd = null;
        int resendCount = 0;
       

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msgBody"></param>
        private void sendMsg(byte[] msgBody, bool isResent = false)
        {
            //SendMessageText.AppendText(ReadMessage.Parser.readMessage(msgBody) + "\n");
            byte[] msg = null;
            if (isResent)
            {
                msg = msgBody;
                resendCount++;
            }
            else
            {
                resendCount = 0;
                msg = msgBody;
            }
            if (resendCount > 2)
            {
                LogUtil.Logger.Error("【发送超时且超出最大发送次数】");
            }
            else
            {
                currentCmd = msg;
                if (ConnectServer())
                {
                    //tcpClient.Send(msg, msg.Length, SocketFlags.None);
                    //Thread.Sleep(1000);
                    this.Dispatcher.Invoke(new Action(() => { tcpClient.Send(msg, msg.Length, SocketFlags.None); SendMessageText.AppendText(ReadMessage.Parser.readMessage(msg) + "\n");SendMessageText.ScrollToEnd(); LogUtil.Logger.Info("【send数据】" + ScaleConvertor.HexBytesToString(msg)); }));

               
                // tcpClient.ReceiveTimeout = 5000;

             
                   
                }
            }
        }

      

        private void ReceiveMessageText_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SendMessageText_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            /// 停止线程
            if (ClientRecieveThread != null && ClientRecieveThread.IsAlive)
            {
                ClientRecieveThread.Abort();
            } 

            /// 关闭client
            if (tcpClient != null)
            {
                tcpClient.Close();
            }
        }

        private void Stabilitybutton_Click(object sender, RoutedEventArgs e)
        {
            IdCount = (Properties.Settings.Default.IdMax) - (Properties.Settings.Default.IdMin)+1;
             Count = IdCount * (Properties.Settings.Default.SendCount);
            for (int SendNr = 0; SendNr < Properties.Settings.Default.SendCount; SendNr++)
            {
                for (int Id = Properties.Settings.Default.IdMin; Id <= Properties.Settings.Default.IdMax; Id++)
            {
                    NumBytes = new byte[2];
                    NumBytes = ScaleConvertor.DecimalToByte((ushort)SendNr);
                    
               
              
                    byte[] msg = new byte[] { 0x88, 0x00, 0x00, 0x01, 0x02, 0x01, 0xC0, 0x01, 0x00, 0x1B, 0xFF, 0xFF, 0x00 };
                    msg[4] = (byte)Id;
                    msg[5] = (byte)SendNr;
                    msg[8] = NumBytes[0];
                    msg[9] = NumBytes[1];
                    CmdQueue.Enqueue(msg);
                    
                }
            }


        }
    }
}
