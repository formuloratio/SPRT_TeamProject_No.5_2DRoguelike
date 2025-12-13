using System.Collections;
using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    public static PlayerSkillController Instance { get; private set; } //UI연결문제로 추가

    [Header("Data")]
    [SerializeField] private CharacterData characterData;
    [SerializeField] private GameObject explosionPrefab; // 궁극기용 폭발 이펙트 프리팹

    private StatHandler statHandler;
    private BaseController baseController;

    // 쿨타임 관리
    private float active1CooldownTimer = 0f;
    private float active2CooldownTimer = 0f;
    private float gameTime = 0f; // 게임 시작 후 경과 시간
    private bool isUltimateUsed = false; // 궁극기 사용 여부 (게임 당 1회)
    private const float ULTIMATE_AVAILABLE_TIME = 30f; // 30초 후 사용 가능

    public float GetActive1Cooldown() => active1CooldownTimer;
    public float GetActive2Cooldown() => active2CooldownTimer;

    private void Awake()
    {
        Instance = this; //UI연결문제로 추가
        statHandler = GetComponent<StatHandler>();
        baseController = GetComponent<BaseController>();
    }

    private void Start()
    {
        ApplyPassives(); // 게임 시작 시 패시브 효과 적용
    }

    private void Update()
    {
        HandleCooldowns();
        //HandleInput();
    }

    private void HandleCooldowns()
    {
        if (active1CooldownTimer > 0) active1CooldownTimer -= Time.deltaTime;
        if (active2CooldownTimer > 0) active2CooldownTimer -= Time.deltaTime;
        gameTime += Time.deltaTime;
    }

    private float GetReducedCooldown(float baseCoolTime)
    {
        // StatHandler가 없으면 기본값 반환
        if (statHandler == null) return baseCoolTime;

        // 예: 쿨감 10%(0.1) -> 1.0 - 0.1 = 0.9 (90% 시간만 적용)
        float multiplier = 1.0f - statHandler.CooldownReduction;

        // 계산된 쿨타임 (최소 0.1초는 보장하도록 설정)
        return Mathf.Max(0.1f, baseCoolTime * multiplier);
    }

    //private void HandleInput()
    //{
    //    // 액티브 1번 (키보드 1)
    //    if (Input.GetKeyDown(KeyCode.Alpha1))
    //    {
    //        if (active1CooldownTimer <= 0)
    //        {
    //            StartCoroutine(UseActiveSkill1());
    //            active1CooldownTimer = GetReducedCooldown(characterData.active1.coolTime);
    //        }
    //        else
    //        {
    //            Debug.Log($"스킬 1 쿨타임: {active1CooldownTimer:F1}초 남음");
    //        }
    //    }

    //    // 액티브 2번 (키보드 2)
    //    if (Input.GetKeyDown(KeyCode.Alpha2))
    //    {
    //        if (active2CooldownTimer <= 0)
    //        {
    //            StartCoroutine(UseActiveSkill2());
    //            active2CooldownTimer = GetReducedCooldown(characterData.active2.coolTime);
    //        }
    //        else
    //        {
    //            Debug.Log($"스킬 2 쿨타임: {active2CooldownTimer:F1}초 남음");
    //        }
    //    }

    //    // 궁극기 (키보드 3)
    //    if (Input.GetKeyDown(KeyCode.Alpha3))
    //    {
    //        AttemptUltimate();
    //    }
    //}

    // --- 패시브 적용 ---
    private void ApplyPassives()
    {
        // 패시브 1: 도, 돈이다! (골드 획득량 증가)
        if (characterData.passive1 != null)
        {
            statHandler.GoldBonusMultiplier += characterData.passive1.value1;
            Debug.Log($"패시브 적용: 골드 획득량 {statHandler.GoldBonusMultiplier * 100}%");
        }

        // 패시브 2: 으아아 이게 뭐야 (투사체 폭발 확률)
        if (characterData.passive2 != null)
        {
            statHandler.HasExplosiveProjectile = true;
            statHandler.ExplosiveChance = characterData.passive2.value1; // 예: 0.1 (10%)
            Debug.Log("패시브 적용: 폭발 화살 활성화");
        }
    }

    // --- 액티브 스킬 구현 ---

    // 스킬 1: 우왓, 저리 가! (3초간 이속 50% 증가)
    private IEnumerator UseActiveSkill1()
    {
        Debug.Log($"스킬 사용: {characterData.active1.skillName}");
        float originalSpeed = statHandler.Speed;

        // 속도 50% 증가
        statHandler.Speed *= (1 + characterData.active1.value1);

        yield return new WaitForSeconds(3f); // 3초 지속

        statHandler.Speed = originalSpeed; // 원상복구
        Debug.Log("스킬 1 효과 종료");
    }

    // 스킬 2: 마력 증강 (4초간 공 50% 증가 -> 이후 1초간 공속 50% 감소)
    private IEnumerator UseActiveSkill2()
    {
        Debug.Log($"스킬 사용: {characterData.active2.skillName}");
        float originalAttack = statHandler.Attack;
        float originalAtkSpd = statHandler.AttackSpeed;

        // 페이즈 1: 공격력 증가 (4초)
        statHandler.Attack *= (1 + characterData.active2.value1);
        yield return new WaitForSeconds(4f);

        // 공격력 원복
        statHandler.Attack = originalAttack;

        // 페이즈 2: 공격속도 감소 (1초) - 패널티
        // 공격 딜레이가 늘어나는 것이므로 수치를 더하거나 곱해서 느리게 만듦
        // (구현 방식에 따라 다름, 여기선 딜레이 50% 증가로 가정)
        statHandler.AttackSpeed *= 1.5f;
        Debug.Log("마력 증강 부작용! 공격 속도 저하");

        yield return new WaitForSeconds(1f);

        // 공속 원복
        statHandler.AttackSpeed = originalAtkSpd;
        Debug.Log("스킬 2 효과 종료");
    }

    // --- 궁극기 구현 ---

    // 궁극기: 비장의 마법이야!
    private void AttemptUltimate()
    {
        if (gameTime < ULTIMATE_AVAILABLE_TIME)
        {
            Debug.Log($"궁극기 준비 중... {ULTIMATE_AVAILABLE_TIME - gameTime:F1}초 남음");
            return;
        }

        if (isUltimateUsed)
        {
            Debug.Log("이번 게임에서 이미 궁극기를 사용했습니다.");
            return;
        }

        UseUltimate();
    }

    private void UseUltimate()
    {
        UIManager.Instance.PlayUltimateCutIn(); //컷씬 출력 추가(UI)

        isUltimateUsed = true;
        Debug.Log($"궁극기 발동! {characterData.ultimate.skillName}");

        // 캐릭터 앞 (Unity Unit 기준 적절히 변환, 예: 10 unit) 위치 계산
        Vector2 spawnPos = (Vector2)transform.position + baseController.LookDirection.normalized * 10f;

        // 폭발 이펙트 생성 (데미지 로직은 프리팹 자체 스크립트 혹은 여기서 처리)
        GameObject boom = Instantiate(explosionPrefab, spawnPos, Quaternion.identity);

        // 폭발 크기 설정 (400x400px -> Unity Scale로 변환 필요, 대략 8x8라 가정)
        boom.transform.localScale = new Vector3(8, 8, 1);

        // 범위 데미지 처리
        Collider2D[] hits = Physics2D.OverlapBoxAll(spawnPos, new Vector2(8, 8), 0);
        float damage = 100 + (statHandler.Attack * 2);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy")) // 적 태그 확인
            {
                hit.GetComponent<Enemy>()?.TakeDamage(damage);
                Debug.Log($"{hit.name}에게 궁극기 데미지 {damage} 입힘!");
            }
        } 
    }

    //UI연결문제로 추가
        public void SetSkillFromHUD(string key, SkillData data)
    {
        switch (key)
        {
            case "Z":
                characterData.active1 = data;
                break;

            case "X":
                characterData.active2 = data;
                break;

            case "C":
                characterData.ultimate = data;
                break;
        }

        Debug.Log($"스킬 등록됨 [{key}] → {data.skillName}");
    }
    public void UseSkillFromHUD(SkillData skill)
    {
        if (skill == null) return;

        // HUD Z → active1 스킬 실행
        if (skill == characterData.active1)
        {
            if (active1CooldownTimer <= 0)
            {
                StartCoroutine(UseActiveSkill1());
                active1CooldownTimer = GetReducedCooldown(characterData.active1.coolTime);
            }
            return;
        }

        // HUD X → active2 스킬 실행
        if (skill == characterData.active2)
        {
            if (active2CooldownTimer <= 0)
            {
                StartCoroutine(UseActiveSkill2());
                active2CooldownTimer = GetReducedCooldown(characterData.active2.coolTime);
            }
            return;
        }

        // HUD C → 궁극기 실행
        if (skill == characterData.ultimate)
        {
            AttemptUltimate();
            return;
        }
    }

}
