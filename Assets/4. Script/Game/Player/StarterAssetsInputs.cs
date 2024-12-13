using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool sprint;
        public bool aim;
        public bool shoot;
        public bool reload;
        public bool hide;
        public bool crouch;
        public bool hideRegulate;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

        private void Start()
        {
            hideRegulate = true;
        }

#if ENABLE_INPUT_SYSTEM
        public bool IsDefaultState()
        {
            return move == Vector2.zero &&
                   !aim &&
                   !crouch;
        }
        public void OnMove(InputValue value)
        {
            if (cursorInputForLook) MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook) LookInput(value.Get<Vector2>());
        }

        public void OnJump(InputValue value)
        {
            if (cursorInputForLook) JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            if (cursorInputForLook) SprintInput(value.isPressed);
        }

        public void OnAim(InputValue value)
        {
            if (cursorInputForLook) AimInput(value.isPressed);
        }

        public void OnShoot(InputValue value)
        {
            ShootInput(value.isPressed);
        }

        public void OnReload(InputValue value)
        {
            ReloadInput(value.isPressed);
        }

        public void OnHide(InputValue value)
        {
            if (hideRegulate) 
            {
                HideInput(value.isPressed);
            }
        }

        public void OnCrouch(InputValue value)
        {
            if (cursorInputForLook) CrouchInput(value.isPressed);
        }
#endif
        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }
        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }
        public void AimInput(bool newAimState)
        {
            aim = newAimState;
        }
        public void ShootInput(bool newShootState)
        {
            shoot = newShootState;
        }
        public void ReloadInput(bool newReloadState)
        {
            reload = newReloadState;
        }
        public void HideInput(bool newHideState)
        {
            hide = newHideState;
        }
        public void CrouchInput(bool newHideState)
        {
            crouch = !crouch;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

}