using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour{

    public int      mCount = 2;             // 몬스터 마릿수
    public float    mSpawnTime = 1.5f;         // 몬스터 스폰 주기
    public float mMaximumSpawnRange = 7.0f; // 몬스터 최대 스폰 범위
    public float mMinimumSpawnRange = 6.0f; // 몬스터 최소 스폰 범위

    // TODO: 플레이어 목록 (DB)에서 가져오기
    public Player clericPlayer;
    public Player barbarianPlayer;
    
    public static List<Monster> monsterList = new List<Monster>();
    public static List<Player> playerList = new List<Player>();
    
    private void Start(){
        StartCoroutine(SpawnMonsterCoroutine());
        
        // TODO: 현재 플레이어 목록에서 가져오기
        // playerList.Add(clericPlayer);
        // playerList.Add(barbarianPlayer);
    }

    /// <summary>
    /// 몬스터 소환 Coroutine
    /// TODO: UniTask로 변환하기
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnMonsterCoroutine(){
        for (var i = 0; i < mCount; i++){

            // 몬스터 소환 위치 지정
            // insideUnitSphere: Vector3에서 랜덤한 값을 부여
            // insideUnitCircle: Vector2에서 랜덤한 값을 부여
            var pos = Vector3.zero + Random.insideUnitSphere * mMaximumSpawnRange;
            pos.y = 0f;

            // 몬스터 최소거리 보장
            while (Vector3.Distance(pos, Vector3.zero) <= mMinimumSpawnRange){
                pos = Vector3.zero + Random.insideUnitSphere * mMaximumSpawnRange;
                pos.y = 0f;
            }

            // 몬스터 소환
            // var go = Instantiate(monsterPrefab, pos, Quaternion.identity);

            // Pool manager를 이용한 몬스터 스폰
            var go = BaseManager.Pool.PoolingObject("Skeleton").Get(value => {
                
                // 기존의 start() 이벤트 함수는 생성될 때 한 번만 호출된다.
                // 따라서, pool에서 나올 때마다 생성한다. 
                value.GetComponent<Monster>().Init();
                
                value.transform.position = pos;
                value.transform.LookAt(Vector3.zero);
                
                // 스폰된 몬스터 리스트에 삽입
                monsterList.Add(value.GetComponent<Monster>());
            });

            // StartCoroutine(DespawnMonsterCoroutine(go));
        }

        // 시간까지 대기
        yield return new WaitForSeconds(mSpawnTime);
        
        // Coroutine 재시작
        StartCoroutine(SpawnMonsterCoroutine());
    }

    /// <summary>
    /// 몬스터 디스폰 Coroutine
    /// </summary>
    /// <param name="obj">디스폰할 몬스터 object</param>
    /// <returns></returns>
    private IEnumerator DespawnMonsterCoroutine(GameObject obj){

        yield return new WaitForSeconds(3.5f);
        
        BaseManager.Pool.poolDictionary["Skeleton"].Return(obj);
    }
}
