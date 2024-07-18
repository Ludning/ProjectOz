using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControl
{
    public void OnInputAttack(KeyType type);
    public void OnInputJump(KeyType type);
}
