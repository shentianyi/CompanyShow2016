using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScmWcfService.Model.Enum
{
    public enum OrderStatus
    {
        INIT=100,
        PICKING=200,
        PICKED=300,
        DELIVERED=400,
        ABORTED=500
    }
}