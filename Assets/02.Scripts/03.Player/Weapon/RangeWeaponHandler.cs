using UnityEngine;
using System.Linq;

public class RangeWeaponHandler : WeaponHandler
{
    [Header("Ranged Attack Data")]
    [SerializeField] private Transform projectileSpawnPosition;

    [SerializeField] private int bulletIndex;
    public int BulletIndex { get { return bulletIndex; } }

    [SerializeField] private float bulletSize = 1f;
    public float BulletSize { get { return bulletSize; } }

    [SerializeField] private float duration;
    public float Duration { get { return duration; } }

    [SerializeField] private float spread;
    public float Spread { get { return spread; } }

    [SerializeField] private Color projectileColor;
    public Color ProjectileColor { get { return projectileColor; } }

    [Header("한번에 발사할 발사체 수")]
    [SerializeField] private int numberOfPrijectilesPerShot;
    public int NumberOfPrijectilesPerShot { get { return numberOfPrijectilesPerShot; } }

    [Header("발사체 간의 각도 간격")]
    [SerializeField] private float multipleProjectileAngle;
    public float MultipleProjectileAngle { get { return multipleProjectileAngle; } }

    [Header("Auto Targeting")]
    [SerializeField] private bool isAutoTargeting = false; // 자동 타겟팅 활성화 여부
    [SerializeField] private bool isManualTargeting = false; // 수동 타겟팅 활성화 여부
    [SerializeField] private LayerMask enemyLayer; // 적 레이어 마스크

    private ProjectileManager projectileManager;

    protected override void Start()
    {
        base.Start();
        projectileManager = ProjectileManager.Instance;
    }

    public override void Attack()
    {
        base.Attack();

        // 자동 타겟팅 모드일 경우
        if (isAutoTargeting)
        {
            HandleAutoTargetingAttack();
        }
        // 기존 수동(마우스/키보드 방향) 공격 모드일 경우
        else if(isManualTargeting)
        {
            HandleManualAttack();
        }
    }

    // 기존의 공격 로직을 별도 메서드로 분리
    private void HandleManualAttack()
    {
        float projectileAngleSpace = multipleProjectileAngle;
        int numberOfPrijectilePerShot = numberOfPrijectilesPerShot;

        float minAngle = -(numberOfPrijectilePerShot / 2f) * projectileAngleSpace;

        for (int i = 0; i < numberOfPrijectilePerShot; i++)
        {
            float angle = minAngle + projectileAngleSpace * i;
            float randomSpread = Random.Range(-spread, spread);
            angle += randomSpread;
            CreateProjectile(Controller.LookDirection, angle);
        }
    }

    // 새로운 자동 타겟팅 공격 로직
    private void HandleAutoTargetingAttack()
    {
        // 1. 사정거리(AttackRange) 내의 모든 적(enemyLayer)을 탐지
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, AttackRange, enemyLayer);

        if (hitColliders.Length == 0) return; // 적이 없으면 발사 안 함

        // 2. 거리순으로 정렬 후, 발사체 개수(NumberOfPrijectilesPerShot)만큼만 가장 가까운 적을 선택
        // 거리 계산 성능을 위해 sqrMagnitude 사용 권장 (여기서는 가독성을 위해 Distance 사용)
        var closestEnemies = hitColliders
            .OrderBy(x => Vector2.Distance(transform.position, x.transform.position))
            .Take(numberOfPrijectilesPerShot)
            .ToList();

        // 3. 선택된 적들을 향해 각각 발사
        foreach (var enemy in closestEnemies)
        {
            // 적을 향하는 방향 벡터 계산
            Vector2 directionToEnemy = (enemy.transform.position - transform.position).normalized;

            // 랜덤 스프레드 적용 (원한다면 제거 가능)
            float randomSpread = Random.Range(-spread, spread);

            // 해당 적을 향해 탄환 1개 생성 (각도 오프셋은 스프레드만 적용)
            CreateProjectile(directionToEnemy, randomSpread);
        }
    }

    private void CreateProjectile(Vector2 _lookDirection, float angle)
    {
        projectileManager.ShootBullet(
            this,
            projectileSpawnPosition.position,
            RotateVector2(_lookDirection, angle)
            );
    }

    private static Vector2 RotateVector2(Vector2 v, float degree)
    {
        return Quaternion.Euler(0, 0, degree) * v;
    }
}
