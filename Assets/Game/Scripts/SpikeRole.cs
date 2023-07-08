using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Roles/Spike")]
public class SpikeRole : Role
{
    protected override Type GetBehaviourType() => typeof(SpikeBehaviour);
}
