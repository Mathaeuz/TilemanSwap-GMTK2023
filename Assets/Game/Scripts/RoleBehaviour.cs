using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class RoleBehaviour : MonoBehaviour
{
    public RoleObject Object { get; private set; }
    public Action<RoleObject> OnSetObject;

    public void SetObject(RoleObject obj)
    {
        Object = obj;
        OnSetObject?.Invoke(obj);
    }

    protected void SwapLayer()
    {
    }

    protected void RestoreLayer()
    {
    }
}