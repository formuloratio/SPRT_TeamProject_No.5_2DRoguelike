using UnityEngine;

public class EnemyProjectile : MonoBehaviour, IProjectilable
{
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;
    [SerializeField] private Vector2 spriteForward = Vector2.up;

    private Vector2 targetDirection;
    private float damage;
    private GameObject projectilePrefab;
    private EnemyObjectPoolManager objectPoolManager;

    private float currentLifeTime;

    private void Awake()
    {
        if(objectPoolManager == null)
        {
            objectPoolManager = EnemyObjectPoolManager.Instance;
            if(objectPoolManager == null )
            {
                Debug.LogError("Projectile: EnemyObjectPoolManager를 찾을 수 없음");
            }
        }
    }

    void OnEnable()
    {
        currentLifeTime = lifeTime;
    }

    public void Initialize(Transform target, float projectileDamage, GameObject projectilePrefab)
    {
        if (target != null)
        {
            targetDirection = (target.position - transform.position).normalized;
        }
        else
        {
            targetDirection = Vector2.left;
            Debug.LogWarning("EnemyProjectile: target이 없음. 기본 설정된 방향으로 발사");
        }

        damage = projectileDamage;
        this.projectilePrefab = projectilePrefab;
        currentLifeTime = lifeTime;

        float angle = Vector2.SignedAngle(spriteForward, targetDirection);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void SetLifeTime(float lifeTime)
    {
        this.lifeTime = lifeTime;
        currentLifeTime = lifeTime;
    }

    public void SetObjectPoolManager(EnemyObjectPoolManager objectPoolManager)
    {
        this.objectPoolManager = objectPoolManager;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    private void Update()
    {
        transform.Translate(targetDirection * speed *  Time.deltaTime, Space.World);

        currentLifeTime -= Time.deltaTime;
        if (currentLifeTime <= 0f)
        {
            ReturnToPool();
        }
    }

    // 
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ResouceController resouceController = other.GetComponent<ResouceController>();
            if (resouceController != null)
            {
                resouceController.ChangeHealth((int)-damage);
                Debug.Log($"Player에게 {-damage} 데미지");
                Debug.Log($"남은 player 체력 :{resouceController.CurrentHealth}");
            }

            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (objectPoolManager != null && projectilePrefab != null)
        {
            objectPoolManager.ReturnToPool(gameObject, projectilePrefab);
        }
        else
        {
            Debug.LogError("EnemyProjectile: PoolManager 또는 projectilePrefab이 없음");
            Destroy(gameObject);
        }
    }
}
