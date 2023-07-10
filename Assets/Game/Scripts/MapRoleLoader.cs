using Cinemachine;
using LDtkUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapRoleLoader : MonoBehaviour
{
    public RoleSettings RoleSettings;
    public RoleObject RolePrefab;
    public Collider2D CellPrefab;
    public CinemachineVirtualCamera Camera;
    public CinemachineConfiner2D Confiner;
    public bool ConfigureCams = true;

    [Serializable]
    public class RolePosition
    {
        public Vector3Int Position;
        public Role Role;
    }

    private void Awake()
    {
        ConfigureCameras();
        //Process();
    }

    private void ConfigureCameras()
    {
        if (!ConfigureCams)
        {
            return;
        }
        var spawner = FindObjectOfType<PlayerSpawn>();
        spawner.AddListener(SetCameraTarget);
        Confiner.m_BoundingShape2D = GetComponent<PolygonCollider2D>();
    }

    private void SetCameraTarget(Player player)
    {
        Camera.LookAt = player.transform;
        Camera.Follow = player.transform;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
            Camera.gameObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
            Camera.gameObject.SetActive(false);
    }

    private void Process()
    {
        Tilemap map = FindMap();

        if (map == null)
        {
            return;
        }

        List<RolePosition> tiles = GetTiles(map);
        var Roles = UpdateColliders(map, tiles);
        UpdateRenderers(Roles);
    }

    private void UpdateRenderers(List<RoleObject> roles)
    {
        for (int i = 0; i < roles.Count; i++)
        {
            var view = roles[i].GetComponent<RoleView>();
            view.SpriteSwap = roles[i].GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
            view.VisibilitySwap = roles[i].GetComponentsInChildren<Renderer>(includeInactive: true);
            view.ColorSwap = new Renderer[0];
        }
    }

    private List<RoleObject> UpdateColliders(Tilemap map, List<RolePosition> tiles)
    {
        var roles = new List<RoleObject>(GetComponentsInChildren<RoleObject>(includeInactive: true));
        var groups = tiles.GroupBy(x => x.Role).ToDictionary(x => x.Key, x => x.ToArray());

        for (int i = 0; i < roles.Count; i++)
        {
            for (int j = roles[i].transform.childCount - 1; j >= 0; j--)
            {
                DestroyImmediate(roles[i].transform.GetChild(j).gameObject);
            }

            if (groups.ContainsKey(roles[i].ActiveRole))
            {
                BuildColliders(map, roles[i], groups[roles[i].ActiveRole]);
                groups.Remove(roles[i].ActiveRole);
            }
        }

        //MissingRoles;
        foreach (var item in groups)
        {
            var obj = Instantiate(RolePrefab);
            obj.ActiveRole = item.Key;
            obj.name = item.Key.name;
            obj.transform.SetParent(transform);
            BuildColliders(map, obj, item.Value);
            roles.Add(obj);
        }
        return roles;
    }

    private Tilemap FindMap()
    {
        var layers = GetComponentsInChildren<LDtkComponentLayer>();
        Tilemap map = null;
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].LayerType == TypeEnum.IntGrid)
            {
                map = layers[i].GetComponentInChildren<Tilemap>(includeInactive: true);
            }
        }

        return map;
    }

    private List<RolePosition> GetTiles(Tilemap map)
    {
        var translations = RoleSettings.Roles.ToDictionary(x => x.Tile, x => x);
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

        return roles;
    }

    private void BuildColliders(Tilemap map, RoleObject roleObject, RolePosition[] rolePositions)
    {
        SpriteRenderer cell;
        for (int i = 0; i < rolePositions.Length; i++)
        {
            cell = Instantiate(CellPrefab).GetComponent<SpriteRenderer>();
            cell.transform.SetParent(roleObject.transform);
            cell.transform.position = map.CellToWorld(rolePositions[i].Position) + (Vector3)Vector2.one / 2f;
            cell.sprite = roleObject.ActiveRole.Theme.Sprite;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(MapRoleLoader))]
    [CanEditMultipleObjects]
    public class _Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Process"))
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    (targets[i] as MapRoleLoader).Process();
                }
            }
            base.OnInspectorGUI();
        }
    }
#endif
}