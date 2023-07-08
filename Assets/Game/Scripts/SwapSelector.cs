using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SwapSelector : Singleton<SwapSelector>
{
    public SwapButton SwapPrefab;
    public Button CancelButton;
    public float LerpDuration = 0.3f;
    public SwapEffect SwapEffect;
    SwapHolder SwapWithHolder;
    Player Target;
    CanvasGroup Group;
    RectTransform Rtransform;
    public bool Off;

    private void Awake()
    {
        Rtransform = (RectTransform)transform.parent;
        Group = GetComponent<CanvasGroup>();
        Group.interactable = false;
        Off = false;
        CancelButton.onClick.AddListener(EndSelection);
        Invoke(nameof(AllowSelection), 0.2f);
    }

    public void AddSwapButton(SwapHolder holder)
    {
        var bt = Instantiate(SwapPrefab);
        bt.transform.SetParent(transform);
        bt.Configure(holder);
        bt.gameObject.SetActive(true);
    }

    public void BeginSelection(Player player)
    {
        Off = false;
        Target = player;
        StopCoroutine(nameof(HideGroup));
        StartCoroutine(nameof(ShowGroup));
    }

    public void EndSelection()
    {
        StopCoroutine(nameof(ShowGroup));
        StartCoroutine(nameof(HideGroup));
        Invoke(nameof(AllowSelection), 0.2f);

    }

    private void AllowSelection()
    {
        Off = true;
    }
    public void EndSelection(SwapHolder holder)
    {
        if (SwapWithHolder != null)
        {
            return;
        }

        if (Target.CanSwap(holder.ActiveRole))
        {
            SwapWithHolder = holder;
            StartCoroutine(nameof(SwapAnimation));
        }
        else
        {
            EndSelection();
        }
    }

    private IEnumerator SwapAnimation()
    {
        SwapEffect.Prepare(SwapWithHolder.transform, Target.transform);
        while (SwapEffect.Next())
        {
            yield return new WaitForSecondsRealtime(0.2f);
        }
        Target.SwapWith(SwapWithHolder.ActiveRole);
        SwapWithHolder = null;
        EndSelection();
        SwapEffect.Hide();
    }

    private IEnumerator ShowGroup()
    {
        var alpha = Group.alpha;
        var time = LerpDuration * (1 - Group.alpha);
        while (time > 0)
        {
            Group.alpha = Mathf.SmoothStep(1, 0, time / LerpDuration);
            Time.timeScale = 1 - Group.alpha;
            time -= Time.unscaledDeltaTime;
            yield return 0;
        }
        Group.alpha = 1;
        Time.timeScale = 0;

        Group.interactable = true;
    }

    private IEnumerator HideGroup()
    {
        Group.interactable = false;
        var alpha = Group.alpha;
        var time = LerpDuration * Group.alpha;
        while (time > 0)
        {
            Group.alpha = Mathf.SmoothStep(0, 1, time / LerpDuration);
            Time.timeScale = 1 - Group.alpha;
            time -= Time.unscaledDeltaTime;
            yield return 0;
        }
        Group.alpha = 0;
        Time.timeScale = 1;
    }

    public Vector3 GetCanvasPosition(Vector3 worldPosition)
    {
        Vector3 viewPortPos = Camera.main.WorldToViewportPoint(worldPosition);
        return new Vector3(
        ((viewPortPos.x * Rtransform.sizeDelta.x) - (Rtransform.sizeDelta.x * 0.5f)),
        ((viewPortPos.y * Rtransform.sizeDelta.y) - (Rtransform.sizeDelta.y * 0.5f)),
        viewPortPos.z);
    }
}
