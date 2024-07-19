using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MeteorExplosion : MonoBehaviour
{
    private OzmagicData _meteorData_OzMagic;

    IObjectPool<MeteorExplosion> pool;

    private Vector3 _upScale;

    private float _explosionRadius;
    private float _explosionDamage;
    [SerializeField] private float _upScaleValue;

    private bool _isDestroyed = false;
    public bool isPlayerHit;

    private void Awake()
    {
        _meteorData_OzMagic = DataManager.Instance.GetGameData<OzmagicData>("O101");
        _explosionRadius = _meteorData_OzMagic.value3;
        _explosionDamage = _meteorData_OzMagic.chainPowerRate;

        _upScale = new Vector3(_upScaleValue, _upScaleValue, 0f) * Time.deltaTime;
    }

    private  void OnEnable()
    {
        _isDestroyed = false;
    }

    private void Update()
    {
        Explosion();

        if (transform.localScale.x >= _explosionRadius)
        {
            DestroyExplosion();
        }
    }

    public void Explosion()
    {
        transform.localScale += _upScale;
    }

    public void SetManagedPool(IObjectPool<MeteorExplosion> Pool)
    {
        pool = Pool;
    }
    public void DestroyExplosion()
    {
        if (_isDestroyed) return;

        _isDestroyed = true;
        pool.Release(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (other.TryGetComponent(out Combat combat))
            {
                combat.Damaged(_explosionDamage);
            }
        }
        if (other.gameObject.CompareTag("Player"))
        {
            if (isPlayerHit) return;

            if (other.TryGetComponent(out Combat combat))
            {
                combat.Damaged(_explosionDamage);
            }
        }
    }
}
