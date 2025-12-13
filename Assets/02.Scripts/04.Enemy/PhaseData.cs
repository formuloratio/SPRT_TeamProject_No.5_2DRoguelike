using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Phase", menuName = "PhaseData")]
public class PhaseData : ScriptableObject
{
    [Header("페이즈 기본 정보")]
    public int phaseID;
    public string phaseName;
    public bool isBossPhase;

    [Header("몬스터 스폰 설정")]
    public float activeAtGameTime;  // 웨이브 시작 시간
    public List<EnemyData> spawnableEnemies; // 적 종류
    public float spawnInterval; // 적 개체 간 스폰 딜레이
    public int totalEnemiesToSpawnPerCycle;    // 한 번의 스폰 주기마다 스폰 될 적의 갯수

    [Header("페이즈 난이도 스탯 계수")]
    public float healthMultiplier = 1.0f; // 체력 계수
    public float damageMultiplier = 1.0f; // 공격력 계수
}
