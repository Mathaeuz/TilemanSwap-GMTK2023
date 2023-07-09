using System;
using System.Collections.Generic;
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

    Dictionary<AudioClip, float> Timeouts = new Dictionary<AudioClip, float>();
    public void PlayWithCooldown(AudioClip clip, float cooldown)
    {
        if (!Timeouts.ContainsKey(clip) || Timeouts[clip] < Time.time)
        {
            Timeouts[clip] = Time.time + cooldown;
            Play(clip);
        }
    }
}