using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ScmWcfService.Model;
using ScmWcfService.Model.Message;
using ScmWcfService.Provider;
using ScmWcfService.Config;
using Brilliantech.Framwork.Utils.JsonUtil;
using System.ServiceModel.Web;
using System.Diagnostics;

namespace ScmWcfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“OrderService”。
    public class OrderService : IOrderService
    {
        public ResponseMessage<Order> CreateOrderByLed(string ip, string led_id, int qty)
        {
            var msg = new ResponseMessage<Order>();
            try
            {
                var client = new ApiClient();
                var req = client.GenRequest(ApiConfig.CreateOrderByLedAction, RestSharp.Method.POST);
                req.AddParameter("ip", ip);
                req.AddParameter("led_id", led_id);
                req.AddParameter("qty", qty);
                var res = client.Execute(req);
                Debug.WriteLine(res.Content);
                msg = JsonUtil.parse<ResponseMessage<Order>>(res.Content);
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

        public ResponseMessage<object> BindBoxAndLed(string nr, string led)
        {
            var msg = new ResponseMessage<object>();
            try
            {
                var client = new ApiClient();
                var req = client.GenRequest(ApiConfig.BindBoxAndLedAction);
                req.AddParameter("id", nr);
                req.AddParameter("led_id", led);
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

        public ResponseMessage<OrderCar> GetOrderCarByNr(string nr)
        {
            var msg = new ResponseMessage<OrderCar>();
            try
            {
                var client = new ApiClient();
                var req = client.GenRequest(ApiConfig.GetOrderCarByNrAction);
                req.AddParameter("nr", nr);
                var res = client.Execute(req);
                msg = JsonUtil.parse<ResponseMessage<OrderCar>>(res.Content);
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


        public ResponseMessage<object> UpdateOrderCarById(int id, Model.Enum.OrderCarStatus status)
        {
            throw new NotImplementedException();
        }

        public ResponseMessage<OrderBox> GetOrderBoxByNr(string nr)
        {
            var msg = new ResponseMessage<OrderBox>();
            try
            {
                var client = new ApiClient();
                var req = client.GenRequest(ApiConfig.GetOrderBoxByNrAction);
                req.AddParameter("nr", nr);
                var res = client.Execute(req);
                Debug.WriteLine(res.Content);

                msg = JsonUtil.parse<ResponseMessage<OrderBox>>(res.Content);
            }
            catch (WebFaultException<string> e)
            {
                msg.http_error = true;
                msg.meta.error_message = e.Detail;
            }
            catch (Exception e) {
                msg.http_error = true;
                msg.meta.error_message = "系统服务错误，请联系管理员";
            }
            
            return msg;
        } 

        public ResponseMessage<List<OrderBox>> GetOrderBoxByNrs(List<string> nrs)
        {
            var msg = new ResponseMessage<List<OrderBox>>();
            try
            {
                var client = new ApiClient();
                var req = client.GenRequest(ApiConfig.GetOrderBoxByNrsAction);
                req.AddParameter("nrs", string.Join(",",nrs.ToArray()));
                var res = client.Execute(req);
                Debug.WriteLine(res.Content);
                msg = JsonUtil.parse<ResponseMessage<List<OrderBox>>>(res.Content);
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

        public ResponseMessage<object> UpdateOrderBoxById(int id, Model.Enum.OrderCarStatus status)
        {
            throw new NotImplementedException();
        }

        public ResponseMessage<object> UpdateOrderBoxByIds(List<int> id, Model.Enum.OrderCarStatus status)
        {
            throw new NotImplementedException();
        }

        public ResponseMessage<Order> CreateOrderByOrderCar(int order_car_id, List<int> order_box_ids)
        {
            throw new NotImplementedException();
        }
    }
}
