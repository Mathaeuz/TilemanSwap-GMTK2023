﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SharedParticles))]
public class RoleView : MonoBehaviour
{
    RoleObject Object;
    public SpriteRenderer[] SpriteSwap;
    public Tilemap[] TileSwap;
    public Renderer[] VisibilitySwap;
    public SharedParticles Burst;
    Vector3[] BurstPositions;
    Sprite BurstSprite;

    private void Awake()
    {
        Object = GetComponent<RoleObject>();
        Object.OnChangeRole.AddListener(SetRoleStyle);
        Object.RoleDestroyed.AddListener(BurstBlocks);
        Object.RoleRestored.AddListener(RestoreBlocks);
        BuildBurstDictionary();
        SetRoleStyle(null, Object.ActiveRole);
        Burst.Init();
    }

    public void BuildBurstDictionary()
    {
        List<Vector3> positions = new();
        for (int k = 0; k < TileSwap.Length; k++)
        {
            var bounds = TileSwap[k].cellBounds;
            Vector3Int pos;
            for (int i = bounds.min.y; i < bounds.max.y; i++)
            {
                for (int j = bounds.min.x; j < bounds.max.x; j++)
                {
                    pos = new Vector3Int(j, i);
                    if (TileSwap[k].HasTile(pos))
                    {
                        positions.Add(TileSwap[k].CellToWorld(pos));
                    }
                }
            }
        }
        BurstPositions = positions.ToArray();
    }

    private void OnDestroy()
    {
        if (Object == null)
        {
            return;
        }
        Object.OnChangeRole.RemoveListener(SetRoleStyle);
    }

    private void RestoreBlocks()
    {
        SharedSoundEmiter.Instance.Play(Object.ActiveRole.RespawnEffect);
    }

    private void BurstBlocks(float time)
    {
        SharedSoundEmiter.Instance.Play(Object.ActiveRole.PopEffect);
        Burst.SetSprite(BurstSprite);
        for (int i = 0; i < SpriteSwap.Length; i++)
        {
            Burst.Emit(SpriteSwap[i].transform.position);
        }
        for (int i = 0; i < BurstPositions.Length; i++)
        {
            Burst.Emit(BurstPositions[i]);
        }
    }


    public void SetRoleStyle(Role oldRole, Role role)
    {
        if (role == null)
        {
            return;
        }
        BurstSprite = role.Theme.Sprite;

        if (oldRole == role)
        {
            return;
        }

        for (int i = 0; i < SpriteSwap.Length; i++)
        {
            SpriteSwap[i].sprite = role.Theme.Sprite;
        }

        if (oldRole != null)
        {
            for (int i = 0; i < TileSwap.Length; i++)
            {
                SwapTilemap(TileSwap[i], oldRole.Theme.Tiles, role.Theme.Tiles);
            }
        }
    }

    public void SetVisible(bool value)
    {
        for (int i = 0; i < VisibilitySwap.Length; i++)
        {
            VisibilitySwap[i].enabled = value;
        }
    }

    private void SwapTilemap(Tilemap tilemap, TileBase[] old, TileBase[] current)
    {
        for (int i = 0; i < old.Length; i++)
        {
            if (old[i] != null && current[i] != null)
            {
                tilemap.SwapTile(old[i], current[i]);
            }
        }
    }
}