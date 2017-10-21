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
        //TODO: 제거 예정
        //NetworkService RefService;
        //AutoResetEvent LogicEvent = new AutoResetEvent(false);
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


        //public class LogicMessageEntry : IPacketDispatcher
        //{
        //    //TODO: 제거 예정
        //    NetworkService RefService;

        //    ILogicQueue MessageQueue = new DoubleBufferingQueue();
        //    AutoResetEvent LogicEvent = new AutoResetEvent(false);


        //    public LogicMessageEntry(NetworkService service)
        //    {
        //        RefService = service;

        //        Start();
        //    }


        //    /// <summary>
        //    /// 로직 스레드 시작.
        //    /// </summary>
        //    public void Start()
        //    {
        //        Thread logic = new Thread(DoLogic);
        //        logic.Start();
        //    }


        //    public void OnMessage(Session user, ArraySegment<byte> buffer)
        //    {
        //        // 여긴 IO스레드에서 호출된다.
        //        // 완성된 패킷을 메시지큐에 넣어준다.
        //        Packet msg = new Packet(buffer, user);
        //        MessageQueue.Enqueue(msg);

        //        //TODO: 패킷이 하나라도 받으면 그때마다 이 메소드가 호출된다. 너무 빈번하게 이벤트를 끄고 켜는 것 같다.
        //        // 이벤트를 없애고 스레드를 아무일 없으면 1밀리세컨드 단위로 호출 시키는 것이 좋을 듯 하다.
        //        // DoLogic에서는 DispatchAll 호출 후 가져온 패킷이 없으면 1ms 대기하고, 있으면 처리한다.

        //        // 로직 스레드를 깨워 일을 시킨다.
        //        LogicEvent.Set();
        //    }


        //    /// <summary>
        //    /// 로직 스레드. 
        //    /// </summary>
        //    void DoLogic()
        //    {
        //        // 반복문을 빠져나오도록 true 대신 bool 변수 사용하기
        //        while (true)
        //        {
        //            // 패킷이 들어오면 알아서 깨워 주겠지.
        //            LogicEvent.WaitOne();

        //            // 메시지를 분배한다.
        //            DispatchAll(MessageQueue.TakeAll());
        //        }
        //    }


        //    void DispatchAll(Queue<Packet> queue)
        //    {
        //        while (queue.Count > 0)
        //        {
        //            var msg = queue.Dequeue();

        //            if (!RefService.UserManager.IsExist(msg.Owner))
        //            {
        //                continue;
        //            }

        //            //TODO: 여기서 컨텐츠 처리를 한다.
        //            msg.Owner.OnMessage(msg);
        //        }
        //    }
        }
}
