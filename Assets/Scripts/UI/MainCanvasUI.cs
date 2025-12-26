using TMPro;
using UnityEngine;

/// <summary>
/// 게임 메인에 등장하는 UI 관리
/// </summary>
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

    private void Update(){
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        
        // 뒤로가기 버튼
        if (Utils.UIHolder.Count > 0){
            Utils.ClosePopupUI();
        }
        else{
            // TODO: 게임 종료 팝업
        }
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
    
    /// <summary>
    /// UI 생성 함수
    /// </summary>
    /// <param name="uiName"></param>
    public void GetUI(string uiName){
        
        // 생성할 UI 가져오기
        var ui = Instantiate(Resources.Load<BaseUI>("UI/" + uiName), transform);

        // UI stack에 삽입
        Utils.UIHolder.Push(ui);
    }
    
}
