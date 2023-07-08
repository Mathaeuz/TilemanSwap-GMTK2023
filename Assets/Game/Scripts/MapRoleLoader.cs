using LDtkUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class MapRoleLoader : MonoBehaviour
{
    public RoleSettings RoleSettings;
    public RoleObject RolePrefab;
    public Collider2D CellPrefab;
    [Serializable]
    public class RolePosition
    {
        public Vector3Int Position;
        public Role Role;
    }

    private void Process()
    {
        BuildTileList();
    }
#if UNITY_EDITOR
    private void BuildTileList()
    {
        var layers = GetComponentsInChildren<LDtkComponentLayer>();
        Tilemap map = null;
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].LayerType == TypeEnum.IntGrid)
            {
                map = layers[i].GetComponentInChildren<Tilemap>();
            }
        }

        if (map == null)
        {
            return;
        }

        var translations = RoleSettings.Settings.ToDictionary(x => x.Tile, x => x.Role);
        var roles = new List<RolePosition>();
        for (int i = 0; i < map.size.y; i++)
        {
            for (int j = 0; j < map.size.x; j++)
            {
                var pos = new Vector3Int(j, i);
                if (!map.HasTile(pos))
                {
                    continue;
                }

                roles.Add(new RolePosition
                {
                    Role = translations[map.GetTile(pos)],
                    Position = pos
                });
            }
        }

        var groups = roles.GroupBy(x => x.Role).ToDictionary(x => x.Key, x => x.ToArray());
        var existingRoles = GetComponentsInChildren<RoleObject>();

        for (int i = 0; i < existingRoles.Length; i++)
        {
            for (int j = 0; j < existingRoles[i].transform.childCount; j++)
            {
                DestroyImmediate(existingRoles[i].transform.GetChild(0).gameObject);
            }

            if (groups.ContainsKey(existingRoles[i].ActiveRole))
            {
                BuildColliders(map, existingRoles[i], groups[existingRoles[i].ActiveRole]);
                groups.Remove(existingRoles[i].ActiveRole);
            }
        }

        foreach (var item in groups)
        {
            var obj = PrefabUtility.InstantiatePrefab(RolePrefab).GetComponent<RoleObject>();
            obj.ActiveRole = item.Key;
            obj.transform.SetParent(transform);
            BuildColliders(map, obj, item.Value);
        }
    }

    private void BuildColliders(Tilemap map, RoleObject roleObject, RolePosition[] rolePositions)
    {
        SpriteRenderer cell;
        for (int i = 0; i < rolePositions.Length; i++)
        {
            cell = PrefabUtility.InstantiatePrefab(CellPrefab).GetComponent<SpriteRenderer>();
            cell.transform.SetParent(roleObject.transform);
            cell.transform.position = map.CellToWorld(rolePositions[i].Position);
            cell.sprite = RoleSettings.Get(roleObject.ActiveRole).Theme.Sprite;
        }
    }

    [CustomEditor(typeof(MapRoleLoader))]
    public class _Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Process"))
            {
                (target as MapRoleLoader).Process();
            }
        }
    }
#endif
}