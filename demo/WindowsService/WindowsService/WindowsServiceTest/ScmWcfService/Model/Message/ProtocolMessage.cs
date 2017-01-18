using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ScmWcfService.Model.Message
{
    [DataContract]
    public class ProtocolMessage<T>
    {
        public ProtocolMessage() { }
        public ProtocolMessage(Boolean result)
        {
            this.result = result;
        }
        public Boolean result { get; set; }

        public string content { get; set; }

        //[DataMember]
        public T data { get; set; }
    }
}