﻿using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class RoleManager : Singleton<RoleManager>
{
    public RoleSettings RoleSettings;
    Role[] Roles;
    List<RoleObject>[] Instances;
    List<SwapHolder>[] Holders;

    public UnityEvent<bool> AllowEffects = new();
    Role[] FullRollback;


    public Role[] NewRoleState()
    {
        return new Role[RoleSettings.Roles.Length];
    }

    public void ReadCurrentSwaps(Role[] output)
    {
        for (int i = 0; i < FullRollback.Length; i++)
        {
            output[i] = Roles[i];
        }
    }

    public void ReadRollbackToRole(Role[] output, Role rollbackRole)
    {
        int player = -1, role = -1;
        for (int i = 0; i < FullRollback.Length; i++)
        {
            output[i] = FullRollback[i];
            if (output[i].RoleEnum == RoleSwap.Player)
            {
                player = i;
            }
            if (output[i] == rollbackRole)
            {
                role = i;
            }
        }

        if (player != -1 && role != -1 && player != role)
        {
            (output[player], output[role]) = (output[role], output[player]);
        }
    }

    public void RollbackSwaps(Role[] memory)
    {
        List<int> apply = new();
        for (int i = 0; i < Roles.Length; i++)
        {
            if (Roles[i] == memory[i])
            {
                continue;
            }
            var altIndex = Array.IndexOf(Roles, memory[i]);
            (Roles[i], Roles[altIndex]) = (Roles[altIndex], Roles[i]);
            apply.Add(i);
            apply.Add(altIndex);
        }

        for (int i = 0; i < apply.Count; i++)
        {
            ApplyRole(apply[i], true);
        }
    }

    private void Awake()
    {
        Instances = new List<RoleObject>[RoleSettings.Roles.Length];
        Holders = new List<SwapHolder>[RoleSettings.Roles.Length];
        Roles = new Role[RoleSettings.Roles.Length];
        FullRollback = new Role[RoleSettings.Roles.Length];
        for (int i = 0; i < RoleSettings.Roles.Length; i++)
        {
            RoleSettings.Roles[i].Init();
            Instances[i] = new List<RoleObject>();
            Holders[i] = new List<SwapHolder>();

            Roles[i] = RoleSettings.Roles[i];
            FullRollback[i] = Roles[i];
        }
    }

    public void Swap(Role a, Role b)
    {
        if (!a.Swappable || !b.Swappable)
        {
            return;
        }

        var idA = Array.IndexOf(Roles, a);
        if (idA == -1)
        {
            return;
        }
        var idB = Array.IndexOf(Roles, b);
        if (idB == -1)
        {
            return;
        }

        (Roles[idB], Roles[idA]) = (Roles[idA], Roles[idB]);
        ApplyRole(idA);
        ApplyRole(idB);
    }

    private void ApplyRole(int index, bool forceRespawn = false)
    {
        for (int i = 0; i < Instances[index].Count; i++)
        {
            Instances[index][i].Change(Roles[index], forceRespawn);
        }
        for (int i = 0; i < Holders[index].Count; i++)
        {
            Holders[index][i].Change(Roles[index]);
        }
    }

    public void Register(RoleObject item)
    {
        var idx = Array.IndexOf(Roles, item.ActiveRole);
        if (idx == -1)
        {
            return;
        }
        Instances[idx].Add(item);

        if (Roles[idx].Swappable)
        {
            item.Behaviours.Clear();
            for (int i = 0; i < Roles.Length; i++)
            {
                item.Behaviours[Roles[i]] = Roles[i].Install(item);
            }
        }
        else
        {
            item.Behaviours[Roles[idx]] = Roles[idx].Install(item);
        }

        item.Change(Roles[idx], false);
    }

    public void Release(RoleObject item)
    {
        var idx = Array.IndexOf(Roles, item);
        if (idx == -1)
        {
            return;
        }
        Instances[idx].Remove(item);
    }

    public void Register(SwapHolder item)
    {
        var idx = Array.IndexOf(Roles, item.ActiveRole);
        if (idx == -1)
        {
            return;
        }
        Holders[idx].Add(item);
        item.Change(Roles[idx]);
    }

    public Role[] BuildSwapsFromIndex(int[] swaps)
    {
        var result = new Role[swaps.Length];
        for (int i = 0; i < swaps.Length; i++)
        {
            result[i] = RoleSettings.Roles[swaps[i]];
        }
        return result;
    }

    public int[] BuildIndexFromSwaps(Role[] swaps)
    {
        var result = new int[swaps.Length];
        for (int i = 0; i < swaps.Length; i++)
        {
            result[i] = Array.IndexOf(RoleSettings.Roles, swaps[i]);
        }
        return result;
    }
}