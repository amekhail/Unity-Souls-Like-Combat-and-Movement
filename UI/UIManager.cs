using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVE
{
    public class UIManager : MonoBehaviour
    {
        private PlayerInventory playerInventory;

        [SerializeField] public EquipmentWindowUI equipmentWindowUI;

        [Header("UI Windows")]
        [SerializeField] private GameObject hudWindow;
        [SerializeField] private GameObject selectWindow;

        [SerializeField] private GameObject weaponInventoryWindow;
        [SerializeField] private GameObject equipmentScreenWindow;

        [Header("Equipment Window Slot Selected")]
        public bool rightHandSlot01Selected;
        public bool rightHandSlot02Selected;
        public bool leftHandSlot01Selected;
        public bool leftHandSlot02Selected;

        [Header("Weapon Inventory")]
        public Transform weaponInventorySlotsParent;
        public GameObject weaponInventorySlotPrefab;
        WeaponInventorySlot[] weaponInventorySlots;


        private void Awake()
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
        }

        private void Start()
        {
            weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>(true);
            equipmentWindowUI.LoadWeaponsOnEquipmentScreen(playerInventory);
        }

        public void UpdateUI()
        {
            #region Weapon Inventory Slots
            for (int i = 0; i < weaponInventorySlots.Length; i++)
            {
                if (i < playerInventory.weaponsInventory.Count)
                {
                    if (weaponInventorySlots.Length < playerInventory.weaponsInventory.Count)
                    {
                        Instantiate(weaponInventorySlotPrefab, weaponInventorySlotsParent);
                        weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>(true);
                    }
                    weaponInventorySlots[i].AddItem(playerInventory.weaponsInventory[i]);
                }
                else
                {
                    weaponInventorySlots[i].ClearInventorySlot();
                }
            }

            #endregion
        }


        public void OpenSelectWindow()
        {
            selectWindow.SetActive(true);
        }

        public void CloseSelectWindow()
        {
            selectWindow.SetActive(false);
        }

        public void SetActiveHUDWindow(bool isActive)
        {
            hudWindow.SetActive(isActive);
        }

        public void CloseAllInventoryWindows()
        {
            ResetAllSelectedSlots();
            weaponInventoryWindow.SetActive(false);
            equipmentScreenWindow.SetActive(false);
        }

        public void ResetAllSelectedSlots()
        {
            rightHandSlot01Selected = false;
            rightHandSlot02Selected = false;
            leftHandSlot01Selected = false;
            leftHandSlot02Selected = false;
        }
    }
}