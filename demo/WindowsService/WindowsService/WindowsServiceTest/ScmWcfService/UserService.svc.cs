using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using ScmWcfService.Model;
using ScmWcfService.Model.Message;
using RestSharp;
using ScmWcfService.Provider;
using Brilliantech.Framwork.Utils.JsonUtil;
using ScmWcfService.Config;

namespace ScmWcfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“UserService”。
    public class UserService : IUserService
    {
        public ResponseMessage<UserSession> Login(string nr, string pwd)
        {
            var msg = new ResponseMessage<UserSession>();
            try
            {
                var client = new ApiClient();
                var req = client.GenRequest(ApiConfig.LoginAction, Method.POST);
                req.AddParameter("nr", nr);
                req.AddParameter("password", pwd);
                var res = client.Execute(req);
                msg = JsonUtil.parse<ResponseMessage<UserSession>>(res.Content);
            }
            catch (WebFaultException<string> e)
            {
                msg.http_error = true;
                msg.meta.error_message = e.Detail;
            }
            return msg;
        }
    }
}
