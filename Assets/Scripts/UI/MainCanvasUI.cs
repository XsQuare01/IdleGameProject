using TMPro;
using UnityEngine;

public class MainCanvasUI : MonoBehaviour{

    // --- Singleton
    public static MainCanvasUI Instance = null;
    
    // --- 캐릭터 레벨 UI
    [SerializeField] private TextMeshProUGUI levelText;

    // --- 캐릭터 전투력 UI
    [SerializeField] private TextMeshProUGUI combatPowerText;
    
    
    private void Awake(){
        if (Instance == null){
            Instance = this;
        }
    }

    private void Start(){
        SetLevelText(BaseManager.Player.Level);
        SetCombatPowerText(BaseManager.Player.CombatPower);
    }
    
    /// <summary>
    /// 캐릭터 레벨 UI 설정
    /// </summary>
    /// <param name="level">캐릭터 레벨</param>
    public void SetLevelText(int level){
        levelText.text = $"Lv. {level}";
    }

    /// <summary>
    /// 캐릭터 전투력 UI 설정
    /// </summary>
    /// <param name="combatPower">캐릭터 전투력</param>
    public void SetCombatPowerText(double combatPower){
        combatPowerText.text = combatPower.ToCurrencyString();
    }
}
