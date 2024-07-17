using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageRigger : MonoBehaviour
{
    [SerializeField] private MageControl mageControl;
    public void UseMageController()
    {
        mageControl.OnAttack();
    }
}
