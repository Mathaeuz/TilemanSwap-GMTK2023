using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Role : ScriptableObject
{
    public bool Swappable = true;
    public PhysicsMaterial2D Material;
    public TagSelector Tag = new TagSelector { Value = "Untagged" };
    public LayerSelector Layer;
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

[Serializable]
public class Theme
{
    public Sprite Sprite;
    public Color Color = Color.white;
    public Texture2D TileSource;
    public TileBase[] Tiles;
}