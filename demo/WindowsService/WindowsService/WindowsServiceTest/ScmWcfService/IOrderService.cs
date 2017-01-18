using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ScmWcfService.Model.Message;
using ScmWcfService.Model;
using ScmWcfService.Model.Enum;

namespace ScmWcfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IOrderService”。
    [ServiceContract]
    public interface IOrderService
    {
        [OperationContract]
        ResponseMessage<OrderCar> GetOrderCarByNr(string nr);

        [OperationContract]
        ResponseMessage<Object> UpdateOrderCarById(int id,OrderCarStatus status);

        [OperationContract]
        ResponseMessage<OrderBox> GetOrderBoxByNr(string nr);


        [OperationContract]
        ResponseMessage<List<OrderBox>> GetOrderBoxByNrs(List<string> nrs);

        [OperationContract]
        ResponseMessage<Object> UpdateOrderBoxById(int id, OrderCarStatus status);

        [OperationContract]
        ResponseMessage<Object> UpdateOrderBoxByIds(List<int> id, OrderCarStatus status);


        [OperationContract]
        ResponseMessage<Order> CreateOrderByOrderCar(int order_car_id,List<int> order_box_ids);

        [OperationContract]
        ResponseMessage<object> BindBoxAndLed(string nr, string led);

    }
}
