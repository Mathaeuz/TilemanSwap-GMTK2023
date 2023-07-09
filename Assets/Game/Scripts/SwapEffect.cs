using UnityEngine;
using UnityEngine.UI;

public class SwapEffect : MonoBehaviour
{
    public GameObject[] Stages;
    Image[] Images;
    int index = 0;

    RectTransform Rtransform;

    private void Awake()
    {
        Rtransform = GetComponent<RectTransform>();
        Images = GetComponentsInChildren<Image>(includeInactive: true);
    }

    public void Hide()
    {
        for (int i = 0; i < 5; i++)
        {
            Stages[i].SetActive(false);
        }
    }

    public bool Next()
    {
        if (index == Stages.Length)
        {
            return false;
        }
        Stages[index++].SetActive(true);
        return true;
    }

    public void Prepare(Transform transform1, Transform transform2)
    {
        index = 0;
        var delta = transform1.position - transform2.position;
        Rtransform.right = delta.normalized;
        Rtransform.anchoredPosition = SwapSelector.Instance.GetCanvasPosition(transform2.position + delta / 2);

        var scale = delta.magnitude;
        Rtransform.localScale = Vector3.one * delta.magnitude;

        var imageScale = Vector3.one / scale;
        for (int i = 0; i < Images.Length; i++)
        {
            Images[i].transform.localScale = imageScale;
        }
    }
}