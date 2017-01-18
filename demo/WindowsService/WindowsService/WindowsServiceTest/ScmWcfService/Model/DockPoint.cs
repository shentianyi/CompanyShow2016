using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ScmWcfService.Model
{
    [DataContract]
    public class DockPoint
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string nr { get; set; }

    }

}