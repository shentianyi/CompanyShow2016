using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace ScmWcfService.Model
{

    [DataContract]
    public class Part
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string nr { get; set; }

        [DataMember]
        public float weight { get; set; }

        /// <summary>
        /// percent
        /// </summary>
        [DataMember]
        public float weight_range { get; set; }
    }
}