using System.Collections;
using UnityEngine;

public class Tooltip : PlayerTrigger
{
    public SpriteRenderer Renderer;
    public float FadeLength = 0.2f;

    // Start is called before the first frame update
    void Awake()
    {
        ContactEnter.AddListener((other) => ContactEvent(true, other));
        ContactExit.AddListener((other) => ContactEvent(false, other));
        Renderer.color = Color.clear;
    }

    private void ContactEvent(bool contact, Collider2D other)
    {
        if (other.CompareTag(nameof(Player)))
        {
            StartCoroutine(Fade(contact ? Color.white : Color.clear));
        }
    }

    private IEnumerator Fade(Color target)
    {
        var origin = Renderer.color;
        var timer = FadeLength;
        while (timer > 0)
        {
            Renderer.color = Color.Lerp(target, origin, timer / FadeLength);
            timer -= Time.unscaledDeltaTime;
            yield return 0;
        }
        Renderer.color = target;
    }
}
