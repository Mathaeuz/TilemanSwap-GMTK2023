using UnityEngine;

public class PlayerAdaptor : MonoBehaviour
{
    public Animator Animator;
    public SpriteRenderer Sprite;
    public Player Player;
    bool flipFacing;

    int MovH, MovV, Ground, Ball;

    private void Awake()
    {
        MovH = Animator.StringToHash("MovementH");
        MovV = Animator.StringToHash("MovementV");
        Ground = Animator.StringToHash("Ground");
        Ball = Animator.StringToHash("Ball");
    }

    private void Update()
    {
        var hspeed = Mathf.Abs(Player.Velocity.x);
        if (hspeed > 0.01f)
        {
            flipFacing = Player.Velocity.x < 0;
        }
        Sprite.flipX = flipFacing;
        Animator.SetFloat(MovH, hspeed);
        Animator.SetFloat(MovV, Player.Velocity.y);
        Animator.SetFloat(Ground, Player.State.HasFlag(Player.StateFlags.Ground) ? 1f : 0f);
        Animator.SetBool(Ball, Player.State.HasFlag(Player.StateFlags.Ball));
    }
}