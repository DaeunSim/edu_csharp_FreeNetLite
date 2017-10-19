using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FreeNet
{
    /// <summary>
    /// 수신된 패킷을 받아 로직 스레드에서 분배하는 역할을 담당한다.
    /// </summary>
    public class LogicMessageEntry : IMessageDispatcher
    {
        NetworkService RefService;
        ILogicQueue MessageQueue = new DoubleBufferingQueue();
        AutoResetEvent LogicEvent = new AutoResetEvent(false);


        public LogicMessageEntry(NetworkService service)
        {
            RefService = service;
        }


        /// <summary>
        /// 로직 스레드 시작.
        /// </summary>
        public void Start()
        {
            Thread logic = new Thread(DoLogic);
            logic.Start();
        }


        public void OnMessage(Session user, ArraySegment<byte> buffer)
        {
            // 여긴 IO스레드에서 호출된다.
            // 완성된 패킷을 메시지큐에 넣어준다.
            Packet msg = new Packet(buffer, user);
            MessageQueue.Enqueue(msg);

            //TODO: 패킷이 하나라도 받으면 그때마다 이 메소드가 호출된다. 너무 빈번하게 이벤트를 끄고 켜는 것 같다.
            // 이벤트를 없애고 스레드를 아무일 없으면 1밀리세컨드 단위로 호출 시키는 것이 좋을 듯 하다.
            // DoLogic에서는 DispatchAll 호출 후 가져온 패킷이 없으면 1ms 대기하고, 있으면 처리한다.

            // 로직 스레드를 깨워 일을 시킨다.
            LogicEvent.Set();
        }


        /// <summary>
        /// 로직 스레드.
        /// </summary>
        void DoLogic()
        {
            // 반복문을 빠져나오도록 true 대신 bool 변수 사용하기
            while (true)
            {
                // 패킷이 들어오면 알아서 깨워 주겠지.
                LogicEvent.WaitOne();

                // 메시지를 분배한다.
                DispatchAll(MessageQueue.TakeAll());
            }
        }


        void DispatchAll(Queue<Packet> queue)
        {
            while (queue.Count > 0)
            {
                var msg = queue.Dequeue();

                if (!RefService.UserManager.IsExist(msg.Owner)) {
                    continue;
                }

                // 세션이 아닌 컨텐츠 처리 클래스를 만든다
                msg.Owner.OnMessage(msg);
            }
        }
    }
}
