using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace ScmWcfService.Model
{
    [DataContract]
    public class UserSession
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string nr { get; set; }

        [DataMember]
        public string token { get; set; }
    }
}