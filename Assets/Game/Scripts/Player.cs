﻿using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector3 SpawnPosition;
    public PlayerCheckpoint Checkpoint;

    public PlayerBindings Bindings;
    public RoleObject RoleObject;
    public Rigidbody2D Body;
    PlayerBindings.Input UserInput;

    /// <summary>
    /// reusing the flicker for death;
    /// </summary>
    BaloonFlicker BaloonFlicker;


    [Flags]
    public enum StateFlags
    {
        None,
        Ball = 1,
        Ground = 2,
        LeftWall = 4,
        RightWall = 8,
        Ceil = 16,
        Dead = 32,
        Selecting = 64,
        Preserve = Ball | Dead | Selecting,
    };
    public bool Jumped { get; private set; }
    public bool SwapAccepted { get; private set; }
    StateFlags LastState;
    public StateFlags State;
    public Action<StateFlags, StateFlags> OnStateChange;

    public GameObject Legs, Block;
    public float MaxSpeed = 7f;
    public float Acceleration = 14f;
    public float Decceleration = 7f;
    public float JumpHeight = 4f;
    public float RespawnEarly = 0.5f;
    public float MaxVelocity = 48f;
    float JumpPower;

    public Vector2 LastVelocity, Velocity;

    private void Awake()
    {
        RoleObject = GetComponent<RoleObject>();
        Body = GetComponent<Rigidbody2D>();
        BaloonFlicker = GetComponent<BaloonFlicker>();
        RoleObject.RoleDestroyed.AddListener(ExternalDeath);
        RoleObject.RoleRestored.AddListener(ExternalRespawn);
        UserInput = Bindings.Init();
        Legs.SetActive(true);
        Block.SetActive(false);

        JumpPower = Mathf.Sqrt(-2 * Body.gravityScale * Physics2D.gravity.y * JumpHeight);
    }

    private void Death()
    {
        RoleObject.RoleDestroyed.Invoke(3f);
    }

    private void ExternalDeath(float time)
    {
        if (State.HasFlag(StateFlags.Dead))
        {
            return;
        }
        State |= StateFlags.Dead;

        if (State.HasFlag(StateFlags.Selecting))
        {
            State ^= StateFlags.Selecting;
            SwapSelector.Instance.EndSelection();
        }

        Velocity = Vector3.zero;
        Body.simulated = false;
        Body.velocity = Vector3.zero;
        Legs.SetActive(false);
        Block.SetActive(false);
        UserInput.Lock();
        Invoke(nameof(ExternalRespawn), time - RespawnEarly);
    }

    private void ExternalRespawn()
    {
        if (!State.HasFlag(StateFlags.Dead))
        {
            return;
        }
        State = StateFlags.None;
        Legs.SetActive(true);
        Block.SetActive(false);
        Body.simulated = true;

        if (Checkpoint == null)
        {
            Body.position = SpawnPosition;
        }
        else
        {
            Checkpoint.Return();
            Body.position = Checkpoint.transform.position;
        }
        UserInput.Unlock();
    }

    private void FixedUpdate()
    {
        LastVelocity = Velocity;
        Velocity = Body.velocity;
        if (State.HasFlag(StateFlags.Ball))
        {
            BallState();
        }
        else
        {
            RegularState();
        }
        HandleVelocity();
        TrySelect();

        HandleStateChange();
        UserInput.Reset();
    }

    private void HandleVelocity()
    {
        if (MaxVelocity * MaxVelocity < Velocity.sqrMagnitude)
        {
            Velocity = Velocity.normalized * MaxVelocity;
        }
        Body.velocity = Velocity;
    }

    private void TrySelect()
    {
        if (!State.HasFlag(StateFlags.Selecting) && UserInput.Swap.Down && SwapSelector.Instance.Off)
        {
            BeginSelection();
        }
    }

    public void BeginSelection()
    {
        SwapAccepted = false;
        State |= StateFlags.Selecting;
        SwapSelector.Instance.BeginSelection(this);
    }

    public void SelectionDone()
    {
        State ^= StateFlags.Selecting;
    }

    private void HandleStateChange()
    {
        if (State != LastState)
        {
            OnStateChange?.Invoke(LastState, State);
        }
        LastState = State;
        State &= StateFlags.Preserve;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("KillPlane"))
        {
            Death();
            return;
        }

        for (int i = 0; i < collision.contacts.Length; i++)
        {
            DigestNormal(collision.contacts[i].normal);
        }
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
        var shouldBall = UserInput.Crouch.Hold;

        //Rettach
        /* if (State.HasFlag(StateFlags.Ground))
        {
            if (LastVelocity.y < 0)
            {
                Body.position += Vector2.up * LastVelocity.y * Time.fixedDeltaTime;
            }
            Velocity.y = 0;
        }
        else*/
        if (State.HasFlag(StateFlags.Ceil) && Velocity.y > 0)
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
            var acc = Acceleration * hor * Time.fixedDeltaTime;
            if (hor * Velocity.x < 0)
            {
                acc *= 2;
            }
            Velocity.x = Mathf.Max(-maxSpeed, Mathf.Min(maxSpeed, Velocity.x + acc));
        }

        if (State.HasFlag(StateFlags.Ground))
        {
            if (shouldBall)
            {
                EnterBallState();
            }
            else if (UserInput.Jump.Down)
            {
                Velocity.y = JumpPower;
                Jumped = true;
            }
        }
        else
        {
            Body.gravityScale = 2;
            if (Velocity.y > 0 && (Jumped && !UserInput.Jump.Hold))
            {
                //break jump
                Body.gravityScale = 4;
            }
            else if (Velocity.y < 0 && (shouldBall || UserInput.Jump.Hold))
            {
                EnterBallState();
            }
        }
    }

    private void EnterBallState()
    {
        State |= StateFlags.Ball;
        Legs.SetActive(false);
        Block.SetActive(true);
    }

    private void ExitBallState()
    {
        State ^= StateFlags.Ball;
        Legs.SetActive(true);
        Block.SetActive(false);
    }

    private void BallState()
    {
        if (State.HasFlag(StateFlags.Ground) && State.HasFlag(StateFlags.Ceil))
        {
            if (Mathf.Abs(Velocity.x) < 0.01)
            {
                Death();
            }
            return;
        }

        if (UserInput.Crouch.Hold || UserInput.Jump.Hold)
        {
            return;
        }
        ExitBallState();
    }


    private void DigestNormal(Vector2 normal)
    {
        if (normal.y > 0.7f && Velocity.y <= 0)
        {
            State |= StateFlags.Ground;
            Jumped = false;
        }
        else if (normal.y < -0.7f && Velocity.y >= 0)
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

    public void SwapWith(Role activeRole)
    {
        SwapAccepted = true;
        RoleManager.Instance.Swap(RoleObject.ActiveRole, activeRole);
    }

    public bool CanSwap(Role activeRole)
    {
        return RoleObject.ActiveRole != activeRole;
    }
}
