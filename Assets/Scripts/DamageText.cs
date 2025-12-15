using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour{
    private Vector3 targetPos;
    private Camera cam;
    public TextMeshProUGUI damageText;

    [SerializeField] private GameObject criticalImage;
    
    [Range(0.0f, 5.0f)]
    [SerializeField] private float upRange = 0.0f;

    private void Start(){
        cam = Camera.main;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="dmg"></param>
    /// <param name="critical"></param>
    public void Init(Vector3 pos, double dmg, bool critical = false){
        targetPos = pos;
        damageText.text = dmg.ToString();
        
        // Damage text 위치의 랜덤성
        targetPos.x += Random.Range(-0.2f, 0.2f);
        targetPos.z += Random.Range(-0.2f, 0.2f);
        
        // Damage text가 출력될 때, Canvas로 이동되어야 함
        // UI 스케일 유지 및 로컬 좌표 유지를 위해 SetParent() 사용
        transform.SetParent(BaseCanvasUI.Instance.transform, false);

        // 크리티컬 이펙트
        criticalImage.SetActive(critical);
        damageText.enableVertexGradient = critical;
        
        // Damage text를 object pool로 리턴하기
        BaseManager.Instance.ReturnObject(gameObject, "DamageText", 1.5f);
    }

    private void Update(){
        // 몬스터 위치에 따른 damage text 이동
        var pos = new Vector3(targetPos.x, targetPos.y + upRange, targetPos.z);
        transform.position = cam.WorldToScreenPoint(pos);

        // 텍스트 살짝 올라가는 연출
        if (upRange <= 0.3f){
            upRange += Time.deltaTime;
        }
    }

    /// <summary>
    /// Damage text 사용 후 object pool로 반환하기
    /// </summary>
    private void ReturnText(){
        BaseManager.Pool.poolDictionary["DamageText"].Return(gameObject);
    }


}
