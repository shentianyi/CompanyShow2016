using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
namespace 智能仓库控制系统
{
	public partial class frmMain : Form
	{
		/// <summary>
		/// 手动AGV动作小车选择
		/// </summary>
		Int16 Test_agv_sel = 1;
		/// <summary>
		/// 主工作流程线程
		/// </summary>
		Thread mainActionFunc = null;
		public frmMain()
		{
			InitializeComponent();
			CheckForIllegalCrossThreadCalls = false;
            App.FrmMain = this;
			//this.ListViewLog.in
		}

        void Logs_LogAddEvent(WRItem Log_item)
        {
            ListViewItem lvi =new ListViewItem((ListViewLog.Items.Count + 1).ToString());
            lvi.SubItems.Add(Log_item.ItemValue.ToString());
            ListViewLog.Items.Add(lvi);
            //ListViewLog.Items.Insert(0, lvi);
			if (chkAutoReLoad.Checked)
			{
				ListViewLog.Items[ListViewLog.Items.Count - 1].Selected = true;
                //lvi.Selected = true;
			}
			labMainStates.Text = Log_item.ItemValue.ToString();
			Application.DoEvents();
        }
		private void frmMain_Shown(object sender, EventArgs e)
		{

			App.Logs.LogAddEvent += new Log.LogAddEventDel(Logs_LogAddEvent);

            App.Logs.AddLog("初始化OPC", true);
            App.OPC = new OPC();
            App.Logs.AddLog("OPC开始监视", true);
            App.OPC.OPCStartWatch();
            App.Logs.AddLog("添加OPC条目", true);
            listViewOPC.BeginUpdate();
            foreach (var item in App.OPC.ReadItems.Keys)
            {
                ListViewItem lvi = new ListViewItem(listViewOPC.Items.Count.ToString());
                lvi.SubItems.Add(item);
                lvi.SubItems.Add("");
                //lvi.SubItems.Add(App.OPC.ReadItems[item].Data.Item.ToString());
                lvi.Name = item;
                listViewOPC.Items.Add(lvi);
            }
			listViewOPC.EndUpdate();

            App.Logs.AddLog("加载AGV小车路径信息", true);
			agvPathInit();

			App.Logs.AddLog("初始化TCP通讯", true);
			App.TCP = new TCPServer();
            App.Logs.AddLog("开始TCP监听服务", true);
			App.TCP.Start();
			App.Logs.AddLog("初始化AGV小车信息", true);
			App.AGVsInfo = new List<AGVInfo>();
			for (int i = 0; i < 6; i++)
			{
				var info = new AGVInfo();
				info.ID = (Int16)(i + 1);
				App.AGVsInfo.Add(info);
			}
			foreach (var item in agVinfoPan1.Parent.Controls)
			{
				if (item is Control.AGVinfoPan)
				{
					Control.AGVinfoPan pan = item as Control.AGVinfoPan;
					pan.info = App.AGVsInfo.Single(n => n.ID == pan.ID);
					pan.runmode = true;
					pan.Refresh();
				}
			}
			App.Logs.AddLog("系统初始化完毕", true);
			tmrMain.Start();

			App.initOK = true;
			picAGVpath.Refresh();

            if (App.AutoRunMode == false) {
                if (App.Config["/Config/soft/auto_start_tcp"] == "1")
                {
                    btnStartRun_Click(null, null);
                }
            }
            //自动启动时OPC还未初始化完毕，造成连接失败。
            //if (App.AutoRunMode == false)
            //{
            //    if (App.Config["/Config/soft/windows_auto_start"] == "1")
            //    {
            //        WindowsAutoStart.Enable();
            //    }
            //}
            

		}

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void agv_test_Sel(object sender, EventArgs e)
        {//AGV手动测试-小车选择
            Test_agv_sel = Convert.ToInt16(((RadioButton)sender).Text);
        }

        private void btnAGVtestRun_Click(object sender, EventArgs e)
        {
            //AGV小车手动运行
            var lineNumber=Convert.ToByte( listAGVtestLineNumber.SelectedItem.ToString().Split(' ').First());
            if (AGV.SendAGVRun(Test_agv_sel, lineNumber, (radAGVtestBack.Checked ? true : false)) == false) {
                var msg = "手动小车运行指令失败";
                App.Logs.AddLog(msg, true);
                MessageBox.Show(msg);

            }
        }

        private void btnAgvTestStop_Click(object sender, EventArgs e)
        {
            if (AGV.SendAGVStop(Test_agv_sel)==false)
            {
                var msg = "手动小车停止指令失败";
                App.Logs.AddLog(msg, true);
                MessageBox.Show(msg);

            }
        }

