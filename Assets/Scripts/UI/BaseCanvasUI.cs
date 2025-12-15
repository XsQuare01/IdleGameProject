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
}
