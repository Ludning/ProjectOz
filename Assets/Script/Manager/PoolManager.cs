using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingleTonMono<PoolManager>
{
    GameObject sceneRig;
    public GameObject SceneRig
    {
        get 
        {
            if (sceneRig != null) 
                return sceneRig;
            
            sceneRig = GameObject.Find("SceneInstaller");
            
            if (sceneRig != null) 
                return sceneRig;
            
            sceneRig = new GameObject("SceneInstaller");
            
            return sceneRig; 
        }
        set { sceneRig = value; }
    }

    Dictionary<string, Queue<GameObject>> _pool = new Dictionary<string, Queue<GameObject>>();
    int startPoolSize = 10;

    public GameObject GetGameObject(GameObject prefab)
    {
        if (!_pool.ContainsKey(prefab.name))
            CreatePool(prefab);
        if (_pool[prefab.name].Count <= 1)
            ExpansionPool(prefab);
        GameObject copy = _pool[prefab.name].Dequeue();
        copy.transform.SetParent(SceneRig.transform);
        copy.transform.SetParent(null);
        copy.SetActive(true);
        return copy;
    }
    public GameObject GetGameObject(GameObject prefab, Vector3 position)
    {
        if (!_pool.ContainsKey(prefab.name))
            CreatePool(prefab);
        if (_pool[prefab.name].Count <= 1)
            ExpansionPool(prefab);
        GameObject copy = _pool[prefab.name].Dequeue();
        copy.transform.SetParent(SceneRig.transform);
        copy.transform.SetParent(null);
        copy.transform.position = position;
        copy.SetActive(true);
        return copy;
    }
    public GameObject GetGameObject(GameObject prefab, Transform parent)
    {
        if (!_pool.ContainsKey(prefab.name))
            CreatePool(prefab);
        if (_pool[prefab.name].Count <= 1)
            ExpansionPool(prefab);
        GameObject copy = _pool[prefab.name].Dequeue();
        copy.transform.SetParent(SceneRig.transform);
        copy.transform.SetParent(parent);
        copy.transform.localPosition = Vector3.zero;
        copy.SetActive(true);
        return copy;
    }
    public void ReturnToPool(GameObject go)
    {
        if(go == null) 
            return;
        if (!_pool.ContainsKey(go.name))
            CreatePool(go);
        go.transform.SetParent(transform);
        go.SetActive(false);
        _pool[go.name].Enqueue(go);
    }
    public void CreatePool(GameObject go)
    {
        if (_pool.ContainsKey(go.name))
            return;
        _pool.Add(go.name, new Queue<GameObject>());

        for (int i = 0; i < startPoolSize; i++) 
        {
            GameObject copy = Instantiate(go);
            copy.name = go.name;
            ReturnToPool(copy);
        }
    }
    private void ExpansionPool(GameObject go)
    {
        for (int i = 0; i < startPoolSize; i++)
        {
            GameObject copy = Instantiate(go);
            copy.name = go.name;
            ReturnToPool(copy);
        }
    }
}