using System;
using System.Collections.Generic;
using System.Text;

namespace FreeNet
{
    public class NetworkDefine
    {
#region SYSTEM_PACKET
        public const short SYS_NTF_CONNECTED = 1;
        
        public const short SYS_NTF_CLOSED = 2;

        // 리모트에서 받은 패킷의 경우 이 숫자를 넘어서는 것은 에러
        public const short SYS_NTF_MAX = 100;
#endregion

        // 하트비트 시작. S -> C
        public const short SYS_START_HEARTBEAT = 111;
        // 하트비트 갱신. C -> S
        public const short SYS_UPDATE_HEARTBEAT = 112;



        
    }
}
