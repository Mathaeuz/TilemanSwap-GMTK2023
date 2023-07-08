using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Role Settings")]
public class RoleSettings : ScriptableObject
{
    [Serializable]
    public class Data
    {
        public Role Role;
        public TileBase Tile;
        public Theme Theme;
        public RoleSwap RoleEnum;
    }

    public Data[] Settings;

    public Data Get(Role role)
    {
        for (int i = 0; i < Settings.Length; i++)
        {
            if (Settings[i].Role == role)
            {
                return Settings[i];
            }
        }
        return null;
    }

    public Data Get(RoleSwap roleEnum)
    {
        for (int i = 0; i < Settings.Length; i++)
        {
            if (Settings[i].RoleEnum == roleEnum)
            {
                return Settings[i];
            }
        }
        return null;
    }
}