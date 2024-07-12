using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;


public class ResourceManager : SingleTon<ResourceManager>
{
    private Dictionary<Type, Dictionary<string, Object>> _resourceDictionary;
    
    public T LoadResource<T>(string resourceName) where T : UnityEngine.Object
    {
        string resourcePath = DataManager.Instance.GetAssetAddress(resourceName);
        
        T resource = Addressables.LoadAssetAsync<T>(resourcePath).WaitForCompletion();
        if (resource == null)
            throw new System.NotImplementedException();
        return resource;
    }
    public T LoadResourceWithCaching<T>(string resourceName) where T : UnityEngine.Object
    {
        if(_resourceDictionary == null)
            _resourceDictionary = new Dictionary<Type, Dictionary<string, Object>>();
        
        if(!_resourceDictionary.ContainsKey(typeof(T)))
            _resourceDictionary.Add(typeof(T), new Dictionary<string, Object>());
        
        return LoadResourceWithCaching<T>(resourceName, _resourceDictionary[typeof(T)]);
    }
    
    private T LoadResourceWithCaching<T>(string resourceName, Dictionary<string, Object> resourceDict) where T : UnityEngine.Object
    {
        if(!resourceDict.ContainsKey(resourceName))
            resourceDict.Add(resourceName, LoadResource<T>(resourceName));
        
        return (T)resourceDict[resourceName];
    }
}
