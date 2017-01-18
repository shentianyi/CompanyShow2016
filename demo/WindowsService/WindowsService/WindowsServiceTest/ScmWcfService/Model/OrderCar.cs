using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using ScmWcfService.Model.Enum;

namespace ScmWcfService.Model
{
    [DataContract]
    public class OrderCar
    {
        [DataMember]
        public int id { get; set; }
        
        [DataMember]
        public string nr { get; set; }
        
        [DataMember]
        public string rfid_nr { get; set; }

        [DataMember]
        public OrderCarStatus status { get; set; } 

        [DataMember]
        public Warehouse warehouse { get; set; }


        // for view
        public string status_display { get { return OrderCarStatusDisplay.GetStatusOfOrderCar(this.status); } }



    }
}