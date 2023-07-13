using System.Collections;
using UnityEngine;

public class ForceRollback : PlayerTrigger
{
    public SwapHolder Holder;
    Role[] TargetState;

    private void Awake()
    {
        ContactEnter.AddListener(BeginSwap);
        TargetState = RoleManager.Instance.NewRoleState();
        RoleManager.Instance.ReadRollbackToRole(TargetState, Holder.ActiveRole);
    }

    private void BeginSwap(Collider2D arg0)
    {
        var player = arg0.GetComponentInParent<Player>();
        if (player.RoleObject.ActiveRole != Holder.ActiveRole)
        {
            StartCoroutine(SwapAnimation(player));
        }
        else
        {
            Rollback();
        }
    }

    private IEnumerator SwapAnimation(Player player)
    {
        player.BeginSelection();
        yield return new WaitForSecondsRealtime(0.1f);
        SwapSelector.Instance.EndSelection(Holder);
        yield return new WaitForSecondsRealtime(0.6f);
        Rollback();
    }

    private void Rollback()
    {
        RoleManager.Instance.RollbackSwaps(TargetState);
    }
}