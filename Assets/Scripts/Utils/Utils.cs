using UnityEngine;
using UnityEngine.U2D;

public class Utils
{
    public static SpriteAtlas Atlas = Resources.Load<SpriteAtlas>("Atlases/HeroProfileAtlas");
    
    /// <summary>
    /// 희귀도에 따른 색상 반환 utility 함수
    /// </summary>
    /// <param name="rarity">희귀도</param>
    /// <returns> 희귀도 색상 리치 텍스트 </returns>
    public static string GetRarityColorUtility(Rarity rarity){
        return rarity switch{
            Rarity.Common => "<color=#FFFFFF>",
            Rarity.Uncommon => "<color=#66FF1A>",
            Rarity.Rare => "<color=#33BBFF>",
            Rarity.Epic => "<color=#6F26FF>",
            Rarity.Unique => "<color=#FF9F1A>",
            Rarity.Legendary => "<color=#FFDF80>",
            _ => "<color=#000000>"
        };
    }

    public static Sprite GetAtlas(string str){
        return Atlas.GetSprite(str);
    }

    
}
