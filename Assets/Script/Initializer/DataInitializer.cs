using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Playables;

public static class DataInitializer
{
    const string _dataJsonPath = "Assets/Resource/Data/GameData.json";
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void DataLoad()
    {
        TextAsset jsonFile = Addressables.LoadAssetAsync<TextAsset>(_dataJsonPath).WaitForCompletion();
        GameData gameData = JsonConvert.DeserializeObject<GameData>(jsonFile.text);
        foreach(OzmagicData data in gameData.OzmagicDatas.Values)
        {
            GameObject prefab = ResourceManager.Instance.LoadResource<GameObject>(data.prefabName);
            prefab.GetComponent<OzMagic>()._ozMagicPercentage = data.ozSkillPercentage;
        }
    }
}
