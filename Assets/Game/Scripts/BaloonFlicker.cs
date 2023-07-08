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
        GetComponent<BaloonBehaviour>().OnSetObject += (obj) => obj.RoleDestroyed.AddListener(Hide);
    }

    public void Hide(float time)
    {
        RoutineTime = time;
        StopCoroutine(nameof(HideRoutine));
        StartCoroutine(nameof(HideRoutine));
    }

    private IEnumerator HideRoutine()
    {
        //Hide
        for (int i = 0; i < View.Targets.Length; i++)
        {
            View.Targets[i].enabled = false;
        }
        yield return new WaitForSeconds(RoutineTime - FlickerBeforeShow);

        //Flicker
        var timeout = Time.time + FlickerBeforeShow;
        var val = false;
        while (Time.time < timeout)
        {
            val = !val;
            for (int i = 0; i < View.Targets.Length; i++)
            {
                View.Targets[i].enabled = val;
            }
            yield return 0;
        }

        //Show
        for (int i = 0; i < View.Targets.Length; i++)
        {
            View.Targets[i].enabled = true;
        }
    }
}