        private void btnAVGtestReset_Click(object sender, EventArgs e)
        {
            if (AGV.SendAGVReset(Test_agv_sel)==false)
            {
                var msg = "手动小车停止指令失败";
                App.Logs.AddLog(msg, true);
                MessageBox.Show(msg);

            }
        }

        private void btnAGVtestLock_Click(object sender, EventArgs e)
        {
            if (AGV.SendAGVLock(Test_agv_sel)==false)
            {
                var msg = "手动小车锁定指令失败";
                App.Logs.AddLog(msg, true);
                MessageBox.Show(msg);

            }
        }

        private void btnAGVtestUnlock_Click(object sender, EventArgs e)
        {
            if (AGV.SendAGVUnLock(Test_agv_sel)==false)
            {
                var msg = "手动小车解锁指令失败";
                App.Logs.AddLog(msg, true);
                MessageBox.Show(msg);

            }
        }

        private void btnAGVtestUpBox_Click(object sender, EventArgs e)
        {
            if (AGV.SendAgvUpBox (Test_agv_sel,radAGVtestRight.Checked,AGVinfoBoxType.未知)==false)
            {
                var msg = "手动小车上货指令失败";
                App.Logs.AddLog(msg, true);
                MessageBox.Show(msg);

            }
        }

        private void btnAGVtestDownBox_Click(object sender, EventArgs e)
        {
            if (AGV.SendAgvDownBox (Test_agv_sel,radAGVtestRight.Checked)==false)
            {
                var msg = "手动小车卸货指令失败";
                App.Logs.AddLog(msg, true);
                MessageBox.Show(msg);

            }
        }

        private void btnAGVtestStopBox_Click(object sender, EventArgs e)
        {
            if (AGV.SendAgvStopBox (Test_agv_sel)==false)
            {
                var msg = "手动小车停止上卸货指令失败";
                App.Logs.AddLog(msg, true);
                MessageBox.Show(msg);

            }
        }

        private void btnOPCwrite_Click(object sender, EventArgs e)
        {
            var sel=listViewOPC.SelectedItems;
            if (sel.Count > 0) { 
                App.OPC.WriteOPCItem(sel[0].Name,txtOPCwrite.Text);
                //App.OPC.ReadItems[sel[0].Name].wr
            }
        }

		private void btnAGVtestAlert_Click(object sender, EventArgs e)
		{
            if (AGV.SendAgvAlert(Test_agv_sel)==false)
            {
                var msg = "手动小车停止上卸货指令失败";
                App.Logs.AddLog(msg, true);
                MessageBox.Show(msg);
            }
		}

		private void tmrMain_Tick(object sender, EventArgs e)
		{
			labDateTime.Text = DateTime.Now.ToString();
            //AGVInfo[] infos = new AGVInfo[6];
            //App.AGVsInfo.CopyTo(infos);
            int c = App.AGVsInfo.Count(n=>n.OnLine);
            labAGVCount.Text = "共有" + c.ToString() + "辆AGV小车在线";

            var btn = new ToolStripButton[] { btnCar1, btnCar2, btnCar3, btnCar4, btnCar5, btnCar6 };
            for (int i = 0; i < btn.Count(); i++) {
                btn[i].Text = "小车" + (i+1).ToString() + (App.AGVsInfo[i].OnLine ? "[在线]" : "[离线]");
                btn[i].BackColor = App.AGVsInfo[i].OnLine ? Color.FromArgb(230, 250, 230) : Color.FromArgb(250, 230, 230);//在线，浅绿/离线，浅红
                btn[i].ToolTipText = "";
                btn[i].AutoToolTip = false;
            }
		}



