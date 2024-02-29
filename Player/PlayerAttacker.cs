using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVE
{
    public class PlayerAttacker : MonoBehaviour
    {
        PlayerAnimatorManager _playerAnimatorManager;
        PlayerManager playerManager;
        InputHandler inputHandler;
        WeaponSlotManager weaponSlotManager;
        PlayerInventory playerInventory;
        PlayerStats playerStats;

        LayerMask backStabLayer = 1 << 13;

        public string lastAttack;

        private void Awake()
        {
            _playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerManager = GetComponentInParent<PlayerManager>();
            playerInventory = GetComponentInParent<PlayerInventory>();
            inputHandler = GetComponentInParent<InputHandler>();
            weaponSlotManager = GetComponent<WeaponSlotManager>();
            playerStats = GetComponentInParent<PlayerStats>();
        }

        public void HandleWeaponCombo(WeaponItem weapon)
        {
            if (inputHandler.comboFlag)
            {
                _playerAnimatorManager.animator.SetBool("canDoCombo", false);
                if (lastAttack == weapon.OH_Light_Attack_1)
                {
                    _playerAnimatorManager.PLayTargetAnimation(weapon.OH_Light_Attack_2, true);
                }
                else if (lastAttack == weapon.TH_Light_Attack_1)
                {
                    _playerAnimatorManager.PLayTargetAnimation(weapon.TH_Light_Attack_2, true);
                    lastAttack = weapon.TH_Light_Attack_2;
                }
                else if (lastAttack == weapon.TH_Light_Attack_2)
                {
                    _playerAnimatorManager.PLayTargetAnimation(weapon.TH_Light_Attack_3, true);
                }

            }
        }

        public void HandleLightAttack(WeaponItem weapon)
        {
            weaponSlotManager.attackingWeapon = weapon;
            if (inputHandler.twoHandFlag)
            {
                _playerAnimatorManager.PLayTargetAnimation(weapon.TH_Light_Attack_1, true);
                lastAttack = weapon.TH_Light_Attack_1;
            }
            else
            {
                _playerAnimatorManager.PLayTargetAnimation(weapon.OH_Light_Attack_1, true);
                lastAttack = weapon.OH_Light_Attack_1;
            }
        }

        public void HandleHeavyAttach(WeaponItem weapon)
        {
            weaponSlotManager.attackingWeapon = weapon;
            if (inputHandler.twoHandFlag)
            {
                _playerAnimatorManager.PLayTargetAnimation(weapon.TH_Heavy_Attack_1, true);
                lastAttack = weapon.TH_Heavy_Attack_1;
            }
            else
            {
                _playerAnimatorManager.PLayTargetAnimation(weapon.OH_Heavy_Attack_1, true);
                lastAttack = weapon.OH_Heavy_Attack_1;
            }
        }

        #region Input Actions
        public void HandleleftClickAction()
        {
            if (playerInventory.rightWeapon.weaponType == WeaponItem.WeaponType.MeleeWeapon)
            {
                // Handle Melee action
                PerformLeftClickMeleeAction();
            }
            else if (CheckForMagic(playerInventory.rightWeapon))
            {
                // Handle Magic action
                PerformLeftClickMagicAction(playerInventory.rightWeapon);
            }
        }

        public void HandleRightCLickACtion()
        {
            if (playerInventory.leftWeapon.weaponType == WeaponItem.WeaponType.MeleeWeapon)
            {
                // Handle Melee action
                performRightClickMeleeAction();
            }
            else if (CheckForMagic(playerInventory.leftWeapon))
            {
                // Handle Magic action
            }

        }
        #endregion

        #region Attack Actions
        private void performRightClickMeleeAction()
        {
            HandleHeavyAttach(playerInventory.rightWeapon);
        }

        private void PerformLeftClickMeleeAction()
        {
            if (playerManager.canDoCombo)
            {
                inputHandler.comboFlag = true;
                HandleWeaponCombo(playerInventory.rightWeapon);
                inputHandler.comboFlag = false;
            }
            else
            {
                if (playerManager.isInteracting || playerManager.canDoCombo)
                {
                    return;
                }
                _playerAnimatorManager.animator.SetBool("isUsingRightHand", true);
                HandleLightAttack(playerInventory.rightWeapon);
            }
        }

        private void PerformLeftClickMagicAction(WeaponItem weapon)
        {
            if (playerManager.isInteracting)
            {
                return;
            }

            if (weapon.weaponType == WeaponItem.WeaponType.FaithCaster)
            {
                if (playerInventory.currentSpell != null && playerInventory.currentSpell.spellType == SpellItem.SpellType.FaithSpell)
                {
                    // Check for FP
                    if (playerStats.currentFocus >= playerInventory.currentSpell.focusPointCost)
                    {
                        // Attempt to cast the spell
                        playerInventory.currentSpell.AttemptToCastSpell(_playerAnimatorManager, playerStats);
                    }
                    else
                    {
                        // play no spell animation.
                    }

                }
            }
        }

        private void SucessfullyCastSpell()
        {
            playerInventory.currentSpell.SucessfullyCastedSpell(_playerAnimatorManager, playerStats);
        }



        #endregion


        private bool CheckForMagic(WeaponItem weapon)
        {
            if (weapon.weaponType is WeaponItem.WeaponType.FaithCaster or WeaponItem.WeaponType.PyroCaster or WeaponItem.WeaponType.PyroCaster)
            {
                return true;
            }
            return false;
        }

        public void AttemptBackStabOrRiposte()
        {
            RaycastHit hit;
            if (Physics.Raycast(inputHandler.criticalAttackRayCastStartPoint.position, transform.TransformDirection(Vector3.forward), out hit, 0.5f, backStabLayer))
            {
                CharacterManager enemyCharacterManager = hit.transform.gameObject.GetComponentInParent<CharacterManager>();
                DamageCollider rightWeapon = weaponSlotManager.rightHandDamageCollider;

                if (enemyCharacterManager != null)
                {
                    // check for team I.D
                    playerManager.transform.position = enemyCharacterManager.backStabCollider.backStabberStandPoint.position;
                    
                    // pull into transform behind the enemy
                    // rotate towards the transform
                    Vector3 rotationDirection = playerManager.transform.root.eulerAngles;
                    rotationDirection = hit.transform.position - playerManager.transform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();

                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 500 * Time.deltaTime);
                    playerManager.transform.rotation = targetRotation;

                    int criticalDamage = playerInventory.rightWeapon.criticalDamageMultiplier *
                                         rightWeapon.currentWeaponDamage;

                    // play the animation
                    _playerAnimatorManager.PLayTargetAnimation("Back Stab", true);

                    // play the animation on enemy
                    enemyCharacterManager.GetComponentInChildren<EnemyAnimatorManager>().PLayTargetAnimation("Back Stabbed", true);

                    // do damage
                    enemyCharacterManager.pendingCriticalDamage = criticalDamage;
                }
            }
        }
    }
}