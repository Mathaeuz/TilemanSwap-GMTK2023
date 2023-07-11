using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
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

    private void Awake()
    {
        ConfigureCameras();
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
        if (collider.CompareTag(nameof(Player)))
            Camera.gameObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag(nameof(Player)))
            Camera.gameObject.SetActive(false);
    }
#if UNITY_EDITOR

    private void Process()
    {
        var renderers = GetComponentsInChildren<TilemapRenderer>();
        Dictionary<Role, TilemapRenderer> rendererMap = new();
        for (int i = 0; i < renderers.Length; i++)
        {
            if (Enum.TryParse(renderers[i].transform.parent.name, out RoleSwap enumVal))
            {
                rendererMap[RoleSettings.Get(enumVal)] = renderers[i];
            }
        }

        var configuredObjects = GetComponentsInChildren<RoleObject>().ToDictionary(x => x.ActiveRole, x => x);
        var objectColliders = GetComponentsInChildren<Rigidbody2D>();

        Role role;
        RoleObject roleObject;
        RoleView view;
        TilemapRenderer renderer;
        for (int i = 0; i < objectColliders.Length; i++)
        {
            role = RoleSettings.Get(objectColliders[i].sharedMaterial);

            if (role == null)
            {
                continue;
            }

            if (configuredObjects.ContainsKey(role))
            {
                roleObject = configuredObjects[role];
            }
            else
            {
                roleObject = objectColliders[i].gameObject.AddComponent<RoleObject>();
                roleObject.ActiveRole = role;
                configuredObjects[role] = roleObject;
            }

            roleObject.Physics = roleObject.GetComponent<RolePhysics>();
            if (roleObject.Physics == null)
            {
                roleObject.Physics = roleObject.gameObject.AddComponent<RolePhysics>();
            }
            roleObject.Physics.AutoConfigure();

            renderer = rendererMap[role];
            view = roleObject.GetComponent<RoleView>();
            if (view == null)
            {
                view = roleObject.gameObject.AddComponent<RoleView>();
            }
            view.TileSwap = new[] { renderer.GetComponent<Tilemap>() };
            view.VisibilitySwap = new[] { renderer };
        }
    }

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