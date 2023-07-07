using System;
using UnityEngine;

public abstract class Role : ScriptableObject
{
    public bool Swappable = true;
    Type Type;

    public void Init()
    {
        Type = GetBehaviourType();
    }

    public RoleBehaviour Install(RoleObject obj)
    {
        var instance = (RoleBehaviour)obj.GetComponent(Type);
        if (instance == null)
        {
            instance = (RoleBehaviour)obj.gameObject.AddComponent(Type);
        }
        instance.Object = obj;
        instance.enabled = false;
        return instance;
    }

    protected abstract Type GetBehaviourType();
}
