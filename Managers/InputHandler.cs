using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVE
{
    public class InputHandler : MonoBehaviour
    {
        public float horizontal, vertical, moveAmount, mouseX, mouseY, rollInputTimer;
        public Transform criticalAttackRayCastStartPoint;
        [Header("Flags")]
        public bool rollFlag;
        public bool sprintFlag;
        public bool comboFlag;
        public bool inventoryFlag;
        public bool lockOnFlag;
        public bool twoHandFlag;

        [Header("Inputs")]
        public bool shit_input;
        public bool leftClick_input;
        public bool criticalAttack_input;
        public bool rightClick_input;
        public bool jump_input;
        public bool e_input;
        public bool y_input;
        public bool inventory_input;
        public bool lockOn_input;
        public bool rightStick_right_input;
        public bool rightStick_left_input;
        [Space(10)]
        // Dpad / arrow keys
        public bool dPadUp;
        public bool dPadDown;
        public bool dPadLeft;
        public bool dPadRight;

        PlayerControls inputActions;
        PlayerAttacker playerAttacker;
        PlayerInventory playerInventory;
        PlayerManager playerManager;
        UIManager uiManager;
        CameraHandler cameraHandler;
        PlayerAnimatorManager _playerAnimatorManager;
        WeaponSlotManager weaponSlotManager;

        Vector2 movementInput;
        Vector2 cameraInput;

        private void Awake()
        {
            playerAttacker = GetComponentInChildren<PlayerAttacker>();
            playerInventory = GetComponent<PlayerInventory>();
            playerManager = GetComponent<PlayerManager>();
            uiManager = FindObjectOfType<UIManager>();
            cameraHandler = FindObjectOfType<CameraHandler>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            _playerAnimatorManager = GetComponentInChildren<PlayerAnimatorManager>();
        }

        public void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();
                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

                inputActions.PlayerActions.LeftClick.performed += i => leftClick_input = true;
                inputActions.PlayerActions.RightClick.performed += i => rightClick_input = true;
                inputActions.PlayerQuickSlots.DPadRight.performed += i => dPadRight = true;
                inputActions.PlayerQuickSlots.DPadLeft.performed += i => dPadLeft = true;
                inputActions.PlayerActions.EButton.performed += i => e_input = true;
                inputActions.PlayerActions.Jump.performed += i => jump_input = true;
                inputActions.PlayerActions.Inventory.performed += i => inventory_input = true;
                inputActions.PlayerActions.LockOn.performed += i => lockOn_input = true;
                inputActions.PlayerMovement.LockOnTargetRight.performed += i => rightStick_right_input = true;
                inputActions.PlayerMovement.LockOnTargetLeft.performed += i => rightStick_left_input = true;
                inputActions.PlayerActions.YButton.performed += i => y_input = true;
                inputActions.PlayerActions.CriticalAttack.performed += i => criticalAttack_input = true;

            }

            inputActions.Enable();
        }

        public void OnDisable()
        {
            inputActions.Disable();
        }

        /// <summary>
        /// Called each Unity game tick. Reads input stream
        /// </summary>
        /// <param name="delta">The current time that has passed</param>
        public void TickInput(float delta)
        {
            HandleMoveInput(delta);
            HandleRollingInput(delta);
            HandleAttackInput(delta);
            HandleQuickSlotInput();
            HandleInventoryInput();
            HandleLockOnInput();
            HandleTwoHandInput();
            HandleCriticalAttackInput();
        }

        private void HandleRollingInput(float delta)
        {
            shit_input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Started;
            sprintFlag = shit_input;

            if (shit_input)
            {
                rollInputTimer += delta;
            }
            else
            {
                if (rollInputTimer > 0 && rollInputTimer < 0.5f)
                {
                    sprintFlag = false;
                    rollFlag = true;
                }

                rollInputTimer = 0;
            }
        }

        public void HandleMoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        public void HandleAttackInput(float delta)
        {

            if (!playerManager.CanAttack())
            {
                return;
            }

            if (leftClick_input)
            {
                playerAttacker.HandleleftClickAction();

            }

            if (rightClick_input)
            {
                playerAttacker.HandleHeavyAttach(playerInventory.rightWeapon);
            }

        }

        private void HandleQuickSlotInput()
        {
            if (dPadRight)
            {
                playerInventory.ChangeRightWeapon();
            }

            if (dPadLeft)
            {
                playerInventory.ChangeLeftWeapon();
            }
        }

        private void HandleInventoryInput()
        {
            if (inventory_input)
            {
                inventoryFlag = !inventoryFlag;
                if (inventoryFlag)
                {
                    uiManager.OpenSelectWindow();
                    uiManager.UpdateUI();
                    uiManager.SetActiveHUDWindow(!inventoryFlag);
                }
                else
                {
                    uiManager.CloseSelectWindow();
                    uiManager.CloseAllInventoryWindows();
                    uiManager.SetActiveHUDWindow(!inventoryFlag);
                }
            }

        }

        private void HandleLockOnInput()
        {
            if (lockOn_input && !lockOnFlag)
            {
                lockOn_input = false;
                cameraHandler.HandleLockOn();
                if (cameraHandler.nearestLockOnTarget != null)
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                    lockOnFlag = true;
                }


            }
            else if (lockOn_input && lockOnFlag)
            {
                lockOnFlag = false;
                lockOn_input = false;
                cameraHandler.ClearLockOnTarget();
            }

            if (lockOnFlag && rightStick_left_input)
            {
                rightStick_left_input = false;
                cameraHandler.HandleLockOn();
                if (cameraHandler.leftLockTarget != null)
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.leftLockTarget;
                }
            }

            if (lockOnFlag && rightStick_right_input)
            {
                rightStick_right_input = false;
                cameraHandler.HandleLockOn();
                if (cameraHandler.rightLockTarget != null)
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.rightLockTarget;
                }
            }

            cameraHandler.SetCameraHeight();
        }

        private void HandleTwoHandInput()
        {
            if (y_input)
            {
                y_input = false;
                twoHandFlag = !twoHandFlag;
                if (twoHandFlag)
                {
                    // Enable two handing
                    weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
                }
                else
                {
                    // disable two handing
                    weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
                    weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftWeapon, true);
                }
            }
        }

        private void HandleCriticalAttackInput()
        {
            if (criticalAttack_input)
            {
                criticalAttack_input = false;
                playerAttacker.AttemptBackStabOrRiposte();
            }
        }

    }
}