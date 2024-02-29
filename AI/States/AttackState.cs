using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVE
{
    public class AttackState : State
    {

        public CombatStanceState combatStanceState;

        public EnemyAttackAction[] enemyAttacks;
        public EnemyAttackAction currentAttack;


        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {
            return HandleAttack(enemyManager, enemyAnimatorManager);
        }

        private State HandleAttack(EnemyManager enemyManager, EnemyAnimatorManager enemyAnimatorManager)
        {
            Vector3 targetDirection = enemyManager.currentTarget.transform.position - transform.position;
            float distanceFromTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
            float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

            base.HandleRotateTowardsTarget(enemyManager);

            if (enemyManager.isPerformingAction)
            {
                return combatStanceState;
            }
            if (currentAttack != null)
            {
                // check if close, get new attack
                if (distanceFromTarget < currentAttack.minimumDistanceNeededToAttack)
                {
                    return this;
                }
                else if (distanceFromTarget < currentAttack.maximumDistanceNeededToAttack)
                {
                    if (viewableAngle <= currentAttack.maximumAttackAngle &&
                        viewableAngle >= currentAttack.minimumAttackAngle)
                    {
                        if (enemyManager.currentRecoveryTime <= 0 && !enemyManager.isPerformingAction)
                        {
                            enemyAnimatorManager.animator.SetFloat("Vertical", 0, 0.1f, Time.deltaTime);
                            enemyAnimatorManager.animator.SetFloat("Horizontal", 0, 0.1f, Time.deltaTime);
                            enemyAnimatorManager.PLayTargetAnimation(currentAttack.actionAnimation, true);
                            enemyManager.isPerformingAction = true;
                            enemyManager.currentRecoveryTime = currentAttack.recoveryTime;
                            currentAttack = null;
                            return combatStanceState;

                        }
                    }
                }
            }
            else
            {
                GetNewAttack(enemyManager);
            }


            return combatStanceState;
        }

        private void GetNewAttack(EnemyManager enemyManager)
        {
            Vector3 targetsDirection = enemyManager.currentTarget.transform.position - transform.position;
            float viewableAngle = Vector3.Angle(targetsDirection, transform.forward);
            float distanceFromTarget = Vector3.Distance(
                enemyManager.currentTarget.transform.position, transform.position);

            int maxScore = 0;
            foreach (EnemyAttackAction e in enemyAttacks)
            {
                if (distanceFromTarget <= e.maximumDistanceNeededToAttack &&
                    distanceFromTarget >= e.minimumDistanceNeededToAttack)
                {
                    if (viewableAngle <= e.maximumAttackAngle && viewableAngle >= e.minimumAttackAngle)
                    {
                        maxScore += e.attakScore;
                    }
                }
            }

            int randomValue = Random.Range(0, maxScore);
            int temporaryScore = 0;

            foreach (EnemyAttackAction e in enemyAttacks)
            {
                if (distanceFromTarget <= e.maximumDistanceNeededToAttack &&
                    distanceFromTarget >= e.minimumDistanceNeededToAttack)
                {

                    if (viewableAngle <= e.maximumAttackAngle && viewableAngle >= e.minimumAttackAngle)
                    {
                        if (currentAttack != null)
                        {
                            return;
                        }

                        temporaryScore += e.attakScore;

                        if (temporaryScore > randomValue)
                        {
                            currentAttack = e;
                        }
                    }
                }
            }
        }
    }
}
