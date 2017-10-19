using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace FreeNet
{
    //TODO: 하트 비트 분리하기
    public class SessionClient : Session
    {
        public SessionClient(Int64 uniqueId, IMessageDispatcher dispatcher) : base(uniqueId, dispatcher)
        {            
        }
    }
}
