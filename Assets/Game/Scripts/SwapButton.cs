using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;

public class SwapButton : MonoBehaviour
{
    public Button Button;
    public Image Border, Icon;
    public WorldToUi WorldToUI;

    SwapHolder Holder;

    public void Configure(SwapHolder holder)
    {
        Button.onClick.AddListener(SwapSelect);
        Holder = holder;
        Holder.OnChangeRole += ChangeUI;
        ChangeUI(holder.ActiveRole);
        WorldToUI.Target = holder.transform;
    }

    private void ChangeUI(Role role)
    {
        var data = RoleManager.Instance.RoleSettings.Get(role);
        if (data == null)
        {
            return;
        }
        Border.color = data.Theme.Color;
        Icon.sprite = data.Theme.Sprite;
    }

    private void SwapSelect()
    {
        SwapSelector.Instance.EndSelection(Holder);
    }
}
