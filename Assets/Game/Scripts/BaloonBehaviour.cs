using System.Collections;
using UnityEngine;

public class BaloonBehaviour : RoleBehaviour
{
    public bool DisableCollidersOnPop = true;
    BaloonRole ActiveRole;
    bool Popped;

    private void Awake()
    {
        SetupColliderManipulator();

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
        manipulator.SwapPhysicsMaterial(Object.ActiveRole.Material);
    }

    private void OnDisable()
    {
        manipulator.RestorePhysicsMaterial();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleContacts(collision);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        HandleContacts(collision);
    }

    private void HandleContacts(Collision2D collision)
    {
        if (!isActiveAndEnabled)
        {
            return;
        }

        if (collision.collider.CompareTag(nameof(SpikeRole)))
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
            manipulator.SetEnabled(false);
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
            manipulator.SetEnabled(true);
        }
    }
}
