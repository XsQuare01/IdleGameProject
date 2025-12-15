using System.Collections;
using UnityEngine;

// 모든 매니저 스크립트의 집합체
public class BaseManager : MonoBehaviour
{
    public static BaseManager Instance = null;

    public static PoolManager Pool{ get; } = new PoolManager();

    private void Awake(){
        Initialize();
    }

    private void Initialize(){
        if (Instance == null){
            Instance = this;
            Pool.Initialize(transform);
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }

    public GameObject InstantiatePath(string path){
        return Instantiate(Resources.Load<GameObject>(path));
    }

    public void ReturnParticle(GameObject obj, string path){
        StartCoroutine(ReturnParticleCoroutine(obj,path));
    }

    public void ReturnObject(GameObject obj, string path, float duration){
        StartCoroutine(ReturnObjectCoroutine(obj,path, duration));
    }

    private IEnumerator ReturnParticleCoroutine(GameObject obj, string path){
        var ps = obj.GetComponent<ParticleSystem>();
        yield return new WaitWhile(() => ps.IsAlive(true));
        
        Pool.poolDictionary[path].Return(obj);
    }

    private IEnumerator ReturnObjectCoroutine(GameObject obj, string path, float duration){
        yield return new WaitForSeconds(duration);
        
        Pool.poolDictionary[path].Return(obj);
    }
}
