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
			this.token.set_peer(this);
		}

		void IPeer.on_removed()
		{
			//Console.WriteLine("The client disconnected.");

			Program.remove_user(this);
		}

		public void send(Packet msg)
		{
            msg.record_size();
            this.token.send(new ArraySegment<byte>(msg.buffer, 0, msg.position));
		}

        public void send(ArraySegment<byte> data)
        {
            this.token.send(data);
        }

		void IPeer.disconnect()
		{
            this.token.ban();
		}

		void IPeer.on_message(Packet msg)
		{
            // ex)
            PROTOCOL protocol = (PROTOCOL)msg.pop_protocol_id();
            //Console.WriteLine("------------------------------------------------------");
            //Console.WriteLine("protocol id " + protocol);
            switch (protocol)
            {
                case PROTOCOL.CHAT_MSG_REQ:
                    {
                        string text = msg.pop_string();
                        Console.WriteLine(string.Format("text {0}", text));

                        var response = Packet.create((short)PROTOCOL.CHAT_MSG_ACK);
                        response.push(text);
                        send(response);

                        if (text.Equals("exit"))
                        {
                            // 대량의 메시지를 한꺼번에 보낸 후 종료하는 시나리오 테스트.
                            for (int i = 0; i < 1000; ++i)
                            {
                                var dummy = Packet.create((short)PROTOCOL.CHAT_MSG_ACK);
                                dummy.push(i.ToString());
                                send(dummy);
                            }

                            this.token.ban();
                        }
                    }
                    break;
            }
        }
	}
}
