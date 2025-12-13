using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Melee,
    Range,
    Boss,
}

[CreateAssetMenu(fileName = "Enemy", menuName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("적 유닛 정보")]
    public string enemyName;
    public EnemyType enemyType;
    public GameObject enemyPrefab;

    [Header("적 유닛 스탯")]
    public float moveSpeed;
    public float attackRange;
    public float attackRate;
    public float shootingAttackRate;
    public float attackDamage;
    public float maxHealth;

    [Header("드랍 설정")]
    public int expAmount;   // 경험치 양
    public GameObject expOrbPrefab; // 경험치 오브젝트 프리팹
    public List<GameObject> dropItemPrefabs;    // 드랍될 아이템의 리스트
    [Range(0f, 1f)] public float dropChance;    // 아이템 드랍 확률
}
