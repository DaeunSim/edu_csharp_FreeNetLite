using System;
using FreeNet;

namespace EchoServerIOThreadPacketProcess
{	
	/// <summary>
	/// 하나의 session객체를 나타낸다.
	/// </summary>
	class GameUser : IPeer
	{
		SessionClient Session;

		public GameUser(SessionClient session)
		{
			Session = session;
			Session.SetPeer(this);
		}

		void IPeer.OnRemoved()
		{
			//Console.WriteLine("The client disconnected.");

			Program.RemoveUser(this);
		}

		public void Send(Packet pkt)
		{
			pkt.RecordSize();
			Session.PreSend(new ArraySegment<byte>(pkt.Buffer, 0, pkt.Position));
		}
				
		public void DisConnect()
		{
			Session.Ban();
		}

		public void OnMessage(Packet pkt)
		{			
		}


		void Send(ArraySegment<byte> data)
		{
			Session.PreSend(data);
		}
	}
}
