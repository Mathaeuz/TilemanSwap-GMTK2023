using UnityEngine;

public class PhysicsMaterialSwapper
{
    Collider2D[] Contacts;
    PhysicsMaterial2D[] Original;

    public PhysicsMaterialSwapper(Component target)
    {
        Contacts = target.GetComponentsInChildren<Collider2D>(includeInactive:true);
    }

    public void Swap(PhysicsMaterial2D material)
    {
        if (Contacts == null || Contacts.Length == 0)
            return;

        if (Original == null)
        {
            Original = new PhysicsMaterial2D[Contacts.Length];
            for (int i = 0; i < Contacts.Length; i++)
            {
                Original[i] = Contacts[i].sharedMaterial;
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

    public void Restore()
    {
        if (Original == null)
        {
            return;
        }

        for (int i = 0; i < Contacts.Length; i++)
        {
            Contacts[i].sharedMaterial = Original[i];
        }
        Original = null;
    }
}