        /// <summary>
        /// AGV小车路径，仅绘图
        /// </summary>
        public List<AGVpath_path> agvPath_LinePath = new List<AGVpath_path>();
		/// <summary>
		/// AGV小车线路号
		/// </summary>
		public List<AGVpath_line> agvPath_LineName = new List<AGVpath_line>();
		/// <summary>
		/// 站点信息,地标卡
		/// </summary>
		public List<AGVpath_station> agvPath_station = new List<AGVpath_station>();
		/// <summary>
		/// 加载初始化小车路径信息 绘图显示用
		/// </summary>
		void agvPathInit() {
			//路径
			var nodes = App.Config.XmlDoc.SelectNodes("/Config/agv_path_dat/path");
			foreach (XmlNode item in nodes)
			{
				AGVpath_path path = new AGVpath_path();
				var color = item.Attributes["color"].Value.Split(',').Select<string, byte>(n => Convert.ToByte(n)).ToArray();
				path.Color = Color.FromArgb(color[0], color[1], color[2]);
				path.Name = item.Attributes["name"].Value;
				var paths = item.InnerText.Split(',');
				foreach (var item2 in paths)
				{
					var _item2 = item2.Split('.').Select<string, int>(n => Convert.ToInt32(n)).ToArray();
					path.Path.Add(new Point(_item2[0], _item2[1]));
				}
				agvPath_LinePath.Add(path);
			}
			//小车线
			nodes = App.Config.XmlDoc.SelectNodes("/Config/agv_path_dat/line");
			foreach (XmlNode item in nodes)
			{
				AGVpath_line line = new AGVpath_line();
				var color = item.Attributes["color"].Value.Split(',').Select<string, byte>(n => Convert.ToByte(n)).ToArray();
				line.Color = Color.FromArgb(color[0], color[1], color[2]);
				line.ID = Convert.ToInt16(item.Attributes["id"].Value);
				line.Name = item.Attributes["name"].Value;
				var _lines = item.InnerText.Split(',');
				foreach (var item2 in _lines)
				{
					var path = agvPath_LinePath.Single(n => n.Name == item2);
					line.line.Add(path);
				}
				agvPath_LineName.Add(line);
			}
			//小车站点，路标号
			nodes = App.Config.XmlDoc.SelectNodes("/Config/agv_path_dat/station");
			foreach (XmlNode item in nodes)
			{
				AGVpath_station sta = new AGVpath_station();
				sta.ID = Convert.ToInt16(item.Attributes["id"].Value);
				sta.Name = item.Attributes["name"].Value;
				var p = item.InnerText.Split('.').Select<string, int>(n => Convert.ToInt32(n)).ToArray();
				sta.Point = new Point(p[0], p[1]);
				agvPath_station.Add(sta);
			}
		}
		private void picAGVpath_Paint(object sender, PaintEventArgs e)
		{
			if (App.initOK == false)
				return;

            //return;
			Graphics g = e.Graphics;
            //绘制图线名称
            Font font=new Font("宋体",10);
            Font Bfont = new System.Drawing.Font("宋体", 10, FontStyle.Bold);
            g.DrawString("路线说明", Bfont, Brushes.Blue, 10, 2);
            var b = new SolidBrush(Color.FromArgb(80, 100, 0, 200));
            for(byte i=1;i<13;i++){
                var ss = i.ToString() + ":" + AGV.GetAGVLineName(i);
                var sizef = g.MeasureString(ss, font); 
                g.FillRectangle(b,new RectangleF(new PointF(10,i*18+5), sizef));
                g.DrawString(ss,font,Brushes.BlueViolet,10,i*18+5);
            }
            int y = 260;
            g.DrawString("站点说明", Bfont, Brushes.Blue, 10, y);
            y += 18;
            foreach (var item in AGV.GetAGVstationList()) {
                var ss = item.Key.ToString() + ":" + item.Value;
                var sizef = g.MeasureString(ss, font); 
                g.FillRectangle(b, new RectangleF(new PointF(10, y), sizef));
                g.DrawString(ss, font, Brushes.Green, 10, y);
                y += 18;
            }
            //绘制站点名称

            //g.DrawImage(Image.FromFile("line.png"), new Point(0, 0));
            return;

			var s = App.Config["/Config/agv_path_dat/size"].Split('.').Select<string,float>(n=>Convert.ToSingle(n));
			var s1 = (picAGVpath.Width-10) / s.First();
			var s2 = (picAGVpath.Height-10) / s.Last();

			Single zoom = s1>s2?s2:s1;
			//路径
			foreach (var item in agvPath_LinePath)
			{
				Pen pen = new Pen(item.Color, 2);
				var t=item.Path.Select<PointF, byte>(n => (byte)PathPointType.Line).ToArray();
				GraphicsPath path = new GraphicsPath(item.Path.Select<PointF, PointF>(n => new PointF( n.X *zoom,n.Y*zoom)) .ToArray(), t);
				g.DrawPath(pen, path);
			}
			//小车线
			foreach (var item in agvPath_LineName) {
				var c= Color.FromArgb(30, item.Color);
				Pen pen = new Pen(c, 10);
				foreach(var item2 in item.line)
				{
					var t=item2.Path.Select<PointF, byte>(n => (byte)PathPointType.Line).ToArray();
					GraphicsPath path = new GraphicsPath(item2.Path.Select<PointF, PointF>(n => new PointF( n.X *zoom,n.Y*zoom)) .ToArray(), t);
					g.DrawPath(pen, path);
				}
			}
			//小车站点，路标号
			foreach (var item in agvPath_station) {
				int r = 10;
				Point p = new Point(Convert.ToInt32((item.Point.X - r / 2)*zoom),Convert.ToInt32( (item.Point.Y - r / 2)*zoom));

				Rectangle rct = new Rectangle(p, new Size(r, r));
				g.FillPie(Brushes.GreenYellow, rct, 0, 360);
			}
		}

