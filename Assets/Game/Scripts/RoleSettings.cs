using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Role Settings")]
public class RoleSettings : ScriptableObject
{
    public Role[] Roles;

    public Role Get(RoleSwap roleEnum)
    {
        for (int i = 0; i < Roles.Length; i++)
        {
            if (Roles[i].RoleEnum == roleEnum)
            {
                return Roles[i];
            }
        }
        return null;
    }

    public Role Get(TileBase tile)
    {
        for (int i = 0; i < Roles.Length; i++)
        {
            if (Roles[i].Tile == tile)
            {
                return Roles[i];
            }
        }
        return null;
    }
}