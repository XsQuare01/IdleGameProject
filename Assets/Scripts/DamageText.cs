using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    protected Vector3 targetPos;
    protected Camera cam;
    
    [SerializeField] protected TextMeshProUGUI damageText;
    [SerializeField] protected GameObject criticalImage;
    
    [Range(0.0f, 5.0f)]
    [SerializeField] protected float upRange = 0.0f;

    protected virtual void Awake(){
        cam = Camera.main;
    }
    
    public virtual void SetDamageText(Vector3 pos, double dmg, bool critical = false){
        // 데미지 대상 및 데미지 수치
        targetPos = pos;
        damageText.text = dmg.ToCurrencyString();
        
        // Damage text 위치의 랜덤성
        targetPos.x += Random.Range(-0.2f, 0.2f);
        targetPos.z += Random.Range(-0.2f, 0.2f);
        
        // Damage text가 출력될 때, Canvas로 이동되어야 함
        // UI 스케일 유지 및 로컬 좌표 유지를 위해 SetParent() 사용
        transform.SetParent(BaseCanvasUI.Instance.GetLayer(1), false);
        
        // 크리티컬 이펙트
        criticalImage.SetActive(critical);
        damageText.enableVertexGradient = critical;
        
        // UI 위치 초기 설정
        transform.position = cam.WorldToScreenPoint(pos);
    }

    protected virtual void Update(){
        // 타겟 위치에 따른 damage text 이동
        var pos = new Vector3(targetPos.x, targetPos.y + upRange, targetPos.z);
        transform.position = cam.WorldToScreenPoint(pos);

        // 텍스트 살짝 올라가는 연출
        if (upRange <= 0.3f){
            upRange += Time.deltaTime;
        }
    }
}
