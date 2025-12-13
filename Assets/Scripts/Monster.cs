using System;
using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    private static readonly int IsIdle = Animator.StringToHash("IsIdle");
    private static readonly int IsMove = Animator.StringToHash("IsMove");
    [SerializeField] private float mSpeed = 0.5f;

    [SerializeField] private Animator animator;

    private bool isSpawn = false;
    
    private void Start() {
        animator = animator ? animator : GetComponent<Animator>();
    }

    private void Update() {

        // 스폰되지 않은 상태: 이동하지 않음
        if (!isSpawn){
            return;
        }
        
        // 몬스터가 이동 방향을 바라보도록
        transform.LookAt(Vector3.zero);
        
        // 캐릭터와의 거리에 따라 상태 변경
        var targetDistance = Vector3.Distance(transform.position, Vector3.zero);
        if (targetDistance < 0.1f) {
            AnimatorChange(IsIdle);
        }
        else {
            transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, mSpeed * Time.deltaTime);
            AnimatorChange(IsMove);
        }
    }

    public void Init(){
        StartCoroutine(StartSpawnCoroutine());
    }

    IEnumerator StartSpawnCoroutine(){

        var current = 0.0f; // 현재 시각
        var percent = 0.0f; // 진행도
        var startScale = 0.0f;   // 초기 크기
        var endScale = transform.localScale.x;

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

    /// <summary>
    /// 몬스터 애니메이션 적용 함수
    /// </summary>
    /// <param name="param">이동 상태</param>
    private void AnimatorChange(int param) {
        animator.SetBool(IsIdle, false);
        animator.SetBool(IsMove, false);
        
        animator.SetBool(param, true);
    }
}
