using System;
using UnityEngine;

public class RolePhysics : MonoBehaviour
{
    public TargetSet ActiveState, Material, Tag, Layer;

    [Serializable]
    public class TargetSet
    {
        public Rigidbody2D[] Bodies;
        public Collider2D[] Colliders;
    }

    public void AutoConfigure()
    {
        Rigidbody2D[] bodies;
        Collider2D[] colliders;

        bodies = GetComponentsInChildren<Rigidbody2D>();
        if (bodies.Length > 0)
        {
            colliders = new Collider2D[0];
        }
        else
        {
            colliders = GetComponentsInChildren<Collider2D>();
        }

        ActiveState = new TargetSet
        {
            Bodies = bodies,
            Colliders = colliders
        };
        Material = new TargetSet
        {
            Bodies = bodies,
            Colliders = colliders
        };
        Tag = new TargetSet
        {
            Bodies = bodies,
            Colliders = colliders
        };
        Layer = new TargetSet
        {
            Bodies = bodies,
            Colliders = colliders
        };
    }


    public void SwapPhysicsMaterial(PhysicsMaterial2D material)
    {
        for (int i = 0; i < Material.Colliders.Length; i++)
        {
            Material.Colliders[i].sharedMaterial = material;
        }
        for (int i = 0; i < Material.Bodies.Length; i++)
        {
            Material.Bodies[i].sharedMaterial = material;
        }
    }

    public void SwapTags(string tag)
    {
        for (int i = 0; i < Tag.Colliders.Length; i++)
        {
            Tag.Colliders[i].tag = tag;
        }
        for (int i = 0; i < Tag.Bodies.Length; i++)
        {
            Tag.Bodies[i].tag = tag;
        }
    }

    public void SwapLayer(int layer)
    {
        for (int i = 0; i < Layer.Colliders.Length; i++)
        {
            Layer.Colliders[i].gameObject.layer = layer;
        }
        for (int i = 0; i < Layer.Bodies.Length; i++)
        {
            Layer.Bodies[i].gameObject.layer = layer;
        }
    }

    public void SetEnabled(bool value)
    {
        for (int i = 0; i < ActiveState.Colliders.Length; i++)
        {
            ActiveState.Colliders[i].enabled = value;
        }
        for (int i = 0; i < ActiveState.Bodies.Length; i++)
        {
            ActiveState.Bodies[i].simulated = value;
        }
    }
}