﻿public class BlockBehaviour : RoleBehaviour
{
    private void Awake()
    {
        SetupColliderManipulator();
    }

    private void OnEnable()
    {
        if (Object == null)
        {
            return;
        }
        manipulator.SwapPhysicsMaterial(Object.ActiveRole.Material);
    }

    private void OnDisable()
    {
        manipulator.RestorePhysicsMaterial();
    }
}