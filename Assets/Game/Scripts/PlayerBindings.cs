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
        public Button Jump = new(), Swap = new();

        public void Reset()
        {
            Jump.Reset();
            Swap.Reset();
        }

        internal void HorizontalPerformed(InputAction.CallbackContext obj)
        {
            Horizontal = obj.ReadValue<Vector2>().x;
        }

        internal void JumpPerformed(InputAction.CallbackContext obj)
        {
            if (obj.ReadValueAsButton())
            {
                Jump.Set();
            }
            else
            {
                Jump.Unset();
            }
        }

        internal void SwapPerformed(InputAction.CallbackContext obj)
        {
            if (obj.ReadValueAsButton())
            {
                Swap.Set();
            }
            else
            {
                Swap.Unset();
            }
        }
    }
    public InputActionReference Movement, Jump, Swap;

    public Input Init()
    {
        var output = new Input();
        if (Movement != null)
        {
            Movement.action.Enable();
            Movement.action.performed += output.HorizontalPerformed;
            Movement.action.canceled += output.HorizontalPerformed;
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
        return output;
    }
}