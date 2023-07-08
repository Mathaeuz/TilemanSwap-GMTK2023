using UnityEngine;

public class SwapEffect : MonoBehaviour
{
    public GameObject[] Stages;
    int index = 0;

    SpriteRenderer[] Renderers;

    private void Awake()
    {
        Renderers = GetComponentsInChildren<SpriteRenderer>(includeInactive:true);
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
        transform.position = transform2.position + delta / 2;
        transform.right = delta.normalized;
        var scale = delta.magnitude;

        for (int i = 0; i < Renderers.Length; i++)
        {
            Renderers[i].transform.localPosition = Renderers[i].transform.localPosition.normalized * scale / 2f;
        }
    }
}