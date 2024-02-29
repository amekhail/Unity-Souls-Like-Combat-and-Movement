using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVE
{
    public class CombatStanceState : State
    {
        public AttackState attackState;
        public PursueTargetState pursueTargetState;

        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

            base.HandleRotateTowardsTarget(enemyManager);

            if (enemyManager.isPerformingAction)
            {
                enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
            }

            // Check if AI is in range of target and can attack
            if (CanAttack(enemyManager) && IsInMaximumAttackRange(enemyManager, distanceFromTarget))
            {

                return attackState;
            }
            else if (!IsInMaximumAttackRange(enemyManager, distanceFromTarget))
            {
                return pursueTargetState;
            }
            else
            {
                return this;
            }
        }

        private bool CanAttack(EnemyManager enemyManager)
        {
            return enemyManager.currentRecoveryTime <= 0;
        }

        private bool IsInMaximumAttackRange(EnemyManager enemyManager, float distanceFromTarget)
        {
            return distanceFromTarget <= enemyManager.maximumAttackRange;
        }
    }
}
