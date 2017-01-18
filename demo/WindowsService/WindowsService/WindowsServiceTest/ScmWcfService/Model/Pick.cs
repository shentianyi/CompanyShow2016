using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using ScmWcfService.Model.Enum;

namespace ScmWcfService.Model
{
    [DataContract]
    public class Pick
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string nr { get; set; }

        [DataMember]
        public PickStatus status { get; set; } 
        
        [DataMember]
        public DateTime created_at { get; set; }

        [DataMember]
        public string orderable_nr { get; set; }

        [DataMember]
        public List<PickItem> pick_items { get; set; }
        

        public string status_display { get { return PickStatusDisplay.GetStatusOfPick(this.status); } }
    }
}