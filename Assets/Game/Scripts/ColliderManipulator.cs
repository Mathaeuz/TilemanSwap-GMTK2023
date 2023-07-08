using UnityEngine;

public class ColliderManipulator
{
    Collider2DManipulator collider;
    Composite2DManipulator composite;

    public ColliderManipulator(Component target)
    {
        var c = target.GetComponent<CompositeCollider2D>();
        if (c != null)
        {
            composite = new Composite2DManipulator(c);
            return;
        }
        collider = new(target.GetComponentsInChildren<Collider2D>(includeInactive: true));
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
    PhysicsMaterial2D[] OriginalMaterials;
    string[] OriginalTags;

    public Collider2DManipulator(Collider2D[] contacts)
    {
        Contacts = contacts;
        OriginalMaterials = new PhysicsMaterial2D[contacts.Length];
        OriginalTags = new string[contacts.Length];
        for (int i = 0; i < Contacts.Length; i++)
        {
            OriginalMaterials[i] = Contacts[i].sharedMaterial;
            OriginalTags[i] = Contacts[i].tag;
        }
    }

    public void SwapPhysicsMaterial(PhysicsMaterial2D material)
    {
        for (int i = 0; i < Contacts.Length; i++)
        {
            Contacts[i].sharedMaterial = material;
        }
    }

    public void RestorePhysicsMaterial()
    {
        for (int i = 0; i < Contacts.Length; i++)
        {
            Contacts[i].sharedMaterial = OriginalMaterials[i];
        }
    }

    public void SwapTags(string tag)
    {
        for (int i = 0; i < Contacts.Length; i++)
        {
            Contacts[i].tag = tag;
        }
    }

    public void RestoreTags()
    {
        for (int i = 0; i < Contacts.Length; i++)
        {
            Contacts[i].tag = OriginalTags[i];
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