using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Vector3 targetPos;
    private Camera cam;
    RectTransform[] coinChilds = new RectTransform[5];
    
    [Range(0f, 500f)]
    [SerializeField] private float coinRange, coinSpeed;
    
    private void Awake(){
        cam = Camera.main;
        for (var i = 0; i < coinChilds.Length; i++){
            coinChilds[i] = transform.GetChild(i).GetComponent<RectTransform>();
        }
    }

    public void Init(Vector3 pos){
        // 코인 이미지 parent 설정
        transform.SetParent(BaseCanvasUI.Instance.GetLayer(0), false);
        
        // 코인 위치 설정
        targetPos = pos;
        transform.position =  cam.WorldToScreenPoint(pos);
        
        foreach (var t in coinChilds){
            t.anchoredPosition = Vector2.zero;
        }
        
        // 코인 이펙트 시작
        StartCoroutine(CoinEffectCoroutine());
    }

    /// <summary>
    /// Coin이 몬스터 주변에 흩뿌려지는 이펙드 생성 coroutine
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoinEffectCoroutine(){

        // 몬스터 주변에 생성될 위치
        var randomPos = new Vector2[coinChilds.Length];
        for (var i = 0; i < coinChilds.Length; i++){
            randomPos[i] = new Vector2(targetPos.x, targetPos.y) + Random.insideUnitCircle * Random.Range(-coinRange, coinRange);
        }

        // 모든 코인이 흩뿌려질 때가지 loop
        // TODO: 무한루프가 최선인가? 일정 시간이 지나고 끝나도록 해야 하지 않을까?
        while (true){
            for(var i = 0; i < coinChilds.Length; i++){
                var rect = coinChilds[i];
                
                // parent를 기준으로(로컬 좌표 기준으로) 코인들이 이동함
                rect.anchoredPosition = Vector2.MoveTowards(rect.anchoredPosition, randomPos[i], Time.deltaTime * coinSpeed);
            }

            // 모든 코인이 드랍됨
            if (IsCoinDropped(randomPos, 0.5f)){
                break;
            }

            yield return null;
        }
        
        // 코인 드랍된 이후 대기시간
        yield return new WaitForSeconds(0.3f);

        // 코인 인벤토리 UI로 이동
        while (true){
            foreach (var rect in coinChilds){
                // Screen 좌표 기준으로 이동
                rect.position = Vector2.MoveTowards(rect.position, BaseCanvasUI.Instance.coinTransform.position, Time.deltaTime * coinSpeed * 20f);
            }

            // 모든 코인을 획득함
            if (IsCoinLooted(0.5f)){
                
                // Pool object에 반환
                BaseManager.Pool.poolDictionary["Coin"].Return(gameObject);
                break;
            }
            
            yield return null;
        }
        
        yield return null;
    }

    /// <summary>
    /// 생성된 코인이 모두 드랍되었는지 판단하는 함수
    /// </summary>
    /// <param name="end">코인 드랍 위치 배열</param>
    /// <param name="range">거리 오차 범위</param>
    /// <returns></returns>
    private bool IsCoinDropped(Vector2[] end, float range){
        for (var i = 0; i < coinChilds.Length; i++){
            var distance = Vector2.Distance(coinChilds[i].anchoredPosition, end[i]);

            // Coin이 도착하지 않음
            if (distance > range){
                return false;
            }
        }

        // 모든 coin이 도착함
        return true;
    }

    /// <summary>
    /// 생성된 코인들을 모두 획득했는지 판단하는 함수
    /// </summary>
    /// <param name="range">코인 획득 오차 범위</param>
    /// <returns></returns>
    private bool IsCoinLooted(float range){
        for (var i = 0; i < coinChilds.Length; i++){
            var distance = Vector2.Distance(coinChilds[i].anchoredPosition, BaseCanvasUI.Instance.coinTransform.position);

            // Coin이 도착하지 않음
            if (distance > range){
                return false;
            }
        }

        // 모든 coin이 도착함
        return true;
    }
}
