using UnityEngine;

public class HeroUI : BaseUI{
    [SerializeField] private Transform content;
     [SerializeField] private GameObject heroProfileUI;
    
    void Start(){
        var data = Resources.LoadAll<CharacterScriptableObject>("ScriptableObjects");
        foreach (var item in data){
            var go = Instantiate(heroProfileUI, content);
            var profile = go.GetComponent<HeroProfileUI>();
            profile.Initialize(item);
        }
    }
}
