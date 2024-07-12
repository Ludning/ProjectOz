using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2;
using ExcelDataReader;
using Newtonsoft.Json;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class DataConverter
{
    public static void LoadExcel<T>(string xlsxPath, string jsonPath, Type enumType) where T : class, new()
    {
        Debug.Log("ReadExcel");

        //파일 존재 체크
        if (IsFileExists(xlsxPath) == false)
            return;

        ConvertExcelToJson<T>(xlsxPath, jsonPath, enumType);
        
        //EditorUtility.SetDirty(asset);
        //AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    //파일이 있는지 확인
    private static bool IsFileExists(string path)
    {
        Debug.Log("Path : " + path);
        var isExist = File.Exists(path);
        if (isExist == false)
            Debug.LogError("Xlsx 파일이 존재하지 않습니다.");

        return isExist;
    }
    
    private static void ConvertExcelToJson<T>(string excelPath, string jsonPath, Type enumType) where T : class, new()
    {
        try
        {
            using var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            
            var result = reader.AsDataSet();
            
            if (result.Tables.Count <= 0)
            {
                Debug.LogError("The Excel file does not contain any data.");
                return;
            }
            
            T data = new T();
            string[] tableNames = Enum.GetNames(enumType);

            foreach (var tableName in tableNames)
            {
                DataTable dataTable = GetDataTableByName(result.Tables, tableName);
                FieldInfo fieldInfo = typeof(T).GetField($"{tableName}Datas");

                Type valueType = fieldInfo.FieldType.GetGenericArguments()[1];
                //Debug.Log($"dataTableName : {dataTable.TableName}, TypeName : {valueType}");
                
                //함수 리플렉션 호출
                var method = typeof(DataConverter).GetMethod(nameof(DataTableToDictionary), BindingFlags.Static | BindingFlags.NonPublic)?.MakeGenericMethod(valueType);
                
                if (method == null)
                    continue;
                
                var fieldData = method.Invoke(null, new object[] { dataTable });

                fieldInfo.SetValue(data, fieldData);
            }
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(jsonPath, json);
            Debug.Log("Excel data has been converted to JSON and saved to " + jsonPath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error reading excel file: " + ex.Message);
        }
    }
    private static DataTable GetDataTableByName(DataTableCollection tables, string tableName)
    {
        if (tables == null || string.IsNullOrEmpty(tableName))
        {
            throw new ArgumentException("tables 및 tableName은 null이거나 비어있을 수 없습니다.");
        }

        foreach (DataTable table in tables)
        {
            if (table.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase))
            {
                return table;
            }
        }

        return null; // 테이블을 찾지 못한 경우 null 반환
    }


    private static Dictionary<string, T> DataTableToDictionary<T>(DataTable table)
    {
        var dict = new Dictionary<string, T>();

        var fieldInfos = typeof(T).GetFields();

        Dictionary<string, int> columnTypeDic = RecordTypeName(table, fieldInfos);

        for (int i = 1; i < table.Rows.Count; i++) // 첫 줄은 헤더이므로 제외
        {
            if (typeof(T) == typeof(string))
            {
                // T가 string인 경우
                string key = table.Rows[i][0].ToString();
                string value = table.Rows[i][1].ToString();
                dict[key] = (T)(object)value; // 명시적 형변환을 통해 object에서 T로 변환
            }
            else
            {
                // T가 클래스인 경우
                var obj = Activator.CreateInstance<T>(); // T에 대해 객체 생성

                
                foreach (var fieldInfo in fieldInfos)
                {
                    Type type = fieldInfo.FieldType;
                    if (type.IsEnum)
                    {
                        var value = table.Rows[i].ItemArray[columnTypeDic[fieldInfo.Name]].ToString();
                        fieldInfo.SetValue(obj, Enum.Parse(type, value));
                    }
                    else if (type == typeof(string))
                    {
                        var value = table.Rows[i].ItemArray[columnTypeDic[fieldInfo.Name]].ToString();
                        fieldInfo.SetValue(obj, Convert.ChangeType(value, type));
                    }
                    else if (type.IsPrimitive)
                    {
                        var value = table.Rows[i].ItemArray[columnTypeDic[fieldInfo.Name]].ToString();
                        if (value == "-")
                            value = "0";
                        fieldInfo.SetValue(obj, Convert.ChangeType(value, type));
                    }
                }

                string key = table.Rows[i][0].ToString();
                if (!string.IsNullOrWhiteSpace(key))
                {
                    dict[key] = obj;
                }
            }
        }

        return dict;
    }
    
    private static Dictionary<string, int> RecordTypeName(DataTable dataTable, FieldInfo[] fieldInfos)
    {
        Dictionary<string, int> columnTypeDic = new Dictionary<string, int>();
        for (int fieldColumn = 0; fieldColumn < dataTable.Columns.Count; fieldColumn++)
        {
            string typeName = (string)(dataTable.Rows[0].ItemArray[fieldColumn]);
            if (string.IsNullOrWhiteSpace(typeName))
                break;
            if(fieldInfos.Any(fieldInfo => fieldInfo.Name == typeName))
                columnTypeDic.Add(typeName, fieldColumn);
        }

        return columnTypeDic;
    }
    
    
}
