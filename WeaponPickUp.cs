using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AVE
{
    public class WeaponPickUp : Interactable
    {
        public WeaponItem weapon;


        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);

            PickUpItem(playerManager);
        }

        private void PickUpItem(PlayerManager playerManager)
        {
            PlayerInventory playerInventory;
            PlayerLocomotion playerLocomotion;
            PlayerAnimatorManager playerAnimatorManager;

            playerInventory = playerManager.GetComponent<PlayerInventory>();
            playerLocomotion = playerManager.GetComponent<PlayerLocomotion>();
            playerAnimatorManager = playerManager.GetComponentInChildren<PlayerAnimatorManager>();

            // Stops the player from moving while picking up an item
            playerLocomotion.rigidbody.velocity = Vector3.zero;
            // Play loot animation
            playerAnimatorManager.PLayTargetAnimation("Pick Up Item", true);
            playerInventory.weaponsInventory.Add(weapon);
            // poop
            playerManager.itemInteractableGameObject.GetComponentInChildren<TMP_Text>().text = weapon.itemName;
            playerManager.itemInteractableGameObject.GetComponentInChildren<RawImage>().texture = weapon.itemIcon.texture;
            playerManager.itemInteractableGameObject.SetActive(true);
            Destroy(gameObject);
        }
    }
}