using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIndicatorManager : MonoBehaviour
{
    [SerializeField] private GameObject bossIndicatorUIPrefab;
    [SerializeField] private RectTransform indicatorsParent;
    private Transform playerTransform;

    private Dictionary<Enemy, BossIndicatorUI> activeIndicators = new Dictionary<Enemy, BossIndicatorUI>();

    private void Awake()
    {
        if (bossIndicatorUIPrefab == null)
        {
            Debug.LogError("BossIndicatorUIPrefab이 할당되지 않음");
        }
        if (indicatorsParent == null)
        {
            Debug.LogError("IndicatorsParent가 할당되지 않음");
        } 
    }

    private void OnEnable()
    {
        Enemy.OnBossSpawnedGlobal += HandleBossSpawned;
        Enemy.OnBossDiedGlobal += HandleBossDied;
    }

    private void OnDisable()
    {
        Enemy.OnBossSpawnedGlobal -= HandleBossSpawned;
        Enemy.OnBossDiedGlobal -= HandleBossDied;
    }

    private void Start()
    {
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
    }

    private void HandleBossSpawned(Enemy boss)
    {
        if (boss.EnemyData.enemyType == EnemyType.Boss)
        {
            if (activeIndicators.ContainsKey(boss))
            {
                Debug.LogWarning($"이미 {boss.EnemyData.enemyName}을 표시하는 Indicator가 존재합니다.");
                return;
            }

            GameObject indicatorInstance = Instantiate(bossIndicatorUIPrefab, indicatorsParent);
            BossIndicatorUI indicatorUI = indicatorInstance.GetComponent<BossIndicatorUI>();

            if (indicatorUI == null)
            {
                Debug.LogError($"BossIndicatorUIPrefab에 BossIndicatorUI 스크립트가 없음");
                Destroy(indicatorInstance);
                return;
            }

            indicatorUI.SetUp(boss, playerTransform);
            indicatorUI.gameObject.SetActive(true);

            activeIndicators.Add(boss, indicatorUI);
        }
    }

    private void HandleBossDied(Enemy diedBoss)
    {
        if (activeIndicators.TryGetValue(diedBoss, out BossIndicatorUI indicatorUI))
        {
            activeIndicators.Remove(diedBoss);
            Destroy(indicatorUI.gameObject);
        }
    }
}
