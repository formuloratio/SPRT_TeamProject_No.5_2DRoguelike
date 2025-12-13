using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaphysicsPatern : IBossAttackable
{
    private float patternInterval;  // 패턴 발동 주기
    private float warningDuration = 0.8f;  // 패턴 전조 시간
    private float dashDistance;     // 돌진 거리
    private float dashSpeed;        // 돌진 스피드
    private float dashStopDuration = 0.5f; // 돌진 후 정지 시간

    private float currentPatternTimer;
    private bool _isActive = false;
    public bool IsActive { get { return _isActive; } }

    private Vector2 dashTargetPosition;
    private Vector2 dashStartPosition;

    private Enemy bossEnemyRef;
    private Transform targetPlayerRef;

    private Coroutine currentPatternCoroutine;

    private Collider2D physicalCollider;

    private GameObject warningSign;

    public MetaphysicsPatern(float initialPatternInterval, Collider2D physicalCollider, GameObject warningSign, float dashDst = 20f, float dashSpd = 14f, float warnDur = 0.8f, float postDur = 0.5f)
    {
        patternInterval = initialPatternInterval;
        dashDistance = dashDst;
        dashSpeed = dashSpd;
        warningDuration = warnDur;
        dashStopDuration = postDur;
        this.physicalCollider = physicalCollider;
        this.warningSign = warningSign;

        currentPatternTimer = patternInterval;
    }

    public void BossAttack(Enemy enemy, Transform target)
    {
        if (_isActive) return;

        bossEnemyRef = enemy;
        targetPlayerRef = target;

        currentPatternTimer -= Time.fixedDeltaTime;

        if (currentPatternTimer <= 0)
        {
            _isActive = true;
            currentPatternCoroutine = enemy.StartCoroutine(DashPatternCoroutine());
        }
    }

    private IEnumerator DashPatternCoroutine()
    {
        Debug.Log("Boss: 패턴 시작. 0.8초간 플레이어 탐색");

        if (physicalCollider != null)
        {
            physicalCollider.enabled = false;
            Debug.Log("Boss: 물리 충돌 콜라이더 비활성화");
        }

        // 느낌표 ui able
        warningSign.SetActive(true);
        bossEnemyRef.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        yield return new WaitForSeconds(warningDuration);

        warningSign.SetActive(false);
        // 느낌표 ui disable
        Debug.Log("Boss: 돌진 시작");

        Vector2 playerLastPosition = targetPlayerRef.position;
        dashStartPosition = bossEnemyRef.transform.position;

        Vector2 directionTowardsPlayer = (playerLastPosition - dashStartPosition).normalized;

        dashTargetPosition = dashStartPosition + directionTowardsPlayer * dashDistance;

        float actualDashSpeed = dashSpeed;

        float totalDashLength = Vector2.Distance(dashStartPosition, dashTargetPosition);
        float dashDuration = totalDashLength / actualDashSpeed;

        float timer = 0f;
        while (timer < dashDuration)
        {
            bossEnemyRef.GetComponent<Rigidbody2D>().velocity = directionTowardsPlayer * actualDashSpeed;
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        bossEnemyRef.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        Debug.Log("Boss: 돌진 종료 0.5초 정지");
        if (physicalCollider != null)
        {
            physicalCollider.enabled = true;
            Debug.Log("Boss: 물리 충돌 콜라이더 활성화");
        }
        yield return new WaitForSeconds(dashStopDuration);

        Debug.Log("Boss: 패턴 종료. 플레이어 추적");
        currentPatternTimer = patternInterval;
        _isActive = false;
        currentPatternCoroutine = null;
    }

    public void CancelPattern()
    {
        if (currentPatternCoroutine != null && bossEnemyRef != null)
        {
            bossEnemyRef.StopCoroutine(currentPatternCoroutine);
            currentPatternCoroutine = null;
        }
        if(bossEnemyRef != null)
        {
            bossEnemyRef.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        if (physicalCollider != null)
        {
            physicalCollider.enabled = true;
        }
        _isActive = false;
        currentPatternTimer = patternInterval;
    }
}
