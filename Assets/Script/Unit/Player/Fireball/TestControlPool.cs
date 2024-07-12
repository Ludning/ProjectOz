using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TestControlPool : MonoBehaviour
{
    private IObjectPool<FireballMove> _fireballPool;
    private IObjectPool<FlameballMove> _flameballPool;

    [SerializeField] private GameObject _fireballPrefab;
    [SerializeField] private GameObject _flameballPrefab;

    private void Awake()
    {
        _fireballPool = new ObjectPool<FireballMove>(CreateFireball, OnGetFireball, OnReleaseFireball, OnDestroyFireball);
        _flameballPool = new ObjectPool<FlameballMove>(CreateFlameball, OnGetFlameball, OnReleaseFlameball, OnDestroyFlameball);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnAttack();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private void OnAttack(MonoBehaviour ball, IObjectPool<MonoBehaviour> ballPool)
    {
        ball = ballPool.Get();
        ball.transform.position = transform.position;
        ball.transform.rotation = transform.rotation;
        ball.Shoot();
    }





    private FireballMove CreateFireball()
    {
        FireballMove fireball = Instantiate(_fireballPrefab, transform.position, transform.rotation).GetComponent<FireballMove>();
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

    private FlameballMove CreateFlameball()
    {
        FlameballMove flameball = Instantiate(_flameballPrefab, transform.position, transform.rotation).GetComponent<FlameballMove>();
        flameball.SetManagedPool(_flameballPool);
        return flameball;
    }
    private void OnGetFlameball(FlameballMove flameball)
    {
        flameball.gameObject.SetActive(true);
    }
    private void OnReleaseFlameball(FlameballMove flameball)
    {
        flameball.gameObject.SetActive(false);
    }
    private void OnDestroyFlameball(FlameballMove flameball)
    {
        Destroy(flameball.gameObject);
    }
}
