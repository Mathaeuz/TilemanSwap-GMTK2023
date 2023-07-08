using System.Collections.Generic;
using UnityEngine;

public class SwapSense : MonoBehaviour
{
    public class Option
    {
        public Role Role;
        public Vector2 Position;
    }

    Dictionary<Collider2D, RoleObject> mObjectMap = new();

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var obj = collider.GetComponent<RoleObject>();
        if (obj == null)
        {
            return;
        }
        if (obj.ActiveRole.Swappable)
        {
            mObjectMap[collider] = obj;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        mObjectMap.Remove(collision);
    }

    public bool HasOptions()
    {
        return mObjectMap.Count > 0;
    }

    public List<Option> GetOptions()
    {
        var options = new List<Option>();

        foreach (var item in mObjectMap.Values)
        {
            options.Add(new Option
            {
                Role = item.ActiveRole,
                Position = item.transform.position
            });
        }
        return options;
    }
}
