using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Player Bindings")]
public class PlayerBindings : ScriptableObject
{
    public class Button
    {
        public bool Down { get; private set; }
        public bool Up { get; private set; }
        public bool Hold { get; private set; }
        public void Set()
        {
            Down = true;
            Hold = true;
        }
        public void Unset()
        {
            Up = true;
            Hold = false;
        }
        public void Reset()
        {
            Down = false;
            Up = false;
        }
    }

    public class Input
    {
        public float Horizontal;
        public float Vertical;
        public Vector2 MousePosition;
        public Button Jump = new(), Swap = new(), Crouch = new();
        bool Locked;

        public void Reset()
        {
            Jump.Reset();
            Swap.Reset();
            Crouch.Reset();
        }

        public void Lock()
        {
            Locked = true;
            Jump.Unset();
            Jump.Reset();
            Swap.Unset();
            Swap.Reset();
            Crouch.Unset();
            Crouch.Reset();
            Horizontal = 0;
            Vertical = 0;
        }

        public void Unlock()
        {
            Locked = false;
        }

        public void MovementPerformed(InputAction.CallbackContext obj)
        {
            if (Locked)
            {
                return;
            }
            var value = obj.ReadValue<Vector2>();
            Horizontal = value.x;
            Vertical = value.y;
        }
        public void MousePerformed(InputAction.CallbackContext obj)
        {
            //if (Locked)
            //{
            //    return;
            //}

            MousePosition = obj.ReadValue<Vector2>();
        }

        public void JumpPerformed(InputAction.CallbackContext obj)
        {
            if (Locked)
            {
                return;
            }
            if (obj.ReadValueAsButton())
            {
                Jump.Set();
            }
            else
            {
                Jump.Unset();
            }
        }

        public void SwapPerformed(InputAction.CallbackContext obj)
        {
            if (Locked)
            {
                return;
            }
            if (obj.ReadValueAsButton())
            {
                Swap.Set();
            }
            else
            {
                Swap.Unset();
            }
        }

        internal void CrouchPerformed(InputAction.CallbackContext obj)
        {
            if (Locked)
            {
                return;
            }
            if (obj.ReadValueAsButton())
            {
                Crouch.Set();
            }
            else
            {
                Crouch.Unset();
            }
        }
    }
    public InputActionReference Movement, Jump, Swap, Crouch, MousePosition;

    public Input Init()
    {
        var output = new Input();
        if (Movement != null)
        {
            Movement.action.Enable();
            Movement.action.performed += output.MovementPerformed;
            Movement.action.canceled += output.MovementPerformed;
        }
        if (Jump != null)
        {
            Jump.action.Enable();
            Jump.action.performed += output.JumpPerformed;
            Jump.action.canceled += output.JumpPerformed;
        };
        if (Swap != null)
        {
            Swap.action.Enable();
            Swap.action.performed += output.SwapPerformed;
            Swap.action.canceled += output.SwapPerformed;
        };
        if (MousePosition != null)
        {
            MousePosition.action.Enable();
            MousePosition.action.performed += output.MousePerformed;
            MousePosition.action.canceled += output.MousePerformed;
        };
        if (Crouch != null)
        {
            Crouch.action.Enable();
            Crouch.action.performed += output.CrouchPerformed;
            Crouch.action.canceled += output.CrouchPerformed;
        };
        return output;
    }
}