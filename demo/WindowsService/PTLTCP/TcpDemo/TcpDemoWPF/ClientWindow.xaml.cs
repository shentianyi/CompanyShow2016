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
        Socket PTLtcpClient;
        Socket AGVtcpClient;
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

            if (!ConnectPtlServer())
            {
                return;
            }
            if (!ConnectAgvServer())
            {
                return;
            }
            Thread PtlReceiveThread = new Thread(Listen);
            PtlReceiveThread.IsBackground = true;
            PtlReceiveThread.Start();
            Thread AGVReceiveThread = new Thread(AGVListen);
            AGVReceiveThread.IsBackground = true;
            AGVReceiveThread.Start();


        }

        private void Listen()
        {
            
            while (runflag)
            {
                try
                {
                    //固定字节数13
                    byte[] result = new byte[13];
                    byte[] AGVresult = new byte[13];
                    int dataLength = PTLtcpClient.Receive(result);                   
                    PTLtcpClient.ReceiveTimeout = -1;                

                    byte[] MessageBytes = result.Take(dataLength).ToArray();
                   
                    if (MessageBytes[0] != 0)
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            ReceiveMessageText.AppendText(ScaleConvertor.HexBytesToString(MessageBytes) + "\n");
                            ReceiveMessageText.ScrollToEnd();
                        }));
                    }
                    if(MessageBytes[6]==238)
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            ReceiveMessageText.AppendText(ScaleConvertor.HexBytesToString(MessageBytes) + "\n");
                            ReceiveMessageText.ScrollToEnd();
                        }));
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



        private void AGVListen()
        {

            while (runflag)
            {
                try
                {
                    //固定字节数13
                   
                    byte[] AGVresult = new byte[13];
                    int AGVdataLength = AGVtcpClient.Receive(AGVresult);
                    AGVtcpClient.ReceiveTimeout = -1;

                    byte[] MessageBytes = AGVresult.Take(AGVdataLength).ToArray();

                    if (MessageBytes[0] != 0)
                    {
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            ReceiveMessageText.AppendText(ScaleConvertor.HexBytesToString(MessageBytes) + "\n");
                            ReceiveMessageText.ScrollToEnd();
                        }));
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


       
        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <returns></returns>
        private bool ConnectPtlServer()
        {
            try
            {
                if (PTLtcpClient == null)
                {
                    PTLtcpClient=  new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                if (PTLtcpClient.Connected)
                {
                    return true;
                }
                //   Socket SocketTcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                PTLtcpClient.Connect(new IPEndPoint(IPAddress.Parse(serverIPTB.Text), int.Parse(serverPortTB.Text)));

                MessageBox.Show("连接服务器" + serverIPTB.Text+serverPortTB.Text+ " 成功");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }

        private bool ConnectAgvServer()
        {
            try
           {
                if (AGVtcpClient == null)
                {
                    AGVtcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                if (AGVtcpClient.Connected)
                {
                    return true;
                }
                //   Socket SocketTcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                AGVtcpClient.Connect(new IPEndPoint(IPAddress.Parse(AgvServerIp.Text), int.Parse(AgvServerPortTB.Text)));
                
                MessageBox.Show("连接服务器"+ AgvServerIp.Text+ AgvServerPortTB.Text + " 成功");
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
                if (ConnectPtlServer())
                {
                    //tcpClient.Send(msg, msg.Length, SocketFlags.None);
                    //Thread.Sleep(1000);
                    this.Dispatcher.Invoke(new Action(() => { PTLtcpClient.Send(msg, msg.Length, SocketFlags.None); SendMessageText.AppendText(ReadMessage.Parser.readMessage(msg) + "\n");SendMessageText.ScrollToEnd(); LogUtil.Logger.Info("【send数据】" + ScaleConvertor.HexBytesToString(msg)); }));

               
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
            if (PTLtcpClient != null)
            {
                PTLtcpClient.Close();
            }
            if(AGVtcpClient!=null)
            {
                AGVtcpClient.Close();
            }
        }

     



        private void Stabilitybutton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                byte[] msg = new byte[] { 0x88, 0x00, 0x00, 0x01, 0x02, 0x01, 0xC0, 0x01, 0x00, 0x1B, 0xFF, 0xFF, 0x00 };
                byte[] AGVmsg = new byte[] { 0xFD,0x13,0x00,0x06,0x0F,0x01,0x00,0x03,0x02,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00 };

                if (string.IsNullOrEmpty(StationtextBox.Text))
                {
                    AGVmsg = new byte[] { 0x88, 0x00, 0x00, 0x01, 0x02, 0x01, 0xC0, 0x01, 0x00, 0x1B, 0xFF, 0xFF, 0x00 };
                }
                else
                {
                    AGVmsg = new byte[] { 0xFD, 0x13, 0x00, 0x06, 0x0F, 0x01, 0x00, 0x03, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    AGVmsg[7] = (byte)int.Parse(StationtextBox.Text);
                    
                }

                if (string.IsNullOrEmpty(PTLIPtextBox.Text) || string.IsNullOrEmpty(PTLNrtextBox.Text))
                {
                    msg = new byte[] { 0x88, 0x00, 0x00, 0x01, 0x02, 0x01, 0xC0, 0x01, 0x00, 0x1B, 0xFF, 0xFF, 0x00 };


                }
                else
                {
                    msg = new byte[] { 0x88, 0x00, 0x00, 0x01, 0x02, 0x01, 0xC0, 0x01, 0x00, 0x1B, 0xFF, 0xFF, 0x00 };
                    msg[0] = (byte)int.Parse(PTLIPtextBox.Text);
                    msg[4] = (byte)int.Parse(PTLNrtextBox.Text);
                }
                //PTLtcpClient.Send(msg);
                //AGVtcpClient.Send(msg);
                SendMessageText.AppendText(ScaleConvertor.HexBytesToString(msg) + "\n");
                SendMessageText.AppendText(ScaleConvertor.HexBytesToString(AGVmsg) + "\n");
                SendMessageText.ScrollToEnd();
            }
            catch(Exception ee)
            {
                MessageBox.Show(ee.Message);
                LogUtil.Logger.Error(ee.Message);
            }
            

        }

        private void AgvServerPortTB_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void AgvServerIp_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void StationtextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PTLIPtextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PTLNrtextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                byte[] msg = new byte[] { 0x88, 0x00, 0x00, 0x01, 0x02, 0x01, 0xC0, 0x01, 0x00, 0x1B, 0xFF, 0xFF, 0x00 };
                //byte[] AGVmsg = new byte[] { 0xFD, 0x13, 0x00, 0x06, 0x0F, 0x01, 0x00, 0x03, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                //if (string.IsNullOrEmpty(StationtextBox.Text))
                //{
                //    AGVmsg = new byte[] { 0x88, 0x00, 0x00, 0x01, 0x02, 0x01, 0xC0, 0x01, 0x00, 0x1B, 0xFF, 0xFF, 0x00 };
                //}
                //else
                //{
                //    AGVmsg = new byte[] { 0xFD, 0x13, 0x00, 0x06, 0x0F, 0x01, 0x00, 0x03, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                //    AGVmsg[7] = (byte)int.Parse(StationtextBox.Text);

                //}

                if (string.IsNullOrEmpty(PTLIPtextBox.Text) || string.IsNullOrEmpty(PTLNrtextBox.Text))
                {
                    msg = new byte[] { 0x88, 0x00, 0x00, 0x01, 0x02, 0x01, 0xC0, 0x01, 0x00, 0x1B, 0xFF, 0xFF, 0x00 };


                }
                else
                {
                    msg = new byte[] { 0x88, 0x00, 0x00, 0x01, 0x02, 0x01, 0xC0, 0x01, 0x00, 0x1B, 0xFF, 0xFF, 0x00 };
                    msg[0] = (byte)int.Parse(PTLIPtextBox.Text);
                    msg[4] = (byte)int.Parse(PTLNrtextBox.Text);
                }
                PTLtcpClient.Send(msg);
                //AGVtcpClient.Send(msg);
                SendMessageText.AppendText(ScaleConvertor.HexBytesToString(msg) + "\n");
                //SendMessageText.AppendText(ScaleConvertor.HexBytesToString(AGVmsg) + "\n");
                SendMessageText.ScrollToEnd();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                LogUtil.Logger.Error(ee.Message);
            }
        }
    }
}
