using UnityEngine;
using UnityEngine.Pool;

public abstract class OzMagic : MonoBehaviour
{
    protected IObjectPool<OzMagic> _ozMagicPool;

    public float _ozMagicPercentage;

    protected float _lifeTime;

    protected bool _isDestroyed;

    public abstract void Excute();

    protected virtual void OnEnable()
    {
        _isDestroyed = false;
    }

    public void SetManagedPool(IObjectPool<OzMagic> ozPool)
    { 
        _ozMagicPool = ozPool;
    }
    public void DestroyOzMagic()
    {
        if (_isDestroyed) return;

        _isDestroyed = true;
        _ozMagicPool.Release(this);
    }
}
