using UnityEngine;

public class RoleView : MonoBehaviour
{
    RoleObject Behaviour;
    public Renderer[] Targets;

    private void Awake()
    {
        if (Targets == null || Targets.Length == 0)
        {
            Targets = GetComponentsInChildren<Renderer>();
        }
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

    private void SetRoleStyle(Role role)
    {
        if (role == null)
        {
            return;
        }
        var theme = RoleManager.Instance.ThemeMap[role];

        for (int i = 0; i < Targets.Length; i++)
        {
            if (Targets[i] is SpriteRenderer && !Targets[i].CompareTag("NoSwap"))
            {
                (Targets[i] as SpriteRenderer).sprite = theme.Sprite;
                continue;
            }
            Targets[i].material.color = theme.Color;
        }
    }
}