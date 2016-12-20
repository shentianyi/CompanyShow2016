using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadMessage.ENUM
{
    
    public enum NumForIp
    {
        [Description("192.168.1.111:4001")]
        FirstBus= 1,
        [Description("10.10.10.18:8001")]
        SecBus=2,
        [Description("10.10.10.18:8002")]
        ThiBus=3,
        [Description("192.168.1.113:4002")]
        WMS1=4,
        [Description("192.168.1.113:4003")]
        WMS2 = 5,
        [Description("192.168.1.113:4004")]
        WMS3 = 6




    }

  
}
