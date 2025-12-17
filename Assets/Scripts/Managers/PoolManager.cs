using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPool{
    
    // pool object들이 들어있을 부모 object
    Transform parentTransform{ get; set; }
    
    // pool object들이 들어있을 queue
    Queue<GameObject> poolQueue{ get; set; }

    // 몬스터 object를 pool에서 가져오기
    GameObject Get(Action<GameObject> action = null);
    
    // 몬스터 object pool에 다시 집어넣기
    void Return(GameObject obj, Action<GameObject> action = null);
}

public class ObjectPool: IPool{
    public Transform parentTransform{ get; set; }
    public Queue<GameObject> poolQueue{ get; set; } = new Queue<GameObject>();
    
    
    /// <summary>
    /// Pool에 있는 object를 꺼내기
    /// </summary>
    /// <param name="action">object에 실행할 delegate</param>
    /// <returns></returns>
    public GameObject Get(Action<GameObject> action = null){
        var obj = poolQueue.Dequeue();
        
        // Object 활성화
        obj.SetActive(true);

        // Delegate invoke
        action?.Invoke(obj);

        return obj;
    }
    
    public void Return(GameObject obj, Action<GameObject> action = null){
        poolQueue.Enqueue(obj);
        
        obj.SetActive(false);
        obj.transform.SetParent(parentTransform);
        // obj.transform.parent = parentTransform;
        
        action?.Invoke(obj);
    }
}

public class PoolManager
{
    public Dictionary<string, IPool> poolDictionary = new Dictionary<string, IPool>();
    private Transform baseObject = null;    // PoolManager object의 parent: BaseManager object

    public void Initialize(Transform T){
        baseObject = T;
    }

    public IPool PoolingObject(string path){
        
        // 기존에 만들어진 pool이 없다면 새로 생성하기
        if (!poolDictionary.ContainsKey(path)){
            AddPool(path);
        }

        // Queue 내부에 object가 없다면 생성하기
        if (poolDictionary[path].poolQueue.Count <= 0){
            AddQueue(path);
        }
        
        return poolDictionary[path];
    }

    /// <summary>
    /// 기존에 만들어진 Pool이 없다면,
    /// IPool을 상속받은 새로운 object를 dictionary 내부로 삽입
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private GameObject AddPool(string path){
        GameObject obj = new GameObject(path + "@Pool");
        obj.transform.SetParent(baseObject);
        ObjectPool TComponent = new ObjectPool();
        
        poolDictionary.Add(path, TComponent);
        TComponent.parentTransform = obj.transform;
        
        return obj;
    }

    /// <summary>
    /// Pool 내부에 있는 모든 object가 활성화 상태라면
    /// 새로운 object 생성하기
    /// </summary>
    /// <param name="path"></param>
    private void AddQueue(string path){
        
        // Pool에서 object 생성
        var go = BaseManager.Instance.InstantiatePath(path);
        go.transform.SetParent(poolDictionary[path].parentTransform);
        // go.transform.parent = poolDictionary[path].parentTransform;

        poolDictionary[path].Return(go);
    }
}
