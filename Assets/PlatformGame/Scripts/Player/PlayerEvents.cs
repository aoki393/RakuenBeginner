using System;
using UnityEngine.Events;

namespace PLAYERTWO.PlatformerProject
{
    [Serializable] // 注意需要序列化，否则需要在Player.cs里面手动new创建实例
    public class PlayerEvents
    {
        public UnityEvent OnJump;
        public UnityEvent OnRun;
        public UnityEvent OnDashStarted;
        public UnityEvent OnDashEnded;
        public UnityEvent OnHurt;

    }
}
