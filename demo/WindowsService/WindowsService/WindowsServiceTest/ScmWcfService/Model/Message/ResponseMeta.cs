using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace ScmWcfService.Model.Message
{
    [DataContract]
    public class ResponseMeta
    {
        public ResponseMeta() {
            this.code = 400;
        }

        [DataMember]
        public string error_type { get; set; }
        [DataMember]
        public int code { get; set; }
        [DataMember]
        public string error_message { get; set; }
        [DataMember]
        public string message { get; set; }
    }
}