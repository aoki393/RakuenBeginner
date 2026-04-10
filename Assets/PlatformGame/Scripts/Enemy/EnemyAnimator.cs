using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
    [RequireComponent(typeof(Enemy))]
    public class EnemyAnimator : MonoBehaviour
    {
        public Enemy Enemy { get; protected set; }
        public Animator animator;

        void Awake()
        {
            Enemy = GetComponent<Enemy>();
            
            if(animator == null)
                Debug.LogError("EnemyAnimator: Animator 未配置！");
            
        }

        void Update()
        {
            animator.SetBool("idle", Enemy.IsIdle);
            animator.SetBool("loseSight", Enemy.loseSight);
            animator.SetInteger("State", Enemy.States.index);
        }
    }
}
