using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ScmWcfService.Model.Message;
using Brilliantech.Framwork.Utils.JsonUtil;
using ScmWcfService.Config;
using RestSharp;
using System.ServiceModel.Web;
using ScmWcfService.Provider;

namespace ScmWcfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“WarehouseService”。
    public class WarehouseService : IWarehouseService
    {
        public ResponseMessage<object> MoveStorageByCarAndBoxes(int order_car_id, List<int> order_box_ids=null)
        {
            var msg = new ResponseMessage<object>();
            try
            {
                var client = new ApiClient();
                var req = client.GenRequest(ApiConfig.MoveStorageByCarAction, Method.POST);
                req.AddParameter("order_car_id", order_car_id);
                if (order_box_ids != null)
                {
                    req.AddParameter("order_box_ids", string.Join(",", order_box_ids.ToArray()));
                }
                var res = client.Execute(req); 
                msg = JsonUtil.parse<ResponseMessage<object>>(res.Content);
            }
            catch (WebFaultException<string> e)
            {
                msg.http_error = true;
                msg.meta.error_message = e.Detail;
            }
            catch (Exception e)
            {
                msg.http_error = true;
                msg.meta.error_message = "系统服务错误，请联系管理员";
            }

            return msg;
        }
    }
}
