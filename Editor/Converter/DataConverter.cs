using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityVector2;
using ExcelDataReader;
using Sirenix.Utilities;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class DataConverter
{
    public static void LoadExcel<T>(string xlsxPath, string assetPath) where T : ScriptableObject
    {
        Debug.Log("ReadExcel");

        //파일 존재 체크
        if (IsFileExists(xlsxPath) == false)
            return;

        //엑셀파일로 부터 테이블 데이터 로드
        var tables = GetTableFromXlsx(xlsxPath);
        var asset = GetScriptableAsset<T>(assetPath);

        //데이터를 스크립터블로 파싱
        ParseTableToScriptable<T>(asset, tables);
        
        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    //엑셀파일로 부터 테이블 데이터 로드
    private static DataTableCollection GetTableFromXlsx(string xlsxPath)
    {
        using var stream = new FileStream(xlsxPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        // FileStream을 사용한 코드 작성
        using var reader = ExcelReaderFactory.CreateReader(stream);
        // 모든 시트 로드
        return reader.AsDataSet().Tables;
    }
    
    private static T GetScriptableAsset<T>(string assetPath) where T : ScriptableObject
    {
        T loadedAsset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        if (loadedAsset == null)
        {
            loadedAsset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(loadedAsset, assetPath);
        }
        return loadedAsset;
    }

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

    /*public static Dictionary<TKey, TValue> ReadDataFromTable<TKey, TValue>(string sheetName, DataTableCollection tables) where TValue : class, new()
    {
        Debug.Log(sheetName);
        var data = new Dictionary<TKey, TValue>();
        if (!tables.Contains(sheetName))
        {
            throw new ArgumentException($"Sheet {sheetName} does not exist.");
        }
        
        DataTable table = tables[sheetName];
        TKey previousKey = default(TKey);

        foreach (DataRow row in table.Rows)
        {
            if (row == table.Rows[0])
                continue;
            
            // 첫 번째 컬럼의 데이터를 TKey로 설정
            TKey key;
            TValue value;
            if (previousKey.Equals(default(TKey)) && row[0] == DBNull.Value)
                key = previousKey;
            else
                key = (TKey)Convert.ChangeType(row[0], typeof(TKey));

            value = data.TryGetValue(key, out TValue val) ? val : new TValue();
            
            //Debug.Log(key.ToString());

            // 나머지 컬럼의 데이터를 필드로 매핑
            foreach (var field in typeof(TValue).GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (table.Columns.Contains(field.Name))
                {
                    var columnValue = row[field.Name];
                    if (columnValue != DBNull.Value)
                    {
                        if (field.FieldType == typeof(List<SkillElement>))
                        {
                            var element = new SkillElement();
                            var elementFields = element.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                            foreach (var elementField in elementFields)
                            {
                                if (table.Columns.Contains(elementField.Name))
                                {
                                    var elementValue = row[elementField.Name];
                                    if (elementValue != DBNull.Value)
                                    {
                                        elementField.SetValue(element, Convert.ChangeType(elementValue, elementField.FieldType));
                                    }
                                }
                            }

                            var list = (List<SkillElement>)field.GetValue(value) ?? new List<SkillElement>();
                            list.Add(element);
                            field.SetValue(value, list);
                        }
                        else
                        {
                            field.SetValue(value, Convert.ChangeType(columnValue, field.FieldType));
                        }
                    }
                }
            }
            if (!data.ContainsKey(key))
            {
                data.Add(key, value);
            }
            previousKey = key;
        }
        return data;
    }*/
    
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

            /*//리스트 객체 생성
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
            }*/
            
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
            }*/
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
    }

    /*public static Dictionary<TKey, TValue> ReadDataFromTable<TKey, TValue>(string sheetName, DataTableCollection tables) where TValue : class, new()
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
        
        Dictionary<int, string> columnTypeDic = new Dictionary<int, string>();
        
        //0행의 데이터를 가져온다, 0행의 데이터는 자료형을 결정하기 떄문
        RecordTypeName(columnTypeDic, fieldInfos, sheet);

        TKey prevKey = default(TKey);

        foreach (DataRow dataRow in sheet.Rows)
        {
            TKey key;
            object keyObject = dataRow.ItemArray[0];
            if (keyObject != DBNull.Value)
                key = ConvertKey<TKey>(keyObject);
            else if(prevKey != null && !prevKey.Equals(default(TKey)))
                key = prevKey;
            else
                continue;
            TValue data = ret.TryGetValue(key, out TValue value) ? value : new TValue();

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
            }
            
            for (int columnIndex = 1; columnIndex < dataRow.ItemArray.Length; columnIndex++)
            {
                if (!columnTypeDic.TryGetValue(columnIndex, out string itemTypeName))
                    continue;
                FieldInfo[] columnFields = data.GetType().GetFields();
                FieldInfo fieldInfo = Array.Find(columnFields, fi => fi.Name == itemTypeName);
                
            }

            ret[key] = data;
        }
        return ret;
    }*/

    /*private static object GetListInstance<TValue>(FieldInfo info, TValue data)
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
                return info.GetValue(data);
            }
            return info.GetValue(data);
        }
    }*/
        

        //TKey prevKey = default(TKey);

        /*for (var rowIndex = 1; rowIndex < sheet.Rows.Count; rowIndex++)
        {
            // 행 가져오기
            var dataRow = sheet.Rows[rowIndex];
            TKey key;
            if (dataRow.ItemArray[0] == DBNull.Value && prevKey.Equals(default(TKey)))
            {
                continue;
            }
            else if (dataRow.ItemArray[0] == DBNull.Value && !prevKey.Equals(default(TKey)))
            {
                key = prevKey;
            }
            else
            {
                key = ConvertKey<TKey>(dataRow.ItemArray[0]);
            }
            
            TValue data = (ret.TryGetValue(key, out TValue val))? val : new TValue();
            FieldInfo[] dataFields = typeof(TValue).GetFields();
            
            
            foreach (FieldInfo dataFieldInfo in dataFields)
            {
                //리스트일 경우
                if (dataFieldInfo.FieldType.IsGenericType && dataFieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    //리스트가 null일 경우
                    if (dataFieldInfo.GetValue(data) == null)
                    {
                        // GenericArgument로 타입을 가져옵니다.
                        Type elementType = dataFieldInfo.FieldType.GetGenericArguments()[0];
                        // 인스턴스를 생성합니다.
                        object listInstance = Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                        Debug.Log("List 객체가 생성되었습니다. type: " + listInstance.GetType());
                        dataFieldInfo.SetValue(data, listInstance);
                    }
                }
            }
            
            for (var columnIndex = 0; columnIndex < dataRow.ItemArray.Length; columnIndex++)
            {
                var item = dataRow.ItemArray[columnIndex];
                if (!columnTypeDic.TryGetValue(columnIndex, out string value))
                    continue;

                //필드에서 검색
                FieldInfo fieldInfo = Array.Find(fieldInfos, field => field.Name == value);
                if (fieldInfo != null)
                {
                    Type type = fieldInfo.FieldType;
                    Debug.Log($"Type : {type}, Data : {item.ToString()}");

                    if (type.IsEnum)
                        fieldInfo.SetValue(data, Enum.Parse(type, item.ToString()));
                    else if (type == typeof(string))
                        fieldInfo.SetValue(data, item.ToString());
                    else if (type.IsPrimitive)
                        fieldInfo.SetValue(data, Convert.ChangeType(item, type));
                }
                //순회했지만 발견하지 못하면 제네릭 타입 필드를 찾고 순회
                else
                {
                    /*foreach (FieldInfo dataFieldInfo in dataFields)
                    {
                        //리스트일 경우
                        if (dataFieldInfo.FieldType.IsGenericType && dataFieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            Type genericType = dataFieldInfo.FieldType.GetGenericArguments()[0];
                            FieldInfo[] childFields = genericType.GetFields();
                            FieldInfo childFieldInfo = Array.Find(childFields, field => field.Name == value);
                            if (childFieldInfo != null)
                            {
                                object listData = dataFieldInfo.GetValue(data);
                                Type type = childFieldInfo.FieldType;
                                
                                
                                //rowIndex
                                
                                //object listElement = Activator.CreateInstance(genericType);
                                Debug.Log($"Type : {type}, Data : {item.ToString()}");

                                if (type.IsEnum)
                                    childFieldInfo.SetValue(listElement, Enum.Parse(type, item.ToString()));
                                else if (type == typeof(string))
                                    childFieldInfo.SetValue(listElement, item.ToString());
                                else if (type.IsPrimitive)
                                    childFieldInfo.SetValue(listElement, Convert.ChangeType(item, type));
                            }
                        }
                    }#1#
                }
                //찾은 필드에 값을 대입
                //없으면 continue;
                
                //FieldInfo fieldInfo = Array.Find(fieldInfos, field => field.Name == value);
                //FieldInfo fieldInfo = FindFieldInfo(dataType, value);
                //if(fieldInfo == null)
                //    continue;

                
                /*else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type itemType = type.GetGenericArguments()[0];
                    MethodInfo parseListMethod = typeof(YourClass).GetMethod("ParseList").MakeGenericMethod(itemType);
                    object list = parseListMethod.Invoke(null, new object[] { item.ToString() });
                    fieldInfo.SetValue(data, list);
                }#1#

            }
            // 키 값을 가져오기
            //TKey key = (TKey)fieldInfos[0].GetValue(data);

            ret[key] = data;
        }*/


        /*for (var rowIndex = 1; rowIndex < sheet.Rows.Count; rowIndex++)
        {
            // 행 가져오기
            var dataRow = sheet.Rows[rowIndex];
            TValue data = new TValue();

            for (var columnIndex = 0; columnIndex < dataRow.ItemArray.Length; columnIndex++)
            {
                var item = dataRow.ItemArray[columnIndex];
                if (!columnTypeDic.TryGetValue(columnIndex, out string value))
                    continue;

                FieldInfo fieldInfo = Array.Find(fieldInfos, field => field.Name == value);
                if(fieldInfo == null)
                    continue;


                Type type = fieldInfo.FieldType;
                Debug.Log($"Type : {type}, Data : {item.ToString()}");

                if (type.IsEnum)
                    fieldInfo.SetValue(data, Enum.Parse(type, item.ToString()));
                else if (type == typeof(string))
                    fieldInfo.SetValue(data, item.ToString());
                else if (type.IsPrimitive)
                    fieldInfo.SetValue(data, Convert.ChangeType(item, type));
                else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type itemType = type.GetGenericArguments()[0];
                    MethodInfo parseListMethod = typeof(YourClass).GetMethod("ParseList").MakeGenericMethod(itemType);
                    object list = parseListMethod.Invoke(null, new object[] { item.ToString() });
                    fieldInfo.SetValue(data, list);
                }

            }
            // 키 값을 가져오기
            TKey key = (TKey)fieldInfos[0].GetValue(data);

            ret[key] = data;
        }*/
        

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

    //파일이 있는지 확인
    private static bool IsFileExists(string path)
    {

        Debug.Log("Path : " + path);
        var isExist = File.Exists(path);
        if (isExist == false)
            Debug.LogError("Xlsx 파일이 존재하지 않습니다.");

        return isExist;
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
    }
    
    
    /*public static void ReadCsv(string csvPath)
    {
        Debug.Log("ReadExcel");

        if (IsFileExists(csvPath) == false)
            return;

        // 한글 깨짐 현상 해결 가능
        var config = new ExcelReaderConfiguration();
        config.FallbackEncoding = Encoding.GetEncoding("ks_c_5601-1987");

        var stream = new FileStream(csvPath, FileMode.Open, FileAccess.Read);
        using (var reader = ExcelReaderFactory.CreateCsvReader(stream, config))
        {
            // 항상 하나의 시트만 관리된다.
            var sheet = reader.AsDataSet().Tables[0];
            // 시트 이름
            Debug.Log($"Sheet Name: {sheet.TableName}");
            for (var rowIndex = 0; rowIndex < sheet.Rows.Count; rowIndex++)
            {
                // 행 가져오기
                var slot = sheet.Rows[rowIndex];
                for (var columnIndex = 0; columnIndex < slot.ItemArray.Length; columnIndex++)
                {
                    // 열 가져오기
                    var item = slot.ItemArray[columnIndex];
                    Debug.Log($"slot[{rowIndex}][{columnIndex}] : {item}");
                }
            }

            reader.Dispose();
            reader.Close();
        }
    }*/
    /*public static void ReadJson(string jsonPath)
    {
        //xlsx파일 주소
        //string xlsxPath = "Assets/Resource/Xlsx/data.json";

        Debug.Log("ReadJson");

        if (IsFileExists(jsonPath) == false)
            return;

        using (var stream = File.Open(jsonPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            /*var data = await JsonSerializer.DeserializeAsync<YourDataType>(stream);
            Debug.Log($"Name: {data.Name}, Age: {data.Age}");#1#
        }
    }*/
    
    
    /*public static Dictionary<TKey, TValue> ReadDataFromTable<TKey, TValue>(string sheetName, DataTableCollection tables) where TValue : class, new()
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
        
        Dictionary<int, string> columnTypeDic = new Dictionary<int, string>();
        Dictionary<int, FieldInfo> fieldInfoDic = new Dictionary<int, FieldInfo>();

        foreach (FieldInfo fieldInfo in dataType.GetFields())
        {
            
        }
        
        //0행의 데이터를 가져온다, 0행의 데이터는 자료형을 결정하기 떄문
        for (int fieldColumn = 0; fieldColumn < dataType.GetFields().Length; fieldColumn++)
        {
            string val = (string)(sheet.Rows[0].ItemArray[fieldColumn]);
            if (string.IsNullOrWhiteSpace(val))
                break;
            columnTypeDic.Add(fieldColumn, val);
        }

        for (var rowIndex = 1; rowIndex < sheet.Rows.Count; rowIndex++)
        {
            // 행 가져오기
            var dataRow = sheet.Rows[rowIndex];

            object rawKey = dataRow.ItemArray[0];
            TKey key;
            if (typeof(TKey) == typeof(int))
            {
                key = (TKey)(object)Convert.ToInt32(rawKey);
            }
            else if (typeof(TKey) == typeof(string))
            {
                key = (TKey)(object)rawKey.ToString();
            }
            else if (typeof(TKey).IsEnum)
            {
                key = (TKey)Enum.Parse(typeof(TKey), rawKey.ToString());
            }
            else
            {
                key = (TKey)rawKey;
            }


            //중복된 딕셔너리 키가 있으면 해당 키에 해당하는 값을 가져옴, 없으면 새로 생성
            TValue data = ret.TryGetValue(key, out var val) ? val : new TValue();

            for (var columnIndex = 0; columnIndex < dataRow.ItemArray.Length; columnIndex++)
            {
                //데이터테이블의 아이템 추출
                var item = dataRow.ItemArray[columnIndex];
                if (!columnTypeDic.TryGetValue(columnIndex, out string value))
                    continue;

                //데이터의 필드를 순회해 이름에 맞는 필드가 존재하는가?
                FieldInfo fieldInfo = Array.Find(fieldInfos, field => field.Name == value);
                //FieldInfo fieldInfo = FindFieldInfo(dataType, value);
                if (fieldInfo == null)
                    continue;

                Type type = fieldInfo.FieldType;
                Debug.Log($"Type : {type}, Data : {item.ToString()}");

                if (type.IsEnum)
                    fieldInfo.SetValue(data, Enum.Parse(type, item.ToString()));
                else if (type == typeof(string))
                    fieldInfo.SetValue(data, item.ToString());
                else if (type.IsPrimitive)
                    fieldInfo.SetValue(data, Convert.ChangeType(item, type));
                /*else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type itemType = type.GetGenericArguments()[0];
                    MethodInfo parseListMethod = typeof(YourClass).GetMethod("ParseList").MakeGenericMethod(itemType);
                    object list = parseListMethod.Invoke(null, new object[] { item.ToString() });
                    fieldInfo.SetValue(data, list);
                }#1#

            }

            ret[key] = data;
        }


        /*for (var rowIndex = 1; rowIndex < sheet.Rows.Count; rowIndex++)
        {
            // 행 가져오기
            var dataRow = sheet.Rows[rowIndex];
            TValue data = new TValue();

            for (var columnIndex = 0; columnIndex < dataRow.ItemArray.Length; columnIndex++)
            {
                var item = dataRow.ItemArray[columnIndex];
                if (!columnTypeDic.TryGetValue(columnIndex, out string value))
                    continue;

                FieldInfo fieldInfo = Array.Find(fieldInfos, field => field.Name == value);
                if(fieldInfo == null)
                    continue;


                Type type = fieldInfo.FieldType;
                Debug.Log($"Type : {type}, Data : {item.ToString()}");

                if (type.IsEnum)
                    fieldInfo.SetValue(data, Enum.Parse(type, item.ToString()));
                else if (type == typeof(string))
                    fieldInfo.SetValue(data, item.ToString());
                else if (type.IsPrimitive)
                    fieldInfo.SetValue(data, Convert.ChangeType(item, type));
                else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type itemType = type.GetGenericArguments()[0];
                    MethodInfo parseListMethod = typeof(YourClass).GetMethod("ParseList").MakeGenericMethod(itemType);
                    object list = parseListMethod.Invoke(null, new object[] { item.ToString() });
                    fieldInfo.SetValue(data, list);
                }

            }
            // 키 값을 가져오기
            TKey key = (TKey)fieldInfos[0].GetValue(data);

            ret[key] = data;
        }#1#
        return ret;
    }*/
}
