using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace FreeNet
{
    public class SessionServer :  Session
    {
        public SessionServer(Int64 uniqueId, IPacketDispatcher dispatcher, IMessageResolver messageResolver) : 
                                                        base(uniqueId, dispatcher, messageResolver)
        {

        }
    }
}