		private void ListViewLog_DoubleClick(object sender, EventArgs e)
		{
			if (ListViewLog.SelectedItems.Count > 0)
			{
				MessageBox.Show(ListViewLog.SelectedItems[0].SubItems[1].Text, "日志详情");
			}
		}


		private void btnStartRun_Click(object sender, EventArgs e)
		{
            if (App.RunFlag)
            {
                mainActionFunc.Abort();
                App.RunFlag = false;
                btnStartRun.Text = "开始自动运行";
            }
            else
            {

                App.RunFlag = true;
                //tmrMainAction.Start();
                mainActionFunc = new Thread(mainAction.MainFunc);
                mainActionFunc.Name = "主工作流程线程";
                mainActionFunc.Start();
                btnStartRun.Text = "切换至手动模式";
            }
		}

		private void picAGVpath_Move(object sender, EventArgs e)
		{
			//在相关点位上时弹出提示
		}
		/// <summary>
		/// 将小车运行信息写入OPC供PLC工作
		/// </summary>
		/// <param name="id">小车ID</param>
		/// <param name="key">写的关键字</param>
		/// <param name="value">值</param>
		/// <returns>是否成功</returns>
		static bool WriteAGVinfo(int id,string key,object value) {
            string wkey = App.OPC.OPCShortNames["AGV_" + id.ToString() + "_" + key];
            var r= App.OPC.WriteOPCItem(wkey, value);
            return r;
		}
        private void btnAutoRunModeSwitch_Click(object sender, EventArgs e)
        {

        }

        private void btnCar_MouseHover(object sender, EventArgs e)
        {
            if (App.initOK == false)
                return;
            ToolStripButton btn = (ToolStripButton)sender;
            Control.AGVinfoPan pan = new Control.AGVinfoPan();
            pan.ID = Convert.ToInt16(btn.Name.Substring(6, 1));
            pan.info = App.AGVsInfo[pan.ID - 1];
            pan.runmode = true;
            //pan= flowLayoutPanel1.Controls.Cast<Control.AGVinfoPan>().ToList().SingleOrDefault(n => n.ID == Convert.ToInt16(btn.Name.Substring(6, 1)));
            
            picAGVpath.Controls.Add(pan);
            pan.Location = new Point(90 * (pan.ID - 1) + 120, 5);
            pan.Visible = true;
            btn.Tag = pan;
        }

        private void btnCar_MouseLeave(object sender, EventArgs e)
        {
            var btn = (ToolStripButton)sender;
            if (btn.Tag != null && btn.Tag!="")
            {
                Control.AGVinfoPan pan = btn.Tag as Control.AGVinfoPan;
                picAGVpath.Controls.Remove(pan);
                pan.Dispose();
                btn.Tag = null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //历史日志
            System.Diagnostics.Process.Start("notepad", Environment.CurrentDirectory + "\\log.log");
            return;


            label4.Text = "正在加载历史日志";
            Application.DoEvents();
            System.IO.FileStream file2 = new System.IO.FileStream("log.log",System.IO.FileMode.Open,System.IO.FileAccess.Read);
            System.IO.StreamReader file = new System.IO.StreamReader(file2);
            ListViewLog.Items.Clear();
            ListViewLog.BeginUpdate();
            string line=file.ReadLine();
            do
            {
                ListViewItem lvi = new ListViewItem((ListViewLog.Items.Count + 1).ToString());
                lvi.SubItems.Add(line);
                line = file.ReadLine();
            } while (line != null);
            ListViewLog.EndUpdate();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void txtOPCwrite_TextChanged(object sender, EventArgs e)
        {
            if (btnOPCwrite.Enabled == false) {
                if (txtOPCwrite.Text == "*123456*") {
                    btnOPCwrite.Enabled = true;
                    txtOPCwrite.Text = "";
                }
            }
        }



	}
}
