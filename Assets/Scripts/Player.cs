using UnityEngine;

public class Player : Character{
    private Vector3 startPos;
    private Quaternion startRot;
    
    protected override void Start(){
        base.Start();
        startPos = transform.position;
        startRot = transform.rotation;
    }

    void Update(){
        // 타겟이 탐지 범위 내에 없음
        if (target == null){
            
            // 타겟 찾기
            FindClosestTarget(Spawner.monsterList.ToArray());
            
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
}
