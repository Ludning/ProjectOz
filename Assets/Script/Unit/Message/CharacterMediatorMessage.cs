using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct CharacterMediatorMessage<T> : ICharacterMediatorMessage
{
    public T value;
}