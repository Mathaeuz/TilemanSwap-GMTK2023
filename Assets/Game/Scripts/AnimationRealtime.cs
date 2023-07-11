using UnityEngine;

public class AnimationRealtime : MonoBehaviour
{
    Animation Animation;

    void Awake()
    {
        Animation = GetComponent<Animation>();
    }

    private void Update()
    {
        if (Time.timeScale != 0 || !Animation.isPlaying) return;
        AnimationState currentState = Animation[Animation.clip.name];
        currentState.time += Time.unscaledDeltaTime - Time.deltaTime;
        Animation.Sample();
    }
}