using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVE
{
    public class EnemyAnimatorManager : AnimatorManager
    {
        private EnemyManager enemyManager;
        private EnemyStats _enemyStats;

        private void Awake()
        {
            enemyManager = GetComponentInParent<EnemyManager>();
            animator = GetComponent<Animator>();
            _enemyStats = GetComponentInParent<EnemyStats>();
        }

        private void OnAnimatorMove()
        {
            float delta = Time.deltaTime;
            enemyManager.enemyRigidBody.drag = 0;
            Vector3 deltaPosition = animator.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            enemyManager.enemyRigidBody.velocity = velocity;
        }

        public override void TakeCriticalDamageAnimationEvent()
        {
            _enemyStats.TakeDamageNoAnimation(enemyManager.pendingCriticalDamage);
            enemyManager.pendingCriticalDamage = 0;
        }
    }
}