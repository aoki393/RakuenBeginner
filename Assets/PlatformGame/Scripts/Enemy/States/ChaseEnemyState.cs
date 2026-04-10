using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    public class ChaseEnemyState : EnemyState
    {
        protected override void OnEnter(Enemy enemy)
        {            
            enemy.lateralVelocity = Vector3.zero; // 重置水平速度
        }
        protected override void OnExit(Enemy entity)
        {

        }

        protected override void OnStep(Enemy enemy)
        {
            enemy.Gravity(); // 应用重力

            if (!enemy.loseSight)
            {
                var direction = enemy.Player.transform.position - enemy.transform.position;
                direction.y = 0; // 忽略垂直方向
                direction.Normalize();

                enemy.Accelerate(direction, enemy.Stats.current.followAcceleration, enemy.Stats.current.followTopSpeed);
                enemy.FaceDirectionSmooth(direction);
            }            
        }
        public override void OnContact(Enemy enemy, Collider other)
        {
            // if (other.CompareTag("Player"))
            // {
            //     Debug.Log("ChaseEnemyState OnContact Player");
            //     enemy.lateralVelocity = Vector3.zero;
            //     enemy.enemyEvents.OnPlayerContact.Invoke(); // enmey接触到玩家的声音等表现，玩家表现在玩家中处理
            //     enemy.States.Change<PatrolEnemyState>();
            // }            
        }
    }
}
