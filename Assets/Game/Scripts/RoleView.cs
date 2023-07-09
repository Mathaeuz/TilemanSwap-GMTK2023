using System;
using UnityEngine;

[RequireComponent(typeof(SharedParticles))]
public class RoleView : MonoBehaviour
{
    RoleObject Object;
    public SpriteRenderer[] SpriteSwap;
    public Renderer[] ColorSwap;
    public Renderer[] VisibilitySwap;
    public SharedParticles Burst;
    Sprite BurstSprite;

    private void Awake()
    {
        Object = GetComponent<RoleObject>();
        Object.OnChangeRole.AddListener(SetRoleStyle);
        Object.RoleDestroyed.AddListener(BurstBlocks);
        SetRoleStyle(Object.ActiveRole);
        Burst.Init();
    }

    private void BurstBlocks(float arg0)
    {
        Burst.SetSprite(BurstSprite);
        for (int i = 0;  i < SpriteSwap.Length;  i++)
        {
            Burst.Emit(SpriteSwap[i].transform.position);
        }
    }

    private void OnDestroy()
    {
        if (Object == null)
        {
            return;
        }
        Object.OnChangeRole.RemoveListener(SetRoleStyle);
    }

    public void SetRoleStyle(Role role)
    {
        if (role == null)
        {
            return;
        }
        var theme = RoleManager.Instance.ThemeMap[role];
        BurstSprite = theme.Sprite;

        for (int i = 0; i < SpriteSwap.Length; i++)
        {
            SpriteSwap[i].sprite = theme.Sprite;
        }
        for (int i = 0; i < ColorSwap.Length; i++)
        {
            ColorSwap[i].material.color = theme.Color;
        }
    }

    public void SetVisible(bool value)
    {
        for (int i = 0; i < VisibilitySwap.Length; i++)
        {
            VisibilitySwap[i].enabled = value;
        }
    }
}