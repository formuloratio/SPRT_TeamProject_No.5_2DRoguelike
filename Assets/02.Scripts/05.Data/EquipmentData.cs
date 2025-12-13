using System.Collections.Generic;
using UnityEngine;

// 스탯 종류 정의
public enum StatType
{
    MaxHealth,          // 최대 체력 (고정값)
    Attack,             // 공격력 (고정값)
    Defense,            // 방어력 (고정값)
    MoveSpeedPercent,   // 이동 속도 (%)
    AttackSpeedPercent, // 공격 속도 (%)
    CooldownReduction   // 쿨타임 감소 (%)
}

[System.Serializable]
public class StatModifier
{
    public StatType type;
    public float value; // 고정값 혹은 퍼센트(0.1 = 10%)
}

[CreateAssetMenu(fileName = "New Equipment", menuName = "Game Data/Equipment")]
public class EquipmentData : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon; // 아이콘 필요 시 사용

    [Header("Stats")]
    public List<StatModifier> modifiers = new List<StatModifier>();
}