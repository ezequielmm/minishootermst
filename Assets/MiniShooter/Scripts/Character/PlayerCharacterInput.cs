using System;
using UnityEngine;

namespace MiniShooter
{
    public class PlayerCharacterInput : NetworkEntityBehaviour
    {
        private OnlinePlayerInput playerInput;

        public KeyCode[] NumberKeys { get; protected set; }

#if ENABLE_INPUT_SYSTEM
        public override bool IsReady => playerInput && playerInput.Actions != null;
#endif
        protected Vector2 move;
        protected float look;
        protected float zoom;
        protected bool jump;
        protected bool sprint;
        protected bool arm;
        protected bool reload;
        protected bool fire;

        protected override void Awake()
        {
            base.Awake();

            NumberKeys = new KeyCode[9];

            for (int i = 0; i < NumberKeys.Length; i++)
            {
                if (Enum.TryParse($"Alpha{i + 1}", true, out KeyCode result))
                    NumberKeys[i] = result;
            }
        }

        protected virtual void Update()
        {
            if (isOwned && IsReady)
            {
#if ENABLE_INPUT_SYSTEM
                LookInput(playerInput.Actions.Player.Look.ReadValue<float>());
                ZoomInput(playerInput.Actions.Player.ZoomInOut.ReadValue<float>());
                MoveInput(playerInput.Actions.Player.Move.ReadValue<Vector2>());
                JumpInput(playerInput.Actions.Player.Jump.IsPressed());
                SprintInput(playerInput.Actions.Player.Sprint.IsPressed());
                ArmInput(playerInput.Actions.Player.Arm.IsPressed());
                ReloadInput(playerInput.Actions.Player.Reload.IsPressed());
                FireInput(playerInput.Actions.Player.Fire.IsPressed());
#endif
            }
        }

        #region CLIENT

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            if (isOwned)
            {
                playerInput = OnlinePlayer.Local.Input;
                playerInput.EnablePlayerInput();
            }
        }

        private void FireInput(bool fireState)
        {
            fire = fireState;
        }

        private void ReloadInput(bool reloadState)
        {
            reload = reloadState;
        }

        private void ZoomInput(float newZoomMagnitude)
        {
            zoom = newZoomMagnitude;
        }

        private void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        private void LookInput(float newLookAxis)
        {
            look = newLookAxis;
        }

        private void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        private void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }

        private void ArmInput(bool newArmState)
        {
            arm = newArmState;
        }

        public virtual float Horizontal()
        {
            return move.x;
        }

        public virtual float Vertical()
        {
            return move.y;
        }

        public virtual float Look()
        {
            return look;
        }

        public float Scroll()
        {
            return zoom;
        }

        public virtual Vector3 MovementDirection()
        {
            return new Vector3(Horizontal(), 0.0f, Vertical()).normalized;
        }

        public virtual float MovementMagnitude()
        {
            return MovementDirection().magnitude;
        }

        public virtual bool IsMoving()
        {
            return MovementMagnitude() > 0f;
        }

        public virtual bool Armed()
        {
            return arm;
        }

        public virtual bool Reload()
        {
            return reload;
        }

        public virtual bool Fire()
        {
            return fire;
        }

        public virtual bool Jump()
        {
            return jump;
        }

        public virtual bool IsRunnning()
        {
            return sprint && IsMoving();
        }

        public bool MouseToWorldHitPoint(out RaycastHit hit, float maxCheckDistance = Mathf.Infinity)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out hit, maxCheckDistance);
        }

        #endregion
    }
}