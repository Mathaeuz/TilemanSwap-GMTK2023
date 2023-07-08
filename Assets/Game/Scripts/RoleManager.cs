using System;
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

    [Serializable]
    public class RoleThemePair
    {
        public Role Role;
        public Color Theme;
    }

    public Role[] Roles;
    public Color[] Themes;
    public Dictionary<Role, Color> ThemeMap = new();
    List<RoleObject>[] Instances;

    private void Awake()
    {
        Instances = new List<RoleObject>[Roles.Length];
        for (int i = 0; i < Roles.Length; i++)
        {
            Roles[i].Init();
            Instances[i] = new List<RoleObject>();
        }

        if (Themes.Length != Roles.Length)
        {
            Debug.LogError("Theme array has incorrect size");
            return;
        }
        for (int i = 0; i < Themes.Length; i++)
        {
            ThemeMap[Roles[i]] = Themes[i];
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
            Instances[idA][i].Set(Roles[idA]);
        }
        for (int i = 0; i < Instances[idB].Count; i++)
        {
            Instances[idB][i].Set(Roles[idB]);
        }
    }

    public void Register(RoleObject item)
    {
        var idx = Array.IndexOf(Roles, item.ActiveRole);
        if (idx == -1)
        {
            idx = 0;
        }
        Instances[idx].Add(item);

        if (item.ActiveRole.Swappable)
        {
            item.Behaviours.Clear();
            for (int i = 0; i < Roles.Length; i++)
            {
                item.Behaviours[Roles[i]] = Roles[i].Install(item);
            }
        }
        else
        {
            item.Behaviours[item.ActiveRole] = item.ActiveRole.Install(item);
        }

        item.Set(Roles[idx]);
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
