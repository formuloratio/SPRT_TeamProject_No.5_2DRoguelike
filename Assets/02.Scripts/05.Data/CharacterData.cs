using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Game Data/Character")]
public class CharacterData : ScriptableObject
{
    public string characterName; // 세실리
    public string weaponName;    // 마법화살
    public Sprite characterSprite;

    [Header("Base Stats")]
    public int baseHealth = 100;
    public float baseSpeed = 5f;
    public float baseAttack = 10f;
    public float baseAttackSpeed = 1f;

    [Header("Skills")]
    public SkillData passive1; // 도, 돈이다!
    public SkillData passive2; // 으아아 이게 뭐야
    public SkillData active1;  // 우왓, 저리 가!
    public SkillData active2;  // 마력 증강
    public SkillData ultimate; // 비장의 마법이야!

    [Header("Image (얼굴 전용)")]
    public Sprite hudSprite;

    [Header("Unique Weapon")]
    public WeaponData uniqueWeapon;
}