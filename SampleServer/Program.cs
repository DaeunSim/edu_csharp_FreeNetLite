using System;
using System.Collections.Generic;

namespace SampleServer
{
	class Program
	{
		static List<GameUser> UserList = new List<GameUser>();

		static void Main(string[] args)
		{
			var service = new FreeNet.NetworkService();

			// 콜백 매소드 설정.
			service.SessionClientCreatedCallBack += OnSessionCreated;

			var socketOpt = new FreeNet.SocketOption();
			socketOpt.NoDelay = true;

			// 초기화.
			var serverOpt = new FreeNet.ServerOption();
			serverOpt.MaxConnectionCount = 10000;
			serverOpt.ReceiveBufferSize = 1024;

			service.Initialize(serverOpt);
			service.Listen("0.0.0.0", 7979, 100, socketOpt);
			
			Console.WriteLine("Started!");


			while (true)
			{
				//Console.Write(".");
				string input = Console.ReadLine();

				if (input.Equals("users"))
				{
					Console.WriteLine(service.UserManager.GetTotalCount());
				}
				else if (input.Equals("exit"))
				{
					Console.WriteLine("Exit !!!");
					break;
				}

				System.Threading.Thread.Sleep(500);
			}
		}


		/// <summary>
		/// 클라이언트가 접속 완료 하였을 때 호출됩니다.
		/// n개의 워커 스레드에서 호출될 수 있으므로 공유 자원 접근시 동기화 처리를 해줘야 합니다.
		/// </summary>
		/// <returns></returns>
		static void OnSessionCreated(FreeNet.SessionClient token)
		{
			var user = new GameUser(token);
			lock (UserList)
			{
				UserList.Add(user);
			}
		}

		public static void RemoveUser(GameUser user)
		{
			lock (UserList)
			{
				UserList.Remove(user);
			}
		}
	}
}
