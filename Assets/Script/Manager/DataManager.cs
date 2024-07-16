using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DataManager : SingleTon<DataManager>
{
    private GameData _gameData;
    private AssetAddressData _assetAddressData;
    
    const string _dataJsonPath = "Assets/Resource/Data/GameData.json";
    const string _addressJsonPath = "Assets/Resource/Data/AssetAddress.json";
    
    public T GetGameData<T>(string key) where T : class, IDataRepository, new()
    {
        if (_gameData == null)
        {
            TextAsset jsonFile = Addressables.LoadAssetAsync<TextAsset>(_dataJsonPath).WaitForCompletion();
            _gameData = JsonConvert.DeserializeObject<GameData>(jsonFile.text);
        }
        switch (typeof(T).Name)
        {
            case "PcData":
                if (_gameData.PcDatas.TryGetValue(key, out PcData pcData))
                    return pcData as T;
                break;
            case "EnemyData":
                if (_gameData.EnemyDatas.TryGetValue(key, out EnemyData enemyData))
                    return enemyData as T;
                break;
            case "MovesetData":
                if (_gameData.MovesetDatas.TryGetValue(key, out MovesetData movesetData))
                    return movesetData as T;
                break;
            case "ResourceData":
                if (_gameData.ResourceDatas.TryGetValue(key, out ResourceData resourceData))
                    return resourceData as T;
                break;
            case "SkillData":
                if (_gameData.SkillDatas.TryGetValue(key, out SkillData skillData))
                    return skillData as T;
                break;
            case "OzmagicData":
                if (_gameData.OzmagicDatas.TryGetValue(key, out OzmagicData ozmagicData))
                    return ozmagicData as T;
                break;
            case "ProjectileData":
                if (_gameData.ProjectileDatas.TryGetValue(key, out ProjectileData projectileData))
                    return projectileData as T;
                break;
            case "LevelData":
                if (_gameData.LevelDatas.TryGetValue(key, out LevelData levelData))
                    return levelData as T;
                break;
        }

        return new T();
    }

    public Dictionary<string, T> GetGameDataDictionary<T>() where T : class, IDataRepository
    {
        if (_gameData == null)
        {
            TextAsset jsonFile = Addressables.LoadAssetAsync<TextAsset>(_dataJsonPath).WaitForCompletion();
            _gameData = JsonConvert.DeserializeObject<GameData>(jsonFile.text);
        }

        switch (typeof(T).Name)
        {
            case "PcData":
                return _gameData.PcDatas as Dictionary<string, T>;
            case "EnemyData":
                return _gameData.EnemyDatas as Dictionary<string, T>;
            case "MovesetData":
                return _gameData.MovesetDatas as Dictionary<string, T>;
            case "ResourceData":
                return _gameData.ResourceDatas as Dictionary<string, T>;
            case "SkillData":
                return _gameData.SkillDatas as Dictionary<string, T>;
            case "OzmagicData":
                return _gameData.OzmagicDatas as Dictionary<string, T>;
            case "ProjectileData":
                return _gameData.ProjectileDatas as Dictionary<string, T>;
            case "LevelData":
                return _gameData.LevelDatas as Dictionary<string, T>;
        }
        return null;
    }

    public string GetAssetAddress(string key)
    {
        if (_assetAddressData == null)
        {
            TextAsset jsonFile = Addressables.LoadAssetAsync<TextAsset>(_addressJsonPath).WaitForCompletion();
            _assetAddressData = JsonConvert.DeserializeObject<AssetAddressData>(jsonFile.text);
        }

        return _assetAddressData.AssetAddressDatas.GetValueOrDefault(key);
    }
}
