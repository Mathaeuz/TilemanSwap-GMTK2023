using LDtkUnity;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Role Settings")]
public class RoleSettings : ScriptableObject
{
    public LDtkArtifactAssets TargetMap;
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

    public Role Get(PhysicsMaterial2D sharedMaterial)
    {
        for (int i = 0; i < Roles.Length; i++)
        {
            if (Roles[i].Material == sharedMaterial)
            {
                return Roles[i];
            }
        }
        return null;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RoleSettings))]
    public class _Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Load Tiles"))
            {
                LoadTiles(target as RoleSettings);
            }
            base.OnInspectorGUI();
        }

        private void LoadTiles(RoleSettings roleSettings)
        {
            var tiles = roleSettings.TargetMap.TileArtifacts
                .Where(x => x is LDtkArtTile)
                .Select(x => x as LDtkArtTile)
                .GroupBy(x => x._artSprite.texture)
                .ToDictionary(x => x.Key, x => x.OrderBy(x => x.name).ToArray());

            for (int i = 0; i < roleSettings.Roles.Length; i++)
            {
                if (roleSettings.Roles[i].Theme.TileSource == null)
                {
                    Debug.LogWarning($"Missing Tile source on role [{roleSettings.Roles[i].name}]");
                    continue;
                }

                if (!tiles.ContainsKey(roleSettings.Roles[i].Theme.TileSource))
                {
                    Debug.LogWarning($"Role [{roleSettings.Roles[i].name}] has no ArtTiles on Target [${roleSettings.TargetMap.name}]");
                    continue;
                }

                roleSettings.Roles[i].Theme.Tiles = tiles[roleSettings.Roles[i].Theme.TileSource];
                EditorUtility.SetDirty(roleSettings.Roles[i]);
            }
        }
    }
#endif
}