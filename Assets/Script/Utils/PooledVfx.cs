using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PooledVfx : MonoBehaviour
{
    private IObjectPool<PooledVfx> _vfxPool;
    [SerializeField] private GameObject EffectPrefab;
    private void Awake()
    {
        _vfxPool = new ObjectPool<PooledVfx>(CreatePool);
    }

    public PooledVfx CreatePool()
    {
        return Instantiate(EffectPrefab).GetComponent<PooledVfx>();
    }

    public void Play(Vector3 pos)
    {
        PooledVfx currentPooled = _vfxPool.Get();
        ParticleSystem particle = currentPooled.GetComponent<ParticleSystem>();
        
        particle.Play();
        StartCoroutine(DelayedCall(particle.main.duration, currentPooled)) ;
    }

    private IEnumerator DelayedCall(float time, PooledVfx particle)
    {
        yield return new WaitForSeconds(time);
        _vfxPool.Release(particle);
    }

}
