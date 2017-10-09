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
		static Stack<Packet> pool;
		static int pool_capacity;

		public static void initialize(int capacity)
		{
			pool = new Stack<Packet>();
			pool_capacity = capacity;
			allocate();
		}

		static void allocate()
		{
			for (int i = 0; i < pool_capacity; ++i)
			{
				pool.Push(new Packet());
			}
		}

		public static Packet pop()
		{
			lock (cs_buffer)
			{
				if (pool.Count <= 0)
				{
					Console.WriteLine("reallocate.");
					allocate();
				}

				return pool.Pop();
			}
		}

		public static void push(Packet packet)
		{
			lock(cs_buffer)
			{
				pool.Push(packet);
			}
		}
	}
}
