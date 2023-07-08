using LDtkUnity;
using System;
using UnityEngine;

public class SwapHolder : MonoBehaviour
{
    public Role ActiveRole;
    public Action<Role> OnChangeRole;

    public void Change(Role role)
    {
        ActiveRole = role;
        OnChangeRole?.Invoke(role);
    }

    private void Awake()
    {
        var fields = GetComponent<LDtkFields>();
        if (fields != null)
        {
            ActiveRole = RoleManager.Instance.RoleSettings.Get(fields.GetEnum<RoleSwap>("Roles")).Role;
        }
        if (ActiveRole != null)
        {
            RoleManager.Instance.Register(this);
            SwapSelector.Instance.AddSwapButton(this);
        }
    }
}
