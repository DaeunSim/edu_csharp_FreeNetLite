using System;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace FreeNet
{
	class SocketAsyncEventArgsPool
	{
		ConcurrentBag<SocketAsyncEventArgs> Pool = new ConcurrentBag<SocketAsyncEventArgs>();

		public void Push(SocketAsyncEventArgs item)
		{
			if (item == null) {
				throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null");
			}
			
			Pool.Add(item);
		}

		public SocketAsyncEventArgs Pop()
		{
			if(Pool.TryTake(out var result))
			{
				return result;
			}

			return null;
		}		
	}
}
