using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace FreeNet
{
    /// <summary>
    /// 현재 접속중인 전체 유저를 관리하는 클래스.
    /// </summary>
    public class UserManager
    {
        //TODO: ConcureentDictionary를 사용한다. 그런데 foreach에서 스레드 세이프한지 테스트 하자. 스택오버플로어에서는 스레드 세이프하다고 한다.
        object cs_user;
        List<UserToken> Users;
        //ConcurrentDictionary users = new ConcurrentDictionary<UserToken>();

        Timer TimerHeartbeat;
        long HeartbeatDuration;


        public UserManager()
        {
            cs_user = new object();
            Users = new List<UserToken>();
        }


        public void StartHeartbeatChecking(uint check_interval_sec, uint allow_duration_sec)
        {
            HeartbeatDuration = allow_duration_sec * 10000000;
            TimerHeartbeat = new Timer(CheckHeartbeat, null, 1000 * check_interval_sec, 1000 * check_interval_sec);
        }


        public void Add(UserToken user)
        {
            lock (cs_user)
            {
                Users.Add(user);
            }
        }


        public void Remove(UserToken user)
        {
            lock (this.cs_user)
            {
                Users.Remove(user);
            }
        }


        public bool IsExist(UserToken user)
        {
            lock (this.cs_user)
            {
                return this.Users.Exists(obj => obj == user);
            }
        }


        public int GetTotalCount()
        {
            return this.Users.Count;
        }


        void CheckHeartbeat(object state)
        {
            long allowed_time = DateTime.Now.Ticks - this.HeartbeatDuration;

            lock (this.cs_user)
            {
                for (int i = 0; i < this.Users.Count; ++i)
                {
                    long heartbeat_time = this.Users[i].LatestHeartbeatTime;
                    if (heartbeat_time >= allowed_time)
                    {
                        continue;
                    }

                    this.Users[i].DisConnect();
                }
            }
        }


    }
}
