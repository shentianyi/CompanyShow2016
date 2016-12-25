using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace 智能仓库控制系统
{
	public class mainAction
	{
		/// <summary>
		/// 主工作流程线程
		/// </summary>
		public static void MainFunc() {
            bool CrossRoadCar = false;  // 十字路口有车正通过 需要等待 十字路口是否可通行
            int CrossCheckCarID = 0;       //有车经过十字路口，需要检查是否走完（即停止状态），走完清CrossRoadCar标志
            int CrossCheckCarLineCard = 0;  //有车经过十字路口，需要检查是否走到目标点，则清CrossRoadCar标志
            //int CrossCheckCarLineCard2 = 0;  //CrossCheckCarLineCard,CrossCheckCarLineCard2不等于这两个才算过十字路口
            int tmrMainActionCount1 = 0;    //写当前6辆小车信息到OPC中，至少0.5以上写入一次
            int tmrMainActionCount2 = 0;    //其它流程
            int tmrMainActionCount3 = 0;    //检查十字路口状态
            bool BigBoxFirst = false;       //大箱流水线优先调用模式
            bool CDflag = false;            //拐点标识，
            Int16 CDCarID = 0;                //首先进入拐点的车辆
            int BigBoxCount = 0;            //大箱待机数量计数
            int SmallBoxCount = 0;          //小箱待机数量计数
            //0.1秒定时器
            Timer tmr = new Timer(func => {
                tmrMainActionCount1++;
                tmrMainActionCount2++;
                tmrMainActionCount3++;
            }, null, 100, 100);
            /*
            * 开机检测总待机位是否有车
            * 成品大箱/小箱位是否有车
            * 没有则调一辆至成品待机位
            * 成品大箱/小箱十字路口待机位是否有车
            * 没有调一辆
            成品大箱/小箱呼叫		表明工位做完，需走一辆车入库，再来一辆车待机
            */
            while (true)
            {
                try
                {
                    #region 0.5秒写入6辆小车状态
                    if (tmrMainActionCount1 > 5)
                    {    //0.5秒写入一次数据至OPC小车信息 ，小车在线时写入
                        tmrMainActionCount1 = 0;
                        foreach (var item in App.AGVsInfo)
                        {
                            if (item.OnLine)
                            {
                                var s = System.Diagnostics.Stopwatch.StartNew();
                                WriteAGVinfo(item.ID, "stu", (byte)item.RunMode); //AGV运行状态
                                WriteAGVinfo(item.ID, "step", item.RunStep);    //运行步骤
                                WriteAGVinfo(item.ID, "err", item.Error); //AGV故障码
                                WriteAGVinfo(item.ID, "box", item.HasBox ? 1 : 0);  //箱子状态
                                WriteAGVinfo(item.ID, "gun", (byte)item.BoxDir);  //箱子方向
                                WriteAGVinfo(item.ID, "point", item.LineCard);  //地标卡位置
                                s.Stop(); System.Diagnostics.Debug.WriteLine(s.ElapsedMilliseconds);
                            }
                            else
                            {//离线时也要写入标志，暂定为急停标志
                                WriteAGVinfo(item.ID, "stu", AGVinfoRunEnum.停止); //AGV运行状态
                            }
                        }
                    }
                    #endregion
                    #region 十字路口检测
                    if (tmrMainActionCount3 >2) { //检查十字路口状态    不读OPC，可以快点运行 0.2s
                        tmrMainActionCount3 = 0;
                        if (CrossRoadCar && CrossCheckCarID>0)
                        { //有车经过十字路口
                            var h = App.AGVsInfo.SingleOrDefault(n => CrossCheckCarID == n.ID && (n.LineCard == CrossCheckCarLineCard ));
                            if (h != null)
                            {    //该车已停止说明已走完十字路口
                                CrossRoadCar = false;//清十字路口有车标志
                                CrossCheckCarID = 0;
                                CrossCheckCarLineCard = 0;
                            }
                            else {
                               
                            }
                        }
                    }
                    #endregion
                    //主工作流程
                    if (tmrMainActionCount2 > 5)
                    {
                        tmrMainActionCount2 = 0;

                        #region 特殊错误情况处理
                        //如果停在总待机位或充电待机位，则清除setlinenumber编号
                        try
                        {
                            var h81 = App.AGVsInfo.SingleOrDefault(n => n.LineCard == 6 && n.RunMode == AGVinfoRunEnum.停止 && (n.SetLineNumber != 1 || n.SetLineNumber != 2));
                            var h82 = App.AGVsInfo.SingleOrDefault(n => n.LineCard == 4 && n.RunMode == AGVinfoRunEnum.停止 && (n.SetLineNumber != 1 || n.SetLineNumber != 2));
                            if (h81 != null)
                            {
                                h81.SetLineNumber = 0;
                                App.Logs.AddLog("小车异常挪动，已恢复标志.  小车ID:" + h81.ID, true);
                            }
                            if (h82 != null)
                            {
                                h82.SetLineNumber = 0;
                                App.Logs.AddLog("小车异常挪动，已恢复标志.  小车ID:" + h82.ID, true);
                            }
                            foreach (var info in App.AGVsInfo)
                            {
                                if (info.SetLineNumber > 0)
                                { //有发送目标路线,检查是否到目标站点，且停止，如果是清SetLineNumber
                                    if (info.LineCard != AGV.GetAGVLineStationCard(info.SetLineNumber)&&info.RunMode == AGVinfoRunEnum.停止 && info.NotRequest == false)
                                    {
                                        //如果手动停止也会触发
                                        //info.SetLineNumber = 0;
                                        //App.Logs.AddLog("小车" + info.ID + "异常停止",true);
                                    }
                                    //手动路线时
                                    if (info.LineCard == AGV.GetAGVLineStationCard(info.LineNumber) && info.LineCard != AGV.GetAGVLineStationCard(info.SetLineNumber) && info.RunMode == AGVinfoRunEnum.停止 && info.NotRequest == false)
                                    {
                                        info.SetLineNumber = 0;
                                        App.Logs.AddLog("小车" + info.ID + "异常挪动，已恢复相关状态",true);
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                        #endregion

                        #region 大小箱上流水线优先判断
                        //************11   
                        /*
                            大箱，小箱计数
                            在成品工位+1，在十字路口待进位或在路线中+1
                            如果 《2 则优先调配到该空箱流水线，大小箱值小的优先，相同则小箱优先
	                            否则不调配到流水线上
                         */
                        BigBoxCount = 0; SmallBoxCount = 0;
                        if (GetCarInLineCard(14) != null)
                            SmallBoxCount++;
                        if (GetCarInLineCard(11) != null)
                            SmallBoxCount++;
                        if (App.AGVsInfo.Where(n => n.SetLineNumber == 4).Count() > 0)
                            SmallBoxCount++;
                        if (GetCarInLineCard(18) != null)
                            BigBoxCount++;
                        if (GetCarInLineCard(15) != null)
                            BigBoxCount++;
                        if (App.AGVsInfo.Where(n => n.SetLineNumber == 3).Count() > 0)
                            BigBoxCount++;
                        if (BigBoxCount < SmallBoxCount) {
                            BigBoxFirst = true;
                        }else{
                            BigBoxFirst=false;
                        }
                        #endregion

                        #region 拐点锁定延时
                        try
                        {
                            var h71 = App.AGVsInfo.SingleOrDefault(n => n.LineCard == 31);
                            if (h71 != null) { //拐点忙标识 ，后边车辆经过31时暂停
                                if (CDflag && CDCarID > 0)
                                { //拐点有车，暂时锁定
                                    if(CDCarID!=h71.ID)
                                        AGV.SendAGVLock(h71.ID);
                                }
                                else { //没车，置标志
                                    CDflag = true;
                                    CDCarID = h71.ID;
                                }
                            }
                        }catch{}
                        try
                        {
                            var h72 = App.AGVsInfo.SingleOrDefault(n => n.LineCard == 30);
                            if (h72 != null)
                            { //解锁拐点忙标识
                                //解锁后边等待的车轴
                                var h73 = App.AGVsInfo.SingleOrDefault(n => n.RunMode == AGVinfoRunEnum.锁定&&n.LineCard==31);
                                if (h73 != null)
                                {
                                    AGV.SendAGVUnLock(h73.ID);
                                    CDCarID = h73.ID; CDflag = true;
                                }
                                else
                                {
                                    CDCarID = 0; CDflag = false;
                                }
                            }
                        }
                        catch { }
                        #endregion

                        #region 4条流水线状态检测
                        //***********0 上下货指令
                        //检查四条流水线的上下货的OPC标志，是否允许上货或卸货，
                        //并写入相关状态
                        //写入滚动状态
                        //大空箱流水线1 车
                        var h30 = GetCarInLineCard(8);
                        if (h30 != null)
                        {
                            WriteOPC("ssj_1_AGVstu", 1);//小车到达信号
                            WriteOPC("ssj_1_AGVgun", (byte)h30.BoxDir);//流水线1滚状态
                            if (ReadOPC("ssj_1_en").IntValue == 2)
                            { //上货允许 大空箱位
                                if (AGV.SendAgvUpBox(h30.ID, false, AGVinfoBoxType.大箱))
                                {//左上货
                                    //continue;
                                }
                            }
                        }
                        else {
                            WriteOPC("ssj_1_AGVstu",0);//小车到达信号
                        }
                        //小空箱流水线2 车
                        var h31 = GetCarInLineCard(9);
                        if (h31 != null)
                        {
                            WriteOPC("ssj_2_AGVstu", 1);//小车到达信号
                            WriteOPC("ssj_2_AGVgun", (byte)h31.BoxDir);//流水线2滚状态
                            if (ReadOPC("ssj_2_en").IntValue == 2)
                            { //上货允许 
                                if (AGV.SendAgvUpBox(h31.ID, false, AGVinfoBoxType.小箱))
                                {//左上货
                                    //continue;
                                }
                            }
                        }
                        else {
                            WriteOPC("ssj_2_AGVstu",0);//小车到达信号
                        }
                        //大成品流水线3 车
                        var h32 = GetCarInLineCard(18);
                        if (h32 != null)
                        {
                            WriteOPC("ssj_3_AGVstu", 1);//小车到达信号
                            WriteOPC("ssj_3_AGVgun", (byte)h32.BoxDir);//流水线3滚状态
                            if (ReadOPC("ssj_3_en").IntValue == 1)  //卸货
                            { //卸货允许 
                                AGV.SendAgvDownBox(h32.ID, true);//右上货
                            }
                            else if (ReadOPC("ssj_3_en").IntValue == 2)  //上货
                            { //上货允许 
                                if (AGV.SendAgvUpBox(h32.ID, true, AGVinfoBoxType.大箱))
                                {//右上货
                                    //continue;
                                }
                            }
                        }
                        else {
                            WriteOPC("ssj_3_AGVstu", 0);//小车到达信号
                        }
                        //小成品流水线4车
                        var h33 = GetCarInLineCard(14);
                        if (h33 != null)
                        {
                            WriteOPC("ssj_4_AGVstu", 1);//小车到达信号
                            WriteOPC("ssj_4_AGVgun", (byte)h33.BoxDir);//流水线4滚状态
                            if (ReadOPC("ssj_4_en").IntValue == 1)  //卸货
                            { //卸货允许 
                                if (AGV.SendAgvDownBox(h33.ID, true))
                                {//右卸货
                                    //continue;
                                }
                            }
                            else if (ReadOPC("ssj_4_en").IntValue == 2)  //上货
                            { //上货允许 
                                if (AGV.SendAgvUpBox(h33.ID, true, AGVinfoBoxType.小箱))
                                { //右上货
                                    //continue;
                                }
                            }
                        }
                        else {
                            WriteOPC("ssj_4_AGVstu", 0);//小车到达信号
                        }
                        #endregion

                        #region ***小箱1 调车到小箱成品工位 有车时
                        //***********1  调车到小箱成品工位
                        //判断【11-空小箱等待点】是否有车，且在停止状态 
                        var SmallBoxWaitPoint = GetCarInLineCard(11);
                        if (SmallBoxWaitPoint != null)//这个11点有车
                        {   //检查14点，成品小箱点是否有车，且在停止状态，如果没有则可通调一个过来
                            var SmallBoxProPoint = GetCarInLineCard(14);
                            var h413 = GetCarInLineCard(13); //且13站点也没车
                            //小箱成品工位路线 是否有车往这里进入或离开
                            var SmallBoxProLine = App.AGVsInfo.SingleOrDefault(n => (
                                n.SetLineNumber == 6||n.SetLineNumber==8||
                                    (n.SetLineNumber==10&&n.LineCard==13)
                                ));
                            if (SmallBoxProPoint == null && SmallBoxProLine == null && h413==null)
                            {//这个点没有车且没有车往这个路线走
                                if (CrossRoadCar == false)
                                {//检查十字路口是否可通行,可通行则调一辆车往小成品工位路线走
                                    CrossCheckCarID = SmallBoxWaitPoint.ID;
                                    CrossRoadCar = true;
                                    CrossCheckCarLineCard = 13;
                                    if (AGV.SendAGVRun(SmallBoxWaitPoint.ID, 6, false) == false)
                                    {
                                        CrossCheckCarID = 0;
                                        CrossRoadCar = false;
                                        CrossCheckCarLineCard = 0;
                                    }else
                                        continue;
                                }
                            }
                        }
                        #endregion
                        #region ***小箱2 调车到空小箱十字路口等待点
                        if (SmallBoxWaitPoint==null)
                        { //11-空小箱等待点 没车，再检查是否有车往这里调 {4 小箱十字路口待机位进 11}
                            var h = App.AGVsInfo.SingleOrDefault(n => n.SetLineNumber == 4);
                            if (h == null)
                            { //没有小车往 11-空小箱等待点调配,可从【二号流水线小箱点】调一辆过来
                                //检查9号站点，二号流水线小箱点是否有车，没车另一过程处理
                                var l2 = GetCarInLineCard(9);
                                if(l2!=null){   //9号站点 二号流水线小箱点 有车，需下一步检测
                                    WriteOPC("ssj_2_AGVstu", 1);    //2号流水线空小箱 ，小车到达
                                    //调配前检查OPC是否允许
                                    if (ReadOPC("ssj_2_en").IntValue == 4) { //OPC允许离开
                                        WriteOPC("ssj_2_AGVstu", 0);
                                        if (AGV.SendAGVRun(l2.ID, 4, false))
                                        {//到 【4- 到小箱成品进管制位置】位置 
                                            continue;
                                        }
                                    }//不允许说明没上完箱子，等待
                                }else{  //9号站点 二号流水线小箱点 没车或在途中  //没车另一过程处理
                                }
                            }
                            else { //有小车往这里走，等待，不做处理                            
                            }
                        }
                        #endregion
                        #region ***小箱3 调车至小箱流水线
                        //**************2
                        //9号站点没车  二号流水线小箱点  ，或没车往这里调配时，调一辆车
                        var h2 = GetCarInLineCard(9);
                        if (h2 == null && BigBoxFirst==false&&SmallBoxCount<2)
                        {//没车 ，检查是否有往这里调配
                            var h61 = GetCarInLineCard(11);//十字路口小箱等待工位没车才往小箱流水线配车
                            if (h61 == null)
                            {
                                if (App.AGVsInfo.SingleOrDefault(n => n.SetLineNumber == 2) == null)
                                {
                                    //没车往这里调配，调一辆,从6号总待机工位或4号充电完成工位，6号优先
                                    var h3 = GetCarInLineCard(6);
                                    if (h3 != null)
                                    {//6号有车，可调配至9号站点
                                        if (AGV.SendAGVRun(h3.ID, 2, false)) {
                                            BigBoxFirst = false;
                                            continue;
                                        }
                                    }
                                    else
                                    {//没车，需要从4号充电完成工位调
                                        var h4 = GetCarInLineCard(4);
                                        if (h4 != null)
                                        {
                                            if (App.AGVsInfo.Where(n => n.SetLineNumber == 1 || n.SetLineNumber == 2).Count() == 0)//检查待机工位到小箱流水线或大箱流水线没车时方可动作
                                                if (AGV.SendAGVRun(h4.ID, 2, false)) {
                                                    continue;
                                                }
                                        }
                                    }

                                }
                                else
                                {//车在途中，不做处理
                                }
                            }
                        }
                        #endregion
                        #region ***小箱4 小箱出货至十字路口
                        //************3 小箱出货至十字路口

                        //14点检查有车
                        var h5=GetCarInLineCard(14);
                        if (h5 != null) {
                            WriteOPC("ssj_4_AGVstu", 1);    //流水线4小车到达标志
                            if (ReadOPC("ssj_4_en").IntValue == 4) {    //小车允许离开标志
                                WriteOPC("ssj_4_AGVstu", 0);
                                if (AGV.SendAGVRun(h5.ID, 8, true)) {
                                    continue;
                                }//成品小箱到 4 小箱出管制工位 ，倒走
                            }
                        }
                        #endregion
                        #region ***小箱5 小箱十字路口至扫描工位
                        //*************4 小箱十字路口至扫描工位
                        var h6 = GetCarInLineCard(13);
                        if (h6 != null) { //有车
                            if (CrossRoadCar == false) { //十字路口允许通行
                                CrossRoadCar = true;
                                CrossCheckCarID = h6.ID;
                                CrossCheckCarLineCard = 19;
                                if (AGV.SendAGVRun(h6.ID, 10, true) == false)
                                {   //小箱十字路口至扫描工位 倒走
                                    CrossCheckCarID = 0;
                                    CrossRoadCar = false;
                                    CrossCheckCarLineCard = 0;
                                    //CrossCheckCarLineCard2 = 0;
                                }else
                                    continue;
                            }
                        }
                        #endregion

                        #region +++通用1 扫描工位》入库
                        //************5 扫描工位》入库
                        var h7 = GetCarInLineCard(21);
                        if (h7 != null)
                        {   //有车 检查OPC是否允许离开 
                            WriteOPC("scan_AGVstu", 1); //扫描车到达
                            if (ReadOPC("scan_en").IntValue == 4)
                            {
                                WriteOPC("scan_AGVstu", 0); //扫描车到达 清零
                                if (AGV.SendAGVRun(h7.ID, 11, false))
                                {
                                    continue;
                                }   //正向，入库
                            }
                        }
                        else {
                            WriteOPC("scan_AGVstu", 0); 
                        }
                        #endregion
                        #region +++通用2 入库动作
                        //*****************6 入库动作 
                        var h8 = GetCarInLineCard(1);
                        if (h8 != null)
                        {
                            WriteOPC("in_AGVstu", 1);   //入库小车到达
                            if (ReadOPC("in_en").IntValue == 4)
                            {
                                WriteOPC("in_AGVstu", 0);   //入库小车到达 清空
                                var u = Convert.ToInt16(App.Config["/Config/agv/lowU"]);
                                if (h8.U < u)
                                {//电压低，进入允电工位
                                    if (AGV.SendAGVRun(h8.ID, 13, false))
                                    {
                                        continue;
                                    }
                                }
                                else
                                {  //电压正常，进行总待工位
                                    if (AGV.SendAGVRun(h8.ID, 12, false))
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                        else {
                            WriteOPC("in_AGVstu", 0);
                        }
                        #endregion

                        #region ###大箱1 调车到大箱成品工位 有车时
                        //***********7  调车到大箱成品工位
                        //判断【15-空小箱等待点】是否有车，且在停止状态 
                        var h9 = GetCarInLineCard(15);
                        if (h9 != null)//这个15点有车
                        {   //检查18点，成品大箱点是否有车，且在停止状态，如果没有则可通调一个过来
                            var h10 = GetCarInLineCard(18);
                            var h417 = GetCarInLineCard(17);
                            //大箱成品工位路线 是否有车往这里进入或离开
                            var h11 = App.AGVsInfo.SingleOrDefault(n => (
                                n.SetLineNumber == 5||n.SetLineNumber==7||
                                    (n.SetLineNumber==9&&n.LineCard==17)
                                ));
                            if (h10 == null && h11 == null&&h417==null)
                            {//这个点没有车且没有车往这个路线走
                                if (CrossRoadCar == false)
                                {//检查十字路口是否可通行,可通行则调一辆车往大成品工位路线走
                                    if (AGV.SendAGVRun(h9.ID, 5, false) == false)
                                    {

                                    }else
                                        continue;
                                }
                            }
                        }
                        #endregion
                        #region ###大箱2 调车到空大箱十字路口等待点
                        if (h9==null)
                        { //15-空大箱等待点 没车，再检查是否有车往这里调 
                            var h = App.AGVsInfo.SingleOrDefault(n => n.SetLineNumber == 3);
                            if (h == null)
                            { //没有大车往 15-空小箱等待点调配,可从【一号流水线大箱点】调一辆过来
                                //检查8号站点，一号流水线大箱点是否有车，没车另一过程处理
                                var l2 = GetCarInLineCard(8);
                                if (l2 != null)
                                {   //9号站点 二号流水线小箱点 有车，需下一步检测
                                    WriteOPC("ssj_1_AGVstu", 1);    //1号流水线空大箱 ，车到达
                                    //调配前检查OPC是否允许
                                    if (ReadOPC("ssj_1_en").IntValue == 4)
                                    { //OPC允许离开
                                        WriteOPC("ssj_1_AGVstu", 0);
                                        if (AGV.SendAGVRun(l2.ID, 3, false))
                                        {//到 【3- 到大箱成品进管制位置】位置 
                                            continue;
                                        }
                                    }//不允许说明没上完箱子，等待
                                }
                                else
                                {  //8号站点 一号流水线大箱点 没车或在途中  //没车另一过程处理
                                }
                            }
                            else
                            { //有车往这里走，等待，不做处理                            
                            }
                        }
                        #endregion
                        #region ###大箱3 调车至大箱流水线
                        //**************8
                        //8号站点没车  一号流水线大箱点  ，或没车往这里调配时，调一辆车
                        var h20 = GetCarInLineCard(8);
                        if (h20 == null&&BigBoxCount<2)
                        {//没车 ，检查是否有往这里调配
                            var h62 = GetCarInLineCard(15);//十字路口大箱等待工位没车才往大箱流水线配车
                            if (h62 == null)
                            {
                                if (App.AGVsInfo.SingleOrDefault(n => n.SetLineNumber == 1) == null)
                                {
                                    //没车往这里调配，调一辆,从6号总待机工位或4号充电完成工位，6号优先
                                    BigBoxFirst = false;
                                    var h3 = GetCarInLineCard(6);
                                    if (h3 != null)
                                    {//6号有车，可调配至9号站点
                                        if (AGV.SendAGVRun(h3.ID, 1, false)) {
                                            continue;
                                        }
                                    }
                                    else
                                    {//没车，需要从4号充电完成工位调
                                        var h4 = GetCarInLineCard(4);
                                        if (h4 != null)
                                        {
                                            if (App.AGVsInfo.Where(n => n.SetLineNumber == 1 || n.SetLineNumber == 2).Count() == 0)//检查待机工位到小箱流水线或大箱流水线没车时方可动作
                                                if (AGV.SendAGVRun(h4.ID, 1, false)) {
                                                    continue;
                                                }
                                        }
                                    }
                                }
                                else
                                {//车在途中，不做处理
                                }
                            }
                        }
                        #endregion
                        #region ###大箱4 大箱出货至十字路口
                        //************9 大箱出货至十字路口

                        //14点检查有车
                        var h25 = GetCarInLineCard(18);
                        if (h25 != null)
                        {
                            WriteOPC("ssj_3_AGVstu", 1);    //流水线3大车到达标志
                            if (ReadOPC("ssj_3_en").IntValue == 4)
                            {    //小车允许离开标志
                                WriteOPC("ssj_3_AGVstu", 0);
                                if (AGV.SendAGVRun(h25.ID, 7, true))
                                {//成品大箱到 大箱出管制工位 ，倒走
                                    continue;
                                }
                            }
                        }
                        #endregion
                        #region ###大箱5 大箱十字路口至扫描工位
                        //*************10 大箱十字路口至扫描工位
                        var h26 = GetCarInLineCard(17);
                        if (h26 != null)
                        { //有车
                            if (CrossRoadCar == false)
                            { //十字路口允许通行
                                CrossRoadCar = true;
                                CrossCheckCarID = h26.ID;
                                CrossCheckCarLineCard = 19;
                                if (AGV.SendAGVRun(h26.ID, 9, true) == false)
                                {   //大箱十字路口至扫描工位 倒走
                                    CrossCheckCarID = 0;
                                    CrossRoadCar = false;
                                    CrossCheckCarLineCard = 0;
                                }
                                else {
                                    continue;
                                }
                            }
                        }
                        #endregion

                    }

                }
                catch (Exception e2)
                {
                    App.Logs.AddLog("工作主流程发生异常错误:" + e2.ToString(), true);
                }
                System.Threading.Thread.Sleep(10);
            }
		}
        /// <summary>
        /// 获取一辆小车 停止某站点且无任何调用时，如果为NULL说明此站点没车，
        /// 如果有多辆车则返回NULL,
        /// </summary>
        /// <param name="LineCard">路标号，根据这个可能有多个结果</param>
        /// <param name="LineNumber">路线号，如果为空则不检查，根据路标号找</param>
        /// <returns></returns>
        static AGVInfo GetCarInLineCard(byte LineCard) {
            try
            {
                var i = App.AGVsInfo.SingleOrDefault(n => (
                                (n.LineCard == LineCard &&n.OnLine && n.RunMode == AGVinfoRunEnum.停止 && n.SetLineNumber == 0)
                                ));
                return i;
            }
            catch {//有多个元素
                //var i2 = App.AGVsInfo.SingleOrDefault(n => (
                //                (n.LineCard == LineCard && n.RunMode == AGVinfoRunEnum.停止 && n.SetLineNumber == LineNumber)
                //                ));
                return null;
            }
        }
        /// <summary>
        /// 将小车运行信息写入OPC供PLC工作
        /// </summary>
        /// <param name="id">小车ID</param>
        /// <param name="key">写的关键字</param>
        /// <param name="value">值</param>
        /// <returns>是否成功</returns>
        static bool WriteAGVinfo(int id, string key, object value)
        {
            string wkey = App.OPC.OPCShortNames["AGV_" + id.ToString() + "_" + key];
            var r = App.OPC.WriteOPCItem(wkey, value);
            return r;
        }
        /// <summary>
        /// 写入OPC
        /// </summary>
        /// <param name="ShortName">OPC条目短名称</param>
        /// <param name="value">值</param>
        /// <returns>是否成功</returns>
        static bool WriteOPC(string ShortName, object value)
        {
            try
            {
                string wkey = App.OPC.OPCShortNames[ShortName];
                var r = App.OPC.WriteOPCItem(wkey, value);
                return r;
            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// 读取OPC条目值
        /// </summary>
        /// <param name="opcShortName">opc条目名称，短名称</param>
        /// <returns></returns>
        static OPC_item.strData ReadOPC(string opcShortName) {
            return App.OPC.GetReadItem(App.OPC.OPCShortNames[opcShortName]).Data;
        }
	}
}
