using System;
using UnityEngine;

namespace AVE
{
    public class EnemyStats : CharacterStats
    {

        Animator animator;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            currentHealth = maxHealth;
        }

        public void TakeDamageNoAnimation(int damage)
        {
            currentHealth = currentHealth - damage;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isDead = true;
            }
        }

        public void TakeDamage(int damage)
        {
            if (isDead)
            {
                return;
            }
            currentHealth = currentHealth - damage;
            animator.Play("Damage_F");
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                animator.Play("Death_Fall_Back");

                // Handle Enemy Death
                Collider collider = GetComponent<CapsuleCollider>();
                collider.enabled = false;
                isDead = true;
            }
        }
    }
}