using System;
using System.Collections.Generic;

namespace SampleServer
{
	class Program
	{
		

		static void Main(string[] args)
		{
			var serverOpt = new FreeNet.ServerOption();
			serverOpt.MaxConnectionCount = 10000;
			serverOpt.ReceiveBufferSize = 1024;

			var service = new FreeNet.NetworkService(serverOpt);					
			service.Initialize();

			var socketOpt = new FreeNet.SocketOption();
			socketOpt.NoDelay = true;
			service.Listen("0.0.0.0", 7979, 100, socketOpt);
			
			Console.WriteLine("Started!");


			// 패킷 처리기 생성 및 실행
			var packetProcess = new PacketProcess(service);
			packetProcess.Start();


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
					packetProcess.Stop();

					Console.WriteLine("Exit !!!");
					break;
				}

				System.Threading.Thread.Sleep(500);
			}
		}		
	}
}
