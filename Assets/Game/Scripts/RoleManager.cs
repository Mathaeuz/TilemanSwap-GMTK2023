﻿using System;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        Instances = new List<RoleObject>[RoleSettings.Settings.Length];
        Roles = new Role[RoleSettings.Settings.Length];
        for (int i = 0; i < RoleSettings.Settings.Length; i++)
        {
            RoleSettings.Settings[i].Role.Init();
            Instances[i] = new List<RoleObject>();
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

        var l = Instances[idA];
        Instances[idA] = Instances[idB];
        Instances[idB] = l;

        for (int i = 0; i < Instances[idA].Count; i++)
        {
            Instances[idA][i].Change(Roles[idA]);
        }
        for (int i = 0; i < Instances[idB].Count; i++)
        {
            Instances[idB][i].Change(Roles[idB]);
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
}

[Serializable]
public class Theme
{
    public Sprite Sprite;
    public Color Color;
}