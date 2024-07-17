using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBox : MonoBehaviour
{
    [SerializeField] private Combat _owner;
    [SerializeField] private Vector3 _halfSize = new Vector3(1f, 1f, 1f);
    private Vector3 HalfSize
    {
        get
        {
            return new Vector3()
            {
                x = _halfSize.x * transform.lossyScale.x,
                y = _halfSize.y * transform.lossyScale.y,
                z = _halfSize.z * transform.lossyScale.z
            };
        }
    }
    private float _damage;

    private void Awake()
    {
        _owner = transform.parent.GetComponent<Combat>();
    }
    private void Start()
    {
        enabled = false;
    }
    private void OnEnable()
    {
        Collider[] result = Physics.OverlapBox(transform.position, HalfSize, transform.rotation);
        
        foreach (Collider hit in result)
        {

            Combat combat = hit.GetComponent<Combat>();
            if ( combat == null)
            {
                continue;
            }
            if (_owner.transform == hit.transform)
            {
                continue;
            }
            combat.Damaged(_damage);
        }
        enabled = false;
    }

    private void OnDrawGizmos()
    {
        if(enabled == false)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, HalfSize);
    }

    public void SetDamage(float damage)
    {
        _damage = damage;
    }
}
