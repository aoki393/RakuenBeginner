using System.Collections.Generic;
using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    public class EnemyStateManager : EntityStateManager<Enemy>
    {
        public string[] states;
        protected override List<EntityState<Enemy>> GetStateList()
        {
            return EnemyState.CreateListFromStringArray(states);
        }
    }
}

