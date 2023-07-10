using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Role : ScriptableObject
{
    public bool Swappable = true;
    public PhysicsMaterial2D Material;
    public TileBase Tile;
    public Theme Theme;
    public RoleSwap RoleEnum;
    public AudioClip PopEffect, RespawnEffect;
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
        instance.SetObject(obj);
        instance.enabled = false;
        return instance;
    }

    protected abstract Type GetBehaviourType();
}
