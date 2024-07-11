using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDebugManager : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;
    public void TestKnockback()
    {
        _enemy.KnockbackOnSurface(Vector3.back, 5f);
    }
}
