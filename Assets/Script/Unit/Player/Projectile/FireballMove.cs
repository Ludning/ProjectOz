using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FireballMove : BallMove
{
    private SkillData _fireballData_Skill;
    private ProjectileData _fireballData_Projectile;

    private int _maxBounceCount;
    private int _bounceCount;

    private float _damage;
    private float _gravityValue;

    protected override void Awake()
    {
        base.Awake();

        _fireballData_Skill = DataManager.Instance.GetGameData<SkillData>("S101");
        _fireballData_Projectile = DataManager.Instance.GetGameData<ProjectileData>("P101");

        _bulletSpeed = _fireballData_Projectile.projectileSpeedRate;
        _bulletLifeTime = _fireballData_Projectile.projectileLifeTime;

        _maxBounceCount = _fireballData_Projectile.projectileBounceCount;

        _damage = _fireballData_Skill.skillPowerRate;
        _gravityValue = 5.81f;
        _gravity = new Vector3(0f, -_gravityValue, 0f);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _bounceCount = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Platform") || other.gameObject.CompareTag("Wall"))
        {
            _bounceCount++;

            if (_bounceCount >= _maxBounceCount)
            {
                DestroyBall();
            }

            //if (other.gameObject.CompareTag("ground") || other.gameObject.CompareTag("platform"))
            if (transform.position.y - other.gameObject.transform.position.y > other.gameObject.transform.localScale.y / 2f - 0.15f)
                _rb.velocity = new Vector3(_rb.velocity.x, 4.0f + _gravityValue, _rb.velocity.z) + _gravity;
            else
                _rb.velocity = new Vector3(-_rb.velocity.x, _rb.velocity.y, -_rb.velocity.z);
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
