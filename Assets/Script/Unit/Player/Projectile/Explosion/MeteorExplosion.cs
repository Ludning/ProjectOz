using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class MeteorExplosion : MonoBehaviour
{
    private OzmagicData _meteorData_OzMagic;

    IObjectPool<MeteorExplosion> pool;

    private Vector3 _upScale;
    private Vector3 _defaultScale;

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

        _upScale = new Vector3(_upScaleValue, _upScaleValue, 0f);
        _defaultScale = transform.localScale;
    }

    private  void OnEnable()
    {
        _isDestroyed = false;
        transform.localScale = _defaultScale;
    }

    private void Update()
    {
        Explosion();

        if (transform.localScale.x >= _explosionRadius)
        {
            DestroyExplosion();
        }
    }

    public void RushHit(bool isHit)
    {
        isPlayerHit = isHit;
    }

    private void Explosion()
    {
        transform.localScale += (_upScale * Time.deltaTime);
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
