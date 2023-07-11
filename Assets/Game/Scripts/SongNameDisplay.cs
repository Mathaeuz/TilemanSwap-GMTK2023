using UnityEngine;
using UnityEngine.UI;

public class SongNameDisplay : MonoBehaviour
{
    public Text Text;

    void Awake()
    {
        MusicPlayer.Instance.OnChangeSong.AddListener(ShowNewSong);
    }

    private void ShowNewSong(AudioClip clip)
    {
        Text.text = $"♫ {clip.name}";
        Text.gameObject.SetActive(false);
        Text.gameObject.SetActive(true);
    }
}
