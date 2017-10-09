using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FreeNet
{
    class HeartbeatSender
    {
        UserToken Remote;
        Timer TimerHeartBeat;
        uint Interval;

        float elapsed_time;


        public HeartbeatSender(UserToken remote, uint interval)
        {
            Remote = remote;
            Interval = interval;
            TimerHeartBeat = new Timer(this.OnTimer, null, Timeout.Infinite, Interval * 1000);
        }


        void OnTimer(object state)
        {
            Send();
        }


        void Send()
        {
            Packet msg = Packet.create((short)UserToken.SYS_UPDATE_HEARTBEAT);
            this.Remote.send(msg);
        }


        public void Update(float time)
        {
            this.elapsed_time += time;
            if (this.elapsed_time < this.Interval)
            {
                return;
            }

            this.elapsed_time = 0.0f;
            Send();
        }


        public void Stop()
        {
            this.elapsed_time = 0;
            this.TimerHeartBeat.Change(Timeout.Infinite, Timeout.Infinite);
        }


        public void Play()
        {
            this.elapsed_time = 0;
            this.TimerHeartBeat.Change(0, this.Interval * 1000);
        }
    }
}
