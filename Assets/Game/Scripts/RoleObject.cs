using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RoleObject : MonoBehaviour
{
    public Dictionary<Role, RoleBehaviour> Behaviours = new();
    [HideInInspector]
    public RoleBehaviour ActiveBehaviour { get; private set; }

    [field: SerializeField]
    public Role ActiveRole { get; set; }

    public UnityEvent<Role> OnChangeRole = new();
    public UnityEvent<float> RoleDestroyed = new();
    public UnityEvent RoleRestored = new();

    public ColliderManipulator ColliderManagement;
    public Collider2D[] IgnoreMaterial, IgnoreTag;

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

    public void Change(Role role, bool forceRespawn)
    {
        if (ColliderManagement == null)
        {
            ColliderManagement = new ColliderManipulator(this, IgnoreMaterial, IgnoreTag);
        }

        ActiveRole = role;
        if (forceRespawn)
        {
            RestoreByRole();
        }

        if (ActiveBehaviour != null)
        {
            ActiveBehaviour.enabled = false;
        }

        if (Behaviours.Any())
        {
            ActiveBehaviour = Behaviours[role];
            ActiveBehaviour.enabled = true;
            OnChangeRole.Invoke(role);
        }
    }

    public void DestroyByRole(float time)
    {
        RoleDestroyed.Invoke(time);
    }

    public void RestoreByRole()
    {
        RoleRestored.Invoke();
    }
}