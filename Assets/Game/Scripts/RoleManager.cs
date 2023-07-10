using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class RoleManager : Singleton<RoleManager>
{
#if UNITY_EDITOR
    [Serializable]
    public class Editor
    {
        public Role A, B;
        public bool Swap;
    }
    public Editor EditorTests;
#endif

    public RoleSettings RoleSettings;
    Role[] Roles;
    List<RoleObject>[] Instances;
    List<SwapHolder>[] Holders;

    public UnityEvent<bool> AllowEffects = new();
    Role[] RoleCheckpoint;

    public void SaveSwaps()
    {
        for (int i = 0; i < Roles.Length; i++)
        {
            RoleCheckpoint[i] = Roles[i];
        }
    }

    public void RollbackSwaps()
    {
        List<int> apply = new();
        for (int i = 0; i < Roles.Length; i++)
        {
            if (Roles[i] == RoleCheckpoint[i])
            {
                continue;
            }
            var altIndex = Array.IndexOf(Roles, RoleCheckpoint[i]);
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
        RoleCheckpoint = new Role[RoleSettings.Roles.Length];
        for (int i = 0; i < RoleSettings.Roles.Length; i++)
        {
            RoleSettings.Roles[i].Init();
            Instances[i] = new List<RoleObject>();
            Holders[i] = new List<SwapHolder>();

            Roles[i] = RoleSettings.Roles[i];
            RoleCheckpoint[i] = Roles[i];
        }
    }

    private void FixedUpdate()
    {
#if UNITY_EDITOR
        if (EditorTests.Swap)
        {
            EditorTests.Swap = false;
            Swap(EditorTests.A, EditorTests.B);
        }
#endif
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
}