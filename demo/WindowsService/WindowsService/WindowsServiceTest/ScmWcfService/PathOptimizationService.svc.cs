using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ScmWcfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“PathOptimizationService”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 PathOptimizationService.svc 或 PathOptimizationService.svc.cs，然后开始调试。
    public class PathOptimizationService : IPathOptimizationService
    {
        public void DoWork()
        {
        }

        int calcStationCount(int src1, int src2, int des1, int des2)
        {
            return src1-src2 + des1 - des2;
        }

        public byte GetBestDirection(int source, int destination)
        {
            byte direction = 0x00;
            if (source>0 && source <= 5)
            {
                if (destination > 0 && destination <= 5)
                {
                    if (destination > source)
                    {
                        direction = 0x01;
                    }else if (destination < source)
                    {
                        direction = 0x02;
                    }
                }
                else if (destination >= 6 && destination <= 15)
                {
                    direction = 0x01;
                }
                else if (destination >= 16 && destination <= 20)
                {
                    //前进>后退
                    if (calcStationCount(5, source, 20, destination) > calcStationCount(source, 1, destination, 16))
                    {
                        direction = 0x02;
                    }
                    else if (calcStationCount(5, source, 20, destination) < calcStationCount(source, 1, destination, 16))
                    {
                        direction = 0x01;
                    }
                }
                else if (destination >= 21 && destination <= 30)
                {
                    direction = 0x01;
                }
                else if (destination >= 31 && destination <= 35)
                {
                    //前进>后退
                    if (calcStationCount(5, source, 35, destination) > calcStationCount(source, 1, destination, 30))
                    {
                        direction = 0x02;
                    }
                    else if (calcStationCount(5, source, 35, destination) < calcStationCount(source, 1, destination, 30))
                    {
                        direction = 0x01;
                    }
                }
                else if (destination == 50)//待机点
                {
                    direction = 0x02;
                }
                else if (destination == 41)//启动牵引点
                {
                    direction = 0x02;
                }
                else if (destination == 42)//卸货点
                {
                    direction = 0x01;
                }
            }
            else if (source >= 6 && source <= 15)
            {
                if (destination > 0 && destination <= 5)
                {
                    direction = 0x02;
                }
                else if (destination >= 6 && destination <= 15)
                {
                    if (destination > source)
                    {
                        direction = 0x01;
                    }
                    else if (destination < source)
                    {
                        direction = 0x02;
                    }
                }
                else if (destination >= 16 && destination <= 20)
                {
                    direction = 0x02;
                }
                else if (destination >= 21 && destination <= 30)
                {
                    //前进>后退
                    if (calcStationCount(15, source, 30, destination) > calcStationCount(source, 6, destination, 21))
                    {
                        direction = 0x02;
                    }
                    else if (calcStationCount(15, source, 30, destination) < calcStationCount(source, 6, destination, 21))
                    {
                        direction = 0x01;
                    }
                }
                else if (destination >= 31 && destination <= 35)
                {
                    direction = 0x02;
                }
                else if (destination == 50)//待机点
                {
                    direction = 0x02;
                }
                else if (destination == 41)//启动牵引点
                {
                    direction = 0x02;
                }
                else if (destination == 42)//卸货点
                {
                    direction = 0x02;
                }
            }
            else if (source >= 16 && source <= 20)
            {
                if (destination > 0 && destination <= 5)
                {
                    //前进>后退
                    if (calcStationCount(20, source, 5, destination) > calcStationCount(source, 16, destination, 1))
                    {
                        direction = 0x02;
                    }
                    else if (calcStationCount(20, source, 5, destination) < calcStationCount(source, 16, destination, 1))
                    {
                        direction = 0x01;
                    }
                }
                else if (destination >= 6 && destination <= 15)
                {
                    direction = 0x01;
                }
                else if (destination >= 16 && destination <= 20)
                {
                    if (destination > source)
                    {
                        direction = 0x01;
                    }
                    else if (destination < source)
                    {
                        direction = 0x02;
                    }
                }
                else if (destination >= 21 && destination <= 30)
                {
                    direction = 0x01;
                }
                else if (destination >= 31 && destination <= 35)
                {
                    //前进>后退
                    if (calcStationCount(20, source, 35, destination) > calcStationCount(source, 16, destination, 30))
                    {
                        direction = 0x02;
                    }
                    else if (calcStationCount(20, source, 35, destination) < calcStationCount(source, 16, destination, 30))
                    {
                        direction = 0x01;
                    }
                }
                else if (destination == 50)//待机点
                {
                    direction = 0x02;
                }
                else if (destination == 41)//启动牵引点
                {
                    direction = 0x02;
                }
                else if (destination == 42)//卸货点
                {
                    direction = 0x01;
                }
            }
            else if (source >= 21 && source <= 30)
            {
                if (destination > 0 && destination <= 5)
                {
                    direction = 0x02;
                }
                else if (destination >= 6 && destination <= 15)
                {
                    //前进>后退
                    if (calcStationCount(30, source, 15, destination) > calcStationCount(source, 21, destination, 6))
                    {
                        direction = 0x02;
                    }
                    else if (calcStationCount(30, source, 15, destination) < calcStationCount(source, 21, destination, 6))
                    {
                        direction = 0x01;
                    }
                }
                else if (destination >= 16 && destination <= 20)
                {
                    direction = 0x02;
                }
                else if (destination >= 21 && destination <= 30)
                {
                    if (destination > source)
                    {
                        direction = 0x01;
                    }
                    else if (destination < source)
                    {
                        direction = 0x02;
                    }
                }
                else if (destination >= 31 && destination <= 35)
                {
                    direction = 0x02;
                }
                else if (destination == 50)//待机点
                {
                    direction = 0x02;
                }
                else if (destination == 41)//启动牵引点
                {
                    direction = 0x02;
                }
                else if (destination == 42)//卸货点
                {
                    direction = 0x02;
                }
            }
            else if (source >= 31 && source <= 35)
            {
                if (destination > 0 && destination <= 5)
                {
                    //前进>后退
                    if (calcStationCount(source, 31, destination, 1) > calcStationCount(35, source, 5, destination))
                    {
                        direction = 0x02;
                    }
                    else if (calcStationCount(source, 31, destination, 1) < calcStationCount(35, source, 5, destination))
                    {
                        direction = 0x01;
                    }
                }
                else if (destination >= 6 && destination <= 15)
                {
                    direction = 0x02;
                }
                else if (destination >= 16 && destination <= 20)
                {
                    //前进>后退
                    if (calcStationCount(source, 31, destination, 16) > calcStationCount(35, source, 20, destination))
                    {
                        direction = 0x02;
                    }
                    else if (calcStationCount(source, 31, destination, 16) < calcStationCount(35, source, 20, destination))
                    {
                        direction = 0x01;
                    }
                }
                else if (destination >= 21 && destination <= 30)
                {
                    direction = 0x02;
                }
                else if (destination >= 31 && destination <= 35)
                {
                    if (destination > source)
                    {
                        direction = 0x01;
                    }
                    else if (destination < source)
                    {
                        direction = 0x02;
                    }
                }
                else if (destination == 50)//待机点
                {
                    direction = 0x01;
                }
                else if (destination == 41)//启动牵引点
                {
                    direction = 0x01;
                }
                else if (destination == 42)//卸货点
                {
                    direction = 0x01;
                }
            }
            else if (source == 50)//待机点
            {
                if (destination > 0 && destination <= 5)
                {
                    direction = 0x02;
                }
                else if (destination >= 6 && destination <= 15)
                {
                    direction = 0x02;
                }
                else if (destination >= 16 && destination <= 20)
                {
                    direction = 0x02;
                }
                else if (destination >= 21 && destination <= 30)
                {
                    direction = 0x02;
                }
                else if (destination >= 31 && destination <= 35)
                {
                    direction = 0x02;
                }
                else if (destination == 50)//待机点
                {
                    direction = 0x00;
                }
                else if (destination == 41)//启动牵引点
                {
                    direction = 0x02;
                }
                else if (destination == 42)//卸货点
                {
                    direction = 0x02;
                }
            }
            else if (source == 41)//启动牵引点
            {
                if (destination > 0 && destination <= 5)
                {
                    direction = 0x01;
                }
                else if (destination >= 6 && destination <= 15)
                {
                    direction = 0x01;
                }
                else if (destination >= 16 && destination <= 20)
                {
                    direction = 0x01;
                }
                else if (destination >= 21 && destination <= 30)
                {
                    direction = 0x01;
                }
                else if (destination >= 31 && destination <= 35)
                {
                    direction = 0x01;
                }
                else if (destination == 50)//待机点
                {
                    direction = 0x02;
                }
                else if (destination == 41)//启动牵引点
                {
                    direction = 0x00;
                }
                else if (destination == 42)//卸货点
                {
                    direction = 0x01;
                }
            }
            else if (source == 42)//卸货点
            {
                if (destination > 0 && destination <= 5)
                {
                    direction = 0x02;
                }
                else if (destination >= 6 && destination <= 15)
                {
                    direction = 0x01;
                }
                else if (destination >= 16 && destination <= 20)
                {
                    direction = 0x02;
                }
                else if (destination >= 21 && destination <= 30)
                {
                    direction = 0x02;
                }
                else if (destination >= 31 && destination <= 35)
                {
                    direction = 0x02;
                }
                else if (destination == 50)//待机点
                {
                    direction = 0x02;
                }
                else if (destination == 41)//启动牵引点
                {
                    direction = 0x02;
                }
                else if (destination == 42)//卸货点
                {
                    direction = 0x00;
                }
            }

            return direction;
        }
    }
}
