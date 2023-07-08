using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class RoleBehaviour : MonoBehaviour
{
    public RoleObject Object { get; private set; }
    public Action<RoleObject> OnSetObject;

    protected ColliderManipulator manipulator;

    public void SetObject(RoleObject obj)
    {
        Object = obj;
        OnSetObject?.Invoke(obj);
    }

    protected void SetupColliderManipulator()
    {
        manipulator = new ColliderManipulator(this);
    }
}