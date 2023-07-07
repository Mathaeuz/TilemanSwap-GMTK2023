using System;
using UnityEngine;

[CreateAssetMenu(menuName ="Roles/Baloon")]
public class BaloonRole : Role
{
    public PhysicsMaterial2D Material;

    protected override Type GetBehaviourType() => typeof(BaloonBehaviour);
}
