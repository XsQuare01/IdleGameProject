using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class HeroUI : BaseUI{
    [Header("영웅 프로필 UI")]
    [SerializeField] private Transform content;
    [SerializeField] private GameObject heroProfileUI;
    
    List<HeroProfileUI> heroProfileUIList = new List<HeroProfileUI>();
    Dictionary<string, CharacterScriptableObject> heroDic = new Dictionary<string, CharacterScriptableObject>();

    private int currentSortOrder = 0;

    public override bool Init(){
        // Scriptable object로 생성된 영웅 정보 받아오기
        var data = Resources.LoadAll<CharacterScriptableObject>("ScriptableObjects");
        
        foreach (var item in data){
            heroDic.Add(item.name, item);
        }
        
        // 영웅 등급 기준으로 정렬
        // 초기 정렬: 오름차순
        var sortedHeroDic = heroDic.OrderBy(x => x.Value.rarity).ThenBy(x => x.Key);
        
        // 영웅 프로필 UI 생성
        foreach (var item in sortedHeroDic){
            var go = Instantiate(heroProfileUI, content);
            var profile = go.GetComponent<HeroProfileUI>();
            
            heroProfileUIList.Add(profile);
            profile.Initialize(item.Value);
        }
        
        return base.Init();
    }

    /// <summary>
    /// 희귀도 등급에 따라 정렬
    /// </summary>
    /// <param name="sortOrder">오름차순/내림차순</param>
    public void SortByRarity(int sortOrder){
        
        // 이미 원하는 대로 졍렬됨
        if (sortOrder == currentSortOrder){
            return;
        }

        IOrderedEnumerable<KeyValuePair<string, CharacterScriptableObject>> sortedHeroDic;
        
        switch (sortOrder){
            // 오름차순
            case 0:
                sortedHeroDic = heroDic.OrderBy(x => x.Value.rarity).ThenBy(x => x.Key);
                break;
            
            // 내림차순
            case 1:
                sortedHeroDic = heroDic.OrderByDescending(x => x.Value.rarity).ThenByDescending(x => x.Key);
                break;
            // 예외 처리 - 오름차순 정렬
            default:
                sortedHeroDic = heroDic.OrderBy(x => x.Value.rarity).ThenBy(x => x.Key);
                break;
        }
        
        // 영웅 프로필 UI 변경
        var index = 0;
        foreach (var item in sortedHeroDic){
            heroProfileUIList[index++].Initialize(item.Value);
        }
        currentSortOrder = sortOrder;
    }
}
