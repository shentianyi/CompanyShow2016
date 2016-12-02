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
using Brilliantech.Framwork.Utils.LogUtil;
using Brilliantech.Framwork.Utils.ConvertUtil;
using System.Threading;
using PTLCanTest.Properties;
using System.IO.Ports;
using PTLCanTest.Services;


namespace PTLCanTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool BtnTest = false;
        private int LampId = 0;
        PTLCanController.Controller controller;
        public MainWindow()
        {
            InitializeComponent();
            this.Closing += Window_Closing;
        }

        /// <summary>
        /// 主程序入口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartTestbutton_Click(object sender, RoutedEventArgs e)
        {
            /*
            OffConnTest();
            SetId();
            CheckId();
            NumTest(LampId);
            ColorTest(LampId);
            StabilityTest ss = new StabilityTest();
            ss.Stability(LampId);
            */
            
            PressAfterShow PAS = new PressAfterShow();
            PAS.Stability(0);
            
        }


        /// <summary>
        /// 窗体关闭程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (controller != null)
            {
                try
                {
                    this.controller.Close();

                }
                catch (Exception ex)
                {
                    LogUtil.Logger.Error(ex.Message, ex);

                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// 测试下线连通性
        /// </summary>
        private void OffConnTest()
        {
            try
            {
                controller = new PTLCanController.Controller(Settings.Default.com);
                controller.Open();
                // MessageBox.Show("打开端口成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            string mean = string.Empty;
            controller.Send(0, 2, "0000FF");

            MessageBoxResult MSBOXRE = MessageBox.Show("是否显示数字为0002,颜色为红色", "测试结果", MessageBoxButton.YesNo);
            if (MSBOXRE == MessageBoxResult.Yes)
            {
                mean = "连通测试通过";
               


            }
            else
            {
                mean = "连通测试未通过";
               
            }
            LogUtil.Logger.Info(mean);
            MessageBox.Show(mean);


        }


        /// <summary>
        /// 设置Id
        /// </summary>
        private void SetId()
        {
            bool IdFlag = true;
            while (IdFlag)
            {
                int AimedId = MakeId();
                bool SearchRe = SearchForId(AimedId);
                if (SearchRe)
                {
                    IdFlag = false;
                    controller.Set(0, AimedId);
                    controller.Close();
                    controller.Send(AimedId, 9876, "FF0000");

                    string tempa = string.Empty;
                    MessageBoxResult MSBOXRE = MessageBox.Show("是否显示数字为9876,颜色为蓝色", "测试结果", MessageBoxButton.YesNo);
                    if (MSBOXRE == MessageBoxResult.Yes)
                    {

                        LampId = AimedId;
                        if (Create(LampId))
                        {
                            tempa = "设置成功";
                            controller.Close();
                        }
                        else
                        {
                            tempa = "设置成功，但未写入数据库";
                            MessageBox.Show("编号为" + LampId);
                        }


                    }
                    else
                    {
                        tempa = "设置失败";
                    }

                    MessageBox.Show(tempa);
                }

            }

            if (controller != null)
            {
                try
                {
                    this.controller.Close();

                }
                catch (Exception ex)
                {
                    LogUtil.Logger.Error(ex.Message, ex);

                    MessageBox.Show(ex.Message);
                }
            }

        }




        public int MakeId()
        {
            int AimedId = new Random().Next(0, 99);
            return AimedId;
        }

        public bool SearchForId(int id)
        {


            LampDataContext LTX = new LampDataContext(Settings.Default.LampConnectionString);
            ElabelOnPart IdRe = LTX.GetTable<ElabelOnPart>().FirstOrDefault(c => c.LampId.Equals(id));
            return IdRe == null;


        }

        public bool Create(int id)
        {
            LampDataContext LTX = new LampDataContext(Settings.Default.LampConnectionString);
            ElabelOnPart CreateId = new ElabelOnPart
            {
                LampId = id
            };
            try
            {
                LTX.ElabelOnPart.InsertOnSubmit(CreateId);
                LTX.SubmitChanges();
                return true;
            }
            catch (Exception ee)
            {

                LogUtil.Logger.Error(ee.Message, ee);
                MessageBox.Show(ee.Message);
                return false;
            }
        }


        private SerialPort sp;

        public void CheckId()
        {
            MessageBox.Show("请依次测试数字加减键，确认键，取消键,点击确认后开始");
            while (BtnTest == false)
            {
                try
                {
                    if (this.sp == null)
                    {
                        this.sp = new SerialPort(Settings.Default.com, 9600, Parity.None);
                        this.sp.Open();
                        this.sp.DataReceived += new SerialDataReceivedEventHandler(Sp_DataReceived);

                        LogUtil.Logger.Info("Open Success.....");
                    }
                    else
                    {
                        if (sp.IsOpen)
                        {
                            continue;
                        }
                        else
                        {
                            this.sp.Open();
                            this.sp.DataReceived += new SerialDataReceivedEventHandler(Sp_DataReceived);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.Logger.Error("COM 打开错误", ex);
                    throw ex;
                }

            }
        }




        /// <summary>
        /// 接收处理数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string mean = string.Empty;
            Thread.Sleep(15);
            SerialPort sp = (SerialPort)sender;
            byte[] rdata = new byte[sp.BytesToRead];
            sp.Read(rdata, 0, rdata.Length);

            LogUtil.Logger.Info("接收数据：" + ScaleConvertor.HexBytesToString(rdata));
            switch (rdata[5])
            {
                case (byte)209:
                    {
                        int LampId = rdata[3];
                        string back = string.Format("{0}{1}B0010000000000",
                       ScaleConvertor.DecimalToHexString(LampId + 256, true, 8),
                       ScaleConvertor.DecimalToHexString(rdata[4], true, 2));

                        byte[] bback = ScaleConvertor.HexStringToHexByte(back);

                        sp.Write(bback, 0, bback.Length);
                        LogUtil.Logger.Info(string.Format("发送回复: " + ScaleConvertor.HexBytesToString(bback)));
                        int ShowNum = (rdata[7] << 8 | rdata[8]);
                        MessageBoxResult MSBOXRE = MessageBox.Show("是否发送了数码管数字为" + ShowNum + "的确认指令?", "按键测试结果", MessageBoxButton.YesNo);

                        if (MSBOXRE == MessageBoxResult.Yes)
                        {
                            mean = "确认键测试通过";
                        }


                        else
                        {
                            mean = "确认键测试未通过";



                        }
                        LogUtil.Logger.Info(mean);
                        MessageBox.Show(mean);
                        break;

                    }
                case (byte)210:
                    {

                        int LampId = rdata[3];
                        string back = string.Format("{0}{1}B0010000000000",
                      ScaleConvertor.DecimalToHexString(LampId + 256, true, 8),
                       ScaleConvertor.DecimalToHexString(rdata[4], true, 2));

                        byte[] bback = ScaleConvertor.HexStringToHexByte(back);

                        sp.Write(bback, 0, bback.Length);
                        LogUtil.Logger.Info(string.Format("发送回复: " + ScaleConvertor.HexBytesToString(bback)));
                        int ShowNum = (rdata[7] << 8 | rdata[8]);
                        MessageBoxResult MSBOXRE = MessageBox.Show("是否发送了数码管数字为" + ShowNum + "的取消指令?", "按键测试结果", MessageBoxButton.YesNo);

                        if (MSBOXRE == MessageBoxResult.Yes)
                        {
                            mean = "取消键测试通过";
                            BtnTest = true;


                        }


                        else
                        {
                            mean = "取消键测试未通过";
                            BtnTest = true;


                        }
                        LogUtil.Logger.Info(mean);
                        MessageBox.Show(mean);
                        sp_close();
                        break;
                    }
            }




        }


        public void sp_close()
        {
            if (sp != null)
            {
                try
                {
                    sp.Close();

                }
                catch (Exception ex)
                {
                    LogUtil.Logger.Error(ex.Message, ex);

                    MessageBox.Show(ex.Message);
                }
            }
        }


        public void NumTest(int AimedId)
        { if (sp != null || controller != null)
            {
                sp.Close();
                controller.Close();
            }
            try
            {
                controller = new PTLCanController.Controller(Settings.Default.com);
                controller.Open();
                // MessageBox.Show("打开端口成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            string mean = string.Empty;
            controller.Send(AimedId, 1111, "0000FF");
            MessageBoxResult MSBOXRE = MessageBox.Show("是否显示数字为 1111", "测试结果", MessageBoxButton.YesNo);
            if (MSBOXRE == MessageBoxResult.Yes)
            {

                controller.Send(AimedId, 3333, "0000FF");
                MessageBoxResult NumRe1 = MessageBox.Show("是否显示数字为 3333", "测试结果", MessageBoxButton.YesNo);
                if (NumRe1 == MessageBoxResult.Yes)
                {
                    mean = "数字测试通过";
                }
                else
                {
                    mean = "数字测试未通过";
                }

            }
            else
            {
                mean = "数字测试未通过";
            }
            LogUtil.Logger.Info(mean);
            MessageBox.Show(mean);
        }


        private void ColorTest(int AimedId)
        {

            string tempa = string.Empty;
            controller.Send(AimedId, 0, "0000FF");
            MessageBoxResult RRE = MessageBox.Show("是否显示颜色为红色", "颜色测试确认", MessageBoxButton.YesNo);
            if (RRE == MessageBoxResult.Yes)
            {
                controller.Send(AimedId, 0, "00FF00");

                MessageBoxResult GRE = MessageBox.Show("是否显示颜色为绿色", "颜色测试确认", MessageBoxButton.YesNo);

                if (GRE == MessageBoxResult.Yes)
                {
                    controller.Send(AimedId, 0, "FF0000");

                    MessageBoxResult BRE = MessageBox.Show("是否显示颜色为蓝色", "颜色测试确认", MessageBoxButton.YesNo);
                    if (BRE == MessageBoxResult.Yes)
                    {
                        tempa = "颜色测试通过";
                    }
                    else
                    {
                        tempa = "颜色测试未通过";
                    }
                }
                else
                {
                    tempa = "颜色测试未通过";
                }

            }

            else
            {
                tempa = "颜色测试未通过";
            }


            MessageBox.Show(tempa);
            if ((controller != null) || (sp != null))

            {
                try
                {
                    controller.Close();
                    sp.Close();

                }
                catch (Exception ex)
                {
                    LogUtil.Logger.Error(ex.Message, ex);

                    MessageBox.Show(ex.Message);
                }
            }

        }


      

      


    }

}







