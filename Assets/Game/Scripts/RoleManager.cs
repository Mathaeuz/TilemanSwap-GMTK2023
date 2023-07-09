using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    public Dictionary<Role, Theme> ThemeMap = new();
    Role[] Roles;
    List<RoleObject>[] Instances;
    List<SwapHolder>[] Holders;

    public UnityEvent<bool> AllowEffects = new();
    Stack<(Role, Role)> SwapHistory = new();

    public void SaveSwaps()
    {
        SwapHistory.Clear();
    }

    public void RollbackSwaps()
    {
        AllowEffects.Invoke(false);
        while (SwapHistory.Count > 0)
        {
            var p = SwapHistory.Pop();
            Swap(p.Item1, p.Item2, false);
        }
        AllowEffects.Invoke(true);
    }

    private void Awake()
    {
        Instances = new List<RoleObject>[RoleSettings.Settings.Length];
        Holders = new List<SwapHolder>[RoleSettings.Settings.Length];
        Roles = new Role[RoleSettings.Settings.Length];
        for (int i = 0; i < RoleSettings.Settings.Length; i++)
        {
            RoleSettings.Settings[i].Role.Init();
            Instances[i] = new List<RoleObject>();
            Holders[i] = new List<SwapHolder>();
            Roles[i] = RoleSettings.Settings[i].Role;
            ThemeMap[Roles[i]] = RoleSettings.Settings[i].Theme;
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

    public void Swap(Role a, Role b, bool history = true)
    {
        if (!a.Swappable || !b.Swappable)
        {
            return;
        }
        if (history)
        {
            SwapHistory.Push((a, b));
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

        var instances = Instances[idA];
        Instances[idA] = Instances[idB];
        Instances[idB] = instances;

        var holders = Holders[idA];
        Holders[idA] = Holders[idB];
        Holders[idB] = holders;

        for (int i = 0; i < Instances[idA].Count; i++)
        {
            Instances[idA][i].Change(Roles[idA]);
        }
        for (int i = 0; i < Holders[idA].Count; i++)
        {
            Holders[idA][i].Change(Roles[idA]);
        }
        for (int i = 0; i < Instances[idB].Count; i++)
        {
            Instances[idB][i].Change(Roles[idB]);
        }
        for (int i = 0; i < Holders[idB].Count; i++)
        {
            Holders[idB][i].Change(Roles[idB]);
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

        item.Change(Roles[idx]);
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

[Serializable]
public class Theme
{
    public Sprite Sprite;
    public Color Color;
}