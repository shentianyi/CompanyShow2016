using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 智能仓库控制系统
{
    /// <summary>
    /// AGV TCP小车信息
    /// </summary>
    public class AGV
    {
        /// <summary>
        /// TCP数据类
        /// </summary>
        public class TcpData {
            /// <summary>
            /// 通信开始标志
            /// </summary>
            public byte StartFlag;
            /// <summary>
            /// 数据指令长度，目前固定13
            /// </summary>
            public byte DataLength;
            /// <summary>
            /// 目标地址
            /// </summary>
            public Int16 DAddress;
            /// <summary>
            /// 小车数据
            /// </summary>
            public TcpCmdData Data=new TcpCmdData();
            /// <summary>
            /// 来源地址
            /// </summary>
            public Int16 SAddress;
            
        }
        /// <summary>
        /// 将小车错误码转换为实际中文描述
        /// </summary>
        /// <param name="errorCode">原始错误码</param>
        /// <returns>错误码描述</returns>
        public static string GetAGVErrorCode(byte errorCode) {
            string err = @"
                0,无错误
                1,无路径信号信息
                2,电源电量过低
                3,机械防撞生效
                4,驱动器报警
                5,路劲检测报警
                6,路径设置出错
                7,急停按下
                8,有障碍
                9,滚筒异常
                10,（备用）
                11,无法由当前站点寻找目标站点
                12,站点无线io通信异常
                13,没有停车标志位
                14,和中控通讯异常";
            var t1 = err.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                return t1[errorCode].Split(',').Last();
            }
            catch {
                return "@异常，未找到该错误码对应的错误描述信息";
            }
        }
		/// <summary>
		/// 将路线号转换为中文描述信息
		/// </summary>
		/// <param name="lineNumber">路线ID</param>
		/// <returns>路线描述</returns>
		public static string GetAGVLineName(byte lineNumber) {
			string line = App.Config["/Config/agv_path_name/line"];
			var t1 = line.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			try
			{
                return t1[lineNumber].Trim().Split(' ').ElementAt(1);
			}
			catch
			{
				return "@异常，未找到该路线的描述信息";
			}
		}
		/// <summary>
		/// 将路标号转换为中文描述信息
		/// </summary>
		/// <param name="lineNumber">路标号ID</param>
		/// <returns>路标号描述</returns>
		public static string GetAGVstationCard(Int16 id)
		{
			string line = App.Config["/Config/agv_path_name/station"];
			var t1 = line.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			try
			{
                var t = t1.SingleOrDefault(n => n.Trim().Split(' ').First() == id.ToString());
                if (t != null)
                    return t.Trim().Split(' ').ElementAt(1);
                else
                    return "@异常，未找到该路标号的描述信息";
			}
			catch
			{
				return "@异常，未找到该路标号的描述信息";
			}
		}
        /// <summary>
        /// 获取所有站点名称列表
        /// </summary>
        public static Dictionary<Int16, string> GetAGVstationList()
        {
            var r=new Dictionary<Int16,string>();
            string line = App.Config["/Config/agv_path_name/station"];
            var t1 = line.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                foreach (var item in t1) {
                    var item2 = item.Trim().Split(' ');
                    var id = Convert.ToInt16(item2[0]);
                    if(id>0)
                        r.Add(id, item2[1]);
                }
            }
            catch
            {
            }
            return r;
        }
        /// <summary>
        /// 获取路线目标点的站号ID
        /// </summary>
        /// <param name="lineNumber">路线</param>
        /// <returns>站号ID 0代表异常</returns>
        public static byte GetAGVLineStationCard(byte lineNumber) {
            string line = App.Config["/Config/agv_path_name/line"];
            var t1 = line.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                return  Convert.ToByte(t1[lineNumber].Trim().Split(' ').ElementAt(2));
            }
            catch
            {
                return 0;
            }
        } 


		/// <summary>
		/// 小车命令数据
		/// </summary>
		public class TcpCmdData
        {
            /// <summary>
            /// 起始标志0xf
            /// </summary>
            public byte Start;
            /// <summary>
            /// 命令
            /// </summary>
            public AGVtcpCmdEnum Cmd;
            /// <summary>
            /// 命令参数数据,9个字节
            /// </summary>
            public byte[] CmdData = new byte[9];
            /// <summary>
            /// 校验合，目前为空0x00
            /// </summary>
            public Int16 CRCsum;
        }
        /// <summary>
        /// 将接收的原始数据字节转换为小车数据类
        /// </summary>
        /// <param name="srcData"></param>
        /// <returns></returns>
        public static TcpData ConvAGVData(byte[] srcData) {
            TcpData dat = new TcpData();
            if (srcData.Length != 19)
            {
                //System.Windows.Forms.MessageBox.Show("错误的源数据，因为数据长度不为19");
                App.Logs.AddLog("错误的源数据，因为数据长度不为19", true);
                return null;
            }
            else {
                dat.StartFlag = srcData[0];
                dat.DataLength = srcData[1];
                if (dat.DataLength != 0x13) {
                    //System.Windows.Forms.MessageBox.Show("错误的源数据，因为数据指令长度值不为13");
                    App.Logs.AddLog("错误的源数据，因为数据指令长度值不为0x13", true);
                    return null;
                }
                dat.DAddress = Convert.ToInt16( srcData[2] << 8 + srcData[3]);
                dat.Data.Start = srcData[4];
                if (dat.Data.Start!=0xf)
                {
                    //System.Windows.Forms.MessageBox.Show("错误的源数据，因为数据指令内容的起始字节不是0xf");
                    App.Logs.AddLog("错误的源数据，因为数据指令内容的起始字节不是0xf", true);
                    return null;
                }
                dat.Data.Cmd = (AGVtcpCmdEnum)srcData[5];
                dat.Data.CmdData = srcData.Skip(6).Take(9).ToArray();//从第5个字节开始取9个字节数据
                dat.Data.CRCsum = Convert.ToInt16( (Int16)srcData[15]+(Int16)(srcData[16] << 8));//15,16,(17,18)来源地址
                dat.SAddress = Convert.ToInt16((srcData[17] << 8) + srcData[18]);
            }
            return dat;
        }
        /// <summary>
        /// 将AGV小车数据转换为实际发送的TCP字节数据
        /// </summary>
        /// <param name="srcData"></param>
        /// <returns></returns>
        public static byte[] GetAGVData(TcpData srcData) {
            byte[] dat = new byte[19];
            dat[0] = srcData.StartFlag;
            dat[1] = srcData.DataLength;
            dat[3] = (byte)( srcData.DAddress&0xff);//高位在前
            dat[2] = (byte)(srcData.DAddress >> 8);
            dat[4] = srcData.Data.Start;
            dat[5] = (byte)srcData.Data.Cmd;
            dat[6] = srcData.Data.CmdData[0];
            dat[7] = srcData.Data.CmdData[1];
            dat[8] = srcData.Data.CmdData[2];
            dat[9] = srcData.Data.CmdData[3];
            dat[10] = srcData.Data.CmdData[4];
            dat[11] = srcData.Data.CmdData[5];
            dat[12] = srcData.Data.CmdData[6];
            dat[13] = srcData.Data.CmdData[7];
            dat[14] = srcData.Data.CmdData[8];
            dat[15] =(byte)( srcData.Data.CRCsum & 0xff);
            dat[16] = (byte)(srcData.Data.CRCsum >> 8);
            dat[17] = (byte)(srcData.SAddress & 0xff);
            dat[18] = (byte)(srcData.SAddress>>8);
            return dat;
        }
        /// <summary>
        /// 将接收的TCP数据转换为小车信息类
        /// </summary>
        /// <param name="dat">原始接收的TCP数据类</param>
        /// <returns>转换的小车信息</returns>
        public static AGVInfo ConvSrcDatToInfo(TcpData dat,AGVInfo info) {
            if (dat.Data.Cmd != AGVtcpCmdEnum.ReInfo) { //无效的接收数据信息
                //System.Diagnostics.Debug.WriteLine("无效的接收数据信息指令");
                //App.Logs.AddLog("无效的接收数据信息指令",true);
                return null;
            }
            //AGVInfo info = new AGVInfo();
            info.ID = dat.SAddress;
            info.Error = dat.Data.CmdData[2];
            info.LineCard = Convert.ToInt16((Int16)dat.Data.CmdData[0] + (Int16)(dat.Data.CmdData[1] << 8));
            info.RunMode = (AGVinfoRunEnum)dat.Data.CmdData[3];
            info.U = Convert.ToInt16((Int16)dat.Data.CmdData[4] + (Int16)(dat.Data.CmdData[5] << 8));
            info.LineNumber = dat.Data.CmdData[6];
            info.HasBox = dat.Data.CmdData[7] == 1;
            info.BoxDir = (AGVinfoBoxDir)dat.Data.CmdData[8];
            info.LastChange = DateTime.Now;
            return info;
        }
        /// <summary>
        /// 发送AGV小车数据前初始化相关固定的字节内容
        /// </summary>
        /// <param name="agv_id">初始化时小车的ID</param>
        /// <returns></returns>
        public static TcpData InitAGVData(Int16 agv_id) {
            TcpData dat = new TcpData();
            dat.StartFlag = 0xfd;
            dat.DataLength = 0x13;
            dat.Data.Start = 0xf;
            //dat.Data.CmdData[0] = 1;
            dat.DAddress=agv_id;
            return dat;
        }
        /// <summary>
        /// 获取  发送获取小车信息指令 
        /// </summary>
        /// <param name="id">小车编号</param>
        /// <returns>要发送的TCP数据字节</returns>
        public static byte[] SendGetAGVinfo(Int16 id) {
            TcpData dat = InitAGVData(id);
            dat.Data.Cmd = AGVtcpCmdEnum.GetInfo;
            return AGV.GetAGVData( dat);
        }
        /// <summary>
        /// 获取 发送AGV小车运行指令
        /// </summary>
        /// <param name="id">小车ID</param>
        /// <param name="LineNumber">前进路线号</param>
        /// <param name="DirBack">小车方向 后退=1 前进=0</param>
        /// <returns>发送指令是否成功</returns>
        public static bool SendAGVRun(Int16 id,byte LineNumber,bool DirBack) {
            var info = App.AGVsInfo.SingleOrDefault(n => n.ID == id);
            if (info != null) {
                if (info.Error > 0)
                {
                    App.Logs.AddLog("小车有错误产生，不能发送运行指令", true);
                    return false;
                }
            }
            TcpData dat = InitAGVData(id);
            dat.Data.Cmd = AGVtcpCmdEnum.Run;
            dat.Data.CmdData[1] = LineNumber;
            dat.Data.CmdData[2] = (byte)(DirBack ? 2 : 1);
            var t = App.TCP.SendAGVmsg(id, AGV.GetAGVData(dat));
            if (t) {    //发送成功，设置目标路线ID
                info.SetLineNumber = LineNumber;
                info.LastCmd = AGVtcpCmdEnum.Run;
            }
            App.Logs.AddLog("发送小车运行指令" + (t ? "成功" : "失败") + "id=" + id, true);
            return t;
        }
        /// <summary>
        /// 获取 发送AGV小车停止指令
        /// </summary>
        /// <param name="id">小车ID</param>
        /// <param name="LineNumber">前进路线号</param>
        /// <param name="DirBack">小车方向 后退=1 前进=0</param>
        /// <returns>发送指令是否成功</returns>
        public static bool SendAGVStop(Int16 id) {
            TcpData dat = InitAGVData(id);
            dat.Data.Cmd = AGVtcpCmdEnum.Stop;
            var t = App.TCP.SendAGVmsg(id, AGV.GetAGVData(dat));
            App.AGVsInfo.SingleOrDefault(n=>n.ID==id).LastCmd = AGVtcpCmdEnum.Stop;
            App.Logs.AddLog("发送小车停止指令" + (t ? "成功" : "失败") + "id=" + id, true);
            return t;
        }
        /// <summary>
        /// 获取 发送AGV小车复位指令
        /// </summary>
        /// <param name="id">小车ID</param>
        /// <returns>发送指令是否成功</returns>
        public static bool SendAGVReset(Int16 id) {
            TcpData dat = InitAGVData(id);
            dat.Data.Cmd = AGVtcpCmdEnum.Reset;
            var t = App.TCP.SendAGVmsg(id, AGV.GetAGVData(dat));
            App.AGVsInfo.SingleOrDefault(n => n.ID == id).LastCmd = AGVtcpCmdEnum.Reset;
            App.Logs.AddLog("发送小车复位指令" + (t ? "成功" : "失败") + "id=" + id, true);
            return t;
        }
        /// <summary>
        /// 获取 发送AGV小车锁定指令
        /// </summary>
        /// <param name="id">小车ID</param>
        /// <returns>发送指令是否成功</returns>
        public static bool SendAGVLock(Int16 id) {
            TcpData dat = InitAGVData(id);
            dat.Data.Cmd = AGVtcpCmdEnum.Lock;
            var t = App.TCP.SendAGVmsg(id, AGV.GetAGVData(dat));
            App.AGVsInfo.SingleOrDefault(n => n.ID == id).LastCmd = AGVtcpCmdEnum.Lock;
            App.Logs.AddLog("发送小车锁定指令" + (t ? "成功" : "失败") + "id=" + id, true);
            return t;
        }
        /// <summary>
        /// 获取 发送AGV小车解锁指令
        /// </summary>
        /// <param name="id">小车ID</param>
        /// <returns>发送指令是否成功</returns>
        public static bool SendAGVUnLock(Int16 id) {
            TcpData dat = InitAGVData(id);
            dat.Data.Cmd = AGVtcpCmdEnum.UnLock;
            var t = App.TCP.SendAGVmsg(id, AGV.GetAGVData(dat));
            App.AGVsInfo.SingleOrDefault(n => n.ID == id).LastCmd = AGVtcpCmdEnum.UnLock;
            App.Logs.AddLog("发送小车解锁指令" + (t ? "成功" : "失败") + "id=" + id, true);
            return t;
        }

        /// <summary>
        /// AGV小车上货指令
        /// </summary>
        /// <param name="id">小车ID</param>
        /// <param name="isDirLeft">小车方向 1=右 0=左</param>
        /// <param name="BoxType">箱子类型</param>
        /// <returns>发送指令是否成功</returns>
        public static bool SendAgvUpBox(Int16 id, bool isDirRight, AGVinfoBoxType BoxType)
        {
            //如果上次是上货，则检查上次时间，如果在3秒以上则取消操作
            var info=App.AGVsInfo.SingleOrDefault(n=>n.ID==id);
            if (info.LastCmd == AGVtcpCmdEnum.UpBox &&
                (DateTime.Now - info.LastCmdTime).Seconds <= 3) {
                    return true;
            }
            TcpData dat = InitAGVData(id);
            dat.Data.Cmd = AGVtcpCmdEnum.UpBox;
            dat.Data.CmdData[0] = (byte)(isDirRight ? 1 : 0);
            var t =App.TCP.SendAGVmsg(id, AGV.GetAGVData(dat));
            if(info!=null){
                info.BoxType = BoxType;
                info.LastCmd = AGVtcpCmdEnum.UpBox;
            }
            App.Logs.AddLog("发送小车上货指令" + (t ? "成功" : "失败") + "id=" + id, true);
            return t;
        }
        /// <summary>
        /// AGV小车卸货指令
        /// </summary>
        /// <param name="id">小车ID</param>
        /// <param name="isDirLeft">小车方向 1=右 0=左</param>
        /// <returns>发送指令是否成功</returns>
        public static bool SendAgvDownBox(Int16 id, bool isDirRight)
        {
            //如果上次是下货，则检查上次时间，如果在3秒以上则取消操作
            var info = App.AGVsInfo.SingleOrDefault(n => n.ID == id);
            if (info.LastCmd == AGVtcpCmdEnum.DownBox &&
                (DateTime.Now - info.LastCmdTime).Seconds <= 3)
            {
                return true;
            }
            TcpData dat = InitAGVData(id);
            dat.Data.Cmd = AGVtcpCmdEnum.DownBox;
            dat.Data.CmdData[0] = (byte)(isDirRight ? 1 : 0);
            var t= App.TCP.SendAGVmsg(id, AGV.GetAGVData(dat));
            App.AGVsInfo.SingleOrDefault(n => n.ID == id).LastCmd = AGVtcpCmdEnum.DownBox;
            App.Logs.AddLog("发送小车卸货指令" + (t ? "成功" : "失败") + "id=" + id, true);
            return t;
        }

        /// <summary>
        /// AGV小车停止上卸货指令
        /// </summary>
        /// <param name="id">小车ID</param>
        /// <returns>发送指令是否成功</returns>
        public static bool SendAgvStopBox(Int16 id)
        {
            TcpData dat = InitAGVData(id);
            dat.Data.Cmd = AGVtcpCmdEnum.StopUp;
            var t= App.TCP.SendAGVmsg(id, AGV.GetAGVData(dat));
            App.AGVsInfo.SingleOrDefault(n => n.ID == id).LastCmd = AGVtcpCmdEnum.StopUp;
            App.Logs.AddLog("发送小车停止上卸货指令" + (t ? "成功" : "失败") + "id=" + id, true);
            return t;

        }

        /// <summary>
        /// AGV小车警报指令
        /// </summary>
        /// <param name="id">小车ID</param>
        /// <returns>发送指令是否成功</returns>
        public static bool SendAgvAlert(Int16 id)
        {
            TcpData dat = InitAGVData(id);
            dat.Data.Cmd = AGVtcpCmdEnum.Alert;
            var t= App.TCP.SendAGVmsg(id, AGV.GetAGVData(dat));
            App.AGVsInfo.SingleOrDefault(n => n.ID == id).LastCmd = AGVtcpCmdEnum.Alert;
            App.Logs.AddLog("发送AGV小车警报指令" + (t ? "成功" : "失败") + "id=" + id, true);
            return t;
        }



    }
    /// <summary>
    /// AGV小车指令枚举
    /// </summary>
    public enum AGVtcpCmdEnum:byte { 
        /// <summary>
        /// 启动小车
        /// </summary>
        Run=1,
        /// <summary>
        /// 停止小车
        /// </summary>
        Stop=2,
        /// <summary>
        /// 复位小车
        /// </summary>
        Reset=3,
        /// <summary>
        /// 锁定小车
        /// </summary>
        Lock=0xf3,
        /// <summary>
        /// 解锁小车
        /// </summary>
        UnLock=0xf4,
        /// <summary>
        /// 上货
        /// </summary>
        UpBox=0xfa,
        /// <summary>
        /// 卸货
        /// </summary>
        DownBox=0xfb,
        /// <summary>
        /// 停止上卸货
        /// </summary>
        StopUp=0xfc,
        /// <summary>
        /// 小车报警
        /// </summary>
        Alert=0xfd,
        /// <summary>
        /// 获取小车信息
        /// </summary>
        GetInfo=0x30,
        /// <summary>
        /// 小车回复的小车当前状态
        /// </summary>
        ReInfo=0x33,
        /// <summary>
        /// 小车应答指令
        /// </summary>
        ReAccept=0x3f
    }
    /// <summary>
    /// AGV小车当前信息
    /// </summary>
    public class AGVInfo {
        /// <summary>
        /// 小车信息最后更新时间
        /// </summary>
        public DateTime LastChange;
        /// <summary>
        /// 最后主界面读取时间 如果LastChange比本日期新则刷新
        /// </summary>
        public DateTime LastRead;
        /// <summary>
        /// 发出消息时间 获取状态
        /// </summary>
        public DateTime SendTime;
        /// <summary>
        /// 最后发出指令时间
        /// </summary>
        public DateTime LastCmdTime;
		/// <summary>
		/// 是否在线 主程序设置标志
		/// </summary>
		public Boolean OnLine = false;
        /// <summary>
        /// 已发送指令，但未收到任何数据及状态  收到消息后自动清状态
        /// </summary>
        public bool NotRequest = false;
		/// <summary>
		/// 运行步序 是空闲，前往目标处，上箱，下箱，等
		/// </summary>
		public byte RunStep = 0;
        /// <summary>
        /// 小车编号地址
        /// </summary>
        public Int16 ID;
        /// <summary>
        /// 当前经过的线路标识卡号
        /// </summary>
        public Int16 LineCard;
        /// <summary>
        /// 故障码 0无故障，其它均为有故障
        /// </summary>
        public byte Error;
        /// <summary>
        /// 小车运行模式状态
        /// </summary>
        public AGVinfoRunEnum RunMode;
        /// <summary>
        /// 小车当前电压值
        /// </summary>
        public Int16 U;
        /// <summary>
        /// 当前线路号码 小车读取的值
        /// </summary>
        public byte LineNumber;
        /// <summary>
        /// 当前发送的，软件设置的目标路线值,表明正在往此处调车，运行到位后置0，非小车读取值
        /// 当收到消息时检查此值是否》0，如果是则检查是否到目标站点，到了则清此值
        /// </summary>
        public byte SetLineNumber;
        /// <summary>
        /// 上次小车指令 如果上次小车指令发送的是上货或卸货，则不发送
        /// </summary>
        public AGVtcpCmdEnum LastCmd;
        /// <summary>
        /// 当前是否有箱子在上面
        /// </summary>
        public bool HasBox;
		/// <summary>
		/// 箱子类型 大箱/小箱 放箱后要保存箱子类型
		/// </summary>
		public AGVinfoBoxType BoxType;
        /// <summary>
        /// 盒子当前滚动状态
        /// </summary>
        public AGVinfoBoxDir BoxDir;
        public override string ToString()
        {
            return string.Format("AGV状态 车号:{0},线路标识卡号:{1},故障代码:{2},运行模式:{3},电压值:{4},线路号:{5},有箱子:{6},箱子方向:{7}"
                ,                           ID,     LineCard,        Error,   RunMode,     U,       LineNumber, HasBox, BoxDir);
        }
    }
    /// <summary>
    /// 盒子滚动方向
    /// </summary>
    public enum AGVinfoBoxDir : byte { 
        /// <summary>
        /// 停止
        /// </summary>
        停止=0,
        /// <summary>
        /// 左转
        /// </summary>
        左=1,
        /// <summary>
        /// 右转
        /// </summary>
        右=2
    }
	/// <summary>
	/// 小车箱子类型
	/// </summary>
	public enum AGVinfoBoxType : byte {
		/// <summary>
		/// 未知箱子
		/// </summary>
		未知=0,
		/// <summary>
		/// 大箱子
		/// </summary>
		大箱=1,
		/// <summary>
		/// 小箱子
		/// </summary>
		小箱=2
	}
    /// <summary>
    /// 小车运行状态枚兴趣 运行=1,停止=2,锁定=4
    /// </summary>
    public enum AGVinfoRunEnum:byte
    {
        /// <summary>
        /// 小车在运行状态
        /// </summary>
        运行=1,
        /// <summary>
        /// 小车在停止状态
        /// </summary>
        停止=2,
        /// <summary>
        /// 小车在锁定状态
        /// </summary>
        锁定=4
    }
    
}
