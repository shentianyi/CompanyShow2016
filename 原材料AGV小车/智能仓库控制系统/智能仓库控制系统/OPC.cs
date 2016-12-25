using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPCAutomation;
using System.Collections;
using System.IO;
using System.Windows.Forms;
namespace 智能仓库控制系统
{
    
    /// <summary>
    /// OPC条目，接收后设置此值
    /// </summary>
    public class OPC_item
    {
        /// <summary>
        /// 添加后生成的唯一句柄，写入用
        /// </summary>
		public int Hwnd;		//
        /// <summary>
        /// 手工顺序ID，读取用
        /// </summary>
		public int ID;
        /// <summary>
        /// 全称标签
        /// </summary>
        public string Name;
        /// <summary>
        /// 中文说明 来自CSV，手工添加
        /// </summary>
        public string Description;  //
        /// <summary>
        /// 数据类型
        /// </summary>
        public string Type;		//数据类型
        /// <summary>
        /// 测量值
        /// </summary>
        public struct strData {
        object _value;
            /// <summary>
            /// 自动转换为整形
            /// </summary>
            public bool BoolValue {
                    get {
                        if (_value == null)
                            return false;
                        if (_value is bool)
                            return (bool)_value;
                        else
                        {
							//return false;
							bool b = false;
							bool.TryParse(_value.ToString(), out b);
							return b;
						}
                    }
                }
              public string StringValue {
                    get {
                        if (_value == null)
                            return "";
                        if (_value is string)
                            return (string)_value;
                        else
                            return _value.ToString();
                    }
                }
                public object Item {
                    get { return _value; }
                    set { _value = value; }
                }
            public int IntValue {
                get {
                    if (_value == null)
                        return 0;
                    else if (_value is int)
                        return (int)_value;
                    else
                    {
                        int v = 0;
                        int.TryParse(_value.ToString(), out v);
                        return v;
                    }
                }
                set { _value = value; }
            }
        };
        public strData Data;
        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Time;
        /// <summary>
        /// 质量
        /// </summary>
        public string Quality;
        /// <summary>
        /// 短名，用于监视时看
        /// </summary>
        public string ShortName
        {
            get
            {
                string[] temp = Name.Split(',');
                return temp.Last();
            }
        }
    }
    public class OPC
    {
        private const string OPC_GROUP_NAME = "trivers";

        //public string IP;
        public OPCServer Server;
        public OPCGroup Group;
        public OPCGroups Groups;
        public OPCBrowser Browser;
        //public OPCItem Item2222222;
        public OPCItems Items;          //这是添加监视值
        public bool isWatch = false;            //是否已添加监视
        //LIST数据多时查找慢，用HashSet数据多时查找比较快
        /// <summary>
        /// OPC条目，接收后设置此值 / 以后可考虑用 Dictionary(string,opc_item)
        /// </summary>
        public Dictionary<string, OPC_item> ReadItems = new Dictionary<string, OPC_item>();

        /// <summary>
        /// OPC短名称对应的全名称 如，S7:[S7 connection_1]DB309,B0,AGV_1_stu 对应AGV_1_stu
        /// </summary>
        public Dictionary<string, string> OPCShortNames = new Dictionary<string, string>();
        //public HashSet<OPC_item> ReadItems = new HashSet<OPC_item>();
        /// <summary>
        /// 找读取的条目
        /// </summary>
        /// <param name="name">OPC名称</param>
        /// <param name="findmode">模糊找</param>
        /// <returns></returns>
        public OPC_item GetReadItem(string name)
        {
			try
			{

				return ReadItems[name]; //ReadItems.Where(n => n.Name.Contains(name)).FirstOrDefault(); //ReadItems[name];
			}
			catch
			{
				//无此KEY
				var t = "未找到OPC条目" + name;
				MessageBox.Show(t);
                App.Logs.AddLog(t, true);
                return null;
			}
        }


        public OPC_item GetReadItem(int id)
        {
            return ReadItems.Values.Where(n =>  n.ID == id).First();
        }

        public OPCItem this[int ClientHandle]
        {
            get { return Items.GetOPCItem(ClientHandle); }
        }
        //yc_test
        public OPCItem this[string Name]
        {
            get { return Items.GetOPCItem(ReadItems[Name].Hwnd); }//.Where(n => n.Name.Contains(Name)).FirstOrDefault().Hwnd); }
        }
        //		步骤：

        //1.使用GetOPCServers方法，将本地所有OPC Server上所有OPC服务器服务枚举并写入serverNames变量中

        //2.使用ConnectServer方法，连接服务器。此方法自动创建OPC Group，创建方法中的groupname任意写

