using System;
using System.Collections.Generic;
using System.Text;

namespace FreeNet
{
    //TODO:TinyMapper 등을 사용하여 이 클래스를 받는 쪽은 읽기 전용만 되도록 한다
    public class ServerOption
    {
        public int MaxConnectionCount = 10000;
        public int ReserveClosingWaitMilliSecond = 100;
        public int ReceiveSecondaryBufferSize = 4012;

        public int ClientReceiveBufferSize = 4096;
        public int ClientMaxPacketSize = 1024;        
        public int ClientSendPacketMTU = 1024;

        public int ServerSendPacketMTU = 4096;


        public int SendPacketMTUSize(bool isClient)
        {
            return isClient ? ClientSendPacketMTU : ServerSendPacketMTU;
        }
    }
    
}
