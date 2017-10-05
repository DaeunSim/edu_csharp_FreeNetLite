using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeNet
{
    public interface ILogicQueue
    {
        void enqueue(Packet msg);
        Queue<Packet> get_all();
    }
}
