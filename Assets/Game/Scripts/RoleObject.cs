using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoleObject : MonoBehaviour
{
    public Dictionary<Role, RoleBehaviour> Behaviours = new();
    [HideInInspector]
    public RoleBehaviour ActiveBehaviour { get; private set; }

    [field: SerializeField]
    public Role ActiveRole { get; private set; }

    public UnityEvent<Role> OnChangeRole;

    private void Awake()
    {
        RoleManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (RoleManager.Instance != null)
        {
            RoleManager.Instance.Release(this);
        }
    }

    public void Set(Role role)
    {
        ActiveRole = role;
        if (ActiveBehaviour != null)
        {
            ActiveBehaviour.enabled = false;
        }
        ActiveBehaviour = Behaviours[role];
        ActiveBehaviour.enabled = true;
        OnChangeRole.Invoke(role);
    }
}