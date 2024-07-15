using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class OzMagicManager : SingleTonMono<OzMagicManager>, IOzMagic
{
    [SerializeField] private List<GameObject> Prefab_OzMagic;

    private int _ozMagicIndex;

    public void Execute()
    {
        RandomOzMagic();
        Instantiate(Prefab_OzMagic[_ozMagicIndex]);
    }

    private void RandomOzMagic()
    {
        _ozMagicIndex = Random.Range(0, Prefab_OzMagic.Count);
    }
}
