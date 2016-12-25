using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 智能仓库控制系统.Control
{
    public partial class AGVinfoPan : UserControl
    {
        /// <summary>
        /// 小车信息
        /// </summary>
        public AGVInfo info = new AGVInfo();
        Timer tmr1 = new Timer();
        public Int16 ID { get;set ;}
		/// <summary>
		/// 运行模式时置1，设计模式时为0
		/// </summary>
		public bool runmode = false;
        /// <summary>
        /// 上次状态 不一样则刷新
        /// </summary>
        public bool LastOnline = true;
        public AGVinfoPan()
        {
            InitializeComponent();
            tmr1.Interval = 200;
            tmr1.Tick += (s,e) =>
            {
                if (info.LastChange > info.LastRead) {	//更新标志，闪烁灯
                    info.LastRead = DateTime.Now;
                    labTCP.BackColor = labTCP.BackColor == Color.OrangeRed ? Color.LawnGreen : Color.OrangeRed;
                    this.Refresh();
                }else if(LastOnline!=info.OnLine){
                    this.Refresh();
                }
            };
            tmr1.Start();
            this.Refresh();
        }
        public override void Refresh()
        {
			if (runmode == false)
				return;
			this.BackColor = info.OnLine ? Color.FromArgb(230, 250, 230) : Color.FromArgb(250, 230, 230);//在线，浅绿/离线，浅红
            labOnLine.Text = info.OnLine ? "在线" : "离线";
            toolTip1.SetToolTip(labOnLine, "TCP请求：" +  info.SendTime.ToString("s,")+info.SendTime.Millisecond.ToString()
                + "，最后接收：" + info.SendTime.ToString("s,") + info.LastChange.Millisecond.ToString()+",发送目标路线:"+info.SetLineNumber.ToString()
                );
            labOnLine.BackColor = info.OnLine ? Color.FromArgb(128, 255, 128):Color.FromArgb(255, 128, 128) ;
            labID.Text = info.ID.ToString();
            labLineCard.Text = info.LineCard.ToString();
			toolTip1.SetToolTip(labLineCard, AGV.GetAGVstationCard(info.LineCard));
            labError.Text = info.Error.ToString();
            toolTip1.SetToolTip(labError, AGV.GetAGVErrorCode(info.Error));
            labError.BackColor = info.Error > 0 ?Color.FromArgb(255, 128, 128) :  Color.FromArgb(128, 255, 128);
            labRunMode.Text = info.RunMode.ToString();
            var u=Convert.ToDecimal( info.U )/ 10m;
            labU.Text = (u).ToString() + "V";
            if(info.U > Convert.ToInt16(App.Config["/Config/agv/lowU"])){
                toolTip1.SetToolTip(labU, "电压正常");
                labU.BackColor =   Color.FromArgb(230, 250, 230);
            }else{
                labU.BackColor = Color.FromArgb(250, 230, 230);//电压正常，浅绿/低，浅红
                toolTip1.SetToolTip(labU, "电压低\r\n下个入库工位时会自动切换至充电工位");
            }
			labLineNumber.Text = info.LineNumber.ToString();
            toolTip1.SetToolTip(labLineNumber, AGV.GetAGVLineName(info.LineNumber) + "\r\n" + info.SetLineNumber);
			if (info.HasBox==false)
			{
				labHasBox.Text = "无";
			}else
			{
				labHasBox.Text = info.BoxType.ToString();
			}
            
            labBoxDir.Text = info.BoxDir.ToString();
            base.Refresh();

        }
    }
}
