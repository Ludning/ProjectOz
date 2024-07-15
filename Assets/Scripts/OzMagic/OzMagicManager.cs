using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class OzMagicManager : SingleTonMono<OzMagicManager>, IOzMagic
{
    private OzmagicData _ozData;

    [SerializeField] private List<GameObject> Prefab_OzMagic;
    private List<int> _ozMagic_weights;

    private int _ozMagicIndex;

    private void Awake()
    {
        //_ozData = DataManager.Instance.GetGameData<OzmagicData>();
    }

    public void Execute()
    {
        RandomOzMagic();
        Instantiate(Prefab_OzMagic[_ozMagicIndex]);
    }

    private void RandomOzMagic()
    {
        _ozMagicIndex = Random.Range(0, Prefab_OzMagic.Count);
    }

    private int GetRandomByWeight(List<int> weight)
    { 
        int totalWeight = 0;

        foreach (var i in weight)
        {
            totalWeight += i;
        }

        

        return 0;
    }
}
