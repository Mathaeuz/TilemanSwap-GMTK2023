using Cinemachine;
using LDtkUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class MapRoleLoader : MonoBehaviour
{
    public RoleSettings RoleSettings;
    public RoleObject RolePrefab;
    public Collider2D CellPrefab;
    public CinemachineVirtualCamera Camera;
    public CinemachineConfiner2D Confiner;

    [Serializable]
    public class RolePosition
    {
        public Vector3Int Position;
        public Role Role;
    }

    private void Start()
    {
        ConfigureCameras();
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            Process();
        }
#endif
    }

    private void ConfigureCameras()
    {
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

#if UNITY_EDITOR

    private void Process()
    {
        Tilemap map = FindMap();

        if (map == null)
        {
            return;
        }

        List<RolePosition> tiles = GetTiles(map);
        var roles = UpdateColliders(map, tiles);
        UpdateRenderers(roles);
    }

    private void UpdateRenderers(List<RoleObject> roles)
    {
        for (int i = 0; i < roles.Count; i++)
        {
            var view = roles[i].GetComponent<RoleView>();
            view.SpriteSwap = roles[i].GetComponentsInChildren<SpriteRenderer>();
            view.VisibilitySwap = roles[i].GetComponentsInChildren<Renderer>();
            view.ColorSwap = new Renderer[0];

        }
    }

    private List<RoleObject> UpdateColliders(Tilemap map, List<RolePosition> tiles)
    {
        var roles = new List<RoleObject>(GetComponentsInChildren<RoleObject>());
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
        foreach (var item in groups)
        {
            var obj = Instantiate(RolePrefab);//PrefabUtility.InstantiatePrefab(RolePrefab).GetComponent<RoleObject>();
            obj.ActiveRole = item.Key;
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
                map = layers[i].GetComponentInChildren<Tilemap>();
            }
        }

        return map;
    }

    private List<RolePosition> GetTiles(Tilemap map)
    {
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

        return roles;
    }

    private void BuildColliders(Tilemap map, RoleObject roleObject, RolePosition[] rolePositions)
    {
        SpriteRenderer cell;
        for (int i = 0; i < rolePositions.Length; i++)
        {
            cell = PrefabUtility.InstantiatePrefab(CellPrefab).GetComponent<SpriteRenderer>();
            cell.transform.SetParent(roleObject.transform);
            cell.transform.position = map.CellToWorld(rolePositions[i].Position) + (Vector3)Vector2.one / 2f;
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