using System;
using System.Collections.Generic;
using System.Text;

namespace FreeNet
{
    public interface IMessageResolver
    {
        void OnReceive(byte[] buffer, int offset, int transffered, Action<ArraySegment<byte>> callback);

        void ClearBuffer();
    }
}
