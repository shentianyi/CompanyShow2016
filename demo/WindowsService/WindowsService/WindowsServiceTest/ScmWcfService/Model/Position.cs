using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace ScmWcfService.Model
{
    [DataContract]
    public class Position
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string nr { get; set; }

        [DataMember]
        public Led led { get; set; }

        [DataMember]
        public DockPoint dock_point { get; set; }
    }
}