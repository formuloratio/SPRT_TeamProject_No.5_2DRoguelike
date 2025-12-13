using UnityEngine;
public enum SkillType
{
    Normal,
    Ultimate
}

[System.Serializable]
public class SkillOptionData
{
    public Sprite icon;
    public string name;
    public string description;

    // 필요하면 추가
    public float cooldown;
    public string key;  // Z / X / C 같은 키 설명 표시
    public SkillType skillType;
}
