using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVE
{
    public class AnimatorManager : MonoBehaviour
    {
        public Animator animator;
        public bool canRotate;

        public void PLayTargetAnimation(string targetAnimation, bool isInteracting)
        {
            animator.applyRootMotion = isInteracting;
            animator.SetBool("canRotate", false);
            animator.SetBool("isInteracting", isInteracting);
            animator.CrossFade(targetAnimation, 0.2f);
        }

        public virtual void TakeCriticalDamageAnimationEvent()
        {
        }

    }
}