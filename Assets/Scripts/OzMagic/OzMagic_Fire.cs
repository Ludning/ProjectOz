using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OzMagic : MonoBehaviour, IOzMagic
{
    [SerializeField] private GameObject Prefab_projectile;

    public void Execute()
    {
        Debug.Log("Fire");
        Instantiate(Prefab_projectile);
    }
}
