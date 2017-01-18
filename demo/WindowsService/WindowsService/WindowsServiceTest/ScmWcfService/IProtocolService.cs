using ScmWcfService.Model.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ScmWcfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IProtocolService”。
    [ServiceContract]
    public interface IProtocolService
    {
        [OperationContract]
        void DoWork();

        [OperationContract]
        Socket ConnectServer(string ip, int port);

        [OperationContract]
        ProtocolMessage<Socket> SendMessage(Socket socket, byte[] msg);

        [OperationContract]
        ProtocolMessage<int> GetAgvInfo(Socket socket);
    }
}
