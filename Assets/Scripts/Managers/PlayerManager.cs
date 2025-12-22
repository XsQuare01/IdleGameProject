using UnityEngine;

public class PlayerManager{
    // --- 플레이어 레벨
    public int Level{ get; private set; }
    
    // --- 플레이어 경험치 관련 변수
    private double _currentExp;
    private double _requiredExp;
    private double _acquiredExp;
    
    // --- 플레이어 공격력
    public double Atk{ get; set; }
    
    // --- 플레이어 체력
    public double Hp { get; set; } = 50;

    // 플레이어 데이터 가져오기
    // TODO: 서버 DB에서 플레이어 데이터 가져오기
    public void Initialize(){
        Level = 1;
        Atk = 10;
        Hp = 50;
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
    /// 경험치 비약 사용 시 공격력 상승
    /// </summary>
    public void AtkUp(){
        
        // 경험치 비약?
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
    
}
