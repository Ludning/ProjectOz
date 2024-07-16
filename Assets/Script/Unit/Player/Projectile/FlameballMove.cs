using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FlameballMove : BallMove
{
    private ProjectileData _flameballData_Projectile;

    private float _damage;

    protected override void Awake()
    {
        base.Awake();

        _flameballData_Projectile = DataManager.Instance.GetGameData<ProjectileData>("P102");

        _damage = DataManager.Instance.GetGameData<SkillData>("S102").skillPowerRate;
        _bulletSpeed = _flameballData_Projectile.projectileSpeedRate;
        _bulletLifeTime = _flameballData_Projectile.projectileLifeTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            DestroyBall();
        }
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent(out Combat combat))
            {
                combat.Damaged(_damage);
            }
        }
    }

}
