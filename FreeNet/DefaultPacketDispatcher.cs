using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FreeNet
{
    /// <summary>
    /// 패킷을 처리해서 컨텐츠를 실행하는 곳이다.
    /// FreeNet을 사용할 때 LogicMessageEntry을 참고해서 IMessageDispatcher를 상속 받는 클래스를 맞게 구현하자
    /// </summary>
    public class DefaultPacketDispatcher : IPacketDispatcher
    {
        ILogicQueue MessageQueue = new DoubleBufferingQueue();
        

        public DefaultPacketDispatcher()
        {            
        }
                       

        public void IncomingPacket(Session user, ArraySegment<byte> buffer)
        {
            // 여긴 IO스레드에서 호출된다.
            // 완성된 패킷을 메시지큐에 넣어준다.
            Packet msg = new Packet(buffer, user);
            MessageQueue.Enqueue(msg);
        }
        
        //TODO: 이 함수를 호출하는 패킷처리 클래스 만들기
        public Queue<Packet> DispatchAll()
        {
            return MessageQueue.TakeAll();
        }

              
    }
}
