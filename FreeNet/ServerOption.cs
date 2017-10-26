using System;
using System.Collections.Generic;
using System.Text;

namespace FreeNet
{
    //TODO:TinyMapper 등을 사용하여 이 클래스를 받는 쪽은 읽기 전용만 되도록 한다
    public class ServerOption
    {
        public int MaxConnectionCount = 10000;
        public int ReceiveBufferSize = 8012;
        public int MaxPacketSize = 1024;
        public int ReserveClosingWaitMilliSecond = 100;
        public int SendPacketMTUForClient = 1024;
        public int SendPacketMTUForServer = 8012;


        public int SendPacketMTUSize(bool isClient)
        {
            return isClient ? SendPacketMTUForClient : SendPacketMTUForServer;
        }
    }
    
}
