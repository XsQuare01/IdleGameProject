using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroProfileUI : BaseUI{
    
    [Header("영웅 프로필 이미지")]
    [SerializeField] private Image heroProfileImage;
    [SerializeField] private Image heroRarityImage;
    
    [Header("영웅 경험치")]
    [SerializeField] private Image heroExpImage;
    [SerializeField] private TextMeshProUGUI heroExtText;
    
    [Header("영웅 레벨")]
    [SerializeField] private TextMeshProUGUI heroLevelText;

    /// <summary>
    /// 영웅 프로필 초기 정보 가져오기
    /// </summary>
    /// <param name="so"></param>
    public void Initialize(CharacterScriptableObject so){
        heroRarityImage.sprite = Utils.GetAtlas(so.rarity.ToString());
        heroProfileImage.sprite = Utils.GetAtlas(so.characterName.ToString());
    }
    
}
