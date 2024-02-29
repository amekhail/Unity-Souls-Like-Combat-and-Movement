using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVE
{
    public class Interactable : MonoBehaviour
    {

        public float radius = 0.6f;
        public String interactableText;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, radius);
        }

        public virtual void Interact(PlayerManager playerManager)
        {

        }
    }
}