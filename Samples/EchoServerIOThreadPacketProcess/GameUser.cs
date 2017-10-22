using System;
using FreeNet;

namespace EchoServerIOThreadPacketProcess
{	
	/// <summary>
	/// 하나의 session객체를 나타낸다.
	/// </summary>
	class GameUser
	{
		Session Session;

		public GameUser(Session session)
		{
			Session = session;
		}
				
		public void Send(Packet pkt)
		{
			pkt.RecordSize();
			Session.PreSend(new ArraySegment<byte>(pkt.Buffer, 0, pkt.Position));
		}
		

		void Send(ArraySegment<byte> data)
		{
			Session.PreSend(data);
		}
	}
}
