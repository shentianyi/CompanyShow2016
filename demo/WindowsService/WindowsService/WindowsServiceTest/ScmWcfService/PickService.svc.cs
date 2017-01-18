using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ScmWcfService.Model;
using ScmWcfService.Model.Message;
using ScmWcfService.Config;
using ScmWcfService.Provider;
using System.Diagnostics;
using Brilliantech.Framwork.Utils.JsonUtil;
using System.ServiceModel.Web;
using RestSharp;

namespace ScmWcfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“PickService”。
    public class PickService : IPickService
    {

        public ResponseMessage<Pick> CreatePickByOrderCar(int order_car_id, List<int> order_box_ids)
        {
            var msg = new ResponseMessage<Pick>();
            try
            {
                var client = new ApiClient();
                var req = client.GenRequest(ApiConfig.CreatePickByCarAction, Method.POST);
                req.AddParameter("order_car_id", order_car_id);
                //req.AddParameter("order_box_ids", JsonUtil.stringify(order_box_ids));
                req.AddParameter("order_box_ids", string.Join(",", order_box_ids.ToArray()));
                var res = client.Execute(req);
                Debug.WriteLine(res.Content);

                msg = JsonUtil.parse<ResponseMessage<Pick>>(res.Content);
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

        public ResponseMessage<Pick> CreatePickByOrderId(string order_id)
        {
            throw new NotImplementedException();
        }

        public ResponseMessage<Pick> GetPickItemsByPickId(int id)
        {
            throw new NotImplementedException();
        }


        public ResponseMessage<List<PickItem>> GetPickItemByCarNr(string car_nr)
        {
            var msg = new ResponseMessage<List<PickItem>>();
            try
            {
                var client = new ApiClient();
                var req = client.GenRequest(ApiConfig.GetPickItemByCarNrAction);
                req.AddParameter("car_nr", car_nr);
                var res = client.Execute(req);
                Debug.WriteLine(res.Content);
                msg = JsonUtil.parse<ResponseMessage<List<PickItem>>>(res.Content);
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


        public ResponseMessage<object> WeightOrderBox(int order_box_id, 
            string pick_item_id, 
            float weight,
            float weight_qty,
            bool weight_valid)
        {

            var msg = new ResponseMessage<object>();
            try
            {
                var client = new ApiClient();
                var req = client.GenRequest(ApiConfig.WeightOrderBoxAction, Method.POST);
                req.AddParameter("id", order_box_id);
                req.AddParameter("pick_item_id", pick_item_id);
                req.AddParameter("weight", weight);
                req.AddParameter("weight_qty", weight_qty);
                req.AddParameter("weight_valid", weight_valid);
                var res = client.Execute(req);
                Debug.WriteLine(res.Content);

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
