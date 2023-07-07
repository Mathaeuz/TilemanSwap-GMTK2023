using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerBindings Bindings;
    public RoleObject Behaviour;
    public Rigidbody2D Body;
    PlayerBindings.Input UserInput;

    [Flags]
    public enum StateFlags
    {
        None,
        Ball = 1,
        Ground = 2,
        LeftWall = 4,
        RightWall = 8,
        Ceil = 16,
    };
    StateFlags LastState;
    public StateFlags State;
    public Action<StateFlags> OnStateChange;

    public Collider2D Legs;
    public float MaxSpeed = 7f;
    public float Acceleration = 14f;
    public float Decceleration = 7f;
    public float JumpPower = 8f;

    public Vector2 Velocity;

    private void Awake()
    {
        Behaviour = GetComponent<RoleObject>();
        Body = GetComponent<Rigidbody2D>();
        UserInput = Bindings.Init();
    }

    private void FixedUpdate()
    {
        Velocity = Body.velocity;

        if (State.HasFlag(StateFlags.Ball))
        {
            BallState();
        }
        else
        {
            RegularState();
        }

        HandleStateChange();
        UserInput.Reset();
    }

    private void HandleStateChange()
    {
        if (State != LastState)
        {
            OnStateChange?.Invoke(State);
        }
        LastState = State;
        State &= StateFlags.Ball;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            DigestNormal(collision.contacts[i].normal);
        }
    }


    private void RegularState()
    {
        if (State.HasFlag(StateFlags.Ground) | State.HasFlag(StateFlags.Ceil))
        {
            Velocity.y = 0;
        }


        if (Mathf.Abs(UserInput.Horizontal) < 0.3f)
        {
            Velocity.x = Mathf.Max(0, Math.Abs(Velocity.x) - Decceleration * Time.fixedDeltaTime) * Mathf.Sign(Velocity.x);
        }
        else
        {
            var hor = (UserInput.Horizontal - 0.3f) / (1 - 0.3f);
            var maxSpeed = Mathf.Max(MaxSpeed, Mathf.Abs(Velocity.x));
            Velocity.x = Mathf.Max(-maxSpeed, Mathf.Min(maxSpeed, Velocity.x + Acceleration * hor * Time.fixedDeltaTime));
        }

        if (State.HasFlag(StateFlags.Ground))
        {
            if (UserInput.Jump.Down)
            {
                Velocity.y = JumpPower;
            }
        }
        else
        {
            if (Velocity.y > 0 && !UserInput.Jump.Hold)
            {
                //break jump
                Velocity.y *= 0.4f;
            }
            else if (Velocity.y <= 0 && UserInput.Jump.Hold)
            {
                State |= StateFlags.Ball;
                Legs.enabled = false;
            }
        }
        Body.velocity = Velocity;
    }

    private void BallState()
    {
        if (!UserInput.Jump.Hold)
        {
            State ^= StateFlags.Ball;
            Legs.enabled = true;
        }
    }

    private void DigestNormal(Vector2 normal)
    {
        if (normal.y > 0.7f && Velocity.y <= 0)
        {
            State |= StateFlags.Ground;
        }
        else if (normal.y < -0.7f && Velocity.y > 0)
        {
            State |= StateFlags.Ceil;
        }
        else
        {
            if (normal.x > 0.7f)
            {
                State |= StateFlags.LeftWall;
            }
            else if (normal.x < -0.7f)
            {
                State |= StateFlags.RightWall;
            }
        }
    }
}
