using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScmWcfService.Model.Enum
{
    public enum PickStatus
    {
        INIT=100,
        PICKING=200,
        PICKED=300,
        ABORTED=400
    }

    public class PickStatusDisplay {
        public static string GetStatusOfPick(PickStatus status) {
            string result = string.Empty;
            switch (status) { 
                case PickStatus.INIT:
                    result = "新建";
                    break;
                case PickStatus.PICKING:
                    result = "择货中";
                    break;
                case PickStatus.PICKED:
                    result = "已择货";
                    break;
                case PickStatus.ABORTED:
                    result = "已放弃";
                    break;
                default:
                    result = "N/A";
                    break;
            }
            return result;
        }
    }
}