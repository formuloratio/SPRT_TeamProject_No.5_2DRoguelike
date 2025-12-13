using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    [Header("적 유닛 설정")]
    [SerializeField] private EnemyData enemyData;
    public EnemyData EnemyData {  get { return enemyData; } }
    private GameObject originalPrefab;

    private Rigidbody2D rb;
    private Transform playerTransform;

    private float currentHealth;
    public float CurrentHealth { get { return currentHealth; } }
    private float enemyDamage;
    private bool isActive = false;

    public event Action<GameObject, GameObject> OnDeathEvent;

    private EnemyObjectPoolManager objectPoolManager;
    private DifficultyScaler difficultyScaler;

    [Header("투사체 설정(Range타입)")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;

    private float attackCooldown;
    private float shootingAttackCooldown;

    private SpriteRenderer spriteRenderer;

    private IBossAttackable currentBossPattern;

    [Header("물리 충돌용 collider")]
    [SerializeField] private Collider2D physicalCollisionCollider;
    [SerializeField] private GameObject warningSign;

    public static event Action<Enemy> OnBossSpawnedGlobal;
    public static event Action<Enemy> OnBossDiedGlobal;

    public event Action<Enemy> OnThisBossDied;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Enemy: SpriteRenderer가 없음");
        }
    }

    private void Start()
    {
        objectPoolManager = EnemyObjectPoolManager.Instance;
        if (objectPoolManager == null)
        {
            Debug.LogError("EnemyObjectPoolManager이 없음. 드랍 기능 사용 불가");
        }

        difficultyScaler = DifficultyScaler.Instance;
        if (difficultyScaler == null)
        {
            Debug.LogError("DifficultyScaler가 씬에 없음.");
        }

        if(warningSign != null)
        {
            warningSign.SetActive(false);
        }
    }

    public void Initialize(EnemyData data, Transform targetPlayer, GameObject prefabOrigin, float healthMult, float damageMult)
    {
        enemyData = data; // 전달받은 MonsterData로 설정
        originalPrefab = prefabOrigin; // 원본 프리팹 저장 (풀 반환용)
        playerTransform = targetPlayer;

        // 두 가지 계수 (PhaseData 계수 * DifficultyScaler 계수)를 모두 적용
        float finalHealthMultiplier = healthMult * (difficultyScaler != null ? difficultyScaler.CurrentHealthMultiplier : 1.0f);
        float finalDamageMultiplier = damageMult * (difficultyScaler != null ? difficultyScaler.CurrentDamageMultiplier : 1.0f);

        currentHealth = Mathf.RoundToInt(enemyData.maxHealth * finalHealthMultiplier);
        enemyDamage = Mathf.RoundToInt(enemyData.attackDamage * finalDamageMultiplier);

        isActive = true; // 활성화 상태로 전환
        gameObject.SetActive(true);

        if (playerTransform != null)
        {
            UpdateSpriteDirectionBasedOnTarget();
        }

        if (enemyData.enemyType == EnemyType.Boss)
        {
            Debug.Log("Initialize에서 보스 스폰");
            OnBossSpawnedGlobal?.Invoke(this);
            if (enemyData.enemyName == "Metaphysics")
            {
                Debug.Log($"{enemyData.enemyName} 생성됨");
                currentBossPattern = new MetaphysicsPatern(5f, physicalCollisionCollider, warningSign);
                Debug.Log($"{enemyData.enemyName} 패턴 적용됨");
                if (physicalCollisionCollider != null)
                {
                    physicalCollisionCollider.enabled = true;
                }
            }
            else
            {
                Debug.LogWarning($"Enemy: Boss 타입이지만 패턴 정의가 되어있지않음");
                currentBossPattern = null;
            }
        }
        else
        {
            currentBossPattern = null;
        }

        /*Debug.Log($"- 초기 Max 체력: {enemyData.maxHealth} * Phase계수: {healthMult:F2} * 시간계수: {finalHealthMultiplier / healthMult:F2} = 최종 체력: {currentHealth}");
        Debug.Log($"- 초기 공격력: {enemyData.attackDamage} * Phase계수: {damageMult:F2} * 시간계수: {finalDamageMultiplier / damageMult:F2} = 최종 공격력: {enemyDamage}");*/

    }

    private void OnDisable()
    {
        currentBossPattern?.CancelPattern();
        rb.velocity = Vector2.zero;
        isActive = false;

        if (physicalCollisionCollider != null)
        {
            physicalCollisionCollider.enabled = true;
        }
    }

    private void FixedUpdate()
    {
        if (!isActive || playerTransform == null || enemyData == null)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (attackCooldown > 0)
        {
            attackCooldown = Mathf.Max(0, attackCooldown - Time.fixedDeltaTime);
        }

        if (shootingAttackCooldown > 0)
        {
            shootingAttackCooldown = Mathf.Max(0, shootingAttackCooldown - Time.fixedDeltaTime);
        }

        if (enemyData.enemyType == EnemyType.Boss && currentBossPattern != null)
        {
            currentBossPattern.BossAttack(this, playerTransform);

            if (!currentBossPattern.IsActive)
            {
                UpdateMovement();
            }
        }
        else
        {
            UpdateMovement();

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= enemyData.attackRange)
            {
                rb.velocity = Vector2.zero;
                Attack();
            }
        }

        UpdateSpriteDirectionBasedOnTarget();
    }

    private void UpdateMovement()
    {
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
        rb.velocity = directionToPlayer * enemyData.moveSpeed;
    }

    private void Attack()
    {
        switch (enemyData.enemyType)
        {
            case EnemyType.Melee:
                break;
            case EnemyType.Range:
                if (shootingAttackCooldown <= 0f && projectilePrefab != null && shootPoint != null)
                {
                    ShootProjectile();
                    shootingAttackCooldown = enemyData.shootingAttackRate;
                }
                break;
            case EnemyType.Boss:
                break;
        }
    }

    private void ShootProjectile()
    {
        // 원거리 몬스터 공격 사운드 추가
        //SoundManager.Instance.PlaySFX(SoundManager.Instance.enemyLongAttack, 1f);     // 해당 부분 존재시 원거리 유닛이 투사체 발사 안함 확인 필요

        if (objectPoolManager == null)
        {
            Debug.LogError("Enemy: ObjectPoolManager를 찾을 수 없음");
            return;
        }
        if (projectilePrefab == null)
        {
            Debug.LogError("Enemy: 투사체 프리팹이 없음");
            return;
        }
        if (shootPoint == null)
        {
            Debug.LogError("Enemy: 발사 지점이 할당되지않음");
            return;
        }

        GameObject projectileObj = objectPoolManager.SpawnFromPool(projectilePrefab);
        if (projectileObj == null)
        {
            Debug.LogError("투사체 프리팹을 가져오지 못했음.");
            return;
        }

        projectileObj.transform.position = shootPoint.position;
        projectileObj.transform.rotation = Quaternion.identity;

        IProjectilable projectileScript = projectileObj.GetComponent<IProjectilable>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(playerTransform, enemyDamage, projectilePrefab);
            projectileScript.SetObjectPoolManager(objectPoolManager);
        }
        else
        {
            Debug.LogError("투사체 프리팹에 IProjectiable을 구현한 스크립트가 없음");
        }

    }
    public void TakeDamage(float damage)
    {
        if (!isActive) return;

        currentHealth -= damage;
        Debug.Log($"{enemyData.enemyName}이 {damage} 데미지 입음. 남은 체력: {currentHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 사망 처리 메서드
    private void Die()
    {
        Debug.Log($"{enemyData.enemyName} 사망.");

        if (enemyData.expOrbPrefab != null && objectPoolManager != null)
        {
            GameObject expOrb = objectPoolManager.SpawnFromPool(enemyData.expOrbPrefab);
            var orb = expOrb.GetComponent<ExpOrb>();
            if (expOrb != null)
            {
                expOrb.transform.position = transform.position;
                orb.OnSpawned(objectPoolManager, enemyData.expOrbPrefab);
                Debug.Log("경험치 오브젝트 드랍");
            }
        }
        else if (enemyData.expOrbPrefab == null)
        {
            Debug.LogWarning("EnemyData에 경험치 오브젝트 프리팹이 할당되지 않음");
        }

        if (enemyData.dropItemPrefabs != null && enemyData.dropItemPrefabs.Count > 0 && objectPoolManager != null)
        {
            if (UnityEngine.Random.value <= enemyData.dropChance)
            {
                int randomIndex = UnityEngine.Random.Range(0, enemyData.dropItemPrefabs.Count);
                GameObject selectedItem = enemyData.dropItemPrefabs[randomIndex];

                if (selectedItem != null)
                {
                    GameObject droppedItem = objectPoolManager.SpawnFromPool(selectedItem);
                    droppedItem.transform.position = transform.position;
                    Debug.Log($"아이템 {droppedItem.name}드랍");
                }
                else
                {
                    Debug.LogWarning("dropItemPrefabs에 null 아이템이 포함됨");
                }
            }
        }
        else
        {
            Debug.LogWarning("드랍 아이템 리스트가 비어있거나 오브젝트 풀 매니저가 할당되지 않음");
        }

        OnThisBossDied?.Invoke(this);
        OnBossDiedGlobal?.Invoke(this);
        // 사망 이벤트 발생 시 자신과 원본 프리팹 전달
        OnDeathEvent?.Invoke(this.gameObject, originalPrefab); 
        isActive = false; // 비활성화 상태
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out ResouceController resouceController))
            {
                if (attackCooldown <= 0f)
                {
                    Debug.Log("플레이어와 접촉");
                    // IDamagable을 상속받으면 해당 스크립트로 변경
                    resouceController.ChangeHealth((int)-enemyData.attackDamage);
                    Debug.Log($"Player에게 {-enemyData.attackDamage} 데미지");
                    Debug.Log($"남은 player 체력 :{resouceController.CurrentHealth}");
                    attackCooldown = enemyData.attackRate;
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            attackCooldown = enemyData.attackRate;
        }
    }

    private void UpdateSpriteDirectionBasedOnTarget()
    {
        if (spriteRenderer == null || playerTransform == null) return;

        if (transform.position.x < playerTransform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else if (transform.position.x > playerTransform.position.x)
        {
            spriteRenderer.flipX = false;
        }
    }
}
