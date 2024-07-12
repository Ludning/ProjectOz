using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FlameballMove : BallMove
{
    protected override void Awake()
    {
        base.Awake();

        _bulletSpeed = 10.0f;
        _bulletLifeTime = 10.0f;
    }
}
