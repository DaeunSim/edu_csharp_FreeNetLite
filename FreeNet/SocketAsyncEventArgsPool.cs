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

		// ConcurrentStack의 Count는 호출 시마다 일일이 계산하므로 가능하면 사용하지 않는 것이 좋다.
		//public int Count
		//{
		//	get { return Pool.Count; }
		//}
	}
}
