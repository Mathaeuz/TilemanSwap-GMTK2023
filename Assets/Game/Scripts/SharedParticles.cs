using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SharedParticles : MonoBehaviour
{
    public int EmitCount = 10;
    public ParticleSystem Prefab;
    static Dictionary<ParticleSystem, ParticleSystem> PrefabMap = new Dictionary<ParticleSystem, ParticleSystem>();

    public void Init()
    {
        if (!PrefabMap.ContainsKey(Prefab) || PrefabMap[Prefab] == null)
        {
            PrefabMap[Prefab] = Instantiate(Prefab);
        }
    }

    public void Emit(Vector3 position)
    {
        var ps = PrefabMap[Prefab];
        ps.transform.position = position;
        ps.Emit(EmitCount);
    }

    public void Play(Vector3 position)
    {
        var ps = PrefabMap[Prefab];
        ps.transform.position = position;
        ps.Play();
    }

    public void SetSprite(Sprite sprite)
    {
        var ps = PrefabMap[Prefab];
        ps.textureSheetAnimation.SetSprite(0, sprite);
    }
}
