using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class Combat : MonoBehaviour
{
    [SerializeField] Transform _owner;

    [SerializeField] float initalMaxHp = 100f;
    [SerializeField] float _maxHp = 100f;

    [SerializeField] float _hp = 100f;

    [SerializeField] bool _dead = false;

    [SerializeField] float _invincibleTimeOnHit = .1f;
    [SerializeField] float _prevHitTime = 0f;

    public Transform transform { get { return _owner; } }

    public Func<bool> AdditionalDamageableCheck { get; set; }
    public Action<Combat> OnDamaged;
    public Action OnHeal;
    public Action<Combat, Combat> OnDead;


    public Action OnAttackSucceeded;
    public Action OnKillEnemy;
    public Action<Combat, float> OnAttack;


    public void Init(Transform owner, float maxHp)
    {
        Heal(_maxHp);
        initalMaxHp = maxHp;
        _owner = owner;
    }
    public float GetHp() { return _hp; }
    public void SetMaxHp(float maxHp)
    {
        _maxHp = maxHp;
    }
    public float GetMaxHp()
    {
        return _maxHp;
    }
    public void AddMaxHp(float add)
    {
        _maxHp = initalMaxHp + add;
    }
    public void ResetHpWithRatio(float ratio)
    {
        _hp = _maxHp * ratio;
    }


    private bool IsDamageable()
    {
        if (Time.time < _prevHitTime + _invincibleTimeOnHit)
        {
            return false;
        }
        if (_dead)
        {
            return false;
        }
        bool result = true;
        if (AdditionalDamageableCheck != null)
        {
            result = result && AdditionalDamageableCheck.Invoke();
        }
        if (!result)
        {
            return false;
        }
        return true;
    }
    public bool TakeDamage(Combat attacker, float damage)
    {
        if (!IsDamageable())
            return false;

        CalcTakeDamage(damage);
        OnDamaged?.Invoke(attacker);
        Debug.Log($"DamageTaken Test NonObject",_owner );
        Debug.Log($"DamageTaken ",_owner );
        if (_hp <= 0f)
        {
            _dead = true;
            OnDead?.Invoke(attacker, this);
        }
        return true;
    }
    private void CalcTakeDamage(float damage)
    {
        _prevHitTime = Time.time;
        damage = Mathf.Max(0f, damage);
        _hp -= damage;
    }
    public void Heal(float amount)
    {
        if (_hp < _maxHp)
        {
            _hp += amount;
            if (_hp > _maxHp)
            {
                _hp = _maxHp;
            }
        }
        if (OnHeal != null)
        {
            OnHeal.Invoke();
        }
    }
    public bool IsDead()
    {
        return _dead;
    }

    public void Die()
    {
        TakeDamage(null, _hp);
    }
    public void ResetDead()
    {
        Heal(999999999999f);
        _dead = false;
    }



    public void DealDamage(Combat target, float damage)
    {
        if (target == null)
        {
            return;
        }

        bool isAttackSucceeded = target.TakeDamage(this, damage);
        if (isAttackSucceeded)
        {
            if (OnAttackSucceeded != null)
            {
                OnAttackSucceeded.Invoke();
            }
            if (target.IsDead())
            {
                if (OnKillEnemy != null)
                {
                    OnKillEnemy.Invoke();
                }
            }
            //return false;
        }
        //return true;
    }

    public void TriggerAttack(Combat targetCombat, float damage)
    {
        OnAttack?.Invoke(targetCombat, damage);
    }
}





