using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeNet;

namespace SampleServer
{	
	/// <summary>
	/// 하나의 session객체를 나타낸다.
	/// </summary>
	class GameUser : IPeer
	{
		UserToken token;

		public GameUser(UserToken token)
		{
			this.token = token;
			this.token.SetPeer(this);
		}

		void IPeer.OnRemoved()
		{
			//Console.WriteLine("The client disconnected.");

			Program.RemoveUser(this);
		}

		public void Send(Packet pkt)
		{
            pkt.RecordSize();
            this.token.Send(new ArraySegment<byte>(pkt.Buffer, 0, pkt.Position));
		}
                
        public void DisConnect()
		{
            this.token.Ban();
		}

        public void OnMessage(Packet pkt)
		{
            // ex)
            PROTOCOL_ID protocol = (PROTOCOL_ID)pkt.PopProtocolId();
            //Console.WriteLine("------------------------------------------------------");
            //Console.WriteLine("protocol id " + protocol);
            switch (protocol)
            {
                case PROTOCOL_ID.CHAT_MSG_REQ:
                    {
                        string text = pkt.PopString();
                        Console.WriteLine(string.Format("text {0}", text));

                        var response = Packet.Create((short)PROTOCOL_ID.CHAT_MSG_ACK);
                        response.Push(text);
                        Send(response);

                        if (text.Equals("exit"))
                        {
                            // 대량의 메시지를 한꺼번에 보낸 후 종료하는 시나리오 테스트.
                            for (int i = 0; i < 1000; ++i)
                            {
                                var dummy = Packet.Create((short)PROTOCOL_ID.CHAT_MSG_ACK);
                                dummy.Push(i.ToString());
                                Send(dummy);
                            }

                            this.token.Ban();
                        }
                    }
                    break;
            }
        }


        void send(ArraySegment<byte> data)
        {
            this.token.Send(data);
        }
    }
}
