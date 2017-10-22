using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FreeNet
{
    class HeartbeatSender
    {
        Session Remote;

        Timer TimerHeartBeat;
        UInt32 IntervalSecondTime;

        Int32 ElapsedSecondime;


        public HeartbeatSender(Session remote, UInt32 intervalSecondTime)
        {
            Remote = remote;

            IntervalSecondTime = intervalSecondTime;
            TimerHeartBeat = new Timer(OnTimer, null, Timeout.Infinite, IntervalSecondTime * 1000);
        }


        void OnTimer(object state)
        {
            Send();
        }


        void Send()
        {
            var msg = Packet.Create((short)NetworkDefine.SYS_UPDATE_HEARTBEAT);
            Remote.Send(msg);
        }


        public void Update(int secondTime)
        {
            ElapsedSecondime += secondTime;

            if (ElapsedSecondime < IntervalSecondTime) {
                return;
            }

            ElapsedSecondime = 0;
            Send();
        }


        public void Stop()
        {
            ElapsedSecondime = 0;
            TimerHeartBeat.Change(Timeout.Infinite, Timeout.Infinite);
        }


        public void Play()
        {
            ElapsedSecondime = 0;
            TimerHeartBeat.Change(0, IntervalSecondTime * 1000);
        }
    }
}
