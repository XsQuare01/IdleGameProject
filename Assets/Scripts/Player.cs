using UnityEngine;

public class Player : Character{
    public CharacterScriptableObject characterSO;
    
    private Vector3 startPos;
    private Quaternion startRot;
    
    protected override void Start(){
        base.Start();
        
        // TODO: 체력 테스트
        hp = 20;
        
        // TODO: 캐릭터 목록 받아오기
        DataSet(Resources.Load<CharacterScriptableObject>("ScriptableObjects/" + gameObject.name));
        Spawner.playerList.Add(this);
        
        startPos = transform.position;
        startRot = transform.rotation;
        
        
    }

    /// <summary>
    /// Scriptable object에서 정보 받아오는 함수
    /// </summary>
    /// <param name="so"></param>
    private void DataSet(CharacterScriptableObject so){
        characterSO = so;
        attackRange = so.attackRange;
    }

    private void Update(){
        // 타겟 찾기
        FindClosestTarget(Spawner.monsterList.ToArray());
        
        // 타겟이 탐지 범위 내에 없음
        if (target == null){
            // 맨 처음 위치로 복귀
            var targetPos = Vector3.Distance(startPos, transform.position);
            if (targetPos > 0.1f){
                transform.position = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime);
                transform.LookAt(startPos);
                AnimatorChange(IsMove);
            }
            // 처음 위치와 동일 -> 대기
            else{
                transform.rotation = startRot;
                AnimatorChange(IsIdle);
            }
            return;
        }

        // 타겟이 이미 죽음
        if (target.GetComponent<Character>().isDead){
            
            // 타겟 재탐색
            FindClosestTarget(Spawner.monsterList.ToArray());
        }
        
        // 타겟이 탐지 범위 내에 존재
        transform.LookAt(target.position);
        var distance = Vector3.Distance(target.position, transform.position);
        
        // 타겟이 공격 범위 밖에 존재
        if (distance <= targetRange && distance > attackRange && !isAttacking){
            // 타겟 방향으로 이동
            AnimatorChange(IsMove);
            transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime);
        }
        // 타겟 공격
        else if (distance <= attackRange && !isAttacking){
            
            // 공격 애니메이션
            isAttacking = true;
            AnimatorChange(IsAttack);
            
            // 공격 속도 적용
            Invoke(nameof(InitAttack), attackSpeed);
        }
    }
    
    public override void GetDamaged(double dmg){

        // 이미 죽은 플레이어
        if (isDead){
            return;
        }
        
        // 플레이어 체력 감소
        hp -= dmg;
        
        // 피격 데미지 출력
        BaseManager.Pool.PoolingObject("MonsterDamageText").Get((value) => {
            value.GetComponent<MonsterDamageText>().SetDamageText(transform.position, dmg, true);
        });

        // 플레이어 사망
        if (hp <= 0){
            
            // 사망 처리
            isDead = true;
            Spawner.playerList.Remove(this);
            
            // 몬스터 사망 이펙트
            var smokeObj = BaseManager.Pool.PoolingObject("Smoke").Get((value) => {
                
                // 0.5f 더하는 이유: smoke가 조금 높게 스폰되도록 함
                value.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
                
                // BaseManager에서 particle을 제거
                // TODO: Particle 이름 enum으로 변경 ("Smoke" -> Smoke)
                BaseManager.Instance.ReturnParticle(value, "Smoke");
                
                // StartCoroutine(ReturnParticle("Smoke", value, value.GetComponent<ParticleSystem>()));
            });
            
            // TODO: 플레이어 사망 처리
            gameObject.SetActive(false);
        }
    }
}
