using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace FreeNet
{
    //TODO: 하트 비트 분리하기
    public class SessionClient : Session
    {
        public SessionClient(Int64 uniqueId, IMessageDispatcher dispatcher) : base(uniqueId, dispatcher)
        {            
        }


        /// <summary>
        /// 연결을 종료한다. 단, 종료코드를 전송한 뒤 상대방이 먼저 연결을 끊게 한다.
        /// 주로 서버에서 클라이언트의 연결을 끊을 때 사용한다.
        /// 
        /// TIME_WAIT상태를 서버에 남기지 않으려면 disconnect대신 이 매소드를 사용해서
        /// 클라이언트를 종료시켜야 한다.
        /// </summary>
        public void Ban()
        {
            try
            {
                byebye();
            }
            catch (Exception)
            {
                Close();
            }
        }


        /// <summary>
        /// 종료코드를 전송하여 상대방이 먼저 끊도록 한다.
        /// </summary>
        void byebye()
        {
            Packet bye = Packet.Create(SYS_CLOSE_REQ);
            Send(bye);
        }
    }
}
