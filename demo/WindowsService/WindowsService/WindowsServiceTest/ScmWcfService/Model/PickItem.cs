using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace ScmWcfService.Model
{
    [DataContract]
    public class PickItem
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
        public OrderBox order_box { get; set; }

        [DataMember]
        public string created_at { get; set; }

        [DataMember]
        public float weight{ get; set; }

        [DataMember]
        public float weight_qty { get; set; }


        [DataMember]
        public bool weight_valid { get; set; }




        // partial for view
        public string part_nr
        {
            get {
                return this.part == null ? "" : this.part.nr;
            }
        }

        public string order_box_nr
        {
            get
            {
                return this.order_box == null ? "" : this.order_box.nr;
            }
        }

        public string order_box_type_name {
            get {
                return this.order_box == null ? "" : this.order_box.box_type_name;
            }
        }

        public string positions_nr
        {
            get
            {
                return this.order_box == null ? "" : this.order_box.positions_nr;
            }
        }
    }
}