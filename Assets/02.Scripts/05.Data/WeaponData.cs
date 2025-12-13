using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Game Data/Weapon")]
public class WeaponData : ScriptableObject
{
    [Header("Info")]
    public string weaponName; // 무기 이름
    public Sprite icon;       // UI 표시용 아이콘
    [TextArea] public string description;

    [Header("Prefab")]
    public WeaponHandler weaponPrefab; // 실제 소환될 무기 프리팹
}
