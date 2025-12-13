using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("페이즈 설정")]
    public List<PhaseData> spawnPhases;

    [Header("플레이어 기반 스폰 설정")]
    [SerializeField] private Transform playerTransform; // 플레이어의 transform
    [SerializeField] private float minSpawnDistance;
    [SerializeField] private float maxSpawnDistance;

    private EnemyObjectPoolManager enemyObjectPoolManager;
    
    private int currentPhaseIndex = 0;
    private float nextSpawnTime;

    private List<EnemyData> activeSpawnableEnemy = new List<EnemyData>();
    private float currentSpawnInterval = 1.0f;
    private int currentTotalEnemiesToSpawnPerCycle = 1;
    private float currentHealthMultiplier = 1.0f;
    private float currentDamageMultiplier = 1.0f;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogError("GameManager가 할당되지않음");
        }

        if (spawnPhases == null || spawnPhases.Count == 0)
        {
            Debug.LogError("SpawnPhase 없음. WaveData 할당 필요");
            return;
        }

        spawnPhases.Sort((a, b) => a.activeAtGameTime.CompareTo(b.activeAtGameTime));

        if (playerTransform == null)
        {
            // GameManager등에서 player를 관리하면 해당 오브젝트를 가져올 것
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                Debug.LogError("Player에 Transform이 할당되지 않았고, 'Player' 태그를 가진 오브젝트가 없음");
                enabled = false;
                return;
            }
        }

        enemyObjectPoolManager = EnemyObjectPoolManager.Instance;

        if(enemyObjectPoolManager == null)
        {
            Debug.LogError("씬에 EnemyObjectPoolManager가 없음");
            enabled = false;
            return;
        }

        ApplyPhaseSettings(spawnPhases[0]);
        nextSpawnTime = gameManager.playTime + currentSpawnInterval;

        StartCoroutine(ContinuousSpawnRoutine());
    }

    private void Update()
    {
        if (currentPhaseIndex + 1 < spawnPhases.Count && gameManager.playTime >= spawnPhases[currentPhaseIndex + 1].activeAtGameTime)
        {
            currentPhaseIndex++;
            ApplyPhaseSettings(spawnPhases[currentPhaseIndex]);
            Debug.Log($"게임 시간 {gameManager.playTime:F1}초, 스폰 페이즈 '{spawnPhases[currentPhaseIndex].name}' 시작");

            PerformSpawnCycle();
            nextSpawnTime = gameManager.playTime + currentSpawnInterval;
        }
    }

    void ApplyPhaseSettings(PhaseData phaseData)
    {
        activeSpawnableEnemy = phaseData.spawnableEnemies;
        currentSpawnInterval = phaseData.spawnInterval;
        currentTotalEnemiesToSpawnPerCycle = phaseData.totalEnemiesToSpawnPerCycle;
        currentHealthMultiplier = phaseData.healthMultiplier;
        currentDamageMultiplier = phaseData.damageMultiplier;
    }

    private void PerformSpawnCycle()
    {
        // 현재 페이즈에 스폰 가능한 몬스터 종류가 없다면 스폰하지 않음
        if (activeSpawnableEnemy.Count == 0)
        {
            Debug.LogWarning("현재 페이즈에 스폰할 몬스터가 설정되어 있지 않음.");
        }
        else
        {
            // 설정된 갯수만큼 몬스터 스폰
            for (int i = 0; i < currentTotalEnemiesToSpawnPerCycle; i++)
            {
                EnemyData EnemyToSpawn = activeSpawnableEnemy[Random.Range(0, activeSpawnableEnemy.Count)];
                SpawnSingleEnemy(EnemyToSpawn);
            }
        }
    }

    IEnumerator ContinuousSpawnRoutine()
    {
        while (true) // 게임 끝까지 지속적으로 스폰
        {
            if (gameManager.playTime >= nextSpawnTime)
            {
                PerformSpawnCycle();
                nextSpawnTime = gameManager.playTime + currentSpawnInterval; // 다음 스폰 시간 갱신
            }
            yield return null; // 매 프레임 체크
        }
    }

    void SpawnSingleEnemy(EnemyData enemyData)
    {
        if (playerTransform == null)
        {
            Debug.LogError("플레이어 Transform이 없어 몬스터를 스폰할 수 없음.");
            return;
        }

        Vector2 spawnPosition = GetRandomSpawnPositionAroundPlayer();

        GameObject spawnedEnemyObject = enemyObjectPoolManager.SpawnFromPool(enemyData.enemyPrefab);

        if (spawnedEnemyObject == null) return; // 풀에서 가져오지 못했다면 중단

        spawnedEnemyObject.transform.position = spawnPosition;
        spawnedEnemyObject.transform.rotation = Quaternion.identity;

        Enemy enemyComponent = spawnedEnemyObject.GetComponent<Enemy>();

        if (enemyComponent != null)
        {
            enemyComponent.Initialize(enemyData, playerTransform, enemyData.enemyPrefab, currentHealthMultiplier, currentDamageMultiplier); // 원본 프리팹 전달
            enemyComponent.OnDeathEvent += OnEnemyKilled; // 몬스터 사망 시 호출될 이벤트 연결
        }
        /*Debug.Log($"{enemyData.enemyName}이(가) 스폰 위치: {spawnPosition}");*/
    }

    // 플레이어 주변에 랜덤 스폰 위치를 계산하는 함수
    Vector2 GetRandomSpawnPositionAroundPlayer()
    {
        Vector2 playerPos = playerTransform.position;
        Vector2 spawnPos = Vector2.zero;

        float randomAngle = Random.Range(0f, 360f);
        float randomRadius = Random.Range(minSpawnDistance, maxSpawnDistance);

        spawnPos.x = playerPos.x + randomRadius * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
        spawnPos.y = playerPos.y + randomRadius * Mathf.Sin(randomAngle * Mathf.Deg2Rad);

        return spawnPos;
    }

    public void OnEnemyKilled(GameObject deadEnemyObject, GameObject originalPrefab)
    {
        // 몬스터 사망 이벤트 발생 시 해당 몬스터 오브젝트를 풀로 반환
        enemyObjectPoolManager.ReturnToPool(deadEnemyObject, originalPrefab);

        // 이벤트 구독 해제
        Enemy enemyComponent = deadEnemyObject.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.OnDeathEvent -= OnEnemyKilled;
        }
    }
}
