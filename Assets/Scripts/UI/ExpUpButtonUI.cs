using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ExpUpButtonUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler{
    
    // --- 경험치 증가 Slider
    [SerializeField] private Image ExpSlider;
    
    // --- 경험치 증가 Text
    [SerializeField] private TextMeshProUGUI ExpText;
    [SerializeField] private TextMeshProUGUI AcquiredExpText;
    
    // --- 수치 증가 Text
    [SerializeField] private TextMeshProUGUI AtkText;
    [SerializeField] private TextMeshProUGUI HpText;
    [SerializeField] private TextMeshProUGUI CirticalText;
    [SerializeField] private TextMeshProUGUI BossATKText;
    
    // --- 레벌업 비용
    [SerializeField] private TextMeshProUGUI GoldText;
    
    // --- 눌림 여부 flag
    private bool isPush = false;

    // --- 연속 레벨업
    private float continuousLevelUpTimer = 0.0f;
    Coroutine continuousLevelUpCoroutine;

    private void Start(){
        UpdateExp();
        UpdateButtonStats();
    }

    public void ExpUp(){
        
        // 경험치 비약 사용
        BaseManager.Player.ExpUp();
        BaseManager.Player.StatsUp();
        
        // UI 변경
        UpdateExp();
        UpdateButtonStats();
        
        // Tween UI 애니메이션
        transform.DORewind();
        transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.25f);
    }

    private void Update(){
        if (!isPush){
            return;
        }
        
        continuousLevelUpTimer += Time.deltaTime;
        if (continuousLevelUpTimer >= 0.1f){
            ExpUp();
            continuousLevelUpTimer = 0.0f;
        }
    }

    /// <summary>
    /// 경험치 UI 업데이트
    /// </summary>
    private void UpdateExp(){
        
        var expPercentage = BaseManager.Player.GetCurrentExpPercentage();
        var acquiredExpPercentage = BaseManager.Player.GetAcquiredExpPercentage();

        // 경험치 이미지 UI 업데이트
        ExpSlider.fillAmount = expPercentage;
        
        // 경험치 텍스트 UI 업데이트
        ExpText.text = $"{expPercentage * 100.0f:0.000}%";
        
        // 획득 경험치 텍스트 UI 업데이트
        AcquiredExpText.text = $"+ {acquiredExpPercentage * 100.0f: 0.000}%";
    }

    private void UpdateButtonStats(){
        var atk =  BaseManager.Player.GetAcquiredAtk();
        var hp =  BaseManager.Player.GetAcquiredHp();

        AtkText.text = $"+{atk.ToCurrencyString()}";
        HpText.text = $"+{hp.ToCurrencyString()}";
    }
    
    /// <summary>
    /// 레벨업 버튼 키다운
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData){
        
        // 초기 레벨업
        ExpUp();
        
        // Debug.Log("터치 시작");
        continuousLevelUpCoroutine = StartCoroutine(ContinuousPushCoroutine());
    }
    
    /// <summary>
    /// 레벨업 버튼 키업
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData){
        
        // Debug.Log("터치 해제");

        isPush = false;
        if (continuousLevelUpCoroutine != null){
            StopCoroutine(continuousLevelUpCoroutine);
        }

        continuousLevelUpTimer = 0.0f;
    }

    IEnumerator ContinuousPushCoroutine(){
        
        // 연속 터치 임계점: 1.0f
        yield return new WaitForSeconds(1.0f);
        isPush = true;
    }
}
