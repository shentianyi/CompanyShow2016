using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using ScmWcfService.Model.Enum;

namespace ScmWcfService.Model
{
    [DataContract]
    public class OrderBox
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string nr { get; set; }

        [DataMember]
        public string rfid_nr { get; set; }

        [DataMember]
        public OrderBoxStatus status { get; set; }

        [DataMember]
        public float weight { get; set; }

        [DataMember]
        public OrderBoxType order_box_type { get; set; }

        [DataMember]
        public Warehouse warehouse { get; set; }


        [DataMember]
        public Position position { get; set; }

        [DataMember]
        public Warehouse source_warehouse { get; set; }

        [DataMember]
        public Part part { get; set; }

        [DataMember]
        public float? quantity { get; set; }

        [DataMember]
        public float actual_quantity { get; set; }

        [DataMember]
        public float? stock { get; set; }

        [DataMember]
        public List<string> positions { get; set; }

        [DataMember]
        public Led box_led { get; set; }

        [DataMember]
        public List<Position> position_leds { get; set; }


        // partial for view
        public string box_type_name { get { return this.order_box_type == null ? "" : this.order_box_type.name; } }
        public string warehouse_nr { get { return this.warehouse == null ? "" : this.warehouse.nr; } }
        public string position_nr { get { return this.position == null ? "" : this.position.nr; } }
        public string part_nr { get { return this.part == null ? "" : this.part.nr; } }
        public string source_warehouse_nr { get { return this.source_warehouse == null ? "" : this.source_warehouse.nr; } }
        public bool over_stock { get { return this.quantity > stock; } }
        public string positions_nr { get { return this.positions == null ? "" : string.Join(",", this.positions); } }
        public string status_display { get { return OrderBoxStatusDisplay.GetStatusOfOrderBox(this.status); } }



        public static List<int> GetAllIds(List<OrderBox> orderBoxes)
        {
            return (from bb in orderBoxes select bb.id).ToList();
        }


        public static List<string> GetAllNrs(List<OrderBox> orderBoxes)
        {
            return (from bb in orderBoxes select bb.nr).ToList();
        }

        public static int GetNotNullCount(List<OrderBox> orderBoxes)
        {
            return GetNotNull(orderBoxes).Count;
        }


        public static List<OrderBox> GetNotNull(List<OrderBox> orderBoxes)
        {
            return (from bb in orderBoxes where bb.id > 0 select bb).ToList();
        }

        public static void Updates(List<OrderBox> froms, List<OrderBox> tos)
        {
            foreach (var b in tos)
            {
                var oob = (from bb in froms where bb.nr.Equals(b.nr) select bb).FirstOrDefault();
                if (oob != null)
                {
                    int index = froms.IndexOf(oob);
                    froms.Remove(oob);
                    froms.Insert(index, b);
                }
            }
        }
    }
}