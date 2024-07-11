using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DataPostprocessor : AssetPostprocessor
{
    const string dataXlsxPath = "Assets/Resource/Xlsx/GameData.xlsx";
    const string addressXlsxPath = "Assets/Resource/Xlsx/AssetAddress.xlsx";
    const string dataAssetPath = "Assets/Resource/Data/GameData.asset";
    const string addressAssetPath = "Assets/Resource/Data/AssetAddress.asset";
    
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
    {
        //이곳에서 추가되거나 수정된 파일에 대한 작업을 수행한다.
        foreach (string str in importedAssets)
        {
            Debug.Log("추가된 에셋: " + str);
            
            if (str == dataXlsxPath)
                DataConverter.LoadExcel<GameData>(dataXlsxPath, dataAssetPath);
            if (str == addressXlsxPath)
                DataConverter.LoadExcel<AssetAddressData>(addressXlsxPath, addressAssetPath);
            
            /*string jsonPath = "Assets/Resource/Json/data.json";
            if (str == xlsxPath)
                DataConverter.ReadExcel(xlsxPath);
            if (str == jsonPath)
                DataConverter.ReadJson(jsonPath);*/
            /*
            var languageXlsxPath = "Assets/Resource/Xlsx/languageData.xlsx";
            if (str == languageXlsxPath)
                DataConverter.ReadLanguageExcel();*/
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
}
