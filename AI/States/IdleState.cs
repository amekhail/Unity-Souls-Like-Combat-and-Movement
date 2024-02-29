using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVE
{
    public class IdleState : State
    {

        public PursueTargetState pursueTargetState;

        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {

            // Handle Enemy Target Detection
            HandleTargetDetection(enemyManager);

            // switch to pursue target
            return returnNextState(enemyManager);

        }

        private void HandleTargetDetection(EnemyManager enemyManager)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, enemyManager.detectionLayer);
            foreach (Collider c in colliders)
            {
                CharacterStats characterStats = c.transform.GetComponent<CharacterStats>();
                if (characterStats != null)
                {
                    // Check for team ID

                    Vector3 targetDirection = characterStats.transform.position - transform.position;
                    float viewableAngle = Vector3.Angle(targetDirection, transform.forward);

                    if (viewableAngle > enemyManager.minimumDetectionAngle && viewableAngle < enemyManager.maximumDetectionAngle)
                    {
                        enemyManager.currentTarget = characterStats;
                    }
                }
            }
        }

        private State returnNextState(EnemyManager enemyManager)
        {
            if (enemyManager.currentTarget != null)
            {
                return pursueTargetState;
            }
            else
            {

                return this;
            }
        }


    }
}
