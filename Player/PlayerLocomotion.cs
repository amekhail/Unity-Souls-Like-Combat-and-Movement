using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace AVE
{
    public class PlayerLocomotion : MonoBehaviour
    {
        private CameraHandler cameraHandler;
        private Transform _cameraObject;
        private InputHandler _inputHandler;
        public Vector3 movedirection;
        private PlayerManager _playerManager;

        [HideInInspector] public Transform myTransform;
        [FormerlySerializedAs("animatorHandler")] [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        [Header("Stats")]
        #region Stats
        [SerializeField] private float movementSpeed = 5;
        [SerializeField] private float walkingSpeed = 1;
        [SerializeField] private float rotationSpeed = 10;
        [SerializeField] private float sprintSpeed = 7;
        [SerializeField] private float fallSpeed = 45;
        #endregion

        [Header("Ground & Air Detection stats")]
        #region Ground and Air Detection
        [SerializeField] float groudDetectionRayStartPoint = 0.5f;
        [SerializeField] float minimumDistanceNeededToBeginFall = 1f;
        [SerializeField] float groundDirectionRayDistance = 0.2f;
        LayerMask ignoreForGroundCheck;
        public float inAirTimer;

        public CapsuleCollider characterCollider;
        public CapsuleCollider characterCollisionBlocker;

        #endregion

        private void Awake()
        {
            cameraHandler = FindObjectOfType<CameraHandler>();
        }

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            _inputHandler = GetComponent<InputHandler>();
            _playerManager = GetComponent<PlayerManager>();
            playerAnimatorManager = GetComponentInChildren<PlayerAnimatorManager>();
            _cameraObject = Camera.main.transform;
            myTransform = transform;
            playerAnimatorManager.Initialize();

            _playerManager.isGrounded = true;
            ignoreForGroundCheck = ~(1 << 8 | 1 << 12);
            Physics.IgnoreCollision(characterCollider, characterCollisionBlocker, true);
        }

        #region Movement

        Vector3 _normalVector;
        Vector3 _targetPosition;

        public void HandleRotation(float delta)
        {
            if (playerAnimatorManager.canRotate)
            {
                if (_inputHandler.lockOnFlag)
                { 
                    Quaternion tr;

                if (_inputHandler.sprintFlag || _inputHandler.rollFlag)
                {
                    Vector3 targetDirection = Vector3.zero;
                    targetDirection = cameraHandler.cameraTransform.forward * _inputHandler.vertical;
                    targetDirection += cameraHandler.cameraTransform.right * _inputHandler.horizontal;
                    targetDirection.Normalize();
                    targetDirection.y = 0;

                    if (targetDirection == Vector3.zero)
                    {
                        targetDirection = transform.forward;
                    }

                    tr = Quaternion.LookRotation(targetDirection);

                }
                else
                {
                    Vector3 rotationDirection = movedirection;
                    rotationDirection = cameraHandler.currentLockOnTarget.position - transform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();

                    tr = Quaternion.LookRotation(rotationDirection);
                }

                Quaternion targetRotaion = Quaternion.Slerp(transform.rotation, tr, rotationSpeed * Time.deltaTime);
                transform.rotation = targetRotaion;

                }
                else
                {
                    Vector3 targetDir = Vector3.zero;
                    float moveOverride = _inputHandler.moveAmount;
    
                    targetDir = _cameraObject.forward * _inputHandler.vertical;
                    targetDir += _cameraObject.right * _inputHandler.horizontal;
                    targetDir.Normalize();
                    targetDir.y = 0;
    
                    if (targetDir == Vector3.zero)
                    {
                        targetDir = myTransform.forward;
                    }
    
                    float rs = rotationSpeed;
    
                    Quaternion tr = Quaternion.LookRotation(targetDir);
                    Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);
    
                    myTransform.rotation = targetRotation;
                }
            }
        }

        public void HandleMovement(float delta)
        {
            if (_inputHandler.rollFlag || _playerManager.isInteracting)
            {
                return;
            }

            movedirection = _cameraObject.forward * _inputHandler.vertical;
            movedirection += _cameraObject.right * _inputHandler.horizontal;
            movedirection.Normalize();
            movedirection.y = 0;

            float speed = movementSpeed;

            if (_inputHandler.sprintFlag && _inputHandler.moveAmount > 0.5f)
            {
                speed = sprintSpeed;
                _playerManager.isSprinting = true;
                movedirection *= speed;
            }
            else
            {
                if (_inputHandler.moveAmount < 0.5f)
                {
                    movedirection *= walkingSpeed;
                    _playerManager.isSprinting = false;
                }
                else
                {
                    movedirection *= speed;
                    _playerManager.isSprinting = false;
                }
            }

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(movedirection, _normalVector);
            rigidbody.velocity = projectedVelocity;

            if (_inputHandler.lockOnFlag && !_inputHandler.sprintFlag)
            {
                playerAnimatorManager.UpdateAnimatorValues(_inputHandler.vertical, _inputHandler.horizontal, _playerManager.isSprinting);
            }
            else
            {
                playerAnimatorManager.UpdateAnimatorValues(_inputHandler.moveAmount, 0, _playerManager.isSprinting);
            }
        }

        public void HandleRollingAndSprinting(float delta)
        {
            if (playerAnimatorManager.animator.GetBool("isInteracting"))
            {
                return;
            }

            if (_inputHandler.rollFlag)
            {
                movedirection = _cameraObject.forward * _inputHandler.vertical;
                movedirection += _cameraObject.right * _inputHandler.horizontal;

                if (_inputHandler.moveAmount > 0)
                {
                    playerAnimatorManager.PLayTargetAnimation("Rolling", true);
                    movedirection.y = 0;
                    Quaternion rollRotation = Quaternion.LookRotation(movedirection);
                    myTransform.rotation = rollRotation;
                }
                else
                {
                    playerAnimatorManager.PLayTargetAnimation("Backstep", true);
                }
            }
        }

        public void HandleJumping()
        {
            if (_playerManager.isInteracting)
            {
                return;
            }
            if (_inputHandler.jump_input)
            {
                if (_inputHandler.moveAmount > 0)
                {
                    movedirection = _cameraObject.forward * _inputHandler.vertical;
                    movedirection += _cameraObject.right * _inputHandler.horizontal;

                    // Vector3 currentVelocity = GetComponent<Rigidbody>().velocity;

                    // Vector3 jumpDirection = (movedirection + Vector3.up).normalized;
                    // GetComponent<Rigidbody>().velocity = currentVelocity + jumpDirection * 10;

                    playerAnimatorManager.PLayTargetAnimation("Jump", true);
                    movedirection.y = 0;
                    Quaternion jumpRotation = Quaternion.LookRotation(movedirection);
                    myTransform.rotation = jumpRotation;
                }
            }
        }

        public void HandleFalling(float delta, Vector3 moveDirection)
        {
            _playerManager.isGrounded = false;
            RaycastHit hit;
            Vector3 origin = myTransform.position;
            origin.y = origin.y + groudDetectionRayStartPoint;

            if (Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
            {
                moveDirection = Vector3.zero;
            }

            if (_playerManager.isInAir)
            {
                rigidbody.AddForce(-Vector3.up * fallSpeed);
                rigidbody.AddForce(moveDirection * fallSpeed / 10f);
            }

            Vector3 dir = moveDirection;
            dir.Normalize();
            origin = origin + dir * groundDirectionRayDistance;

            _targetPosition = myTransform.position;

            Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);

            if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
            {
                _normalVector = hit.normal;
                Vector3 targetPos = hit.point;
                _playerManager.isGrounded = true;
                _targetPosition.y = targetPos.y;

                if (_playerManager.isInAir)
                {
                    if (inAirTimer > 0.5f)
                    {
                        playerAnimatorManager.PLayTargetAnimation("Land", true);
                        inAirTimer = 0;
                    }
                    else
                    {
                        playerAnimatorManager.PLayTargetAnimation("Empty", false);
                        inAirTimer = 0;
                    }
                    _playerManager.isInAir = false;
                }
            }
            else
            {
                if (_playerManager.isGrounded)
                {
                    _playerManager.isGrounded = false;
                }
                if (_playerManager.isInAir == false)
                {
                    if (_playerManager.isInteracting == false)
                    {
                        playerAnimatorManager.PLayTargetAnimation("Falling", true);
                    }
                    Vector3 vel = rigidbody.velocity;
                    vel.Normalize();
                    rigidbody.velocity = vel * (movementSpeed / 2);
                    _playerManager.isInAir = true;

                }
            }
            if (_playerManager.isGrounded)
            {
                if (_playerManager.isInteracting || _inputHandler.moveAmount > 0)
                {
                    myTransform.position = Vector3.Lerp(myTransform.position, _targetPosition, Time.deltaTime / 0.1f);
                }
                else
                {
                    myTransform.position = _targetPosition;
                }
            }
        }

        #endregion

    }
}