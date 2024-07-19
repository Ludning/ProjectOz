using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : SingleTonMono<TimeManager>
{
    private event Action timeAction;
    private void Update()
    {
        timeAction?.Invoke();
    }

    public void RegistCooldownAction(Action action)
    {
        timeAction += action;
    }
    public void DeregistCooldownAction(Action action)
    {
        timeAction -= action;
    }
}
