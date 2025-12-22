using TMPro;
using UnityEngine;

public class PlayerDamageText : DamageText{
    protected override void Awake(){
        base.Awake();
    }
    
    /// <summary>
    /// 플레이어가 공격할 때 데미지 적용
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="dmg"></param>
    /// <param name="critical"></param>
    public override void SetDamageText(Vector3 pos, double dmg, bool critical = false){
        // 데미지 텍스트 출력 로직
        base.SetDamageText(pos, dmg, critical);
        
        // 데미지 텍스트를 object pool로 리턴하기
        BaseManager.Instance.ReturnObject(gameObject, "PlayerDamageText", 1.5f);
        
        // UI 위치 초기 설정
        transform.position = cam.WorldToScreenPoint(pos);
    }

    protected override void Update(){
        base.Update();
    }
}
