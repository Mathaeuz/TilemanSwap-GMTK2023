using UnityEngine;
using UnityEngine.UIElements;

public class WorldToUi : MonoBehaviour
{
    public Transform Target;
    RectTransform Rtransform;

    private void Awake()
    {
        Rtransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (Target == null)
        {
            return;
        }

        var anchor = SwapSelector.Instance.GetCanvasPosition(Target.position);
        Rtransform.localScale = anchor.z > 0 ? Vector3.one : Vector3.zero;
        Rtransform.anchoredPosition = (Vector3)anchor;
    }
}