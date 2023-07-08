using System;
using UnityEngine;

[CreateAssetMenu(menuName ="Roles/Baloon")]
public class BaloonRole : Role
{
    public float PopDuration = 3f;

    protected override Type GetBehaviourType() => typeof(BaloonBehaviour);
}
