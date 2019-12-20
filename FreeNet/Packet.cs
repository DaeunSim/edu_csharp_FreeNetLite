using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeNet
{
	/// <summary>
	/// byte[] 버퍼를 참조로 보관하여 pop_xxx 매소드 호출 순서대로 데이터 변환을 수행한다.
	/// </summary>
	public class Packet : IPacket
	{
		public Session Owner { get; set; }

		public byte[] BodyData { get; set; }
		
		//TODO 아래 2개 작업이 끝나면 제거한다.
		public int Position { get; private set; }
		public int Size { get; private set; }

		public UInt16 ProtocolId { get; private set; }

		//public static Packet Create(Int16 protocol_id)
		//{
		//	Packet packet = new Packet();
		//	packet.SetProtocol(protocol_id);
		//	return packet;
		//}
				
		public Packet(UInt16 PacketId, byte[] body)
		{
			// 참조로만 보관하여 작업한다.
			// 복사가 필요하면 별도로 구현해야 한다.
			BodyData = body;
			ProtocolId = PacketId;			
		}

		//public Packet(byte[] buffer, Session owner)
		//{
		//	// 참조로만 보관하여 작업한다.
		//	// 복사가 필요하면 별도로 구현해야 한다.
		//	BodyData = buffer;

		//	// 헤더는 읽을필요 없으니 그 이후부터 시작한다.
		//	Position = Defines.HEADERSIZE;

		//	Owner = owner;
		//}

		//public Packet(int size = 1024)
		//{
		//	BodyData = new byte[size];
		//}

        //public Packet(byte[] buffer)
        //{
        //    BodyData = buffer;
        //}

        public Int16 PopProtocolId()
		{
			return PopInt16();
		}

		//public void CopyTo(Packet target)
		//{
		//	target.SetProtocol(this.ProtocolId);
		//	target.OverWrite(this.BodyData, this.Position);
		//}

		public void OverWrite(byte[] source, int position)
		{
			Array.Copy(source, this.BodyData, source.Length);
			this.Position = position;
		}

		public byte PopByte()
		{
			byte data = this.BodyData[this.Position];
			this.Position += sizeof(byte);
			return data;
		}

		public Int16 PopInt16()
		{
			Int16 data = BitConverter.ToInt16(this.BodyData, this.Position);
			this.Position += sizeof(Int16);
			return data;
		}

		public Int32 PopInt32()
		{
			Int32 data = BitConverter.ToInt32(this.BodyData, this.Position);
			this.Position += sizeof(Int32);
			return data;
		}

		public string PopString()
		{
			// 문자열 길이는 최대 2바이트 까지. 0 ~ 32767
			Int16 len = BitConverter.ToInt16(this.BodyData, this.Position);
			this.Position += sizeof(Int16);

			// 인코딩은 utf8로 통일한다.
			string data = System.Text.Encoding.UTF8.GetString(this.BodyData, this.Position, len);
			this.Position += len;

			return data;
		}

		public float PopFloat()
		{
			float data = BitConverter.ToSingle(this.BodyData, this.Position);
			this.Position += sizeof(float);
			return data;
		}



		//public void SetProtocol(UInt16 protocol_id)
		//{
		//	ProtocolId = protocol_id;
		//	//this.buffer = new byte[1024];

		//	// 헤더는 나중에 넣을것이므로 데이터 부터 넣을 수 있도록 위치를 점프시켜놓는다.
		//	Position = Defines.HEADERSIZE;

		//	PushInt16(protocol_id);
		//}

		public void RecordSize()
		{
			// header + body 를 합한 사이즈를 입력한다.
			byte[] header = BitConverter.GetBytes(this.Position);
			header.CopyTo(this.BodyData, 0);
		}

		public void PushInt16(Int16 data)
		{
			byte[] temp_buffer = BitConverter.GetBytes(data);
			temp_buffer.CopyTo(this.BodyData, this.Position);
			Position += temp_buffer.Length;
		}

		public void Push(byte data)
		{
			byte[] temp_buffer = BitConverter.GetBytes(data);
			temp_buffer.CopyTo(this.BodyData, this.Position);
			Position += sizeof(byte);
		}

		public void Push(Int16 data)
		{
			byte[] temp_buffer = BitConverter.GetBytes(data);
			temp_buffer.CopyTo(this.BodyData, this.Position);
			Position += temp_buffer.Length;
		}

		public void Push(Int32 data)
		{
			byte[] temp_buffer = BitConverter.GetBytes(data);
			temp_buffer.CopyTo(this.BodyData, this.Position);
			Position += temp_buffer.Length;
		}

		public void Push(string data)
		{
			byte[] temp_buffer = Encoding.UTF8.GetBytes(data);

			Int16 len = (Int16)temp_buffer.Length;
			byte[] len_buffer = BitConverter.GetBytes(len);
			len_buffer.CopyTo(this.BodyData, this.Position);
			Position += sizeof(Int16);

			temp_buffer.CopyTo(this.BodyData, this.Position);
			Position += temp_buffer.Length;
		}

		public void Push(float data)
		{
			byte[] temp_buffer = BitConverter.GetBytes(data);
			temp_buffer.CopyTo(this.BodyData, this.Position);
			Position += temp_buffer.Length;
		}
	}
}
