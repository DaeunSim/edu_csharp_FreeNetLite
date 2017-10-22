using System;
using System.Collections.Generic;

using FreeNet;

namespace EchoServerIOThreadPacketProcess
{
    class IoThreadPacketDispatcher : IPacketDispatcher
    {
        public IoThreadPacketDispatcher()
        {
        }


        public void IncomingPacket(Session user, ArraySegment<byte> buffer)
        {
            var packet = new Packet(buffer, user);

            var protocol = (PROTOCOL_ID)packet.PopProtocolId();
            //Console.WriteLine("------------------------------------------------------");
            //Console.WriteLine("protocol id " + protocol);

            switch (protocol)
            {
                case PROTOCOL_ID.ECHO_REQ:
                    {
                        string text = packet.PopString();
                        Console.WriteLine(string.Format("text {0}", text));

                        var response = FreeNet.Packet.Create((short)PROTOCOL_ID.ECHO_ACK);
                        response.Push(text);
                        packet.Owner.Send(response);
                    }
                    break;
                default:
                    {
                        Console.WriteLine("Unknown protocol id " + protocol);
                    }
                    break;
            }
        }

        public Queue<Packet> DispatchAll()
        {
            return null;
        }
    }
}
