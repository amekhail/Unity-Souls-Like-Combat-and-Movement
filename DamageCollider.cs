using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace AVE
{
    public class DamageCollider : MonoBehaviour
    {
        Collider damageCollider;

        [SerializeField] public int currentWeaponDamage = 25;


        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.enabled = false;
            damageCollider.isTrigger = true;
        }

        public void EnableDamageCollider()
        {
            damageCollider.enabled = true;
        }

        public void DisableDamageCollider()
        {
            damageCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.CompareTag("Player"))
            {
                PlayerStats playerStats = collision.GetComponent<PlayerStats>();
                if (playerStats)
                {
                    playerStats.TakeDamage(currentWeaponDamage);

                }
            }

            if (collision.CompareTag("Enemy"))
            {
                EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
                if (enemyStats)
                {
                    enemyStats.TakeDamage(currentWeaponDamage);
                }
            }
        }
    }
}