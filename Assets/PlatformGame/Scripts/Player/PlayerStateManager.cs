using System.Collections.Generic;
using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    public class PlayerStateManager : EntityStateManager<Player>
    {
        // [ClassTypeName(typeof(PlayerState))] // 这个特性需要自定义实现，暂时注释掉

        /// <summary>
        /// 玩家的状态名称数组，每个元素对应一个 PlayerState 类的名称。
        /// 注意：状态名称必须与 PlayerState 类的名称一致，否则会导致状态切换失败。
        /// 第一个需要填写IdleState，因为EntityStateManager中初始化时会进入第一个状态
        /// </summary>
        public string[] states; // 注意在 Inspector 中输入状态名称时要与 PlayerState 类的名称一致
        
        protected override List<EntityState<Player>> GetStateList()
        {
            return PlayerState.CreateListFromStringArray(states);
            // return EntityState<Player>.CreateListFromStringArray(states);
        }
    }
}
