using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeNet
{
    public interface IMessageDispatcher
    {
        void OnMessage(Session user, ArraySegment<byte> buffer);
    }
}
