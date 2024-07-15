
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class OzMagic : MonoBehaviour
{
    protected IObjectPool<OzMagic> _ozMagicPool;

    public float _ozMagicPercentage;

    protected float _lifeTime;

    protected bool _isDestroyed;

    protected void OnEnable()
    {
        
    }

    public void Execute()
    {

    }
}
