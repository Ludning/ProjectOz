using System;
using UnityEngine;


[Serializable]
public class Combat : MonoBehaviour
{
    [SerializeField] private float _maxHp = 100f;

    [SerializeField] private float _hp = 100f;

    [SerializeField] private bool _dead = false;

    [SerializeField] private float _invincibleTimeOnHit = .1f;
    [SerializeField] private float _prevHitTime = 0f;

    [SerializeField, ReadOnly] public bool IsInvincibility = false;

    public Func<bool> AdditionalDamageableCheck { get; set; }
    public Action OnDamaged;
    public Action OnHeal;
    public Action OnDead;


    public Action OnAttackSucceeded;
    public Action OnKillEnemy;
    public Action<Combat, float> OnAttack;


    public void Init(float maxHp)
    {
        _maxHp = maxHp;
        _hp = maxHp;
        ResetDead();
    }
    public float GetHp() { return _hp; }
    public float GetMaxHp()
    {
        return _maxHp;
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
    public bool Damaged(float damage)
    {
        if (IsInvincibility == true)
            return false;
        
        if (!IsDamageable())
            return false;

        _prevHitTime = Time.time;
        damage = Mathf.Max(0f, damage);
        _hp -= damage;

        OnDamaged?.Invoke();

        Debug.Log($"DamageTaken ", transform);
        if (_hp <= 0f)
        {
            _dead = true;
            OnDead?.Invoke();
        }
        return true;
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
        Damaged(9999999999f);
    }
    public void ResetDead()
    {
        ResetHp();
        _dead = false;
    }




    private void ResetHp()
    {
        Heal(9999999999f);
    }



}





