using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBox : MonoBehaviour
{
    [SerializeField] private Combat _owner;
    [SerializeField] private Vector3 _halfSize = new Vector3(1f,1f,1f);

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
        Collider[] result = Physics.OverlapBox(transform.position, _halfSize, transform.rotation);
        
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
            _owner.Attack(combat, _damage);
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
        Gizmos.DrawWireCube(transform.position, _halfSize);
    }

    public void SetDamage(float damage)
    {
        _damage = damage;
    }
}
