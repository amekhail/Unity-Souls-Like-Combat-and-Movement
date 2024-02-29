using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AVE
{
    public class EnemyManager : CharacterManager
    {
        public LayerMask detectionLayer;

        EnemyLocomotionManager enemyLocomotionManager;
        EnemyAnimatorManager enemyAnimatorManager;
        EnemyStats enemyStats;

        public NavMeshAgent navMeshAgent;
        public Rigidbody enemyRigidBody;

        [Header("AI Stats")]
        public CharacterStats currentTarget;
        public State currentState;
        public bool isPerformingAction;
        public bool isInteracting;

        [Header("AI Settings")]
        public float detectionRadius = 20;
        public float minimumDetectionAngle = -50;
        public float maximumDetectionAngle = 50;
        public float rotationSpeed = 15;
        public float maximumAttackRange = 1.5f;


        public float currentRecoveryTime = 0;


        private void Awake()
        {
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
            enemyAnimatorManager = GetComponentInChildren<EnemyAnimatorManager>();
            enemyStats = GetComponent<EnemyStats>();
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            enemyRigidBody = GetComponent<Rigidbody>();
            backStabCollider = GetComponentInChildren<BackStabCollider>();
        }

        private void Start()
        {
            navMeshAgent.enabled = false;
            enemyRigidBody.isKinematic = false;
        }

        private void FixedUpdate()
        {
            HandleStateMachine();
        }

        private void Update()
        {
            HandleRecoveryTimer();
            isInteracting = enemyAnimatorManager.animator.GetBool("isInteracting");
            enemyAnimatorManager.animator.SetBool("isDead", enemyStats.isDead);
        }

        private void HandleStateMachine()
        {
            if (currentState != null)
            {
                State nextState = currentState.Tick(this, enemyStats, enemyAnimatorManager);

                if (nextState != null)
                {
                    SwitchToNextState(nextState);
                }
            }
        }

        private void HandleRecoveryTimer()
        {
            if (currentRecoveryTime > 0)
            {
                currentRecoveryTime -= Time.deltaTime;
            }

            if (isPerformingAction)
            {
                if (currentRecoveryTime <= 0)
                {
                    isPerformingAction = false;
                }
            }
        }

        private void SwitchToNextState(State state)
        {
            currentState = state;
        }
    }
}