using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace FreeNet
{
	/// <summary>
	/// Endpoint정보를 받아서 서버에 접속한다.
	/// 접속하려는 서버 하나당 인스턴스 한개씩 생성하여 사용하면 된다.
	/// </summary>
	public class Connector
	{
		public Action<UserToken> ConnectedCallback = null;

		// 원격지 서버와의 연결을 위한 소켓.
		Socket ClientSocket;

		NetworkService RefNetworkService;


		public Connector(NetworkService networkService)
		{
			RefNetworkService = networkService;
		}

		public void Connect(IPEndPoint remoteEndpoint, SocketOption socketOption)
		{
			ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			
			// TODO: 인자로 선택할 수 있도록 하기
			ClientSocket.NoDelay = socketOption.NoDelay;

			// 비동기 접속을 위한 event args.
			SocketAsyncEventArgs event_arg = new SocketAsyncEventArgs();
			event_arg.Completed += OnConnectCompleted;
			event_arg.RemoteEndPoint = remoteEndpoint;


			bool pending = ClientSocket.ConnectAsync(event_arg);

			if (pending == false)
			{
				OnConnectCompleted(null, event_arg);
			}
		}

		void OnConnectCompleted(object sender, SocketAsyncEventArgs e)
		{
			if (e.SocketError == SocketError.Success)
			{
				//Console.WriteLine("Connect completd!");
				UserToken token = new UserToken(this.RefNetworkService.LogicEntry);

				// 데이터 수신 준비.
				this.RefNetworkService.OnConnectCompleted(this.ClientSocket, token);

				if (this.ConnectedCallback != null)
				{
					this.ConnectedCallback(token);
				}
			}
			else
			{
				//TODO: 로그로 남기기
				// failed.
				Console.WriteLine(string.Format("Failed to connect. {0}", e.SocketError));
			}
		}
	}
}
