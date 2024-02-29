using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AVE
{
    public class CameraHandler : MonoBehaviour
    {
        public Transform targetTransform,
        cameraTransform,
        cameraPivotTransform;

        InputHandler inputHandler;
        PlayerManager playerManager;

        public float lookSpeed = 0.1f,
                     followSpeed = 0.1f,
                     pivotSpeed = 0.1f,
                     minimumPivot = -35,
                     maximumPivot = 35,
                     cameraSphereRadius = 0.2f,
                     cameraCollisionOffset = 0.2f,
                     minimumCollisionOffset = 0.2f,
                     maximumLockOnDistance = 30,
                     distanceFromLeftTarget = 30,
                     lockedPivotPosition = 2.25f,
                     unlockedPivotPosition = 1.65f;

        private float _defualtPosition,
                      _lookAngle,
                      _pivotAngle,
                      _targetPosition,
                      _LockOnViewAngle;

        private Transform _myTransform;
        private Vector3 _cameraTransformPosition;
        private Vector3 _cameraFollowVelocity = Vector3.zero;

        public Transform currentLockOnTarget;
        public Transform leftLockTarget;
        public Transform rightLockTarget;
        public Transform nearestLockOnTarget;

        private List<CharacterManager> availableTargets = new List<CharacterManager>();

        public LayerMask ignoreLayers;
        public LayerMask environmentLayer;

        public static CameraHandler singleton;

        private void Awake()
        {
            singleton = this;
            _myTransform = transform;
            _defualtPosition = cameraTransform.localPosition.z;
            ignoreLayers = ~(1 << 7 | 1 << 9 | 1 << 11);
            inputHandler = FindObjectOfType<InputHandler>();
            playerManager = FindObjectOfType<PlayerManager>();
        }

        private void Start()
        {
            environmentLayer.value = 8;
        }

        public void FollowTarget(float delta)
        {
            Vector3 targetPos = Vector3.SmoothDamp(_myTransform.position, targetTransform.position, ref _cameraFollowVelocity, delta / followSpeed);
            _myTransform.position = targetPos;
            HandleCameraCollisions(delta);
        }

        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            if (!inputHandler.lockOnFlag && currentLockOnTarget == null)
            {
                _lookAngle += (mouseXInput * lookSpeed) / delta;
                _pivotAngle -= (mouseYInput * pivotSpeed) / delta;
                _pivotAngle = Mathf.Clamp(_pivotAngle, minimumPivot, maximumPivot);

                Vector3 rotation = Vector3.zero;
                rotation.y = _lookAngle;
                Quaternion targetRotation = Quaternion.Euler(rotation);
                _myTransform.rotation = targetRotation;

                rotation = Vector3.zero;
                rotation.x = _pivotAngle;

                targetRotation = Quaternion.Euler(rotation);
                cameraPivotTransform.localRotation = targetRotation;
            }
            else
            {
                // float velocity = 0; <- for later date
                Vector3 dir = currentLockOnTarget.position - transform.position;
                dir.Normalize();
                dir.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = targetRotation;

                dir = currentLockOnTarget.position - cameraPivotTransform.position;
                dir.Normalize();

                targetRotation = Quaternion.LookRotation(dir);
                Vector3 eulerAngle = targetRotation.eulerAngles;
                eulerAngle.y = 0;
                cameraPivotTransform.localEulerAngles = eulerAngle;
            }
        }

        /// <summary>
        /// Create sphere cast around the camera holder to check for collisions and move the
        /// camera to avoid clipping with walls/terrain
        /// </summary>
        /// <param name="delta"></param>
        private void HandleCameraCollisions(float delta)
        {
            _targetPosition = _defualtPosition;
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();

            if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(_targetPosition), ignoreLayers))
            {
                float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
                _targetPosition = -(dis - cameraCollisionOffset);
            }

            if (Mathf.Abs(_targetPosition) < minimumCollisionOffset)
            {
                _targetPosition = -minimumCollisionOffset;
            }

            _cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, _targetPosition, delta / 0.2f);
            cameraTransform.localPosition = _cameraTransformPosition;

        }

        public void HandleLockOn()
        {
            float shortestDistance = Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager character = colliders[i].GetComponent<CharacterManager>();
                if (character != null)
                {
                    Vector3 lockTargetDirection = character.transform.position - targetTransform.position;
                    float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);
                    float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);
                    RaycastHit hit;

                    // Dont lock on to self or anything out of bounds
                    if (character.transform.root != targetTransform.transform.root
                    && viewableAngle > -50 && viewableAngle < 50
                    && distanceFromTarget <= maximumLockOnDistance)
                    {
                        if (Physics.Linecast(playerManager.lockOnTransform.position, character.lockOnTransform.position, out hit))
                        {
                            Debug.DrawLine(playerManager.lockOnTransform.position, character.lockOnTransform.position);

                            if (hit.transform.gameObject.layer == environmentLayer)
                            {
                                // Do not lock on
                            }
                            else
                            {
                                availableTargets.Add(character);
                            }
                        }

                    }
                }
            }

            for (int i = 0; i < availableTargets.Count; i++)
            {
                float distanceFromTarget = Vector3.Distance(targetTransform.position, availableTargets[i].transform.position);
                float shortestDistanceofLeftTarget = Mathf.Infinity;
                float shortestDistanceOfRightTarget = Mathf.Infinity;

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[i].lockOnTransform;
                }

                if (inputHandler.lockOnFlag)
                {
                    Vector3 relativeEnemyPosition = currentLockOnTarget.InverseTransformPoint(availableTargets[i].transform.position);
                    var distanceFromLeftTarget = currentLockOnTarget.transform.position.x - availableTargets[i].transform.position.x;
                    var distanceFromRightTarget = currentLockOnTarget.transform.position.x + availableTargets[i].transform.position.x;

                    if (relativeEnemyPosition.x > 0.00 && distanceFromLeftTarget < shortestDistanceofLeftTarget)
                    {
                        shortestDistanceofLeftTarget = distanceFromLeftTarget;
                        leftLockTarget = availableTargets[i].lockOnTransform;
                    }

                    if (relativeEnemyPosition.x < 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                    {
                        shortestDistanceOfRightTarget = distanceFromRightTarget;
                        rightLockTarget = availableTargets[i].lockOnTransform;
                    }
                }
            }
        }

        public void ClearLockOnTarget()
        {
            availableTargets.Clear();
            nearestLockOnTarget = null;
            currentLockOnTarget = null;
        }

        public void SetCameraHeight()
        {
            Vector3 velocity = Vector3.zero;
            Vector3 newLockedPosition = new Vector3(0, lockedPivotPosition);
            Vector3 newUnlockedPosition = new Vector3(0, unlockedPivotPosition);

            if (currentLockOnTarget != null)
            {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.localPosition, newLockedPosition, ref velocity, Time.deltaTime);
            }
            else
            {
                cameraPivotTransform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.localPosition, newUnlockedPosition, ref velocity, Time.deltaTime);
            }
        }
    }
}