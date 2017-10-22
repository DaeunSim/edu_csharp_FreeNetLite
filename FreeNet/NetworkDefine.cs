using System;
using System.Collections.Generic;
using System.Text;

namespace FreeNet
{
    public class NetworkDefine
    {
        // 종료 요청. S -> C
        public const short SYS_CLOSE_REQ = 0;
        // 종료 응답. C -> S
        public const short SYS_CLOSE_ACK = -1;
        // 하트비트 시작. S -> C
        public const short SYS_START_HEARTBEAT = -2;
        // 하트비트 갱신. C -> S
        public const short SYS_UPDATE_HEARTBEAT = -3;
    }
}
