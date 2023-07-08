using System;
using UnityEngine;

public class ColliderManipulator
{
    Collider2D[] Contacts;
    PhysicsMaterial2D[] OriginalMaterials;
    string[] OriginalTags;


    public ColliderManipulator(Component target)
    {
        Contacts = target.GetComponentsInChildren<Collider2D>(includeInactive: true);
    }

    public void SwapPhysicsMaterial(PhysicsMaterial2D material)
    {
        if (Contacts == null || Contacts.Length == 0)
            return;

        if (OriginalMaterials == null)
        {
            OriginalMaterials = new PhysicsMaterial2D[Contacts.Length];
            for (int i = 0; i < Contacts.Length; i++)
            {
                OriginalMaterials[i] = Contacts[i].sharedMaterial;
                Contacts[i].sharedMaterial = material;
            }
        }
        else
        {
            for (int i = 0; i < Contacts.Length; i++)
            {
                Contacts[i].sharedMaterial = material;
            }
        }
    }

    public void RestorePhysicsMaterial()
    {
        if (OriginalMaterials == null)
        {
            return;
        }

        for (int i = 0; i < Contacts.Length; i++)
        {
            Contacts[i].sharedMaterial = OriginalMaterials[i];
        }
        OriginalMaterials = null;
    }

    public void SwapTags(string tag)
    {
        if (Contacts == null || Contacts.Length == 0)
            return;

        if (OriginalTags == null)
        {
            OriginalTags = new string[Contacts.Length];
            for (int i = 0; i < Contacts.Length; i++)
            {
                OriginalTags[i] = Contacts[i].tag;
                Contacts[i].tag = tag;
            }
        }
        else
        {
            for (int i = 0; i < Contacts.Length; i++)
            {
                Contacts[i].tag = tag;
            }
        }
    }

    public void RestoreTags()
    {
        if (OriginalTags == null)
        {
            return;
        }

        for (int i = 0; i < Contacts.Length; i++)
        {
            Contacts[i].tag = OriginalTags[i];
        }
        OriginalTags = null;
    }

    public void SetEnabled(bool value)
    {
        for (int i = 0; i < Contacts.Length; i++)
        {
            Contacts[i].enabled = value;
        }
    }
}
