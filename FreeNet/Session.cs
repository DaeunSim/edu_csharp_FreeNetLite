using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace FreeNet
{
    public class Session
    {
        enum State
        {
            // 대기중.
            Idle,

            // 연결됨.
            Connected,

            // 종료가 예약됨.
            // sending_list에 대기중인 상태에서 disconnect를 호출한 경우,
            // 남아있는 패킷을 모두 보낸 뒤 끊도록 하기 위한 상태값.
            ReserveClosing,

            // 소켓이 완전히 종료됨.
            Closed,
        }

        
        public Int64 UniqueId { get; private set; } = 0;

        // close중복 처리 방지를 위한 플래그.
        // 0 = 연결된 상태.
        // 1 = 종료된 상태.
        private int IsClosed;

        State CurrentState;
        public Socket Sock { get; set; }

        public SocketAsyncEventArgs ReceiveEventArgs { get; private set; }
        public SocketAsyncEventArgs SendEventArgs { get; private set; }

        // 바이트를 패킷 형식으로 해석해주는 해석기.
        IMessageResolver RefMsgResolver;
       
        // BufferList적용을 위해 queue에서 list로 변경.
        List<ArraySegment<byte>> SendingList;

        // sending_list lock처리에 사용되는 객체.
        private object cs_sending_queue;

        IPacketDispatcher Dispatcher;

        public Action<Session> OnSessionClosed;
        
        // heartbeat.
        public long LatestHeartbeatTime;
        HeartbeatSender HeartbeatSender;
        bool AutoHeartbeat;


        public Session(Int64 uniqueId, IPacketDispatcher dispatcher, IMessageResolver messageResolver)
        {
            UniqueId = uniqueId;
            Dispatcher = dispatcher;
            cs_sending_queue = new object();

            RefMsgResolver = messageResolver;        
            SendingList = new List<ArraySegment<byte>>();
            LatestHeartbeatTime = DateTime.Now.Ticks;

            CurrentState = State.Idle;
        }

        public void OnConnected()
        {
            CurrentState = State.Connected;
            IsClosed = 0;
            AutoHeartbeat = true;
            
            var msg = Packet.Create((short)NetworkDefine.SYS_NTF_CONNECTED);
            Dispatcher.IncomingPacket(true, this, new ArraySegment<byte>(msg.Buffer, 0, msg.Position));
        }
        
        public void SetEventArgs(SocketAsyncEventArgs receive_event_args, SocketAsyncEventArgs send_event_args)
        {
            ReceiveEventArgs = receive_event_args;
            SendEventArgs = send_event_args;
        }

        /// <summary>
        ///	이 매소드에서 직접 바이트 데이터를 해석해도 되지만 Message resolver클래스를 따로 둔 이유는
        ///	추후에 확장성을 고려하여 다른 resolver를 구현할 때 CUserToken클래스의 코드 수정을 최소화 하기 위함이다.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="transfered"></param>
        public void OnReceive(byte[] buffer, int offset, int transfered)
        {
            RefMsgResolver.OnReceive(buffer, offset, transfered, OnMessageCompleted);
        }

        void OnMessageCompleted(ArraySegment<byte> buffer)
        {
            // 로직 스레드의 큐를 타고 호출되도록 함.
            Dispatcher.IncomingPacket(false, this, buffer);
        }

        public void Close()
        {
            // 중복 수행을 막는다.
            if (Interlocked.CompareExchange(ref this.IsClosed, 1, 0) == 1)
            {
                return;
            }

            if (CurrentState == State.Closed)
            {
                // already closed.
                return;
            }

            CurrentState = State.Closed;
            Sock.Close();
            Sock = null;

            SendEventArgs.UserToken = null;
            ReceiveEventArgs.UserToken = null;

            SendingList.Clear();
            RefMsgResolver.ClearBuffer();


            OnSessionClosed(this);

            var msg = Packet.Create((short)NetworkDefine.SYS_NTF_CLOSED);
            Dispatcher.IncomingPacket(true, this, new ArraySegment<byte>(msg.Buffer, 0, msg.Position));                
        }


        /// <summary>
        /// 패킷을 전송한다.
        /// 큐가 비어 있을 경우에는 큐에 추가한 뒤 바로 SendAsync매소드를 호출하고,
        /// 데이터가 들어있을 경우에는 새로 추가만 한다.
        /// 
        /// 큐잉된 패킷의 전송 시점 :
        ///		현재 진행중인 SendAsync가 완료되었을 때 큐를 검사하여 나머지 패킷을 전송한다.
        /// </summary>
        /// <param name="msg"></param>
        public void PreSend(ArraySegment<byte> data)
        {
            lock (cs_sending_queue)
            {
                SendingList.Add(data);

                if (SendingList.Count > 1)
                {
                    // 큐에 무언가가 들어 있다면 아직 이전 전송이 완료되지 않은 상태이므로 큐에 추가만 하고 리턴한다.
                    // 현재 수행중인 SendAsync가 완료된 이후에 큐를 검사하여 데이터가 있으면 SendAsync를 호출하여 전송해줄 것이다.
                    return;
                }
            }

            StartSend();
        }


        public void Send(Packet msg)
        {
            msg.RecordSize();
            PreSend(new ArraySegment<byte>(msg.Buffer, 0, msg.Position));
        }


        /// <summary>
        /// 비동기 전송을 시작한다.
        /// </summary>
        void StartSend()
        {
            try
            {
                // 성능 향상을 위해 SetBuffer에서 BufferList를 사용하는 방식으로 변경함.
                this.SendEventArgs.BufferList = this.SendingList;

                // 비동기 전송 시작.
                bool pending = this.Sock.SendAsync(this.SendEventArgs);
                if (!pending)
                {
                    ProcessSend(this.SendEventArgs);
                }
            }
            catch (Exception e)
            {
                if (this.Sock == null)
                {
                    Close();
                    return;
                }

                Console.WriteLine("send error!! close socket. " + e.Message);
                throw new Exception(e.Message, e);
            }
        }

        //static int sent_count = 0;

        static object cs_count = new object();
        
        /// <summary>
        /// 비동기 전송 완료시 호출되는 콜백 매소드.
        /// </summary>
        /// <param name="e"></param>
        public void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred <= 0 || e.SocketError != SocketError.Success)
            {
                // 연결이 끊겨서 이미 소켓이 종료된 경우일 것이다.
                //Console.WriteLine(string.Format("Failed to send. error {0}, transferred {1}", e.SocketError, e.BytesTransferred));
                return;
            }

            lock (this.cs_sending_queue)
            {
                // 리스트에 들어있는 데이터의 총 바이트 수.
                var size = this.SendingList.Sum(obj => obj.Count);

                // 전송이 완료되기 전에 추가 전송 요청을 했다면 sending_list에 무언가 더 들어있을 것이다.
                if (e.BytesTransferred != size)
                {
                    //todo:세그먼트 하나를 다 못보낸 경우에 대한 처리도 해줘야 함.
                    // 일단 close시킴.
                    if (e.BytesTransferred < this.SendingList[0].Count)
                    {
                        string error = string.Format("Need to send more! transferred {0},  packet size {1}", e.BytesTransferred, size);
                        Console.WriteLine(error);

                        Close();
                        return;
                    }

                    // 보낸 만큼 빼고 나머지 대기중인 데이터들을 한방에 보내버린다.
                    int sent_index = 0;
                    int sum = 0;
                    for (int i = 0; i < this.SendingList.Count; ++i)
                    {
                        sum += this.SendingList[i].Count;
                        if (sum <= e.BytesTransferred)
                        {
                            // 여기 까지는 전송 완료된 데이터 인덱스.
                            sent_index = i;
                            continue;
                        }

                        break;
                    }
                    // 전송 완료된것은 리스트에서 삭제한다.
                    this.SendingList.RemoveRange(0, sent_index + 1);

                    // 나머지 데이터들을 한방에 보낸다.
                    StartSend();
                    return;
                }

                // 다 보냈고 더이상 보낼것도 없다.
                this.SendingList.Clear();
 
                // 종료가 예약된 경우, 보낼건 다 보냈으니 진짜 종료 처리를 진행한다.
                if (this.CurrentState == State.ReserveClosing)
                {
                    this.Sock.Shutdown(SocketShutdown.Send);
                }
            }
        }


        /// <summary>
        /// 연결을 종료한다.
        /// 주로 클라이언트에서 종료할 때 호출한다.
        /// </summary>
        public void DisConnect()
        {
            // close the socket associated with the client
            try
            {
                if (this.SendingList.Count <= 0)
                {
                    this.Sock.Shutdown(SocketShutdown.Send);
                    return;
                }

                CurrentState = State.ReserveClosing;
            }
            // throws if client process has already closed
            catch (Exception)
            {
                Close();
            }
        }

        public void StartHeartbeat(uint interval)
        {
            HeartbeatSender = new HeartbeatSender(this, interval);

            if (AutoHeartbeat)
            {
                StartHeartbeat();
            }
        }
        
        public bool IsConnected()
        {
            return CurrentState == State.Connected;
        }


        public void StartHeartbeat()
        {
            if (HeartbeatSender != null)
            {
                HeartbeatSender.Play();
            }
        }


        public void StopHeartbeat()
        {
            if (HeartbeatSender != null)
            {
                HeartbeatSender.Stop();
            }
        }


        public void DisableAutoHeartbeat()
        {
            StopHeartbeat();
            AutoHeartbeat = false;
        }


        public void UpdateHeartbeatManually(Int32 secondTime)
        {
            if (HeartbeatSender != null)
            {
                HeartbeatSender.Update(secondTime);
            }
        }
    }
}
