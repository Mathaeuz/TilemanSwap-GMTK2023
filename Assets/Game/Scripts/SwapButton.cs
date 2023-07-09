using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwapButton : MonoBehaviour
{
    public Button Button;
    public Image Border, Icon;
    public WorldToUi WorldToUI;
    public AudioClip BipClip;
    public EventTrigger Trigger;

    SwapHolder Holder;

    public void Configure(SwapHolder holder)
    {
        Holder = holder;
        if (Holder.ButtonEnabled)
        {
            var ev = new EventTrigger.TriggerEvent();
            ev.AddListener(PlayBip);
            Trigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>() {new EventTrigger.Entry
            {
                callback = ev,
                eventID = EventTriggerType.PointerEnter,
            } };
            Button.onClick.AddListener(SwapSelect);
            Holder.OnChangeRole += ChangeUI;
        }
        else
        {
            Button.enabled = false;
            Border.enabled = false;
        }
        ChangeUI(holder.ActiveRole);
        WorldToUI.Target = holder.transform;
    }

    private void PlayBip(BaseEventData arg0)
    {
        if (Button.IsInteractable())
        {
            SharedSoundEmiter.Instance.Play(BipClip);
        }
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
