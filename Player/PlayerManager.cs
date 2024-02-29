using System.Collections;
using System.Collections.Generic;
using AVE;
using UnityEngine;

namespace AVE
{
    /// <summary>
    /// Handles almost, if not all update methods.
    /// Connects to all other functionality
    /// </summary>
    public class PlayerManager : CharacterManager
    {

        private InputHandler inputHandler;
        private CameraHandler cameraHandler;
        private Animator anim;
        private PlayerLocomotion playerLocomotion;
        private PlayerStats playerStats;
        private PlayerAnimatorManager _playerAnimatorManager;

        InteractableUI interactableUI;
        [SerializeField] private GameObject interactableUIGameObject;
        [SerializeField] public GameObject itemInteractableGameObject;

        [Header("Player Flags")]
        public bool isInteracting;
        public bool isSprinting;
        public bool isInAir;
        public bool isGrounded;
        public bool canDoCombo;
        public bool isUsingRightHand;
        public bool isUsingLeftHand;
        public bool isInvulnerable;


        private void Awake()
        {
            backStabCollider = GetComponentInChildren<BackStabCollider>();
            cameraHandler = CameraHandler.singleton;
            inputHandler = GetComponent<InputHandler>();
            anim = GetComponentInChildren<Animator>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            interactableUI = FindObjectOfType<InteractableUI>();
            playerStats = GetComponent<PlayerStats>();
            _playerAnimatorManager = GetComponentInChildren<PlayerAnimatorManager>();

        }
        
        private void Update()
        {
            float delta = Time.deltaTime;

            isInteracting = anim.GetBool("isInteracting");
            canDoCombo = anim.GetBool("canDoCombo");
            isUsingRightHand = anim.GetBool("isUsingRightHand");
            isUsingLeftHand = anim.GetBool("isUsingLeftHand");
            isInvulnerable = anim.GetBool("isInvulnerable");
            anim.SetBool("isInAir", isInAir);
            anim.SetBool("isDead", playerStats.isDead);
            

            inputHandler.TickInput(delta);
            _playerAnimatorManager.canRotate = anim.GetBool("canRotate");
            playerLocomotion.HandleRollingAndSprinting(delta);
            playerLocomotion.HandleJumping();
            playerStats.RegenerateStamina();

            CheckForInteractableObject();
        }

        private void FixedUpdate()
        {
            float delta = Time.deltaTime;
            playerLocomotion.HandleFalling(delta, playerLocomotion.movedirection);
            playerLocomotion.HandleMovement(delta);
            playerLocomotion.HandleRotation(delta);


        }

        // Handle flags w/ LateUpdate
        private void LateUpdate()
        {
            ResetInputsEndOfFrame();

            float delta = Time.fixedDeltaTime;

            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }

            if (isInAir)
            {
                playerLocomotion.inAirTimer += Time.deltaTime;
            }
        }



        private void ResetInputsEndOfFrame()
        {
            inputHandler.rollFlag = false;
            inputHandler.leftClick_input = false;
            inputHandler.rightClick_input = false;
            inputHandler.dPadUp = false;
            inputHandler.dPadRight = false;
            inputHandler.dPadDown = false;
            inputHandler.dPadLeft = false;
            inputHandler.e_input = false;
            inputHandler.jump_input = false;
            inputHandler.inventory_input = false;
        }

        public void CheckForInteractableObject()
        {

            if (Physics.SphereCast(transform.position, 0.3f, transform.forward, out RaycastHit hit, 1f, cameraHandler.ignoreLayers))
            {
                if (hit.collider.CompareTag("interactable"))
                {
                    Interactable interactableObject = hit.collider.GetComponent<Interactable>();

                    if (interactableObject != null)
                    {
                        string interactableText = interactableObject.interactableText;
                        interactableUI.interactableText.text = interactableText;
                        interactableUIGameObject.SetActive(true);

                        if (inputHandler.e_input)
                        {
                            hit.collider.GetComponent<Interactable>().Interact(this);
                        }
                    }

                }
            }
            else
            {
                if (interactableUIGameObject != null)
                {
                    interactableUIGameObject.SetActive(false);
                }

                if (itemInteractableGameObject != null && inputHandler.e_input)
                {
                    itemInteractableGameObject.SetActive(false);
                }
            }
        }


        public bool CanAttack()
        {
            return playerStats.GetCurrentStamina() > 0;
        }


    }
}