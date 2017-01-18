using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace ScmWcfService.Model.Message
{
    [DataContract]
    public class ResponseMessage<T>
    {
        public ResponseMessage()
        {
            this.meta = new ResponseMeta();
            this.http_error = false;
        }

        [DataMember]
        public ResponseMeta meta { get; set; }

        [DataMember]
        public T data { get; set; }

        [DataMember]
        public bool http_error { get; set; }

        public bool Success
        {
            get
            {
                return this.meta.code == 200;
            }
        }

        public string Message {
            get {
                if (this.Success) {
                    if (typeof(T) ==typeof( string))
                    {
                        return this.data.ToString();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return this.meta.error_message;
                }
            }
        }
    }
}