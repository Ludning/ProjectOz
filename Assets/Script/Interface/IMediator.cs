using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMediator
{
    void Notify(object sender, MessageBase message);
}
