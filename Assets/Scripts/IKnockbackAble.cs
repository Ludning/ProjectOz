using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKnockbackAble 
{
    public void KnockbackOnSurface(Vector3 direction, float force);
}
