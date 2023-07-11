using System;
using UnityEngine;

public abstract class RoleBehaviour : MonoBehaviour
{
    public RoleObject Object { get; private set; }
    public Action<RoleObject> OnSetObject;

    public void SetObject(RoleObject obj)
    {
        Object = obj;
        OnSetObject?.Invoke(obj);
    }

    protected void DefaultSwaps()
    {
        if (Object == null || Object.Physics == null)
        {
            return;
        }
        Object.Physics.SwapPhysicsMaterial(Object.ActiveRole.Material);
        Object.Physics.SwapTags(Object.ActiveRole.Tag.Value);
        Object.Physics.SwapLayer(Object.ActiveRole.Layer.Value);
    }
}