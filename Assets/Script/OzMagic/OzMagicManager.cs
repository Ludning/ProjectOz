using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class OzMagicManager : SingleTonMono<OzMagicManager>
{
    private IObjectPool<OzMagic> _meteorPool;
    private IObjectPool<OzMagic> _timeStop;

    private IObjectPool<MeteorExplosion> _meteorExplosionPool;

    [SerializeField] private List<GameObject> Prefab_OzMagic = new List<GameObject>();
    [SerializeField] private List<float> _ozMagic_weights;

    [SerializeField] private GameObject _meteorExplosion;
    private MeteorExplosion meteorExplosion;
    public MeteorExplosion MeteorExplosion => meteorExplosion;

    private int _ozMagicIndex;

    private void Awake()
    {
        Prefab_OzMagic.Add(ResourceManager.Instance.LoadResource<GameObject>("Meteor"));
        Prefab_OzMagic.Add(ResourceManager.Instance.LoadResource<GameObject>("TimeStop"));

        _ozMagic_weights = new List<float>(new float[Prefab_OzMagic.Count]);

        for (int i = 0; i < Prefab_OzMagic.Count; i++)
        {
            var oz = Prefab_OzMagic[i].GetComponent<OzMagic>();
            _ozMagic_weights[i] = oz._ozMagicPercentage;
        }
        //_ozMagic_weights.Add(1f);

        _meteorPool = new ObjectPool<OzMagic>(() => CreateBall(Prefab_OzMagic[0], _meteorPool), OnGetBall, OnReleaseBall, OnDestroyBall);

        //[todo]
        _timeStop = new ObjectPool<OzMagic>(() => CreateBall(Prefab_OzMagic[1], _timeStop), OnGetBall, OnReleaseBall, OnDestroyBall);

        meteorExplosion = _meteorExplosion.GetComponent<MeteorExplosion>();

        _meteorExplosionPool = new ObjectPool<MeteorExplosion>(CreateExplosion, OnGetExplosion, OnReleaseExplosion, OnDestroyExplosion);
    }

    public AttackType Execute()
    {
        RandomOzMagic();
        switch (_ozMagicIndex)
        {
            case 0:
                OnExcute(_meteorPool);
                return AttackType.Meteor;
            case 1:
                OnExcute(_timeStop);
                return AttackType.TimeStop;
        }
        return AttackType.None;
    }

    private void RandomOzMagic()
    {
        _ozMagicIndex = GetRandomByWeight(_ozMagic_weights);
    }

    private int GetRandomByWeight(List<float> weight)
    {
        float totalWeight = 0;

        foreach (var i in weight)
        {
            totalWeight += i;
        }

        float randomValue = Random.Range(0, totalWeight);
        float accumulatedWeight = 0f;

        for (int i = 0; i < weight.Count; i++)
        {
            accumulatedWeight += weight[i];
            if (randomValue < accumulatedWeight)
            {
                return i;
            }
        }

        return 0;
    }

    private void OnExcute(IObjectPool<OzMagic> ozPool)
    {
        var oz = ozPool.Get();
        oz.Excute();
    }

    private OzMagic CreateBall(GameObject prefab, IObjectPool<OzMagic> ozPool)
    {
        var oz = Instantiate(prefab, transform.position, transform.rotation).GetComponent<OzMagic>();
        oz.SetManagedPool(ozPool);
        return oz;
    }

    private void OnGetBall(OzMagic oz)
    {
        oz.gameObject.SetActive(true);
    }
    private void OnReleaseBall(OzMagic oz)
    {
        oz.gameObject.SetActive(false);
    }
    private void OnDestroyBall(OzMagic oz)
    {
        Destroy(oz.gameObject);
    }



    public void OnExplosion(Vector2 pos)
    {
        var pool = _meteorExplosionPool.Get();
        pool.transform.position = pos;
        pool.Explosion();
    }

    private MeteorExplosion CreateExplosion()
    {
        var pool = Instantiate(_meteorExplosion, transform.position, transform.rotation).GetComponent<MeteorExplosion>();
        pool.SetManagedPool(_meteorExplosionPool);
        return pool;
    }

    private void OnGetExplosion(MeteorExplosion Pool)
    {
        Pool.gameObject.SetActive(true);
    }
    private void OnReleaseExplosion(MeteorExplosion Pool)
    {
        Pool.gameObject.SetActive(false);
    }
    private void OnDestroyExplosion(MeteorExplosion Pool)
    {
        Destroy(Pool.gameObject);
    }
}
