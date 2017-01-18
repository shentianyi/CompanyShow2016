using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using ScmWcfService.Model.Enum;

namespace ScmWcfService.Model
{
    [DataContract]
    public class Order
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string nr { get; set; }

        [DataMember]
        public OrderStatus status { get; set; } 

        [DataMember]
        public DateTime created_at { get; set; }

        [DataMember]
        public int orderable_id{ get; set; }

        [DataMember]
        public string orderable_type { get; set; }

        [DataMember]
        public List<OrderItem> order_items { get; set; }
    }
}