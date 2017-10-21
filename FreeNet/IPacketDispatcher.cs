using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeNet
{
    public interface IPacketDispatcher
    {
        void IncomingPacket(Session user, ArraySegment<byte> buffer);

        Queue<Packet> DispatchAll();
    }
}
