using UnityEngine;

public class PlayerManager{
    // --- 플레이어 레벨
    public int Level{ get; private set; }
    
    // --- 플레이어 경험치 관련 변수
    private double _currentExp;
    private double _requiredExp;
    private double _acquiredExp;
    
    /// <summary>
    /// 플레이어 공격력
    /// </summary>
    public double Atk{ get; private set; }
    
    /// <summary>
    /// 플레이어 체력
    /// </summary>
    public double Hp { get; private set; } = 50;
    
    /// <summary>
    /// 크리티컬 확률
    /// </summary>
    public float CriticalChance{ get; private set; } = 20f;

    /// <summary>
    /// 크리티컬 데미지
    /// </summary>
    public double CriticalDamage{ get; private set; } = 140;
    
    // --- 캐릭터 전투력
    public double CombatPower{ get; private set; }
    
    
    // 플레이어 데이터 가져오기
    // TODO: 서버 DB에서 플레이어 데이터 가져오기
    public void Initialize(){
        
        // TODO: 초기 테스트 데이터
        Level = 1;
        Atk = 10;
        Hp = 50;
        SetCombatPower();
        
        // 요구 경험치
        _requiredExp = float.Parse(CSVImporter.exp[Level]["EXP"].ToString());
        _acquiredExp = float.Parse(CSVImporter.exp[Level]["Get_EXP"].ToString());
    }

    /// <summary>
    /// 경험치 비약 사용 시 경험치 상승
    /// </summary>
    public void ExpUp(){
        
        // 경험치 비약에 따른 경험치 상승량
        _currentExp += _acquiredExp;
        
        // 레벨이 더 이상 오르지 않을 때까지 반복
        while (_currentExp >= _requiredExp){
            
            // 레벨업에 필요한 경험치만큼 감소
            _currentExp -= _requiredExp;
            
            // 레벨업 및 레벨 UI 변경
            ++Level;
            MainCanvasUI.Instance.SetLevelText(Level);
            
            // 요구/획득 경험치량 변경
            _requiredExp = float.Parse(CSVImporter.exp[Level]["EXP"].ToString());
            _acquiredExp = float.Parse(CSVImporter.exp[Level]["Get_EXP"].ToString());
        }
    }

    /// <summary>
    /// 경험치 비약 사용 시 스탯 상승
    /// </summary>
    public void StatsUp(){
        // 스탯 상승
        Atk += GetAcquiredAtk();
        Hp += GetAcquiredHp();

        // TODO: 추가 스탯 올리기

        // 스탯 상승 적용
        foreach (var player in Spawner.playerList){
            player.SetStats();
        }
        
        // 전투력 갱신
        SetCombatPower();
        MainCanvasUI.Instance.SetCombatPowerText(CombatPower);
    }

    /// <summary>
    /// 현재 경험치 비율 반환
    /// </summary>
    /// <returns></returns>
    public float GetCurrentExpPercentage(){
        return (float)(_currentExp / _requiredExp);
    }
    
    /// <summary>
    /// 획득 경험치량 비율 반환
    /// </summary>
    /// <returns></returns>
    public float GetAcquiredExpPercentage(){
        
        var remainedExp = _requiredExp - _currentExp;       // 레벌업까지 남은 경험치
        var lev = Level;                                    // 현재 레벨
        var futureAcquiredExp = _acquiredExp;               // 얻게 될 경험치량
        var futureRequiredExp = _requiredExp;               // 레벨업 요구 경험치

        // 획득 예정인 경험치 비율
        var acquiredExpPercentage = 0.0;

        // 획득 경험치로 인한 레벨업
        while (remainedExp <= futureAcquiredExp){
            acquiredExpPercentage += remainedExp / futureRequiredExp;
            futureAcquiredExp -= remainedExp;
            
            // 레벨업으로 변경된 요구 경험치 변경
            futureRequiredExp =  float.Parse(CSVImporter.exp[++lev]["EXP"].ToString());
            remainedExp = futureRequiredExp;
        }
        
        // 레벨업 후 잔여 경험치 비율
        // TODO: 레벨이 높아질수록 경험치 획득 비율이 낮아지는 문제 발생
        // TODO: 5렙은 exp 10 획득 - 6렙은 exp 20 획득; 레벨에 따라 경험치량이 극적으로 변함
        // TODO: 따라서, 5->6렙 경험치 획득 비율(2.528%)이 6렙 경험치 획득 비율 (2.837%)보다 작음
        // TODO: 레벨디자인 시 고려해야 함
        acquiredExpPercentage += futureAcquiredExp / futureRequiredExp;
        return (float)(acquiredExpPercentage);
    }

    /// <summary>
    /// 획득 공격력 반환
    /// </summary>
    /// <returns></returns>
    public double GetAcquiredAtk(){
        return _acquiredExp * (Level) / 2;
    }
    
    /// <summary>
    /// 획득 체력 반환
    /// </summary>
    /// <returns></returns>
    public double GetAcquiredHp(){
        return _acquiredExp * (Level) * 10;
    }

    /// <summary>
    /// 등급에 따른 공격력 로직
    /// TODO: 레벨디자인에서 수정 요망
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public double GetAtk(Rarity r){
        return Atk * ((int)r + 1);
    }
    
    /// <summary>
    /// 등급에 따른 체력 로직
    /// TODO: 레벨디자인에서 수정 요망
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public double GetHp(Rarity r){
        return Hp * ((int)r + 1);
    }
    
    /// <summary>
    /// 크리티컬 여부 계산
    /// </summary>
    /// <returns></returns>
    public bool IsCritical(){
        return Random.Range(0.0f, 100.0f) <= BaseManager.Player.CriticalChance;
    }

    /// <summary>
    /// 총 전투력 계산 함수
    /// TODO: 전투력 계산 로직 변경
    /// </summary>
    /// <returns></returns>
    private void SetCombatPower(){
        CombatPower = Atk * 2.0 + Hp * 0.5;
    }
}
