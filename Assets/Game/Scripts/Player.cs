using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform RespawnAnchor;

    public SwapSense SwapSensor;
    public PlayerBindings Bindings;
    public RoleObject Behaviour;
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
    };
    StateFlags LastState;
    public StateFlags State;
    public Action<StateFlags> OnStateChange;

    public Collider2D Legs;
    public float MaxSpeed = 7f;
    public float Acceleration = 14f;
    public float Decceleration = 7f;
    public float JumpPower = 8f;

    public Vector2 LastVelocity, Velocity;

    List<SwapSense.Option> SwappingList;

    private void Awake()
    {
        Behaviour = GetComponent<RoleObject>();
        Body = GetComponent<Rigidbody2D>();
        BaloonFlicker = GetComponent<BaloonFlicker>();
        if (SwapSensor == null) SwapSensor = GetComponentInChildren<SwapSense>();
        Behaviour.RoleDestroyed.AddListener(SimulateDeath);
        UserInput = Bindings.Init();
    }

    private void Death()
    {
        BaloonFlicker.Hide(3f);
        SimulateDeath(3f);
    }
    private void SimulateDeath(float time)
    {
        Velocity = Vector3.zero;
        Body.simulated = false;
        Body.velocity = Vector3.zero;
        UserInput.Lock();
        Invoke(nameof(Respawn), time - 0.5f);
    }

    private void Respawn()
    {
        State = StateFlags.None;
        Legs.enabled = true;
        Body.simulated = true;
        Body.position = RespawnAnchor.position;
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
        HandleSwap();

        HandleStateChange();
        UserInput.Reset();
    }

    private void HandleSwap()
    {
        if (SwappingList == null)
        {
            if (UserInput.Swap.Down && SwapSensor.HasOptions())
            {
                StartCoroutine(nameof(SwapRoutine));
            }
        }
    }

    private IEnumerator SwapRoutine()
    {
        UserInput.Reset();
        Time.timeScale = 0.05f;
        SwappingList = SwapSensor.GetOptions();
        yield return 0;
        while (SwappingList != null)
        {

            Vector2 delta, worldpos = Camera.main.ScreenToWorldPoint(UserInput.MousePosition);

            int closest = -1;
            var sqrDist = 9f;

            for (int i = 0; i < SwappingList.Count; i++)
            {
                delta = SwappingList[i].Position - worldpos;
                if (delta.sqrMagnitude < sqrDist)
                {
                    sqrDist = delta.sqrMagnitude;
                    closest = i;
                }
            }

            Vector3 pos;
            for (int i = 0; i < SwappingList.Count; i++)
            {
                pos = (Vector3)SwappingList[i].Position + Vector3.back * 5f;

                if (closest == i)
                {
                    Debug.DrawLine(pos + Vector3.up, pos + Vector3.right, Color.cyan);
                    Debug.DrawLine(pos + Vector3.right, pos + Vector3.down, Color.cyan);
                    Debug.DrawLine(pos + Vector3.down, pos + Vector3.left, Color.cyan);
                    Debug.DrawLine(pos + Vector3.left, pos + Vector3.up, Color.cyan);

                }
                Debug.DrawLine(pos + Vector3.up, pos + Vector3.down, Color.red);
                Debug.DrawLine(pos + Vector3.right, pos + Vector3.left, Color.red);
            }

            if (UserInput.Swap.Down)
            {
                if (closest != -1)
                {
                    RoleManager.Instance.Swap(Behaviour.ActiveRole, SwappingList[closest].Role);
                }
                Time.timeScale = 1f;
                SwappingList = null;
            }
            UserInput.Reset();
            yield return 0;
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            //Debug.DrawLine(collision.contacts[i].point + Vector2.left, collision.contacts[i].point + Vector2.right, Color.red);
            //Debug.DrawLine(collision.contacts[i].point + Vector2.down, collision.contacts[i].point + Vector2.up, Color.red);
            //Debug.DrawLine(Body.position + Vector2.left, Body.position + Vector2.right, Color.green);
            //Debug.DrawLine(Body.position + Vector2.down, Body.position + Vector2.up, Color.green);
            DigestNormal(collision.contacts[i].normal);
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            //Debug.DrawLine(collision.contacts[i].point + Vector2.left, collision.contacts[i].point + Vector2.right, Color.red);
            //Debug.DrawLine(collision.contacts[i].point + Vector2.down, collision.contacts[i].point + Vector2.up, Color.red);
            //Debug.DrawLine(Body.position + Vector2.left, Body.position + Vector2.right, Color.green);
            //Debug.DrawLine(Body.position + Vector2.down, Body.position + Vector2.up, Color.green);
            DigestNormal(collision.contacts[i].normal);
        }
    }


    private void RegularState()
    {
        if (State.HasFlag(StateFlags.Ground))
        {
            if (LastVelocity.y < 0)
            {
                Body.position += Vector2.up * LastVelocity.y * Time.fixedDeltaTime;
            }
            Velocity.y = 0;
        }
        else if (State.HasFlag(StateFlags.Ceil))
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
            if (UserInput.Crouch.Hold)
            {
                State |= StateFlags.Ball;
                Legs.enabled = false;
            }
            else if (UserInput.Jump.Down)
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
            else if (Velocity.y < 0 && (UserInput.Crouch.Hold || UserInput.Jump.Hold))
            {
                State |= StateFlags.Ball;
                Legs.enabled = false;
            }
        }
        Body.velocity = Velocity;
    }

    private void BallState()
    {
        if (UserInput.Crouch.Hold || UserInput.Jump.Hold)
        {
            return;
        }

        if (State.HasFlag(StateFlags.Ground) && State.HasFlag(StateFlags.Ceil))
        {
            if (Velocity == Vector2.zero)
            {
                Death();
            }
        }
        else
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
}
