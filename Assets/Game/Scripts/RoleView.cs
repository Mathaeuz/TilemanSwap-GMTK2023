using UnityEngine;

public class RoleView : MonoBehaviour
{
    RoleObject Behaviour;
    public Renderer Target;

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

    private void SetRoleStyle(Role role)
    {
        var theme = RoleManager.Instance.ThemeMap[role];
        Target.material.color = theme;
    }
}