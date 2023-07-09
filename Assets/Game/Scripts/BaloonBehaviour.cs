using System;
using System.Collections;
using UnityEngine;

public class BaloonBehaviour : RoleBehaviour
{
    public bool DisableCollidersOnPop = true;
    BaloonRole ActiveRole;
    bool Popped;
    public bool CanPlayBounce = true;

    private void Awake()
    {
        //Todo find another way to load into zone blocks
        if (GetComponent<BaloonFlicker>() == null)
        {
            gameObject.AddComponent<BaloonFlicker>();
        }
    }

    private void OnEnable()
    {
        if (Object == null)
        {
            return;
        }
        ActiveRole = Object.ActiveRole as BaloonRole;
        Object.ColliderManagement?.SwapPhysicsMaterial(Object.ActiveRole.Material);
        SwapLayer();
    }

    private void OnDisable()
    {
        if (Object == null)
        {
            return;
        }
        Object.ColliderManagement?.RestorePhysicsMaterial();
        RestoreLayer();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActiveAndEnabled)
        {
            return;
        }
        HandleContacts(collision.collider); 
        if (!Popped && CanPlayBounce)
        {
            SharedSoundEmiter.Instance.PlayWithCooldown(ActiveRole.BounceClip,3f);
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isActiveAndEnabled)
        {
            return;
        }
        HandleContacts(collision.collider);
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isActiveAndEnabled)
        {
            return;
        }
        HandleContacts(collider);
        if (!Popped && CanPlayBounce)
        {
            SharedSoundEmiter.Instance.PlayWithCooldown(ActiveRole.BounceClip,3f);
        }
    }
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (!isActiveAndEnabled)
        {
            return;
        }
        HandleContacts(collider);
    }

    private void HandleContacts(Collider2D collider)
    {
        if (collider.CompareTag(nameof(SpikeRole)))
        {
            Pop();
        }
    }

    private void Pop()
    {
        if (Popped)
        {
            return;
        }
        Popped = true;
        Object.DestroyByRole(ActiveRole.PopDuration);
        if (DisableCollidersOnPop)
        {
            Object.ColliderManagement.SetEnabled(false);
        }
        StartCoroutine(nameof(Unpop));
    }

    private IEnumerator Unpop()
    {
        yield return new WaitForSeconds(ActiveRole.PopDuration);

        Popped = false;
        Object.RestoreByRole();
        if (DisableCollidersOnPop)
        {
            Object.ColliderManagement.SetEnabled(true);
        }
    }
}
