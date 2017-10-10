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
        NetworkService Service;
        ILogicQueue MessageQueue;
        AutoResetEvent LogicEvent;


        public LogicMessageEntry(NetworkService service)
        {
            Service = service;
            MessageQueue = new DoubleBufferingQueue();
            LogicEvent = new AutoResetEvent(false);
        }


        /// <summary>
        /// 로직 스레드 시작.
        /// </summary>
        public void Start()
        {
            Thread logic = new Thread(this.DoLogic);
            logic.Start();
        }


        public void OnMessage(UserToken user, ArraySegment<byte> buffer)
        {
            // 여긴 IO스레드에서 호출된다.
            // 완성된 패킷을 메시지큐에 넣어준다.
            Packet msg = new Packet(buffer, user);
            this.MessageQueue.Enqueue(msg);

            // 로직 스레드를 깨워 일을 시킨다.
            this.LogicEvent.Set();
        }


        /// <summary>
        /// 로직 스레드.
        /// </summary>
        void DoLogic()
        {
            while (true)
            {
                // 패킷이 들어오면 알아서 깨워 주겠지.
                this.LogicEvent.WaitOne();

                // 메시지를 분배한다.
                DispatchAll(this.MessageQueue.TakeAll());
            }
        }


        void DispatchAll(Queue<Packet> queue)
        {
            while (queue.Count > 0)
            {
                Packet msg = queue.Dequeue();
                if (!this.Service.UserManager.IsExist(msg.Owner))
                {
                    continue;
                }

                msg.Owner.OnMessage(msg);
            }
        }
    }
}
