using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeNet
{
	//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
	// Not stable. Do not use this class!!
	//=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
	public class PacketBufferManager
	{
		static object cs_buffer = new object();
		static Stack<Packet> Pool;
		static int PoolCapacity;

		public static void initialize(int capacity)
		{
			Pool = new Stack<Packet>();
			PoolCapacity = capacity;
			Allocate();
		}

		static void Allocate()
		{
			for (int i = 0; i < PoolCapacity; ++i)
			{
				Pool.Push(new Packet());
			}
		}

		public static Packet Pop()
		{
			lock (cs_buffer)
			{
				if (Pool.Count <= 0)
				{
					Console.WriteLine("reallocate.");
					Allocate();
				}

				return Pool.Pop();
			}
		}

		public static void Push(Packet packet)
		{
			lock(cs_buffer)
			{
				Pool.Push(packet);
			}
		}
	}
}
