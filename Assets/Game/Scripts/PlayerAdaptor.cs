using System;
using UnityEngine;

public class PlayerAdaptor : MonoBehaviour
{
    public Animator Animator;
    public SpriteRenderer Sprite;
    public Player Player;
    public BaloonBehaviour PlayerBaloon;
    public BaloonFlicker PlayerFlicker;
    public AudioClip JumpClip, LandClip, BallClip, UnballClip, DieClip, RespawnClip, SelectClip, DeselectClip;
    bool flipFacing;

    int MovH, MovV, Ground, Ball;

    private void Awake()
    {
        MovH = Animator.StringToHash("MovementH");
        MovV = Animator.StringToHash("MovementV");
        Ground = Animator.StringToHash("Ground");
        Ball = Animator.StringToHash("Ball");
        Player.OnStateChange += HandleStateChange;
        Player.RoleObject.RoleDestroyed.AddListener(HandleDeath);
        PlayerBaloon = Player.GetComponent<BaloonBehaviour>();
        PlayerFlicker = Player.GetComponent<BaloonFlicker>();
    }

    private void HandleDeath(float respawnTimer)
    {
        Invoke(nameof(ShowBaloon), respawnTimer);
    }

    private void ShowBaloon()
    {
        PlayerFlicker.Show();
    }

    private void HandleStateChange(Player.StateFlags last, Player.StateFlags current)
    {
        var lostStats = last ^ (last & current);
        var newStats = current ^ (last & current);

        if (lostStats.HasFlag(Player.StateFlags.Dead))
        {
            SharedSoundEmiter.Instance.Play(RespawnClip);
            return;
        }
        //if (newStats.HasFlag(Player.StateFlags.Dead))
        //{
        //    SharedSoundEmiter.Instance.Play(DieClip);
        //}
        if (lostStats.HasFlag(Player.StateFlags.Selecting) && !Player.SwapAccepted)
        {
            SharedSoundEmiter.Instance.Play(DeselectClip);
            return;
        }
        if (newStats.HasFlag(Player.StateFlags.Selecting))
        {
            SharedSoundEmiter.Instance.Play(SelectClip);
            return;
        }
        if (lostStats.HasFlag(Player.StateFlags.Ball))
        {
            SharedSoundEmiter.Instance.Play(UnballClip);
            return;
        }
        if (newStats.HasFlag(Player.StateFlags.Ball))
        {
            SharedSoundEmiter.Instance.Play(BallClip);
            return;
        }
        if (lostStats.HasFlag(Player.StateFlags.Ground) && Player.Jumped)
        {
            SharedSoundEmiter.Instance.Play(JumpClip);
            return;
        }
        if (newStats.HasFlag(Player.StateFlags.Ground))
        {
            SharedSoundEmiter.Instance.Play(LandClip);
            return;
        }
    }

    private void Update()
    {
        var hspeed = Mathf.Abs(Player.Velocity.x);
        if (hspeed > 0.01f)
        {
            flipFacing = Player.Velocity.x < 0;
        }
        PlayerBaloon.CanPlayBounce = Player.State.HasFlag(Player.StateFlags.Ball);
        Sprite.flipX = flipFacing;
        Animator.SetFloat(MovH, hspeed);
        Animator.SetFloat(MovV, Player.Velocity.y);
        Animator.SetFloat(Ground, Player.State.HasFlag(Player.StateFlags.Ground) ? 1f : 0f);
        Animator.SetBool(Ball, Player.State.HasFlag(Player.StateFlags.Ball));
    }
}