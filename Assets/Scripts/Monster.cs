using System;
using System.Collections;
using UnityEngine;

public class Monster : Character{
    [SerializeField] private float mSpeed = 0.5f;

    private bool isSpawn = false;

    protected override void Start(){
        base.Start();
        
        // TODO: 실제 데이터 가져오기
        hp = 25;
    }

    private void Update(){

        // 스폰되지 않은 상태: 이동하지 않음
        if (!isSpawn){
            return;
        }

        // 몬스터가 이동 방향을 바라보도록
        transform.LookAt(Vector3.zero);

        // 캐릭터와의 거리에 따라 상태 변경
        var targetDistance = Vector3.Distance(transform.position, Vector3.zero);
        if (targetDistance < 0.1f){
            AnimatorChange(IsIdle);
        }
        else{
            transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, mSpeed * Time.deltaTime);
            AnimatorChange(IsMove);
        }
    }

    /// <summary>
    /// 재활용한 object 정보 갱신
    /// </summary>
    public void Init(){
        StartCoroutine(StartSpawnCoroutine());
        
        // 몬스터 상태 초기화
        // TODO: 몬스터 실제 데이터 가져오기
        hp = 5;
        isDead = false;
    }

    IEnumerator StartSpawnCoroutine(){

        var current = 0.0f; // 현재 시각
        var percent = 0.0f; // 진행도
        var startScale = 0.0f; // 초기 크기
        var endScale = 7f;

        // 1초동안 진행
        while (percent < 1){
            current += Time.deltaTime;
            percent = current / 1.0f;

            // 선형 보간 (시작값, 끝값, 시간): 시작에서 끝까지 특정 시간 속도로 이동해라. 
            var lerpPos = Mathf.Lerp(startScale, endScale, percent);

            // 선형 보간으로 특정 시간 (1초)동안 몬스터의 크기가 커짐
            transform.localScale = Vector3.one * lerpPos;

            yield return null;
        }

        // 원래 크기로 돌아오면 이동 가능
        yield return new WaitForSeconds(0.3f);
        isSpawn = true;
    }

    public void GetDamaged(double dmg){

        // 이미 죽은 몬스터
        if (isDead){
            return;
        }
        
        // 몬스터 체력 감소
        hp -= dmg;
        
        // 피격 데미지 출력
        BaseManager.Pool.PoolingObject("DamageText").Get((value) => {
            value.GetComponent<DamageText>().Init(transform.position, dmg, true);
        });

        // 몬스터 사망
        if (hp <= 0){
            
            // 사망 처리
            isDead = true;
            Spawner.monsterList.Remove(this);
            
            // 몬스터 사망 이펙트
            var smokeObj = BaseManager.Pool.PoolingObject("Smoke").Get((value) => {
                
                // 0.5f 더하는 이유: smoke가 조금 높게 스폰되도록 함
                value.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
                
                // BaseManager에서 particle을 제거
                // TODO: Particle 이름 enum으로 변경 ("Smoke" -> Smoke)
                BaseManager.Instance.ReturnParticle(value, "Smoke");
                
                // StartCoroutine(ReturnParticle("Smoke", value, value.GetComponent<ParticleSystem>()));
            });
            
            // 코인 드랍 & 이펙트
            var coinObj = BaseManager.Pool.PoolingObject("Coin").Get((value) => {
                value.GetComponent<Coin>().Init(transform.position);
            });

            // 아이템 드랍 & 이펙트
            // TODO: 드랍율 적용하기
            for (var i = 0; i < 3; i++){
                BaseManager.Pool.PoolingObject("Item").Get((value) => {
                    value.GetComponent<Item>().Init(transform.position);
                });
            }
            
            // TODO: 몬스터 이름 string이 아닌 enum으로 변경 ("Skeleton" -> Skeleton)
            BaseManager.Pool.poolDictionary["Skeleton"].Return(gameObject);
        }
    }
}
