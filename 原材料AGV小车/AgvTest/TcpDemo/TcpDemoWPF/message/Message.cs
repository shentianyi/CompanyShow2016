
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TcpDemoWPF.message
{
    //[DataContract]
    public class Message<T>
    {
        public Message() { }
        public Message(Boolean result)
        {
            this.result = result;
        }
        public Boolean result { get; set; }

        public string content { get; set; }

        //[DataMember]
        public T data { get; set; }
    }
}
