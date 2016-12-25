using Brilliantech.Framwork.Utils.ConvertUtil;
using Brilliantech.Framwork.Utils.LogUtil;
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
using TcpDemoWPF.message;

namespace TcpDemoWPF
{
    /// <summary>
    /// TestViewWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestViewWindow : Window
    {
      

        public TestViewWindow()
        {
            InitializeComponent();
        }

        Dictionary<string, Socket> clients;
        public TestViewWindow(Dictionary<string, Socket> clients)
        {
            InitializeComponent();
            this.clients = clients;
        }

        private Socket getClientSocket()
        {
            string ip = clientIpTB.Text;
            int port = int.Parse(clientPortTB.Text);

            return clients[ip + ":" + port];
        }

        private void dingwei_Click(object sender, RoutedEventArgs e)
        {
            Socket socket = null;
            TcpClientService tcs = new TcpClientService();
            byte[] msg = new byte[] {
                0xFD, 0x13, 0x00, 0x06, 0x0F, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

            if (isManualCB.IsChecked == true)
            {
                msg = ScaleConvertor.HexStringToHexByte(cmdTB.Text);
            }
            MessageBox.Show(ScaleConvertor.HexBytesToString(msg));
            
            socket = getClientSocket();

            if (socket == null)
            {
                MessageBox.Show("服务器连接已断开....");
                return;
            }
            
            Message<Socket> rep = tcs.SendMessage(socket, msg);

            if (rep.result)
            {
                //MessageBox.Show("开始接收数据...");
                //byte[] recvBytes = new byte[1024];
                //int bytes = 0;
                //bytes = rep.data.Receive(recvBytes, recvBytes.Length, 0);
                //MessageBox.Show(ScaleConvertor.HexBytesToString(recvBytes));
                //MessageBox.Show(Encoding.Default.GetString(recvBytes));
                //rep.data.Shutdown(SocketShutdown.Both);
                //rep.data.Close();
                MessageBox.Show("结束通讯...");
            }
            else
            {
                MessageBox.Show("发送失败...");
            }

            return;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string clentIp = clientIpTB.Text;
            string clentPort = clientPortTB.Text;
            string ss = clentIp + ":" + clentPort;

            if (clients.Keys.Contains(clentIp+":"+ clentPort))
            {
                MessageBox.Show("客户端已连接，开始测试...");
            }
            else
            {
                clientIpTB.Text = "";
                MessageBox.Show("客户端未连接，请确认已连接！");
            }
        }

        private void sendDesStation(byte[] msg)
        {
            Socket socket = null;
            TcpClientService tcs = new TcpClientService();

            if (isManualCB.IsChecked == true)
            {
                msg = ScaleConvertor.HexStringToHexByte(cmdTB.Text);
            }
            MessageBox.Show(ScaleConvertor.HexBytesToString(msg));

            socket = getClientSocket();

            if (socket == null)
            {
                MessageBox.Show("服务器连接已断开....");
                return;
            }

            Message<Socket> rep = tcs.SendMessage(socket, msg);

            if (rep.result)
            {
                //MessageBox.Show("开始接收数据...");
                //byte[] recvBytes = new byte[1024];
                //int bytes = 0;
                ////bytes = rep.data.Receive(recvBytes, recvBytes.Length, 0);
                ////MessageBox.Show(ScaleConvertor.HexBytesToString(recvBytes));
                ////MessageBox.Show(Encoding.Default.GetString(recvBytes));
                ////rep.data.Shutdown(SocketShutdown.Both);
                ////rep.data.Close();
                MessageBox.Show("结束通讯...");
            }
            else
            {
                MessageBox.Show("发送失败...");
            }

            return;
        }

        private void TestViewWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(clients.Count() > 0)
            {
                string ipPort = clients.Keys.First();
                string []ss = ipPort.Split(':');
                //clientIpTB.Text = ipPort.Split(new[] { ":" });
                clientIpTB.Text = ss[0];
                clientPortTB.Text = ss[1];
            }
            
            comboBox.Items.Add("前进");
            comboBox.Items.Add("后退");
            comboBox.SelectedIndex = 0;

            return;
        }

        private void resetBT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = new byte[] {
                0xFD, 0x13, 0x00, 0x06, 0x0F, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

            sendDesStation(msg);
        }

        private void stopBT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = new byte[] {
                0xFD, 0x13, 0x00, 0x06, 0x0F, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

            sendDesStation(msg);
        }

        private void startBT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = new byte[] {
                0xFD, 0x13, 0x00, 0x06, 0x0F, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

            sendDesStation(msg);
        }

        private void workOnBT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = new byte[] {
                0xFD, 0x13, 0x00, 0x06, 0x0F, 0xfa, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

            sendDesStation(msg);
        }

        private void workOffBT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = new byte[] {
                0xFD, 0x13, 0x00, 0x06, 0x0F, 0xfc, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

            sendDesStation(msg);
        }

        private byte[] station_msg = new byte[] {
                                                          //方向
                0xFD, 0x13, 0x00, 0x06, 0x0F, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };
        private void station1_click(object sender, RoutedEventArgs e)
        {
            //byte[] msg = new byte[] {
            //                                              //方向
            //    0xFD, 0x13, 0x00, 0x06, 0x0F, 0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            //};

            byte[] msg = station_msg;
            msg[7] = 0x01;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station2BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x02;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station3BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x03;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station4BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x04;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station5BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x05;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station6BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x06;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station7BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x07;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station8BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x08;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station9BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x09;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station10BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x0a;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station11BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x0b;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station12BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x0c;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station13BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x0d;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station14BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x0e;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station15BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x0f;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station16BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x10;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station17BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x11;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station18BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x12;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station19BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x13;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station20BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x14;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station21BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x15;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station22BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x16;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station23BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x17;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station24BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x18;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station25BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x19;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station26BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x1a;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station27BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x1b;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station28BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x1c;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station29BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x1d;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station30BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x1e;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station31BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x1f;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station32BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x20;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station33BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x21;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station34BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x22;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }

        private void station35BT_Click(object sender, RoutedEventArgs e)
        {
            byte[] msg = station_msg;
            msg[7] = 0x23;

            if (comboBox.SelectedValue == "前进")
            {
                msg[8] = 0x01;
            }
            else
            {
                msg[8] = 0x02;
            }

            sendDesStation(msg);
        }
    }
}
