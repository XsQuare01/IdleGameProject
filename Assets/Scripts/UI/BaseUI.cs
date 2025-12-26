using UnityEngine;

public class BaseUI : MonoBehaviour{
    protected bool _init = false;
    
    /// <summary>
    /// 초기 실행 시 올바르게 실행되었는지를 반환하는 함수
    /// Start() 매크로 함수를 대체한다. 
    /// </summary>
    /// <returns></returns>
    public virtual bool Init(){
        return !_init;
    }

    private void Start(){
        Init();
    }
    
    /// <summary>
    /// UI 파괴 함수
    /// </summary>
    public virtual void DisableObject(){
        Utils.UIHolder.Pop();
        Destroy(gameObject);
    }
}
