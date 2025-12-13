using UnityEngine;

public enum SkillsType { Passive, Active, Ultimate }

[CreateAssetMenu(fileName = "New Skill", menuName = "Game Data/Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    [TextArea] public string description;
    public SkillsType type;
    public float coolTime; // 쿨타임 (패시브는 0)
    public Sprite icon;

    [Header("Skill Values")]
    public float value1; // 스킬 효과 수치 (예: 50% 증가면 0.5)
    public float value2; // 보조 수치
    public float duration; // 지속 시간
}