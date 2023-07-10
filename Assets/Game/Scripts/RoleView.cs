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
    Sprite BurstSprite;

    private void Awake()
    {
        Object = GetComponent<RoleObject>();
        Object.OnChangeRole.AddListener(SetRoleStyle);
        Object.RoleDestroyed.AddListener(BurstBlocks);
        Object.RoleRestored.AddListener(RestoreBlocks);
        SetRoleStyle(null, Object.ActiveRole);
        Burst.Init();
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
        for (int i = 0; i < TileSwap.Length; i++)
        {
            BurstTilemap(TileSwap[i]);
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

    private void BurstTilemap(Tilemap tilemap)
    {

    }

    private void SwapTilemap(Tilemap tilemap, TileBase[] old, TileBase[] current)
    {
        for (int i = 0; i < old.Length; i++)
        {
            tilemap.SwapTile(old[i], current[i]);
        }
    }
}