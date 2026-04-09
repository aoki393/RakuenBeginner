using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    public class WaypointManager : MonoBehaviour
    {
        // private readonly EnemyStatsManager stats;
        // public float waitTime => stats.PatrolWaitTime;           // 到达路点后的等待时间
        public float waitTime;
        public List<Transform> waypoints; // 路点列表
        protected Transform m_current;   // 当前路点
        protected bool m_changing=false;
        public bool IsIdle => m_changing;

        public Transform Current
        {
            get
            {
                if (!m_current)
                {
                    // 如果当前路点为空，默认为第一个路点
                    m_current = waypoints[0];
                }

                return m_current;
            }
            protected set { m_current = value; }
        }

        private int Index => waypoints.IndexOf(Current);

        /// <summary>
        /// 对 Current 赋值，使Enemy巡逻朝向下一个路点（具体行走在Patrol状态中）
        /// </summary>
        public virtual void Next()
        {
            if (m_changing)
            {
                // 正在切换中时，不执行
                return;
            }

            if (Index + 1 < waypoints.Count)
            {
                StartCoroutine(Change(Index + 1));
            }
            else
            {
                // 到达末路点时从头开始
                StartCoroutine(Change(0));
            }            
        }
        
        protected virtual IEnumerator Change(int to)
        {
            m_changing = true;                 // 标记正在切换
            yield return new WaitForSeconds(waitTime); // 等待指定时间
            Current = waypoints[to];           // 更新当前路点
            m_changing = false;                // 切换完成
        }

        /// <summary>
        /// 从Chase状态回到Patrol状态时，需要设置Current为最近的路点
        /// </summary>
        /// <param name="position">指定路点位置</param>
        public virtual void FindNearestWaypoint(Vector3 position)
        {
            Transform nearest = null;
            float minSqrDistance = float.MaxValue;
            Vector3 currentPos = new Vector3(position.x, 0, position.z); // 忽略 Y
            
            foreach (Transform waypoint in waypoints)
            {
                if (waypoint == null) continue;
                
                Vector3 waypointPos = new Vector3(waypoint.position.x, 0, waypoint.position.z);
                float sqrDistance = (currentPos - waypointPos).sqrMagnitude;
                
                if (sqrDistance < minSqrDistance)
                {
                    minSqrDistance = sqrDistance;
                    nearest = waypoint;
                }
            }
            
            Current = nearest;
        }
    }
}
