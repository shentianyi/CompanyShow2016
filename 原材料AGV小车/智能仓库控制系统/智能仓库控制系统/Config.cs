using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
namespace 智能仓库控制系统
{
	/// <summary>
	/// 读取存储配置信息
	/// </summary>
	public class Config
	{
		/// <summary>
		/// XML文档对象，不建议直接访问
		/// </summary>
		public XmlDocument XmlDoc = null;
		/// <summary>
		/// 是否修改了配置没存储
		/// </summary>
		public bool IsModify = false;

		/// <summary>
		/// 程序初始化，读取配置文件
		/// </summary>
		public Config()
		{
			try
			{
//#if DEBUG

//                Environment.CurrentDirectory = @"C:\Users\Administrator\Desktop\智能仓库控制系统 4\智能仓库控制系统\bin\Debug";
//#endif
				XmlDoc = new XmlDocument();
				XmlDoc.Load("config.xml");
			}
			catch(Exception e)
			{
				System.Windows.Forms.MessageBox.Show("初始化系统配置[config.xml]文档出错\r\n系统即将退出\r\n"+e.ToString(), "严重错误");
//#if !DEBUG
//                Environment.Exit(1);
//#endif
			}
		}
		/// <summary>
		/// 获取配置项的内容
		/// </summary>
		/// <param name="XmlPath">XML配置路径，如 config\agv\port </param>
		/// <returns>XML配置值</returns>
		public string GetValue(string XmlPath)
		{

			try
			{
				var node = XmlDoc.SelectSingleNode(XmlPath);
				if(node==null)
					goto error;
				else
					return node.InnerText;
			}
			catch
			{
				goto error;
			}
			error:
				System.Windows.Forms.MessageBox.Show("读取配置数据出错\r\n" + XmlPath, "错误");
				return null;
		}
        /// <summary>
        /// 获取XML节点列表中值为find_value的节点属性XX值
        /// </summary>
        /// <param name="xml_path">xml路径 </param>
        /// <param name="find_value">要查找的值</param>
        /// <param name="xx">属性</param>
        /// <returns></returns>
        public string GetValueFind(string xml_path, string find_value, string xx) {
            try
            {
                var nodes = XmlDoc.SelectNodes(xml_path);
                foreach (XmlNode item in nodes)
                {
                    if (item.InnerText == find_value)
                    {
                        return item.Attributes.GetNamedItem(xx).Value;

                    }
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 获取节点属性为XX的内容
        /// </summary>
        /// <param name="xml_path">xml路径 </param>
        /// <param name="xx">属性名称</param>
        /// <param name="xx">属性值</param>
        /// <returns></returns>
        public string GetxxFind(string xml_path, string xx,string xxValue)
        {
            try
            {
                var nodes = XmlDoc.SelectNodes(xml_path);
                foreach (XmlNode item in nodes)
                {
                    if (item.Attributes.GetNamedItem(xx).Value == xxValue) {
                        return item.InnerText;
                    }
                }
            }
            catch { }
            return null;
        }

		/// <summary>
		/// 保存配置项的值，注意设置后需要调用 Save 函数才会真正存储到本地
		/// </summary>
		/// <param name="XmlPath">XML配置路径，如 config\agv\port </param>
		/// <param name="Value">XML配置值</param>
		/// <returns>保存是否成功</returns>
		public bool SetValue(string XmlPath, object Value)
		{
			var node = XmlDoc.SelectSingleNode(XmlPath);
			if (node == null)
			{
				System.Windows.Forms.MessageBox.Show("读取配置数据出错\r\n" + XmlPath, "错误");
				return false;
			}
			else
			{
				try
				{
					node.Value = Value.ToString();
					IsModify = true;
					return true;
				}
				catch
				{
					return false;
				}
			}
		}
		/// <summary>
		/// 读取或保存配置项的值，保存时需要调用Save函数才真正存储到本地
		/// </summary>
		/// <param name="XmlPath">XML配置路径，如 config\agv\port</param>
		/// <returns>读取的配置值</returns>
		public string this[string XmlPath]
		{
			get
			{
				return GetValue(XmlPath);
			}
			set
			{
				SetValue(XmlPath, value);
			}
		}
		/// <summary>
		/// 保存配置文件至本地
		/// </summary>
		/// <returns></returns>
		public bool Save()
		{
			try
			{
				XmlDoc.Save("config.xml");
				IsModify = false;
				return true;
			}
			catch
			{
				return false;
			}

		}
	}
}
