using System;
using UnityEngine;

[CreateAssetMenu(menuName ="Roles/Block")]
public class BlockRole : Role
{
    protected override Type GetBehaviourType() => typeof(BlockBehaviour);
}