using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColliderManipulator
{
    Collider2DManipulator collider;
    Composite2DManipulator composite;

    public ColliderManipulator(Component target, Collider2D[] ignoreMaterial, Collider2D[] ignoreTag)
    {
        var c = target.GetComponent<CompositeCollider2D>();
        if (c != null)
        {
            composite = new Composite2DManipulator(c);
            return;
        }
        collider = new(target.GetComponentsInChildren<Collider2D>(includeInactive: true), ignoreMaterial, ignoreTag);
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

    public Collider2DManipulator(Collider2D[] contacts, Collider2D[] ignoreMaterial, Collider2D[] ignoreTag)
    {
        Contacts = contacts;

        MaterialContacts = new();
        OriginalMaterials = new();
        TagContacts = new();
        OriginalTags = new();


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

    public void SetEnabled(bool value)
    {
        for (int i = 0; i < Contacts.Length; i++)
        {
            Contacts[i].enabled = value;
        }
    }
}