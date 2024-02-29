using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVE
{
    [CreateAssetMenu(menuName = "AI/EnemyActions/Attack Action")]
    public class EnemyAttackAction : EnemyActions
    {

        // The higher the score, the more likely it will occur
        public int attakScore = 3;
        public float recoveryTime = 2;

        public float maximumAttackAngle = -35;
        public float minimumAttackAngle = 35;

        public float minimumDistanceNeededToAttack = 0;
        public float maximumDistanceNeededToAttack = 3;

    }
}