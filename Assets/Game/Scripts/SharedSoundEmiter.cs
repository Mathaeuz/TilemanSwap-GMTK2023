using System;
using UnityEngine;


public class SharedSoundEmiter : Singleton<SharedSoundEmiter>
{
    AudioSource[] Sources;
    int index = 0;

    private void Awake()
    {
        Sources = GetComponents<AudioSource>();
        RoleManager.Instance.AllowEffects.AddListener(gameObject.SetActive);
    }

    public void Play(AudioClip clip)
    {
        if (clip == null || !isActiveAndEnabled)
        {
            return;
        }

        //Debug.Log($"Playing: {clip.name}");
        var source = Sources[index];
        index = (index + 1) % Sources.Length;
        source.Stop();
        source.clip = clip;
        source.Play();
    }
}