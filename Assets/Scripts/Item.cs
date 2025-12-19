using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class Item : MonoBehaviour{
    
    // --- 아이템 이름
    [SerializeField] private Transform itemTextTransform;
    [SerializeField] private TextMeshProUGUI itemNameText;
    
    // --- 아이템 드롭
    [SerializeField] private float firingAngle = 45.0f;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float itemDropRange = 0.5f;

    // --- 아이템 희귀도
    [SerializeField] private GameObject[] rarityTrails;
    [SerializeField] private GameObject[] rarityPickups;
    private Rarity rarity;
    
    private bool isDropped = false;    

    /// <summary>
    /// 아이템 초기화 함수
    /// </summary>
    /// <param name="pos"></param>
    public void Init(Vector3 pos){
        // 초기화
        for (var i = 0; i < rarityPickups.Length; i++){
            rarityPickups[i].SetActive(false);
            rarityTrails[i].SetActive(false);
        }
        
        isDropped = false;
        transform.position = pos;
        
        // 아이템 등급
        // TODO: 아이템 등급 실제로 가져오기
        // TODO: 다른 아이템 등급 VFX 적용하기 - 현재는 Unique 등급
        rarity = (Rarity)Random.Range(0, rarityPickups.Length);
        
        // 등급에 따라 보여질 trail 결정
        rarityTrails[(int)rarity].SetActive(true);
        
        // 아이템이 떨어질 위치
        var targetPos = new Vector3(pos.x + Random.insideUnitSphere.x * itemDropRange, pos.y, pos.z + Random.insideUnitSphere.z * itemDropRange);
        StartCoroutine(SimulateProjectileCoroutine(targetPos));
    }

    private void Update(){
        if (!isDropped){
            return;
        }
        
        // 아이템 이름 위치 변경
        itemTextTransform.position = Camera.main.WorldToScreenPoint(transform.position);
    }

    private void ItemRarityCheck(){
        // 아이템 드롭 완료
        isDropped = true;
        
        // 아이템 trail 끄기
        rarityTrails[(int)rarity].SetActive(false);
        
        // 아이템 이름 회전 초기화
        transform.rotation = Quaternion.identity;
        
        // BaseCanvas로 이동
        itemTextTransform.gameObject.SetActive(true);
        itemTextTransform.SetParent(BaseCanvasUI.Instance.GetLayer(2));

        // 아이템 이름 변경
        // TODO: 아이템 이름 데이터 가지고오기
        // itemNameText.text = "Test_item";
        SetItemNameColor("Text_item");
        
        // 등급에 따른 VFX 활성화
        rarityPickups[(int)rarity].SetActive(true);

        // 아이템 획득
        StartCoroutine(LootItemCoroutine());
    }

    /// <summary>
    /// 아이템 희귀도에 따른 아이템 이름 색상 결정
    /// </summary>
    /// <param name="itemName">아이템 이름</param>
    private void SetItemNameColor(string itemName){
        // string.Concat 보간으로 1개의 string만 생성
        // StringBuilder 사용 시 2번의 메모리 사용으로 메모리 낭비
        itemNameText.text = $"{Utils.GetRarityColorUtility(rarity)}{itemName}</color>";
    }

    /// <summary>
    /// 아이템 드랍 시 떨어지는 Coroutine
    /// </summary>
    /// <param name="pos">아이템이 떨어질 위치</param>
    /// <returns></returns>
    private IEnumerator SimulateProjectileCoroutine(Vector3 pos){
        var startPos = transform.position;
        
        // 수평 거리
        var targetDistance = Vector3.Distance(startPos, pos);
        
        // Deg2Rad: 각도 -> 라디안
        var angleRad = firingAngle * Mathf.Deg2Rad;
        var sin2Angle = Mathf.Sin(2f * angleRad);
        
        // 초기 속도 계산
        var velocitySquared = targetDistance * gravity / sin2Angle;
        var speed = Mathf.Sqrt(velocitySquared);

        // 속도 성분 분해
        var velocityX = speed * Mathf.Cos(angleRad);
        var velocityY = speed * Mathf.Sin(angleRad);
        
        // 비행 시간
        var flightDuration = targetDistance / velocityX;
        
        //떨어지는 방향
        transform.rotation = Quaternion.LookRotation(pos - startPos);

        // 이동 공식
        var time = 0.0f;
        while (time < flightDuration){
            /*var yOffset = velocityY * time - 0.5f * gravity * time * time;
            var horizontalOffset = (pos - startPos) * (velocityX * time);

            transform.position = startPos + horizontalOffset + Vector3.up * yOffset;*/
            
            transform.Translate(0, (velocityY - (gravity * time)) * Time.deltaTime, velocityX * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }
        
        // 아이템 드랍 후 희귀도 표시
        ItemRarityCheck();
    }

    /// <summary>
    /// 아이템 획득 & 소멸 coroutine
    /// </summary>
    /// <returns></returns>
    private IEnumerator LootItemCoroutine(){
        
        // 아이템 획득 대기시간
        yield return new WaitForSeconds(Random.Range(1.0f, 1.5f));
        
        // 아이템 등급 희귀도 끄기
        rarityPickups[(int)rarity].SetActive(false);
        
        // 아이템 이름 원상복구
        itemTextTransform.transform.SetParent(transform);
        itemTextTransform.gameObject.SetActive(false);
        
        // TODO: 아이템 획득 particle system play
        // itemLootParticleSystem.Play();
        
        yield return new WaitForSeconds(0.5f);
        
        // Object pool에 다시 반환
        BaseManager.Pool.poolDictionary["Item"].Return(gameObject);
    }
}
