using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MusicPlayer : Singleton<MusicPlayer>
{
    public bool PlayOnWake, StartRandom;
    public AudioClip[] Clips;
    AudioSource Source;
    public float FadeLength = 0.5f;
    float TargetVolume = 0.6f;
    int index = 0;

    public UnityEvent<AudioClip> OnChangeSong = new();

    private void Awake()
    {
        Source = GetComponent<AudioSource>();
        Source.loop = false;
        if (PlayOnWake)
        {
            Play();
        }
    }

    public void FadeTo(float volume)
    {
        TargetVolume = volume;
        StartCoroutine(Fade(volume));
    }

    public void Play()
    {
        if (Clips == null || Clips.Length == 0)
        {
            return;
        }
        if (StartRandom)
        {
            index = Random.Range(0, Clips.Length);
        }

        StopCoroutine(nameof(Playlist));
        StartCoroutine(nameof(Playlist));
    }

    private void OnDestroy()
    {
        StopCoroutine(nameof(Playlist));
    }

    private IEnumerator Playlist()
    {
        while (true)
        {
            Source.Stop();
            Source.clip = Clips[index];
            Source.volume = 0;
            Source.Play();
            OnChangeSong.Invoke(Source.clip);
            index = (index + 1) % Clips.Length;
            StartCoroutine(Fade(TargetVolume));
            yield return new WaitForSeconds(Source.clip.length - FadeLength);
            StartCoroutine(Fade(0f));
        }
    }

    private IEnumerator Fade(float target)
    {
        var origin = Source.volume;
        var timer = FadeLength;
        while (timer > 0)
        {
            Source.volume = Mathf.Lerp(target, origin, timer / FadeLength);
            timer -= Time.unscaledDeltaTime;
            yield return 0;
        }
        Source.volume = target;
    }
}