        //3.使用ReadValue及WriteValue方法读写数据。
        public OPC()
        {
            try
            {
                Server = new OPCServer();
                object obj = Server.GetOPCServers(App.Config["/Config/opc/ip"]);                         //获取指定IP的OPC服务器列表	192.168.1.99
                //if (((Array)obj).Length > 1)
                //{
                //    System.Windows.Forms.MessageBox.Show("您的计算机上安装了多个OPC服务器，自动选择第1个服务器"+((Array)obj).GetValue(1));
                //}
                //Server.Connect(((Array)obj).GetValue(1).ToString(), App.Config["/Config/opc/ip"]);       //192.168.1.99
                Server.Connect(App.Config["/Config/opc/name"], App.Config["/Config/opc/ip"]);       //192.168.1.99
                if (Server.ServerState == (int)OPCServerState.OPCRunning)
                {
                    //连接OK				//tsslServerState.Text = "已连接到-" + KepServer.ServerName + "   ";
                }
                else
                {                     //这里你可以根据返回的状态来自定义显示信息，请查看自动化接口API文档
                    App.Logs.AddLog("连接OPC服务器失败\r\n软件即将退出,错误代码：" + Server.ServerState,true,true);
                    MessageBox.Show("连接OPC服务器失败\r\n软件即将退出,错误代码：" + Server.ServerState);
//#if !DEBUG
					Environment.Exit(3);
//#endif
                }
                var opc_names = App.Config.XmlDoc.SelectSingleNode("/Config/opc_dat").ChildNodes;
                foreach (System.Xml.XmlNode item in opc_names) {
                    if (item.NodeType == System.Xml.XmlNodeType.Element) {
                        OPCShortNames.Add(item.Name, item.InnerText);
                    }
                    //OPCShortNames.Add(item.
                }

                Groups = Server.OPCGroups;

                Group = Groups.Add(OPC_GROUP_NAME);

                //设置属性
                Server.OPCGroups.DefaultGroupIsActive = true;
                Server.OPCGroups.DefaultGroupDeadband = 0;
                Group.UpdateRate = 250;
                Group.IsActive = true;
                Group.IsSubscribed = true;

                Items = Group.OPCItems;

                Browser = Server.CreateBrowser();
                Browser.ShowBranches();
                Browser.ShowLeafs(true);
                foreach (var item in Browser)
                {
                    string tag = item.ToString();
                    if (OPCShortNames.ContainsValue(tag) == false) {//
                        continue;
                    }
                        OPC_item oitem = new OPC_item();
                        oitem.ID = ReadItems.Count;
                        oitem.Name = tag;
                        try
                        {
                            ReadItems.Add(oitem.Name, oitem); 
                        }
                        catch (Exception e2){
                            //System.Diagnostics.Debug.WriteLine("********"+oitem.Name);
                        }
                }
                System.Threading.Thread.Sleep(500);
                Group.DataChange += Group_DataChange;

                if (Groups.Count < 1)
                {
                    App.Logs.AddLog("连接OPC服务器错误,未找到有效的OPCGroups", true,true);
                    MessageBox.Show("连接OPC服务器错误,未找到有效的OPCGroups");
//#if !DEBUG
                Environment.Exit(4);
//#endif
				}
            }
            catch (Exception e2) {
                App.Logs.AddLog("OPC异常" + e2.ToString(), true);
                System.Windows.Forms.MessageBox.Show("OPC异常" + e2.ToString());
//#if !DEBUG
                Environment.Exit(4);
//#endif
            }
        }
        /// <summary>
        /// 把所有的值添加监视
        /// 注意，初始添加监视会出问题/
        /// 返回 添加成功的数目
        /// 只添加trivers和_system的
        /// </summary>
        public void OPCStartWatch()
        {
            if (isWatch)
                return ;
            isWatch = true;
            foreach (var item in ReadItems.Values)
            {
                try
                {
                    OPCItem tt = Items.AddItem(item.Name, item.ID);
                    item.Hwnd = tt.ServerHandle;
                }
                catch { }
            }
        }

        /// <summary>
        /// 每当项数据有变化时执行的事件
        /// </summary>
        /// <param name="TransactionID">处理ID</param>
        /// <param name="NumItems">项个数</param>
        /// <param name="ClientHandles">项客户端句柄</param>
        /// <param name="ItemValues">TAG值</param>
        /// <param name="Qualities">品质</param>
        /// <param name="TimeStamps">时间戳</param>
        private void Group_DataChange(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps)
        {
            for (int i = 1; i <= NumItems; i++)
            {
                try
                {
                    OPC_item item = GetReadItem((Int32)ClientHandles.GetValue(i));
                    item.Data.Item = ItemValues.GetValue(i).ToString();
                    item.Quality = Qualities.GetValue(i).ToString();
                    item.Time = Convert.ToDateTime(TimeStamps.GetValue(i).ToString()).ToLocalTime();

                    if (App.FrmMain != null) {
                        var items = App.FrmMain.listViewOPC.Items.Find(item.Name, false);
                        if (items.Count() > 0) {
                            items.First().SubItems[2].Text = item.Data.Item.ToString();
                        }
                    }
                    
                }
                catch { }
            }
        }

        public bool WriteOPCItem(OPCItem kepItem, string value)
        {
            try
            {
                kepItem.Write(value);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 写入OPC条目
        /// </summary>
        /// <param name="kepItem">条目全名</param>
        /// <param name="value">值.toString</param>
        /// <returns></returns>
        public bool WriteOPCItem(string kepItem, Object value)
        {
            try
            {
                OPCItem item;
                try
                {
					item = Items.GetOPCItem(ReadItems[kepItem].Hwnd); //.Where(n=>n.Name.Contains(kepItem)).FirstOrDefault().Hwnd);
                }
                catch (Exception e2)
                {
                    App.Logs.AddLog("未找到OPC条目" + kepItem, true);
                    System.Windows.Forms.MessageBox.Show("未找到OPC条目" + kepItem);
                    return false;
                }
                item.Write(value);
            }
            catch (Exception e2)
            {
                App.Logs.AddLog("写入OPC条目发生错误" + kepItem+":"+e2.ToString(), true);
                return false;
            }
            return true;
        }



    }
}
