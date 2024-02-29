using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVE
{
    public class PlayerAnimatorManager : AnimatorManager
    {
        private PlayerManager playerManager;
        private InputHandler inputHandler;
        private PlayerLocomotion playerLocomotion;
        private PlayerStats _playerStats;

        private int vertical;
        private int horizontal;
        
        public void Initialize()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            _playerStats = GetComponentInParent<PlayerStats>();
            animator = GetComponent<Animator>();
            inputHandler = GetComponentInParent<InputHandler>();
            playerLocomotion = GetComponentInParent<PlayerLocomotion>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");

        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting)
        {
            float v = getMovementClamped(verticalMovement);
            float h = getMovementClamped(horizontalMovement);

            if (isSprinting)
            {
                v = 2;
                h = horizontalMovement;
            }

            animator.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            animator.SetFloat(horizontal, h, 0.1f, Time.deltaTime);


        }

        public void StartRotate()
        {
            animator.SetBool("canRotate", true);
        }

        public void StopRotation()
        {
            animator.SetBool("canRotate", false);
        }

        private void OnAnimatorMove()
        {
            if (playerManager.isInteracting == false) return;

            float delta = Time.deltaTime;
            playerLocomotion.rigidbody.drag = 0;
            Vector3 deltaPos = animator.deltaPosition;
            deltaPos.y = 0;
            Vector3 velocity = deltaPos / delta;
            playerLocomotion.rigidbody.velocity = velocity;


        }

        /// <summary>
        /// Clamps the provided horizontal or vertical movement between -1 and 1
        /// </summary>
        /// <param name="movement">The movement in either the horizontal or vertical direction</param>
        /// <returns>The movement clamped between -1 and 1</returns> 
        private float getMovementClamped(float movement)
        {
            if (movement > 0 && movement < 0.55f)
            {
                return 0.5f;
            }
            else if (movement > 0.55f)
            {
                return 1;
            }
            else if (movement < 0 && movement > -0.55f)
            {
                return -0.5f;
            }
            else if (movement < -0.55f)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }



        public void EnableCombo()
        {
            animator.SetBool("canDoCombo", true);
        }

        public void DisableCombo()
        {
            animator.SetBool("canDoCombo", false);
        }

        public void EnableIsInvulnerable()
        {
            animator.SetBool("isInvulnerable", true);
        }

        public void DisableIsInvulnerable()
        {
            animator.SetBool("isInvulnerable", false);
        }

        public override void TakeCriticalDamageAnimationEvent()
        {
            _playerStats.TakeDamageNoAnimation(playerManager.pendingCriticalDamage);
            playerManager.pendingCriticalDamage = 0;
        }
    }
}