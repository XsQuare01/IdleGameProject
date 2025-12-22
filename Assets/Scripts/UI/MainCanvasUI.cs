using TMPro;
using UnityEngine;

public class MainCanvasUI : MonoBehaviour{

    public static MainCanvasUI Instance = null;
    
    [SerializeField] private TextMeshProUGUI levelText;

    private void Awake(){
        if (Instance == null){
            Instance = this;
        }
    }

    private void Start(){
        SetLevelText(BaseManager.Player.Level);
    }
    
    public void SetLevelText(int level){
        levelText.text = $"Lv. {BaseManager.Player.Level}";
    }
}
