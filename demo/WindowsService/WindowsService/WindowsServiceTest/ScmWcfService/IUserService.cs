using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using ScmWcfService.Model;
using ScmWcfService.Model.Message;

namespace ScmWcfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract]
    public interface IUserService
    {
        [OperationContract]
        ResponseMessage<UserSession> Login(string nr, string pwd);
    }
}
