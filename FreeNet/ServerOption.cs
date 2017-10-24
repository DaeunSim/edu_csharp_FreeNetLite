using System;
using System.Collections.Generic;
using System.Text;

namespace FreeNet
{
    public class ServerOption
    {
        public int MaxConnectionCount = 10000;
        public int ReceiveBufferSize = 8012;
        public int MaxPacketSize = 1024;
        public int ReserveClosingWaitMilliSecond = 100;
    }
}
