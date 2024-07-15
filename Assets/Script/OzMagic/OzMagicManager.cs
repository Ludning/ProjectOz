using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class OzMagicManager : SingleTonMono<OzMagicManager>
{ 
    private OzmagicData _ozData;
    private OzMagic _ozMagic;

    [SerializeField] private List<GameObject> Prefab_OzMagic = new List<GameObject>();
    [SerializeField] private List<float> _ozMagic_weights;
    private int _listIndex = 0;

    private int _ozMagicIndex;

    private void Awake()
    {
        Prefab_OzMagic.Add(ResourceManager.Instance.LoadResource<GameObject>("Meteor"));
    }

    private void OnEnable()
    {
        Execute();

        _ozMagic_weights = new List<float>(new float[Prefab_OzMagic.Count]);
        foreach (var item in Prefab_OzMagic)
        {
            var oz = item.GetComponent<OzMagic>();
            _ozMagic_weights[_listIndex] = oz._ozMagicPercentage;
            _listIndex++;
        }
    }

    private void Update()
    {
        Debug.Log(_ozMagic_weights[0]);
    }

    public void Execute()
    {
        //RandomOzMagic();
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
