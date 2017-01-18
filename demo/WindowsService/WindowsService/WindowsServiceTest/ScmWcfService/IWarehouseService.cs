using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ScmWcfService.Model.Message;

namespace ScmWcfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IWarehouseService”。
    [ServiceContract]
    public interface IWarehouseService
    {
        [OperationContract]
        ResponseMessage<object> MoveStorageByCarAndBoxes(int order_car_id, List<int> order_box_ids=null);
    }
}
