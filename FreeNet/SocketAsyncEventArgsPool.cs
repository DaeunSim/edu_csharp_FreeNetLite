using System;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace FreeNet
{
	class SocketAsyncEventArgsPool
	{
		ConcurrentStack<SocketAsyncEventArgs> Pool = new ConcurrentStack<SocketAsyncEventArgs>();

		public void Push(SocketAsyncEventArgs item)
		{
			if (item == null) {
				throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null");
			}
			
			Pool.Push(item);
		}

		public SocketAsyncEventArgs Pop()
		{
			if(Pool.TryPop(out var result))
			{
				return result;
			}

			return null;
		}

		public int Count
		{
			get { return Pool.Count; }
		}
	}
}
