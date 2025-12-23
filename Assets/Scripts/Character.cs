using UnityEngine;

public class Character : MonoBehaviour
{
    protected static readonly int IsIdle = Animator.StringToHash("IsIdle");
    protected static readonly int IsMove = Animator.StringToHash("IsMove");
    protected static readonly int IsAttack = Animator.StringToHash("IsAttack");
    
    [SerializeField] private Animator animator;
    
    public double hp;
    public double attack;
    public float attackSpeed = 1.0f;
    protected bool isAttacking;
    protected float attackRange = 3.0f;    // 공격 범위
    protected float targetRange = 5.0f;    // 추적 범위

    
    [SerializeField] private Transform bulletTransform;

    protected Transform target;

    public bool isDead;
    
    protected virtual void Start() {
        animator = animator ? animator : GetComponent<Animator>();
    }
    
    /// <summary>
    /// 몬스터 애니메이션 적용 함수
    /// </summary>
    /// <param name="param">이동 상태</param>
    protected void AnimatorChange(int param) {
        if (param == IsAttack){
            animator.SetTrigger(IsAttack);
            return;
        }
        
        animator.SetBool(IsIdle, false);
        animator.SetBool(IsMove, false);
        
        animator.SetBool(param, true);
    }

    /// <summary>
    /// 가장 가까운 타겟 찾기
    /// </summary>
    protected void FindClosestTarget<T>(T[] targets) where T : Component{
        var maxDistance = targetRange;
        Transform closest = null;
        foreach (var monster in targets){
            var dist = Vector3.Distance(transform.position,monster.transform.position);

            if (dist >= maxDistance) continue;
            
            // 가장 가까운 몬스터
            closest = monster.transform;
            maxDistance = dist;
        }

        target = closest;
        if (target != null){
            transform.LookAt(target.position);
        }
    }

    /// <summary>
    /// 원거리 공격으로 몬스터 공격 함수
    /// Player character의 Event function으로 등록되어 있음
    /// 따라서, 해당 프레임에 자동으로 호출됨   
    /// </summary>
    protected virtual void RangedAttack(){
        // 타겟 없으면 리턴
        if (target == null){
            return;
        }
        
        // 탄환 소환
        BaseManager.Pool.PoolingObject("AttackHelper").Get((value) => {
            
            // 탄환 위치 변경
            value.transform.position = bulletTransform.position;

            // TODO: bullet, muzzle, characterName 이름 변경
            value.GetComponent<Bullet>().PlayerRangedAttack(target, attack, "CH_01", BaseManager.Player.IsCritical());
        });
    }

    /// <summary>
    /// 근접 공격으로 몬스터 공격 함수
    /// </summary>
    protected virtual void MeleeAttack(){
        // 타겟 없으면 리턴
        if (target == null){
            return;
        }
        
        // 탄환 소환
        BaseManager.Pool.PoolingObject("AttackHelper").Get((value) => {
            
            // 타겟의 위치에서 생성
            value.transform.position = target.position;

            // TODO: bullet, muzzle 이름 변경
            value.GetComponent<Bullet>().PlayerMeleeAttack(target, attack, BaseManager.Player.IsCritical());
        });
    }

    public virtual void GetDamaged(double dmg, bool isCritical = false){
        
    }
    
    
    
    protected void InitAttack() => isAttacking = false;
}
