using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour{
    [SerializeField] private float bulletSpeed;
    private Transform target;
    private Vector3 targetPos;
    private double damage;
    private string characterName;

    private bool isRangedAttack;
    private bool isCritical;

    private bool bulletGetHit = false;

    [SerializeField] private ParticleSystem meleeAttackParticle;
    
    Dictionary<string, GameObject> projectiles = new Dictionary<string, GameObject>();
    Dictionary<string, ParticleSystem> muzzles = new Dictionary<string, ParticleSystem>();

    private void Awake(){
        
        // 자식 object 중 index번째 자식을 불러옴
        var projectilesTransform = transform.GetChild(0);
        var muzzlesTransform = transform.GetChild(1);

        // 다양한 캐릭터의 bullets & muzzles 저장
        for (var i = 0; i < projectilesTransform.childCount; i++){
            projectiles.Add(projectilesTransform.GetChild(i).name, projectilesTransform.GetChild(i).gameObject);
        }

        for (var i = 0; i < muzzlesTransform.childCount; i++){
            muzzles.Add(muzzlesTransform.GetChild(i).name, muzzlesTransform.GetChild(i).GetComponent<ParticleSystem>());
        }
    }
    
    /// <summary>
    /// 원거리 공격 bullet 초기화
    /// </summary>
    /// <param name="target">타겟 위치</param>
    /// <param name="dmg">bullet 데미지</param>
    /// <param name="characterName">bullet 소환 플레이어</param>
    /// <param name="isCritical">크리티컬 공격 여부</param>
    public void PlayerRangedAttack(Transform target, double dmg, string characterName, bool isCritical = false){
        this.target = target;
        transform.LookAt(this.target);
        
        targetPos = target.position;
        damage = dmg;
        
        bulletGetHit = false;
        this.characterName = characterName;
        
        
        isRangedAttack = true;
        this.isCritical = isCritical;
        
        // bullet 활성화
        projectiles[characterName].gameObject.SetActive(true);
    }

    /// <summary>
    /// 플레이어의 근거리 공격 bullet 초기화
    /// </summary>
    /// <param name="target">공격 타겟</param>
    /// <param name="dmg">근거리 공격력</param>
    /// <param name="isCritical">크리티컬 공격 여부</param>
    public void PlayerMeleeAttack(Transform target, double dmg, bool isCritical = false){
        this.target = target;
        transform.LookAt(this.target);
        
        targetPos = target.position;
        damage = dmg;
        isRangedAttack = false;
        this.isCritical = isCritical;
        
        if (target != null){
                
            // 몬스터 공격
            target.GetComponent<Character>().GetDamaged(damage, isCritical);
            bulletGetHit = true;
                
            // 근접 공격 이펙드 발생/반환
            meleeAttackParticle.Play();
            StartCoroutine(ReturnMuzzles(meleeAttackParticle));
        }
    }

    /// <summary>
    /// 몬스터의 근접 공격
    /// TODO: 몬스터/플레이어 간 근접공격 로직 변경 가능성을 염두하여 함수 분리
    /// </summary>
    /// <param name="target">타겟 플레이어 위치</param>
    /// <param name="dmg">몬스터 공격력</param>
    /// <param name="isCritical">크리티컬 여부</param>
    public void MonsterMeleeAttack(Transform target, double dmg, bool isCritical = false){
        this.target = target;
        transform.LookAt(this.target);
        
        targetPos = target.position;
        damage = dmg;
        isRangedAttack = false;
        this.isCritical = isCritical;
        
        if (target != null){
                
            // 플레이어 공격
            target.GetComponent<Character>().GetDamaged(damage, isCritical);
            bulletGetHit = true;
                
            // 근접 공격 이펙드 발생/반환
            meleeAttackParticle.Play();
            StartCoroutine(ReturnMuzzles(meleeAttackParticle));
        }
    }
    
    private void Update(){

        if (bulletGetHit){
            return;
        }

        // Bullet이 몬스터 상단 공격하도록 조정
        targetPos.y = 0.5f;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * bulletSpeed);

        if (Vector3.Distance(transform.position, targetPos) <= 0.1f){
            if (target == null){
                return;
            }
            
            // TODO: Bullet damage 수정
            // TODO: GetComponent<> 제거
            target.GetComponent<Character>().GetDamaged(damage, isCritical);
            bulletGetHit = true;
            
            // 충돌 시 bullet 비활성화 & muzzle 활성화
            projectiles[characterName].gameObject.SetActive(false);
            muzzles[characterName].Play();
                
            StartCoroutine(ReturnMuzzles(muzzles[characterName]));
        }
    }

    private IEnumerator ReturnMuzzles(ParticleSystem muzzle){
        // Muzzle 꺼질 때까지 대기
        yield return new WaitWhile(() => muzzle.IsAlive(true));
        
        // Object pool로 다시 돌려놓기
        BaseManager.Pool.poolDictionary["AttackHelper"].Return(gameObject);
    }
    
}
