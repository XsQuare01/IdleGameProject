using UnityEngine;

[CreateAssetMenu(fileName="ScriptableObject", menuName = "Object/Character", order = int.MaxValue)]
public class CharacterScriptableObject : ScriptableObject
{
    public string characterName;
    public float attackRange;
    public Rarity rarity;
    
}
