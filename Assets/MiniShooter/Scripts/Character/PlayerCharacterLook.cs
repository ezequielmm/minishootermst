using Cinemachine;
using Mirror;
using System;
using UnityEngine;

namespace MiniShooter
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerCharacterInput))]
    public class PlayerCharacterLook : NetworkEntityBehaviour
    {
        #region INSPECTOR

        [Header("Base Components"), SerializeField]
        protected PlayerCharacterInput inputController;
        [SerializeField]
        protected PlayerCharacterMovement movementController;
        [SerializeField]
        private CinemachineVirtualCamera virtualCameraPrefab;

        [Header("Base Settings"), SerializeField]
        protected bool resetCameraAfterDestroy = true;

        [Header("Base Rotation Settings"), SerializeField, Range(1f, 5f)]
        protected float rotationSencitivity = 2f;

        [Header("Base Collision Settings"), SerializeField, Range(1f, 15f)]
        protected float collisionDstanceSmoothTime = 5f;
        [SerializeField]
        protected bool useCollisionDetection = true;
        [SerializeField]
        protected LayerMask collisionLayer;

        [Header("TD Look Settings"), SerializeField]
        private Vector3 lookAtPoint = Vector3.zero;
        [SerializeField, Range(3f, 10f)]
        private float followSmoothTime = 5f;

        [Header("TD Distance Settings"), SerializeField, Range(5f, 100f)]
        private float minDistance = 5f;
        [SerializeField, Range(5f, 100f)]
        private float maxDistance = 15f;
        [SerializeField, Range(5f, 100f)]
        private float startDistance = 15f;
        [SerializeField, Range(1f, 15f)]
        private float distanceSmoothTime = 5f;
        [SerializeField, Range(0.01f, 1f)]
        private float distanceScrollPower = 1f;
        [SerializeField]
        private bool applyOffsetDistance = true;
        [SerializeField, Range(1f, 25f)]
        private float maxOffsetDistance = 5f;

        [Header("TD Rotation Settings"), SerializeField, Range(5f, 90f)]
        private float pitchAngle = 65f;

        #endregion

        private CinemachineVirtualCamera virtualCamera;
        private Cinemachine3rdPersonFollow cinemachine3RdPersonFollow;
        private GameObject cameraTarget;

        private float defauldMinDistance = 0f;
        private float defauldMaxDistance = 0f;
        private float defauldStartDistance = 0f;

        private float currentCameraDistance = 0f;
        private float currentCameraYawAngle = 0f;

        [SyncVar]
        protected bool lookIsAllowed = true;

        /// <summary>
        /// The starting parent of the camera. It is necessary to return the camera to its original place after the destruction of the current object
        /// </summary>
        protected Transform initialCameraParent;

        /// <summary>
        /// The starting position of the camera. It is necessary to return the camera to its original place after the destruction of the current object
        /// </summary>
        protected Vector3 initialCameraPosition;

        /// <summary>
        /// The starting rotation of the camera. It is necessaryto return the camera to its original angle after the destruction of the current object
        /// </summary>
        protected Quaternion initialCameraRotation;

        /// <summary>
        /// Check if camera is collided with something
        /// </summary>
        protected bool isCameraCollided = false;

        /// <summary>
        /// Check if camera is collided with something
        /// </summary>
        protected float cameraCollisionDistance = 0f;

        /// <summary>
        /// 
        /// </summary>
        protected Vector3 cameraOffsetPosition = Vector3.zero;

        /// <summary>
        /// 
        /// </summary>
        public float CurrentDistance => currentCameraDistance;

        /// <summary>
        /// 
        /// </summary>
        public float MaxDistance => maxDistance;

        public override bool IsReady => base.IsReady
            && inputController
            && movementController
            && cameraTarget
            && virtualCamera
            && cinemachine3RdPersonFollow
            && isClient;

        protected override void Awake()
        {
            base.Awake();

            defauldMinDistance = minDistance;
            defauldMaxDistance = maxDistance;
            defauldStartDistance = startDistance;
        }

        protected virtual void LateUpdate()
        {
            if (isOwned && IsReady)
            {
                UpdateTargetPosition();
                UpdateCameraDistance();
                UpdateCameraOffset();
                UpdateCameraRotation();
            }
        }

        private void UpdateTargetPosition()
        {
            cameraTarget.transform.position = transform.position;
        }

        #region CLIENT

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            RecalculateDistance();
            CreateCameraControls();
        }

        public override void OnStopAuthority()
        {
            base.OnStopAuthority();

            if (virtualCamera)
            {
                Destroy(virtualCamera.gameObject);
            }

            if (cameraTarget)
            {
                Destroy(cameraTarget);
            }
        }

        /// <summary>
        /// Create camera control elements
        /// </summary>
        protected virtual void CreateCameraControls()
        {
            cameraTarget = new GameObject("--FOLLOW_CAMERA_TARGET");

            if (!virtualCamera)
                virtualCamera = Instantiate(virtualCameraPrefab);

            virtualCamera.name = "--CHARACTER_FOLLOW_CAMERA";
            virtualCamera.Follow = cameraTarget.transform;
            cinemachine3RdPersonFollow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

            cameraOffsetPosition = transform.position;
        }

        /// <summary>
        /// Just calculates the distance. If max distance less than min
        /// </summary>
        protected void RecalculateDistance()
        {
            if (minDistance < 0f)
                minDistance = 0f;

            if (minDistance >= maxDistance)
                maxDistance = minDistance + 0.01f;

            currentCameraDistance = Mathf.Clamp(startDistance, minDistance, maxDistance);
        }

        /// <summary>
        /// Updates camera offset in front of character
        /// </summary>
        protected void UpdateCameraOffset()
        {
            if (applyOffsetDistance && !isCameraCollided)
            {
                cameraOffsetPosition = (transform.forward * maxOffsetDistance + transform.position) + lookAtPoint;
            }
            else
            {
                cameraOffsetPosition = transform.position + lookAtPoint;
            }
        }

        /// <summary>
        /// Updates camera rotation
        /// </summary>
        protected virtual void UpdateCameraRotation()
        {
            currentCameraYawAngle += inputController.Look() * rotationSencitivity * 100f * Time.deltaTime;
            cameraTarget.transform.rotation = Quaternion.Euler(pitchAngle, currentCameraYawAngle, 0f);
        }

        /// <summary>
        /// Updates distance between camera and character
        /// </summary>
        protected virtual void UpdateCameraDistance()
        {
            currentCameraDistance = Mathf.Clamp(currentCameraDistance + inputController.Scroll() * distanceScrollPower, minDistance, maxDistance);
            cinemachine3RdPersonFollow.CameraDistance = currentCameraDistance;
        }

        /// <summary>
        /// Gets camera root rotation angle in <see cref="Quaternion"/>
        /// </summary>
        /// <returns></returns>
        public virtual Quaternion GetCameraRotation()
        {
            return Quaternion.Euler(0f, currentCameraYawAngle, 0f);
        }

        /// <summary>
        /// Direction to the point at what the character is looking in armed mode
        /// </summary>
        /// <returns></returns>
        public virtual Vector3 AimDirection()
        {
            if (inputController.MouseToWorldHitPoint(out RaycastHit hit))
            {
                return hit.point - transform.position;
            }
            else
            {
                return Vector3.forward;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetDistanceToDefault()
        {
            minDistance = defauldMinDistance;
            maxDistance = defauldMaxDistance;
            startDistance = defauldStartDistance;

            RecalculateDistance();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minDist"></param>
        /// <param name="maxDist"></param>
        /// <param name="startDist"></param>
        public void SetDistance(float minDist, float maxDist, float startDist)
        {
            minDistance = minDist;
            maxDistance = maxDist;
            startDistance = startDist;

            RecalculateDistance();
        }

        #endregion
    }
}