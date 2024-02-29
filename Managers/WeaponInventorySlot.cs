using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AVE
{
    public class WeaponInventorySlot : MonoBehaviour
    {
        [SerializeField] private Image icon;
        private WeaponItem item;
        private PlayerInventory playerInventory;
        private UIManager uiManager;
        private WeaponSlotManager weaponSlotManager;

        private void Awake()
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
            uiManager = FindObjectOfType<UIManager>();
            weaponSlotManager = FindObjectOfType<WeaponSlotManager>();
        }

        public void AddItem(WeaponItem newItem)
        {
            item = newItem;
            icon.sprite = item.itemIcon;
            icon.enabled = true;
            gameObject.SetActive(true);
        }

        public void ClearInventorySlot()
        {
            item = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }

        public void EquipThisItem()
        {
            if (uiManager.rightHandSlot01Selected)
            {
                SwapWeapon(0, false);
            }
            else if (uiManager.rightHandSlot02Selected)
            {
                SwapWeapon(1, false);
            }
            else if (uiManager.leftHandSlot01Selected)
            {
                SwapWeapon(0, true);
            }
            else if (uiManager.leftHandSlot02Selected)
            {
                SwapWeapon(1, true);
            }
            else
            {
                return;
            }

            playerInventory.leftWeapon = playerInventory.weaponsInLeftHandSlots[playerInventory.currentLeftWeaponIndex];
            playerInventory.rightWeapon = playerInventory.weaponsInRightHandSlots[playerInventory.currentRightWeaponIndex];

            weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftWeapon, true);

            uiManager.equipmentWindowUI.LoadWeaponsOnEquipmentScreen(playerInventory);
            uiManager.ResetAllSelectedSlots();
        }


        private void SwapWeapon(int slot, bool isLeft)
        {
            if (isLeft)
            {
                playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[slot]);
                playerInventory.weaponsInLeftHandSlots[slot] = item;

            }
            else
            {
                playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[slot]);
                playerInventory.weaponsInRightHandSlots[slot] = item;
            }

            playerInventory.weaponsInventory.Remove(item);
        }
    }
}