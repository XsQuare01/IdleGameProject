using UnityEngine;

public class BaseCanvasUI : MonoBehaviour{
    public static BaseCanvasUI Instance;
    
    private void Awake(){
        Initialize();
    }

    private void Initialize(){
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }
    
    public Transform coinTransform;

    [SerializeField] private Transform layer;
    
    public Transform GetLayer(int value){
        return layer.GetChild(value);
    }
    
    

}
