using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : SingleTonMono<TimeManager>
{
    private event Action cooldownAction;
    private void Update()
    {
        cooldownAction.Invoke();
    }

    public void RegistCooldownAction(Action action)
    {
        cooldownAction += action;
    }
    public void DeregistCooldownAction(Action action)
    {
        cooldownAction -= action;
    }
}
