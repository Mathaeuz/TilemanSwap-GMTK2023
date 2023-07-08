using LDtkUnity;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

//[ExecuteInEditMode]
public class TextureRoleLevelApplier : MonoBehaviour
{
    public TextureRoleTileData DataSource;

    class Data
    {
        public LDtkArtTile[] Tiles;
        public Tilemap Map;
        public Role Role;
    }

    Dictionary<Role, Data> RoleMap = new();

    private void Awake()
    {
        var textureMap = LoadData();
        FindTilemaps(textureMap);
    }

    private Dictionary<Texture2D, Data> LoadData()
    {
        var textureMap = new Dictionary<Texture2D, Data>();
        for (int i = 0; i < DataSource.Packs.Length; i++)
        {
            var data = new Data
            {
                Role = DataSource.Packs[i].Role,
                Tiles = DataSource.Packs[i].Tiles,
            };
            textureMap[DataSource.Packs[i].Texture] = data;
            RoleMap[DataSource.Packs[i].Role] = data;
        }

        return textureMap;
    }

    private void FindTilemaps(Dictionary<Texture2D, Data> textureMap)
    {
        var targets = GetComponentsInChildren<Tilemap>();
        Sprite[] usedSprites;
        Data data;
        for (int i = 0; i < targets.Length; i++)
        {
            usedSprites = new Sprite[targets[i].GetUsedSpritesCount()];
            if (usedSprites.Length == 0)
                continue;

            targets[i].GetUsedSpritesNonAlloc(usedSprites);
            data = textureMap[usedSprites[0].texture];
            Configure(data, targets[i]);
        }
    }

    private void Configure(Data data, Tilemap tilemap)
    {
        data.Map = tilemap;
        tilemap.AddComponent<TilemapCollider2D>();
        var obj = tilemap.AddComponent<RoleObject>();
        obj.ActiveRole = data.Role;
        RoleManager.Instance.Register(obj);
        tilemap.AddComponent<RoleView>();
    }

    public bool Swap;
    public Role A, B;
    private void Update()
    {
        if (Swap)
        {
            Swap = false;

            var a = RoleMap[A];
            var b = RoleMap[B];

            for (int i = 0; i < a.Tiles.Length; i++)
            {
                a.Map.SwapTile(a.Tiles[i], b.Tiles[i]);
                b.Map.SwapTile(b.Tiles[i], a.Tiles[i]);
            }
            var m = a.Map;
            a.Map = b.Map;
            b.Map = m;
        }

    }

    /*
    Dictionary<Role, RuntimeTileData> Data;

    public bool Swap;
    public Role A, B;

    class RuntimeTileData
    {
        public LDtkArtTile[] Tiles;
        public Texture2D Texture;
    }

    private void Awake()
    {
        Data = new Dictionary<Role, RuntimeTileData>();
        for (int i = 0; i < DataSource.Packs.Length; i++)
        {
            Data[DataSource.Packs[i].Role] = new RuntimeTileData
            {
                Tiles = DataSource.Packs[i].Tiles,
                Texture = DataSource.Packs[i].Texture,
            };
        }
    }
    private void Update()
    {
        if (Swap)
        {
            Swap = false;
            var a = Data[A];
            var b = Data[B];
            var t = a.Tiles;
            a.Tiles = b.Tiles;
            b.Tiles = t;

            for (int i = 0; i < a.Tiles.Length; i++)
            {
            }
        }
    }*/
}
