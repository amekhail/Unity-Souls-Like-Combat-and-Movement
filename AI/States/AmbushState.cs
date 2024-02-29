using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVE
{
    public class AmbushState : State
    {
        public bool isSleeping;
        public float detectionRadius = 2;
        public string sleepAnimation;
        public string wakeAnimation;

        public PursueTargetState pursueTargetState;

        public LayerMask detectionLayer;


        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {
            if (isSleeping && !enemyManager.isInteracting)
            {
                enemyAnimatorManager.PLayTargetAnimation(sleepAnimation, true);
            }

            HandleTargetDirection(enemyManager, enemyAnimatorManager);

            return HandleStateChange(enemyManager);

        }

        private void HandleTargetDirection(EnemyManager enemyManager, EnemyAnimatorManager enemyAnimatorManager)
        {
            Collider[] colliders = Physics.OverlapSphere(enemyManager.transform.position, detectionRadius, detectionLayer);

            foreach (Collider c in colliders)
            {
                CharacterStats characterStats = c.transform.GetComponent<CharacterStats>();

                if (characterStats != null)
                {
                    Vector3 targetsDirection = characterStats.transform.position - enemyManager.transform.position;
                    float viewableAngle = Vector3.Angle(targetsDirection, enemyManager.transform.forward);

                    if (viewableAngle > enemyManager.minimumDetectionAngle &&
                        viewableAngle < enemyManager.maximumDetectionAngle)
                    {
                        enemyManager.currentTarget = characterStats;
                        isSleeping = false;
                        enemyAnimatorManager.PLayTargetAnimation(wakeAnimation, true);
                    }
                }

            }
        }

        private State HandleStateChange(EnemyManager enemyManager)
        {
            if (enemyManager.currentTarget != null)
            {
                return pursueTargetState;
            }
            return this;
        }
    }
}
