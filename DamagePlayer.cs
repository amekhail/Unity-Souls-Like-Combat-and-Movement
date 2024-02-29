using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVE
{
    public class DamagePlayer : MonoBehaviour
    {
        int damage = 25;
        PlayerStats playerStats;

        private void OnTriggerEnter(Collider other)
        {
            playerStats = other.GetComponent<PlayerStats>();

            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }

        }
    }
}