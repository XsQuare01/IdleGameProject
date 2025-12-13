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
}
