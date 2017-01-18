using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScmWcfService.Model.Enum
{
    public enum OrderCarStatus
    {
        INIT=100,
        PICKING=200
    }

    public class OrderCarStatusDisplay
    {
        public static string GetStatusOfOrderCar(OrderCarStatus status)
        {
            string result = string.Empty;
            switch (status)
            {
                case OrderCarStatus.INIT:
                    result = "空闲";
                    break;
                case OrderCarStatus.PICKING:
                    result = "择货中";
                    break;
                default:
                    result = "N/A";
                    break;
            }
            return result;
        }
    }
}