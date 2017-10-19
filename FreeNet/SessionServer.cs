using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace FreeNet
{
    //TODO: ClietnSession으로 이름을 바꾼다.
    //TODO: 서버용도 만들어야 하므로 부모 클래스를 상속하도록 한다.
    //TODO: 하트 비트 분리하기
    public class SessionServer :  Session
    {
        public SessionServer(Int64 uniqueId, IMessageDispatcher dispatcher) : base(uniqueId, dispatcher)
        {

        }
    }
}
