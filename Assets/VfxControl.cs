using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class VfxControl : MonoBehaviour
{
    [SerializeField] public float _lifeTime = 0.4f;
    [SerializeField] public List<ParticleSystem> ParticleSystems;

    public void StartParticle()
    {
        foreach (var particle in ParticleSystems)
        {
            particle.Play();
        }
        VfxStopTask().Forget();
    }
    private async UniTaskVoid VfxStopTask()
    {
        int delayTime = (int)(_lifeTime * 1000);
        await UniTask.Delay(delayTime);
        foreach (var particle in ParticleSystems)
        {
            particle.Stop();
        }
    }
}
