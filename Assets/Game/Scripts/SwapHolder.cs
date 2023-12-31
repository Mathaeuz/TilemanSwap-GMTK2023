﻿using LDtkUnity;
using System;
using UnityEngine;

public class SwapHolder : MonoBehaviour
{
    public Role ActiveRole;
    public Action<Role> OnChangeRole;
    public bool ButtonEnabled = true,
        ShouldRegister = true;

    private void Awake()
    {
        var fields = GetComponent<LDtkFields>();
        if (fields != null)
        {
            ActiveRole = RoleManager.Instance.RoleSettings.Get(fields.GetEnum<RoleSwap>("Role"));
        }

        if (ActiveRole == null)
        {
            return;
        }

        if (ShouldRegister)
        {
            RoleManager.Instance.Register(this);
        }
        SwapSelector.Instance.AddSwapButton(this);
    }

    public void Change(Role role)
    {
        ActiveRole = role;
        OnChangeRole?.Invoke(role);
    }
}
