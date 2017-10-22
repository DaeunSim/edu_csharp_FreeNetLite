using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace SampleServer
{
    class PacketProcess
    {
        bool IsStart = false;

        Thread LogicThread = null;

        FreeNet.IPacketDispatcher PacketDispatcher = null;

        public PacketProcess(FreeNet.IPacketDispatcher packetDispatcher)
        {
            PacketDispatcher = packetDispatcher;
        }

        /// <summary>
        /// 로직 스레드 시작.
        /// </summary>
        public void Start()
        {
            IsStart = true;

            LogicThread = new Thread(DoLogic);
            LogicThread.Start();
        }

        public void Stop()
        {
            IsStart = false;
            LogicThread.Join();
        }

        /// <summary>
        /// 로직 스레드. 
        /// </summary>
        void DoLogic()
        {
            // 반복문을 빠져나오도록 true 대신 bool 변수 사용하기
            while (IsStart)
            {
                // 메시지를 분배한다.
                var packetQueue = PacketDispatcher.DispatchAll();

                if (packetQueue.Count > 0)
                {
                    while(packetQueue.TryDequeue(out var packet))
                    {
                        // 패킷 처리를 한다.
                        OnMessage(packet);
                    }
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        void OnMessage(FreeNet.Packet packet)
        {
            // ex)
            PROTOCOL_ID protocol = (PROTOCOL_ID)packet.PopProtocolId();
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
                        if (packet.Owner.OnSystemPacket(packet) == false)
                        {
                            Console.WriteLine("Unknown protocol id " + protocol);
                        }
                    }
                    break;
            }
        }
    }
}
