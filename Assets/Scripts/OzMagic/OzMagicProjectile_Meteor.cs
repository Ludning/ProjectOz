using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Metere : MonoBehaviour
{
    public void Hit()
    {
        Debug.Log("Projectile_Metere Hit");
        Explode();
    }

    private void Explode()
    {
        Debug.Log("Projectile_Metere Explode");
        //데미지 적용
        //파괴 이펙트 등등
    }
}
