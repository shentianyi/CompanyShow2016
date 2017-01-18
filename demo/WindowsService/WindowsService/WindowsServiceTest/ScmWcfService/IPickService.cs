using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ScmWcfService.Model.Message;
using ScmWcfService.Model.Enum;
using ScmWcfService.Model;

namespace ScmWcfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IPickService”。
    [ServiceContract]
    public interface IPickService
    {
        [OperationContract]
        ResponseMessage<Pick> CreatePickByOrderCar(int order_car_id, List<int> order_box_ids);

        [OperationContract]
        ResponseMessage<Pick> CreatePickByOrderId(string order_id);


        [OperationContract]
        ResponseMessage<Pick> GetPickItemsByPickId(int id);

        [OperationContract]
        ResponseMessage<List<PickItem>> GetPickItemByCarNr(string car_nr);

        [OperationContract]
        ResponseMessage<object> WeightOrderBox(int order_box_id,
            string pick_item_id,
            float weight,
            float weight_qty,
            bool weight_valid);

        
    }
}
