using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColliderManipulator
{
    Collider2DManipulator collider;
    Composite2DManipulator composite;

    public ColliderManipulator(Component target, Collider2D[] ignoreMaterial, Collider2D[] ignoreTag, Collider2D[] ignoreLayer)
    {
        var c = target.GetComponent<CompositeCollider2D>();
        if (c != null)
        {
            composite = new Composite2DManipulator(c);
            return;
        }
        collider = new(target.GetComponentsInChildren<Collider2D>(includeInactive: true), ignoreMaterial, ignoreTag, ignoreLayer);
    }

    public void SwapPhysicsMaterial(PhysicsMaterial2D material)
    {
        if (composite != null)
        {
            composite.SwapPhysicsMaterial(material);
            return;
        }
        collider.SwapPhysicsMaterial(material);
    }

    public void RestorePhysicsMaterial()
    {
        if (composite != null)
        {
            composite.RestorePhysicsMaterial();
            return;
        }
        collider.RestorePhysicsMaterial();
    }

    public void SwapTags(string tag)
    {
        if (composite != null)
        {
            composite.SwapTags(tag);
            return;
        }
        collider.SwapTags(tag);
    }

    public void RestoreTags()
    {
        if (composite != null)
        {
            composite.RestoreTags();
            return;
        }
        collider.RestoreTags();
    }

    public void SetEnabled(bool value)
    {
        if (composite != null)
        {
            composite.SetEnabled(value);
            return;
        }
        collider.SetEnabled(value);
    }
}

public class Composite2DManipulator
{
    CompositeCollider2D Contacts;
    PhysicsMaterial2D OriginalMaterials;
    string OriginalTags;

    public Composite2DManipulator(CompositeCollider2D contacts)
    {
        Contacts = contacts;
        OriginalMaterials = Contacts.sharedMaterial;
        OriginalTags = Contacts.tag;
    }

    public void SwapPhysicsMaterial(PhysicsMaterial2D material)
    {
        Contacts.sharedMaterial = material;
    }

    public void RestorePhysicsMaterial()
    {
        Contacts.sharedMaterial = OriginalMaterials;
    }

    public void SwapTags(string tag)
    {
        Contacts.tag = tag;
    }

    public void RestoreTags()
    {
        Contacts.tag = OriginalTags;
    }

    public void SetEnabled(bool value)
    {
        Contacts.attachedRigidbody.simulated = value;
    }
}

public class Collider2DManipulator
{
    Collider2D[] Contacts;

    List<Collider2D> MaterialContacts;
    List<PhysicsMaterial2D> OriginalMaterials;
    List<Collider2D> TagContacts;
    List<string> OriginalTags;
    List<Collider2D> LayerContacts;
    List<int> OriginalLayers;

    public Collider2DManipulator(Collider2D[] contacts, Collider2D[] ignoreMaterial, Collider2D[] ignoreTag, Collider2D[] ignoreLayer)
    {
        Contacts = contacts;

        MaterialContacts = new();
        OriginalMaterials = new();
        TagContacts = new();
        OriginalTags = new();
        LayerContacts = new();
        OriginalLayers = new();


        for (int i = 0; i < Contacts.Length; i++)
        {
            if (Array.IndexOf(ignoreMaterial, Contacts[i]) == -1)
            {
                MaterialContacts.Add(Contacts[i]);
                OriginalMaterials.Add(Contacts[i].sharedMaterial);
            }
            if (Array.IndexOf(ignoreTag, Contacts[i]) == -1)
            {
                TagContacts.Add(Contacts[i]);
                OriginalTags.Add(Contacts[i].tag);
            }
            if (Array.IndexOf(ignoreLayer, Contacts[i]) == -1)
            {
                LayerContacts.Add(Contacts[i]);
                OriginalLayers.Add(Contacts[i].gameObject.layer);
            }
        }
    }

    public void SwapPhysicsMaterial(PhysicsMaterial2D material)
    {
        for (int i = 0; i < MaterialContacts.Count; i++)
        {
            MaterialContacts[i].sharedMaterial = material;
        }
    }

    public void RestorePhysicsMaterial()
    {
        for (int i = 0; i < MaterialContacts.Count; i++)
        {
            MaterialContacts[i].sharedMaterial = OriginalMaterials[i];
        }
    }

    public void SwapTags(string tag)
    {
        for (int i = 0; i < TagContacts.Count; i++)
        {
            TagContacts[i].tag = tag;
        }
    }

    public void RestoreTags()
    {
        for (int i = 0; i < TagContacts.Count; i++)
        {
            TagContacts[i].tag = OriginalTags[i];
        }
    }
    public void SwapLayer(int layer)
    {
        for (int i = 0; i < LayerContacts.Count; i++)
        {
            LayerContacts[i].gameObject.layer = layer;
        }
    }

    public void RestoreLayer()
    {
        for (int i = 0; i < LayerContacts.Count; i++)
        {
            LayerContacts[i].gameObject.layer = OriginalLayers[i];
        }
    }

    public void SetEnabled(bool value)
    {
        for (int i = 0; i < Contacts.Length; i++)
        {
            Contacts[i].enabled = value;
        }
    }
}