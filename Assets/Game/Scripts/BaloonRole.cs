using System;
using UnityEngine;

[CreateAssetMenu(menuName ="Roles/Baloon")]
public class BaloonRole : Role
{
    public float PopDuration = 3f;
    public AudioClip BounceClip;

    protected override Type GetBehaviourType() => typeof(BaloonBehaviour);
}
