using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    public class PatrolEnemyState : EnemyState
    {
        protected override void OnEnter(Enemy enemy){
            enemy.lateralVelocity = Vector3.zero; // 重置水平速度
            enemy.Waypoints.FindNearestWaypoint(enemy.transform.position);
        }

        protected override void OnExit(Enemy entity){ }

        protected override void OnStep(Enemy enemy)
        {
            enemy.Gravity(); // 应用重力

            var destination = enemy.Waypoints.Current.position;
            var direction = new Vector3(destination.x, enemy.Position.y, destination.z) - enemy.Position;
            var distance = direction.magnitude;
            direction.Normalize();

            if (distance <= enemy.Stats.current.waypointMinDistance)
            {
                // 减速
                enemy.Decelerate();
                // 切换到下一个巡逻点
                enemy.Waypoints.Next();
            }
            else
            {
                // 向目标方向加速，使用巡逻加速度和最大速度限制
                enemy.Accelerate(
                    direction, 
                    enemy.Stats.current.waypointAcceleration, 
                    enemy.Stats.current.waypointTopSpeed
                );

                // 如果配置要求朝向巡逻点方向
                if (enemy.Stats.current.faceWaypoint)
                {
                    // 平滑地转向目标方向
                    enemy.FaceDirectionSmooth(direction);
                }
            }

        }
        public override void OnContact(Enemy entity, Collider other)
        {
            
        }
    }
}