using System;
using System.Collections;
using UnityEngine;

public class BaloonFlicker : MonoBehaviour
{
    public float FlickerBeforeShow = 0.5f;
    public RoleView View;

    float RoutineTime;

    void Awake()
    {
        if (View == null)
        {
            View = GetComponent<RoleView>();
        }
        var baloon = GetComponent<BaloonBehaviour>();
        baloon.OnSetObject += ConfigurePop;
        if (baloon.Object != null)
        {
            ConfigurePop(baloon.Object);
        }
    }

    private void ConfigurePop(RoleObject obj)
    {
        obj.RoleDestroyed.AddListener(Hide);
        obj.RoleRestored.AddListener(Show);
    }

    public void Hide(float time)
    {
        RoutineTime = time;
        StopCoroutine(nameof(HideRoutine));
        StartCoroutine(nameof(HideRoutine));
    }

    public void Show()
    {
        StopCoroutine(nameof(HideRoutine));

        //Show
        View.SetVisible(true);
    }

    private IEnumerator HideRoutine()
    {
        //Hide
        View.SetVisible(false);
        yield return new WaitForSeconds(RoutineTime - FlickerBeforeShow);

        //Flicker
        var val = false;
        while (true)
        {
            val = !val;
            View.SetVisible(val);
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }
}