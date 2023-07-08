using LDtkUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "New Role Tile Data")]
public class TextureRoleTileData : ScriptableObject
{
    public LDtkUnity.LDtkComponentProject Map;
    public ArtTileRoles[] Packs;

#if UNITY_EDITOR
    private void Extract()
    {
        List<LDtkArtTile> tiles = new();
        var importer = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(Map));
        for (int i = 0; i < importer.Length; i++)
        {
            if (importer[i] is LDtkArtTile)
            {
                tiles.Add(importer[i] as LDtkArtTile);
            }
        }
        var group = tiles.GroupBy(x => x._artSprite.texture).ToDictionary(x => x.Key, x => x.OrderBy(x => x.name).ToArray());

        for (int i = 0; i < Packs.Length; i++)
        {
            if (!group.ContainsKey(Packs[i].Texture))
            {
                continue;
            }
            Packs[i].Tiles = group[Packs[i].Texture];
        }
    }
    [CustomEditor(typeof(TextureRoleTileData))]
    public class _Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Extract"))
            {
                (target as TextureRoleTileData).Extract();
            }
        }
    }
#endif
}

[Serializable]
public class ArtTileRoles
{
    public Role Role;
    public Texture2D Texture;
    public LDtkArtTile[] Tiles;
}