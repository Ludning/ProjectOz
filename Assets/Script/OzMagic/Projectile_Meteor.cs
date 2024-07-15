using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Meteor : OzMagic
{
    private OzmagicData _meteorData_OzMagic;
    private ProjectileData _meteorData_Projectile;

    private Rigidbody _rb;

    private Camera _camera;

    private Vector3 _spawnPos;

    private float _damageTimer;
    private float _hitCycle;
    private float _meteorDamage;
    private float _gravity;
    private float _rotSpeed;

    private void Awake()
    {
        _meteorData_OzMagic = DataManager.Instance.GetGameData<OzmagicData>("O101");
        _meteorData_Projectile = DataManager.Instance.GetGameData<ProjectileData>("P201");

        _rb = GetComponent<Rigidbody>();

        _ozMagicPercentage = _meteorData_OzMagic.ozSkillPercentage;

        _lifeTime = _meteorData_Projectile.projectileLifeTime;
        _gravity = _meteorData_Projectile.projectileSpeedRate;
        _rotSpeed = 50.0f;
    }


    private void OnEnable()
    {
        _camera = Camera.main;
        _spawnPos = _camera.gameObject.transform.position + new Vector3(0.0f, _camera.orthographicSize, 10.0f);
        transform.position = _spawnPos;
        _rb.velocity = new Vector3(0, -_gravity, 0);
        _damageTimer = 0f;
        Invoke(nameof(DestroyMeteor), _lifeTime);
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, _rotSpeed * Time.deltaTime));
        Debug.Log(_ozMagicPercentage);
        Debug.Log(_lifeTime);
    }

    private void DestroyMeteor()
    { 
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            gameObject.SetActive(false);
        }           
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _damageTimer += Time.deltaTime;
            if (_damageTimer >= _hitCycle)
            {
                // todo
                // 플레이어 데미지
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _damageTimer = 0f;
        }
    }
}
