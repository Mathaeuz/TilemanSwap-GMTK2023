using UnityEngine;
using UnityEngine.UI;

public class SongNameDisplay : MonoBehaviour
{
    public Text Text;
    Animation Animation;
    // Start is called before the first frame update
    void Awake()
    {
        MusicPlayer.Instance.OnChangeSong.AddListener(ShowNewSong);
        Animation = Text.GetComponent<Animation>();
    }

    private void Update()
    {
        if (Time.timeScale != 0 || !Animation.isPlaying) return;
        AnimationState currentState = Animation[Animation.clip.name];
        currentState.time += Time.unscaledDeltaTime - Time.deltaTime;
        Animation.Sample();
    }

    private void ShowNewSong(AudioClip clip)
    {
        Text.text = $"♫ {clip.name}";
        Text.gameObject.SetActive(false);
        Text.gameObject.SetActive(true);

    }
}
