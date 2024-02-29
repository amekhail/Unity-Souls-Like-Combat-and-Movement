using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVE
{
    public class PlayerStats : CharacterStats
    {

        HealthBar healthBar;
        StaminaBar staminaBar;
        FocusBar focusBar;
        PlayerManager playerManager;

        private PlayerAnimatorManager _playerAnimatorManager;

        public float staminaRegenerationAmount = 1;
        private float staminaRegenTimer = 0;

        private void Awake()
        {
            healthBar = FindObjectOfType<HealthBar>();
            staminaBar = FindObjectOfType<StaminaBar>();
            focusBar = FindObjectOfType<FocusBar>();
            playerManager = FindObjectOfType<PlayerManager>();
            _playerAnimatorManager = GetComponentInChildren<PlayerAnimatorManager>();
        }

        private void Start()
        {
            maxHealth = SetMaxHealthFromHealthLevel();
            healthBar.SetHealthBarSizeFromLevel(healthLevel);
            maxStamina = SetMaxStaminaFromEnduranceLevel();
            maxFocus = SetMaxFocusFromFocusLevel();

            currentFocus = maxFocus;
            currentHealth = maxHealth;
            currentStamina = maxStamina;

            healthBar.SetMaxHealth(maxHealth);
            staminaBar.setMaxStamina(maxStamina);
            focusBar.SetMaxFocus(maxFocus);
        }

        public float GetCurrentStamina()
        {
            return currentStamina;
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
            if (isDead || playerManager.isInvulnerable)
            {
                return;
            }
            currentHealth = currentHealth - damage;

            healthBar.SetCurrentHealth(currentHealth);
            _playerAnimatorManager.PLayTargetAnimation("Damage_F", true);

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                _playerAnimatorManager.PLayTargetAnimation("Death_Fall_Back", true);
                isDead = true;
            }

        }

        public void TakeStaminaDamage(int damage)
        {
            currentStamina -= damage;
            // set bar value
            staminaBar.setCurrentStamina(Mathf.RoundToInt(currentStamina));
        }

        public void RegenerateStamina()
        {
            if (playerManager.isInteracting)
            {
                staminaRegenTimer = 0;
                return;
            }
            staminaRegenTimer += Time.deltaTime;
            if (currentStamina < maxStamina && staminaRegenTimer > 1f)
            {
                currentStamina += staminaRegenerationAmount * Time.deltaTime;
                staminaBar.setCurrentStamina(Mathf.RoundToInt(currentStamina));
            }
        }

        public void HealPlayer(int healAmount)
        {
            currentHealth += healAmount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }

            healthBar.SetCurrentHealth(currentHealth);
        }

        public void TakeFocusDamage(int damage)
        {
            currentFocus -= damage;

            if (currentFocus < 0)
            {
                currentFocus = 0;
            }

            focusBar.SetCurrentFocus(Mathf.RoundToInt(currentFocus));
        }

    }
}