using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TestControlPool : MonoBehaviour
{
    private IObjectPool<FireballMove> _fireballPool;

    [SerializeField] private GameObject _fireballPrefab;

    private void Awake()
    {
        _fireballPool = new ObjectPool<FireballMove>(CreateFireball, OnGetFireball, OnReleaseFireball, OnDestroyFireball);
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            OnAttack();
        }
    }

    private void OnAttack()
    { 
        var fireball = _fireballPool.Get();
        fireball.Shoot();
        fireball.transform.position = transform.position;
        fireball.transform.rotation = transform.rotation;
    }
    private FireballMove CreateFireball()
    {
        FireballMove fireball = Instantiate(_fireballPrefab, transform.position, transform.rotation, gameObject.transform).GetComponent<FireballMove>();
        fireball.SetManagedPool(_fireballPool);
        return fireball;
    }
    private void OnGetFireball(FireballMove fireball)
    { 
        fireball.gameObject.SetActive(true);
    }
    private void OnReleaseFireball(FireballMove fireball)
    {
        fireball.gameObject.SetActive(false);
    }
    private void OnDestroyFireball(FireballMove fireball)
    {
        Destroy(fireball.gameObject);
    }

}
