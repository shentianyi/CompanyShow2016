using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace ScmWcfService.Model
{
    [DataContract]
    public class OrderItem
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public float quantity { get; set; }

        [DataMember]
        public Warehouse warehouse { get; set; }

        [DataMember]
        public Part part { get; set; }


        [DataMember]
        public string orderable_id { get; set; }

        [DataMember]
        public string orderable_type { get; set; }


        [DataMember]
        public string created_at { get; set; }
    }
}