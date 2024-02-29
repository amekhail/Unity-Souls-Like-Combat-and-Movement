using System;
using System.Collections.Generic;
using UnityEngine;

namespace AVE
{
    public class CharacterStats : MonoBehaviour
    {
        [Header("Vigor")] // Controls the health and level
        [SerializeField] public int healthLevel = 10;
        [SerializeField] public int maxHealth;
        [SerializeField] public int currentHealth;

        [Header("Endurance")]
        [SerializeField] public int enduranceLevel = 10;
        [SerializeField] public float maxStamina;
        [SerializeField] public float currentStamina;

        [Header("Focus")]
        [SerializeField] public float focusLevel = 10;
        [SerializeField] public float maxFocus;
        [SerializeField] public float currentFocus;


        public bool isDead;

        // Formula for calculating health values
        // Level 1 - 25 --> 300 + 500*(((Lvl - 1) / 24)^1.5)
        // Level 26 - 40 --> 800 + 650*(((Lvl - 25) / 15)^1.1)
        // Level 41 - 60 --> 1450 + 450*(1 - (1 - ((Lvl - 40) / 20))^1.2)
        // Level 61 - 99 --> 1900 + 200*(1 - (1 - ((Lvl - 60) / 39))^1.2)
        // The resulting number is always rounded down
        protected int SetMaxHealthFromHealthLevel()
        {


            if (healthLevel > 0 && healthLevel < 26)
            {
                maxHealth = 300 + 500 * Mathf.RoundToInt((float)Math.Pow(((healthLevel - 1) / 24), 1.5));
            }
            else if (healthLevel > 25 && healthLevel < 41)
            {
                maxHealth = 800 + 650 * Mathf.RoundToInt((float)Math.Pow(((healthLevel - 25) / 15), 1.1));
            }
            else if (healthLevel > 40 && healthLevel < 61) // After level 60, there should be a fall off in health gain
            {
                maxHealth = 1450 + 450 * (1 - Mathf.RoundToInt((float)Math.Pow((1 - ((healthLevel - 40) / 20)), 1.2)));
            }
            else
            {
                maxHealth = 1900 + 200 * (1 - Mathf.RoundToInt((float)Math.Pow((1 - ((healthLevel - 60) / 39)), 1.2)));
            }

            return maxHealth;
        }
        // Level 1 - 15: 80 + 25*((enduranceLevel - 1) / 14)
        // Level 16 - 35: 105 + 25*((enduranceLevel - 15) / 15)
        // Level 36 - 60: 130 + 25*((enduranceLevel - 30) / 20)
        // Level 61 - 99: 155 + 15*((enduranceLevel - 50) / 49)
        // The resulting number is always rounded down
        protected float SetMaxStaminaFromEnduranceLevel()
        {
            if (enduranceLevel > 0 && enduranceLevel < 16)
            {
                maxStamina = 80 + 25 * ((enduranceLevel - 1) / 14);
            }
            else if (enduranceLevel > 15 && enduranceLevel < 36)
            {
                maxStamina = 105 + 25 * ((enduranceLevel - 15) / 15);
            }
            else if (enduranceLevel > 35 && enduranceLevel < 61)
            {
                maxStamina = 130 + 25 * ((enduranceLevel - 30) / 20);
            }
            else
            {
                maxStamina = 155 + 15 * ((enduranceLevel - 50) / 49);
            }
            return maxStamina;
        }


        // Level 1 - 15 --> 50 + 45*((Lvl - 1) / 14)
        // Level 16 - 35 --> 95 + 105*((Lvl - 15) / 20)
        // Level 36 - 60 --> 200 + 150*(1 - (1 - ((Lvl - 35) / 25))^1.2))
        // Level 61 - 99 --> 350 + 100*((Lvl - 60) / 39)
        // The resulting number is always rounded down
        protected float SetMaxFocusFromFocusLevel()
        {
            if (focusLevel > 0 || focusLevel < 16)
            {
                maxFocus = 50 + 45 * ((focusLevel - 1) / 14);

            }
            else if (focusLevel > 15 || focusLevel < 36)
            {
                maxFocus = 95 + 105 * ((focusLevel - 15) / 20);
            }
            else if (focusLevel > 35 || focusLevel < 61)
            {
                maxFocus = 200 + 150 * Mathf.RoundToInt(1 - Mathf.Pow(1 - (currentFocus - 35) / 25, 1.2f));
            }
            else
            {
                maxFocus = 350 + 150 * ((focusLevel - 60) / 39);
            }
            return maxFocus;
        }
    }
}