using UnityEngine;

public class RoleView : MonoBehaviour
{
    RoleObject Behaviour;
    public SpriteRenderer[] SpriteSwap;
    public Renderer[] ColorSwap;
    public Renderer[] VisibilitySwap;

    private void Awake()
    {
        Behaviour = GetComponent<RoleObject>();
        Behaviour.OnChangeRole.AddListener(SetRoleStyle);
        SetRoleStyle(Behaviour.ActiveRole);
    }

    private void OnDestroy()
    {
        if (Behaviour == null)
        {
            return;
        }
        Behaviour.OnChangeRole.RemoveListener(SetRoleStyle);
    }

    public void SetRoleStyle(Role role)
    {
        if (role == null)
        {
            return;
        }
        var theme = RoleManager.Instance.ThemeMap[role];

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