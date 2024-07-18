using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DataPostprocessor : AssetPostprocessor
{
    const string dataXlsxPath = "Assets/Resource/Xlsx/GameData.xlsx";
    const string dataJsonPath = "Assets/Resource/Data/GameData.json";
    const string addressXlsxPath = "Assets/Resource/Xlsx/AssetAddress.xlsx";
    const string addressJsonPath = "Assets/Resource/Data/AssetAddress.json";
    
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
    {
        //이곳에서 추가되거나 수정된 파일에 대한 작업을 수행한다.
        foreach (string str in importedAssets)
        {
            Debug.Log("추가된 에셋: " + str);
            
            if (str == dataXlsxPath)
                DataConverter.LoadExcel<GameData>(dataXlsxPath, dataJsonPath, typeof(GameDataType));
            if (str == addressXlsxPath)
                DataConverter.LoadExcel<AssetAddressData>(addressXlsxPath, addressJsonPath, typeof(AssetAddressType));
        }
        foreach (string str in deletedAssets)
        {
            Debug.Log("삭제된 에셋 : " + str);
        }

        for (int i = 0; i < movedAssets.Length; i++)
        {
            Debug.Log("옮겨진 에셋: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
        }

        if (didDomainReload)
        {
            Debug.Log("Domain has been reloaded");
        }
    }

    [MenuItem("Xlsx/LoadXlsx")]
    public static void OnLoadXlsx()
    {
        DataConverter.LoadExcel<GameData>(dataXlsxPath, dataJsonPath, typeof(GameDataType));
        DataConverter.LoadExcel<AssetAddressData>(addressXlsxPath, addressJsonPath, typeof(AssetAddressType));
    }
}
