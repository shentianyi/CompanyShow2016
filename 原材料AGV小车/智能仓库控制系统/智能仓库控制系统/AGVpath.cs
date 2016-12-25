using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
namespace 智能仓库控制系统
{
	/// <summary>
	/// AGV小车绘图路径
	/// </summary>
	public class AGVpath_path{
		/// <summary>
		/// 路径颜色
		/// </summary>
		public Color Color;
		/// <summary>
		/// 路径坐标点
		/// </summary>
		public List<PointF> Path = new List<PointF>();
		/// <summary>
		/// 线路名称
		/// </summary>
		public string Name;
	}
	/// <summary>
	/// AGV小车线路
	/// </summary>
	public class AGVpath_line {
		/// <summary>
		/// 颜色点
		/// </summary>
		public Color Color;
		/// <summary>
		/// ID号
		/// </summary>
		public Int16 ID;
		/// <summary>
		/// 描述名称
		/// </summary>
		public string Name;
		/// <summary>
		/// 小车线路 指向路径坐标
		/// </summary>
		public List<AGVpath_path> line=new List<AGVpath_path>();
	}
	/// <summary>
	/// 站点信息,地标卡
	/// </summary>
	public class AGVpath_station
	{
		/// <summary>
		/// 站点名称
		/// </summary>
		public string Name;
		/// <summary>
		/// 站点号，地标卡
		/// </summary>
		public Int16 ID;
		/// <summary>
		/// 坐标点
		/// </summary>
		public Point Point;
	}
}
