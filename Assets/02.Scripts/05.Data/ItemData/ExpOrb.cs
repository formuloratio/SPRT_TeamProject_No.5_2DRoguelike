using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    public int amount = 10;          // 경험치량
    public float moveSpeed = 5f;     // 경험치가 딸려오는 속도
    public float detectRange = 3f;   // 경험치가 빨려오기 시작하는 거리

    private Transform player;
    private SpriteRenderer sr;

    private float pulseSpeed = 3f;   // 반짝임 속도
    private float pulseAmount = 0.2f; // 반짝임 크기

    private Vector3 originalScale;

    private EnemyObjectPoolManager poolManager;
    private GameObject originalPrefab;

    // 공통 초기화 함수 (어디서든 호출)
    private void Init()
    {
        // 풀 매니저 자동 찾기 (없으면 null 그대로 둬도 됨)
        if (poolManager == null)
        {
            poolManager = EnemyObjectPoolManager.Instance;
        }

        // 플레이어 찾기
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null)
            {
                player = p.transform;
            }
        }

        // 스프라이트 렌더러, 스케일 초기화
        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
            originalScale = transform.localScale;
        }

        // 시각 효과 리셋
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }

        if (originalScale == Vector3.zero)
        {
            originalScale = transform.localScale;
        }
    }

    // 맵에 그냥 깔려있는 오브젝트나, 풀에서 꺼낸 오브젝트가 활성화될 때 호출
    private void OnEnable()
    {
        Init();
    }

    // (선택) 풀에서 꺼낼 때 추가 정보 설정할 수 있는 함수
    public void OnSpawned(EnemyObjectPoolManager pm, GameObject prefab)
    {
        poolManager = pm;
        originalPrefab = prefab;

        Init(); // 여기서도 공통 초기화 호출
    }

    private void Update()
    {
        // 혹시 모를 경우를 위해 매 프레임 보정
        if (player == null)
        {
            Init();
            if (player == null) return; // 아직도 못 찾았으면 그냥 나감
        }

        float distance = Vector2.Distance(transform.position, player.position);

        //플레이어가 detectRange 안에 들어와야 이동 시작
        if (distance <= detectRange)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
        }

        // 경험치 흡수 거리
        if (distance < 0.5f)
        {
            var receiver = player.GetComponent<IExpReceiver>();
            if (receiver != null)
            {
                receiver.OnExpPickup(amount);
            }

            ReturnToPool();
            return;
        }

        // 반짝임(Pulse)
        float scale = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = originalScale * scale;

        // 알파로 반짝임 추가
        if (sr != null)
        {
            Color c = sr.color;
            
            float minAlpha = 0.5f; // 최소 투명도 
            float maxAlpha = 1f;   // 최대 투명도
            
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f; // 0~1로 변환
            c.a = Mathf.Lerp(minAlpha, maxAlpha, t);
            sr.color = c;
        }
    }

    private void ReturnToPool()
    {
        if (poolManager != null && originalPrefab != null)
        {
            // 풀에 정상 반환
            poolManager.ReturnToPool(gameObject, originalPrefab);
        }
        else
        {
            // 풀 정보가 없으면 그냥 파괴 (맵에 미리 깔아둔 애들은 여기로 옴)
            Destroy(gameObject);
        }
    }
}
