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
        /*try
        {
            using (var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var dataTableCollection = reader.AsDataSet().Tables;

                    foreach (DataTable dataTable in dataTableCollection)
                    {
                        var dataList = DataTableToDataList<T>(dataTable);
                        var json = JsonConvert.SerializeObject(dataList, Formatting.Indented);
                        File.WriteAllText(jsonPath, json);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error reading excel file: " + ex.Message);
        }*/
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

    /*private static Dictionary<string, T> DataTableToDictionary<T>(DataTable table) where T : new()
    {
        if (table.TableName == "Projectile")
            Debug.Log("Test");

        var dict = new Dictionary<string, T>();

        var fieldInfos = typeof(T).GetFields();

        Dictionary<string, int> columnTypeDic = RecordTypeName(table, fieldInfos);

        for (int i = 1; i < table.Rows.Count; i++) // 첫 줄은 헤더이므로 제외
        {
            var obj = new T();
            foreach (var fieldInfo in fieldInfos)
            {
                var value = table.Rows[i].ItemArray[columnTypeDic[fieldInfo.Name]].ToString();
                fieldInfo.SetValue(obj, Convert.ChangeType(value, fieldInfo.FieldType));
            }

            string key = table.Rows[i][0].ToString();
            if (!string.IsNullOrWhiteSpace(key))
            {
                dict[key] = obj;
            }
        }

        return dict;
    }*/
    
    /*private static Dictionary<string, T> DataTableToDictionary<T>(DataTable table) where T : new()
    {
        var dict = new Dictionary<string, T>();

        var fieldInfos = typeof(T).GetFields();

        Dictionary<string, int> columnTypeDic = RecordTypeName(table, fieldInfos);

        for (int i = 1; i < table.Rows.Count; i++) // 첫 줄은 헤더이므로 제외
        {
            var obj = new T();

            if (typeof(T) == typeof(string))
            {
                // T가 string인 경우
                string key = table.Rows[i][0].ToString();
                if (!string.IsNullOrWhiteSpace(key))
                {
                    dict[key] = (T)Convert.ChangeType(table.Rows[i][1].ToString(), typeof(T));
                }
            }
            else
            {
                // T가 클래스인 경우
                foreach (var fieldInfo in fieldInfos)
                {
                    var value = table.Rows[i].ItemArray[columnTypeDic[fieldInfo.Name]].ToString();
                    fieldInfo.SetValue(obj, Convert.ChangeType(value, fieldInfo.FieldType));
                }

                string key = table.Rows[i][0].ToString();
                if (!string.IsNullOrWhiteSpace(key))
                {
                    dict[key] = obj;
                }
            }
        }

        return dict;
    }*/

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
                    var value = table.Rows[i].ItemArray[columnTypeDic[fieldInfo.Name]].ToString();
                    fieldInfo.SetValue(obj, Convert.ChangeType(value, fieldInfo.FieldType));
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
    
    
    
    /*if (type == typeof(string))
    {
        for (int i = 1; i < table.Rows.Count; i++) // 첫 줄은 헤더이므로 제외
        {
            string valueString;
            foreach (var fieldInfo in fieldInfos)
            {
                var value = table.Rows[i].ItemArray[columnTypeDic[fieldInfo.Name]].ToString();
                valueString = value;
            }
            
            string key = table.Rows[i][0].ToString();
            if (!string.IsNullOrWhiteSpace(key))
            {
                dict[key] = valueString;
            }
        }
    }
else */
//if (columnTypeDic.TryGetValue(columnField.Name, out int index))
//    columnField.SetValue(data, dataRow[index].ToString());
    
    /*private static void ConvertExcelToJson(string xlsxPath, string jsonPath)
    {
        try
        {
            using var stream = new FileStream(xlsxPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var result = reader.AsDataSet();
            var json = DataSetToJson(result);
            File.WriteAllText(jsonPath, json);
            Debug.Log("Excel data has been converted to JSON and saved to " + jsonPath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error reading excel file: " + ex.Message);
        }
    }

    private static string DataSetToJson(DataSet ds)
    {
        return JsonConvert.SerializeObject(ds, Formatting.Indented);
    }*/
    
    
    
    //엑셀파일로 부터 테이블 데이터 로드
    /*private static DataTableCollection GetTableFromXlsx(string xlsxPath)
    {
        using var stream = new FileStream(xlsxPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        // FileStream을 사용한 코드 작성
        using var reader = ExcelReaderFactory.CreateReader(stream);
        // 모든 시트 로드
        return null;//reader.AsDataSet().Tables;
    }*/
    /*private static T GetScriptableAsset<T>(string assetPath) where T : ScriptableObject
    {
        T loadedAsset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        if (loadedAsset == null)
        {
            loadedAsset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(loadedAsset, assetPath);
        }
        return loadedAsset;
    }
    */
    /*
    //테이블 데이터를 스크립터블 오브젝트에 저장
    private static void ParseTableToScriptable<T>(T loadedAsset, DataTableCollection tables) where T : ScriptableObject
    {
        var assetType = typeof(T);
        var fieldInfos = assetType.GetFields();
        
        foreach (var fieldInfo in fieldInfos)
        {
            //제네릭타입 알아내기
            Type type = fieldInfo.FieldType.GetGenericArguments()[1];
            Type keyType = fieldInfo.FieldType.GetGenericArguments()[0];
            //함수 리플렉션 호출
            var method = typeof(DataConverter).GetMethod(nameof(ReadDataFromTable), BindingFlags.Static | BindingFlags.Public)?.MakeGenericMethod(keyType, type);
            if (method == null) continue;

            var data = method.Invoke(null, new object[] { fieldInfo.Name, tables });
            
            fieldInfo.SetValue(loadedAsset, data);
        }
    }
    
    public static Dictionary<TKey, TValue> ReadDataFromTable<TKey, TValue>(string sheetName, DataTableCollection tables) where TValue : class, new()
    {
        if (tables.Contains(sheetName) == false)
        {
            Debug.LogError($"Xlsx 파일에 Sheet이름 : {sheetName} 이 존재하지 않습니다");
            return null;
        }

        DataTable sheet = tables[sheetName];
        var dataType = typeof(TValue);
        
        FieldInfo[] fieldInfos = dataType.GetFields();
        Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>();
        
        Dictionary<string, int> columnTypeDic = new Dictionary<string, int>();
        
        //0행의 데이터를 가져온다, 0행의 데이터는 자료형을 결정하기 떄문
        RecordTypeName(columnTypeDic, fieldInfos, sheet);

        TKey prevKey = default(TKey);

        foreach (DataRow dataRow in sheet.Rows)
        {
            if (dataRow == sheet.Rows[0])
                continue;

            #region Key 체크
            TKey key;
            object keyObject = dataRow.ItemArray[0];
            if (keyObject != DBNull.Value)
                key = ConvertKey<TKey>(keyObject);
            else if(prevKey != null && !prevKey.Equals(default(TKey)))
                key = prevKey;
            else
                continue;
            #endregion
            
            //Key로 Value를 받아옴
            TValue data = ret.TryGetValue(key, out TValue value) ? value : new TValue();

            /#1#/리스트 객체 생성
            foreach (FieldInfo info in fieldInfos)
            {
                //리스트일 경우
                if (info.FieldType.IsGenericType && info.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    //리스트가 null일 경우
                    if (info.GetValue(data) == null)
                    {
                        // GenericArgument로 타입을 가져옵니다.
                        Type elementType = info.FieldType.GetGenericArguments()[0];
                        // 인스턴스를 생성합니다.
                        object listInstance = Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                        Debug.Log("List 객체가 생성되었습니다. type: " + listInstance.GetType());
                        info.SetValue(data, listInstance);
                    }
                }
            }#1#
            
            /*FieldInfo[] columnFields = data.GetType().GetFields();
            //FieldInfo fieldInfo = Array.Find(columnFields, fi => fi.Name == itemTypeName);
            foreach (FieldInfo columnField in columnFields)
            {
                Type type = columnField.FieldType;
                if (type.IsEnum)
                {
                    if (columnTypeDic.TryGetValue(columnField.Name, out int index))
                        columnField.SetValue(data, Enum.Parse(type, dataRow[index].ToString()));
                }
                else if (type == typeof(string))
                {
                    if (columnTypeDic.TryGetValue(columnField.Name, out int index))
                        columnField.SetValue(data, dataRow[index].ToString());
                }
                else if (type.IsPrimitive)
                {
                    if (columnTypeDic.TryGetValue(columnField.Name, out int index))
                        columnField.SetValue(data, Convert.ChangeType(dataRow[index], type));
                }
                else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    object genericInstance = columnField.GetValue(data);
                    FieldInfo[] genericFields = type.GetGenericArguments()[0].GetFields();
                    foreach (FieldInfo fi in genericFields)
                    {
                        
                    }
                    if (columnTypeDic.TryGetValue(columnField.Name, out int index))
                        columnField.SetValue(data, Convert.ChangeType(dataRow[index], type));
                }
            }#1#
            SetFieldData<TValue>(columnTypeDic, dataRow, data);

            if (!ret.ContainsKey(key))
            {
                ret.Add(key, data);
            }
            prevKey = key;
        }
        return ret;
    }

    private static void SetFieldData<T>(Dictionary<string, int> columnTypeDic, DataRow dataRow, T data) where T : class, new()
    {
        FieldInfo[] fieldInfos = data.GetType().GetFields();
        foreach (FieldInfo fieldInfo in fieldInfos)
        {
            Type fieldType = fieldInfo.FieldType;
            
            //열거형 처리
            if (fieldType.IsEnum)
            {
                if (columnTypeDic.TryGetValue(fieldInfo.Name, out int index))
                    if(dataRow[index] != DBNull.Value)
                        fieldInfo.SetValue(data, Enum.Parse(fieldType, dataRow[index].ToString()));
            }
            //String 처리
            else if (fieldType == typeof(string))
            {
                if (columnTypeDic.TryGetValue(fieldInfo.Name, out int index))
                    if(dataRow[index] != DBNull.Value)
                        fieldInfo.SetValue(data, dataRow[index].ToString());
            }
            //값 형식 처리
            else if (fieldType.IsPrimitive)
            {
                if (columnTypeDic.TryGetValue(fieldInfo.Name, out int index))
                    if(dataRow[index] != DBNull.Value)
                        fieldInfo.SetValue(data, Convert.ChangeType(dataRow[index], fieldType));
            }
            //List<>처리
            else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                //List<T>인스턴스 추출
                object listInstance = fieldInfo.GetValue(data);
                Type genericType = fieldType.GetGenericArguments()[0];
                
                //리스트가 null일 경우
                if (listInstance == null)
                {
                    listInstance = Activator.CreateInstance(typeof(List<>).MakeGenericType(genericType));
                    fieldInfo.SetValue(data, listInstance);
                }
                
                
                //제네릭 인스턴스 필드들 가져옴
                FieldInfo[] genericFields = genericType.GetFields();
                object genericInstance = Activator.CreateInstance(genericType);
                
                foreach (FieldInfo fi in genericFields)
                {
                    Type fiType = fi.FieldType;
                    if (fiType.IsEnum)
                    {
                        if (columnTypeDic.TryGetValue(fi.Name, out int index))
                            if(dataRow[index] != DBNull.Value)
                                fi.SetValue(genericInstance, Enum.Parse(fiType, dataRow[index].ToString()));
                    }
                    else if (fiType == typeof(string))
                    {
                        if (columnTypeDic.TryGetValue(fi.Name, out int index))
                            if(dataRow[index] != DBNull.Value)
                                fi.SetValue(genericInstance, dataRow[index].ToString());
                    }
                    else if (fiType.IsPrimitive)
                    {
                        if (columnTypeDic.TryGetValue(fi.Name, out int index))
                            if(dataRow[index] != DBNull.Value)
                                fi.SetValue(genericInstance, Convert.ChangeType(dataRow[index], fiType));
                    }
                }
                Type listType = typeof(List<>).MakeGenericType(genericType);
                MethodInfo addMethod = listType.GetMethod("Add");
                addMethod.Invoke(listInstance, new object[] { genericInstance });
            }
        }
    }*/
    /*
    private static TValue GetValue<TKey, TValue>(Dictionary<TKey, TValue> ret, object key) where TValue : class, new()
    {
        return ret.TryGetValue(ConvertKey<TKey>(key), out TValue value) ? value : new TValue();
    }

    private static void RecordTypeName(Dictionary<string, int> columnTypeDic, FieldInfo[] fieldInfos, DataTable sheet)
    {
        for (int fieldColumn = 0; fieldColumn < sheet.Columns.Count; fieldColumn++)
        {
            string typeName = (string)(sheet.Rows[0].ItemArray[fieldColumn]);
            if (string.IsNullOrWhiteSpace(typeName))
                break;
            columnTypeDic.Add(typeName, fieldColumn);
        }
    }

    private static TKey ConvertKey<TKey>(object rawKey)
    {
        if (typeof(TKey) == typeof(int))
        {
           return (TKey)(object)Convert.ToInt32(rawKey);
        }
        else if (typeof(TKey) == typeof(string))
        {
            return (TKey)(object)rawKey.ToString();
        }
        else if (typeof(TKey).IsEnum)
        {
            return (TKey)Enum.Parse(typeof(TKey), rawKey.ToString());
        }
        else
        {
            return (TKey)rawKey;
        }
    }

    

    private static FieldInfo FindFieldInfo(Type type, string fieldName)
    {
        // 현재 클래스의 필드를 검색합니다.
        FieldInfo fieldInfo = type.GetField(fieldName);
        if (fieldInfo != null)
            return fieldInfo;

        // 현재 클래스의 모든 필드를 가져옵니다.
        FieldInfo[] fields = type.GetFields();

        foreach (FieldInfo field in fields)
        {
            // 필드가 제네릭 리스트인지 확인합니다.
            if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                // 리스트의 내부 타입을 가져옵니다.
                Type elementType = field.FieldType.GetGenericArguments()[0];

                // 내부 타입에서 필드를 재귀적으로 검색합니다.
                fieldInfo = FindFieldInfo(elementType, fieldName);
                if (fieldInfo != null)
                    return fieldInfo;
            }
        }
        return null;
    }*/
}
