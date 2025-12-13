# 게임명: Forest without tomorrow
<img width="1592" height="890" alt="스크린샷 2025-12-13 164427" src="https://github.com/user-attachments/assets/ff2a012b-0e2a-4902-8ecc-fb63ae828e15" />

## 목차
1. [프로젝트 장르 및 소개](#프로젝트-장르-및-소개)
2. [주요기능](#주요기능)
3. [역할분담](#역할분담)
4. [구현내용](#구현내용)
5. [기술스택](#기술스택)
6. [사용에셋 목록](#사용에셋-목록)

## 프로젝트 장르 및 소개
* 장르: 탑뷰 2D 슈팅 액션 뱀서라이크
* 소개: 캐릭터를 중심으로 한계 없는 성장을 통해 무한히 몰려오는 적들을 처치하고 생존하는 탑뷰 2D 슈팅 액션 뱀서라이크
* 개발 기간: 총 4일 { 2025.12.01 ~ 2025.12.04 }

## 주요기능
### 게임플레이
- 끊임없이 몰려오는 적들을 해치우며 게임 시작 3분 뒤에 등장하는 보스를 처치해 게임 클리어를 목표로 함.
- 적을 해치우면 나오는 경험치를 모아 레벨업.
- 레벨업하면 나오는 랜덤 장비를 이용해 플레이어 캐릭터를 강화.
- 스테이지 내의 레벨업으로 해금 가능한 플레이어 캐릭터의 궁극기를 이용한 위기 모면 모먼트.
- 플레이어 캐릭터의 체력이 0이 되거나, 보스를 처치하면 GameEnd 팝업을 표시, 팝업의 버튼을 이용한 게임 재시작 가능.

<img width="1593" height="891" alt="스크린샷 2025-12-13 164445" src="https://github.com/user-attachments/assets/7eefe035-cfa8-4f90-b652-89a2ae86f1bb" />
<img width="1595" height="891" alt="스크린샷 2025-12-13 165628" src="https://github.com/user-attachments/assets/1eefde8c-a844-4dd7-9d29-78772906d87c" />
<img width="1589" height="891" alt="스크린샷 2025-12-13 165647" src="https://github.com/user-attachments/assets/1525c214-84e8-4a84-a4c4-858970e32ea2" />
<img width="1590" height="891" alt="스크린샷 2025-12-13 165745" src="https://github.com/user-attachments/assets/1168a1d6-162f-46b7-a101-784c2457480b" />
<img width="1588" height="890" alt="스크린샷 2025-12-13 170009" src="https://github.com/user-attachments/assets/2e2dbeb8-a60e-412e-ab32-196309975bd2" />
<img width="1587" height="887" alt="스크린샷 2025-12-13 170027" src="https://github.com/user-attachments/assets/86d94af1-cb55-4f37-8549-d32174536025" />

### 핵심기술
- GameManager에서 게임 흐름/로직 결정.
- SceneLoader를 이용해 씬 전환 기능.
- 공통으로 사용될 수 있는 IDmagable, IProjectilable, IBossAttackable, IExpReceiver 인터페이스 선언 및 구현
- RangeWeaponHandler를 이용해 플레이어 캐릭터의 사거리 내의 적들을 탐지하고, 가장 가까운 적을 자동 공격(투사체 발사)하는 로직 구현
- StatHandler로 플레이어 캐릭터의 모든 능력치를 관리.
- ScriptableObject로 정의한 캐릭터, 적, 무기, 장비, 아이템, 페이즈, 스킬
- 각 효과의 사운드와 이미지, UI. 중복 재생을 막기 위한 쿨다운 기능. 
- ObjectPoolManager에서 사용할 프리팹을 미리 생성하고 관리.
- SO로 정의한 데이터를 이용한 적 자동 생성 기능.

## 역할분담
* [김지훈](https://github.com/EunHyul769): PM
* [엄성진](https://github.com/formuloratio): Player/Weapon
* [김하늘](https://github.com/Hagill): Enemy
* [박재아](https://github.com/jaeapark): UI(Scene)/GameFlow
* [김동관](https://github.com/kdk7992-sketch): Map/Item/Sound

## 구현내용 [엄성진]
<img width="2326" height="934" alt="UML_N5" src="https://github.com/user-attachments/assets/41746b50-eeea-4bb2-99b0-829be68aa2c0" />

### 스크립트
---
#### 플레이어 컨트롤
<details>
  <summary> BaseController.cs </summary>

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public static BaseController Instance { get; private set; } //UI연결문제로 추가

    protected Rigidbody2D _rigidbody;
    [SerializeField] private SpriteRenderer characterRenderer;
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private CharacterData characterData;

    // 시작 시 기본으로 장착할 무기 데이터 (SO)
    [SerializeField] private WeaponData defaultWeaponData;

    protected Vector2 movementDirection = Vector2.zero;
    public Vector2 MovementDirection { get { return movementDirection; } }
    
    protected Vector2 lookDirection = Vector2.zero;
    public Vector2 LookDirection { get { return lookDirection; } }

    private Vector2 knockback = Vector2.zero;
    private float knockbackDuration = 0.0f;

    protected AnimationHandler animationHandler;
    protected StatHandler statHandler;

    // 무기 장착을 담을 리스트
    protected List<WeaponHandler> activeWeapons = new List<WeaponHandler>();

    protected bool isAttacking;
    private float timeSinceLastAttack = float.MaxValue;

    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 3f;   // 대쉬 이동 거리
    [SerializeField] private float dashDuration = 0.2f; // 대쉬하는데 걸리는 시간
    [SerializeField] private float dashCooldown = 1.0f; // 대쉬 쿨타임

    private bool isDashing = false;
    private float lastDashTime = -10f;

    public bool IsInvincible { get; private set; } = false;
    public float GetDashCooldown() => dashCooldown; //dashcooldown UIManager에서 접근할 수 있도록 수정했습니다

    protected virtual void Awake()
    {
        Instance = this; //UI연결문제로 추가

        _rigidbody = GetComponent<Rigidbody2D>();
        animationHandler = GetComponent<AnimationHandler>();
        statHandler = GetComponent<StatHandler>();
        characterRenderer = GetComponentInChildren<SpriteRenderer>();
        if (characterData != null)
            characterRenderer.sprite = characterData.characterSprite;

        if (defaultWeaponData != null)
        {   // 기본 무기가 설정되어 있다면 장착
            EquipWeapon(defaultWeaponData);
        }
        else
        {
            // 데이터가 없으면 자식에 있는 거라도 가져오기
            WeaponHandler[] existing = GetComponentsInChildren<WeaponHandler>();
            activeWeapons.AddRange(existing);
        }
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        HandleAction();
        if (movementDirection.sqrMagnitude > 0.01f)
            Rotate(movementDirection);
        else
            Rotate(lookDirection); // 멈추면 마우스를 봄
        HandleAttackDelay();
    }

    protected virtual void FixedUpdate()
    {
        if (isDashing) return; //대쉬 중일 때는 일반 이동 로직을 수행하지 않음

        Movement(movementDirection);
        if (knockbackDuration > 0.0f)
        {
            knockbackDuration -= Time.fixedDeltaTime;
        }
    }

    protected virtual void HandleAction()
    {  
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        // lookDirection 업데이트
        lookDirection = (mouseWorld - transform.position).normalized;

    }

    protected virtual void Movement(Vector2  direction)
    {
        direction = direction * statHandler.Speed;
        if (knockbackDuration > 0.0f)
        {
            direction *= 0.2f; //이동 방향의 힘 줄이기
            direction += knockback; //넉백의 힘 적용
        }
        _rigidbody.velocity = direction;
        animationHandler.Move(direction);
    }

    //레벨업으로 아이템 획득 시 아래 메서드로 무기 생성
    //매개변수에 해당 SO 전달해서 호출
    public void EquipWeapon(WeaponData data)
    {
        if (data == null || data.weaponPrefab == null) return;

        WeaponHandler existingWeapon = activeWeapons.Find(w => w.gameObject.name.Contains(data.weaponName));

        if (existingWeapon != null)
        {
            // 이미 있으면 레벨업 등의 로직 (나중에 구현)
            Debug.Log($"{data.weaponName}은(는) 이미 보유 중입니다.");
            return;
        }

        WeaponHandler newWeapon = Instantiate(data.weaponPrefab, weaponPivot);

        newWeapon.weaponData = data;

        newWeapon.name = data.weaponName; // 이름 설정

        activeWeapons.Add(newWeapon);

        Debug.Log($"무기 장착 완료: {data.weaponName}");

        if (UIManager.Instance != null)
        {
            UIManager.Instance.RefreshWeaponSlots(activeWeapons);
        }
        else
        {
            Debug.LogWarning("UIManager가 아직 준비되지 않아 UI 갱신 건너뜀");
        }//UI
    }

    public void AttemptDash()
    {
        // 쿨타임 체크 및 이미 대쉬 중인지 확인
        if (Time.time < lastDashTime + dashCooldown || isDashing) return;

        UIManager.Instance?.OnDashUsed(); //UIManager에게 전달

        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        IsInvincible = true; // 무적 시작
        lastDashTime = Time.time;

        // 대쉬 방향 결정: 이동 중이면 이동 방향, 정지 중이면 바라보는 방향
        Vector2 dashDir = movementDirection.normalized;
        if (dashDir == Vector2.zero) dashDir = lookDirection.normalized;

        // 거리 = 속력 * 시간  =>  속력 = 거리 / 시간
        float dashSpeed = dashDistance / dashDuration;

        // 리지드바디 속도 강제 적용
        _rigidbody.velocity = dashDir * dashSpeed;

        // 대쉬 지속 시간 동안 대기
        yield return new WaitForSeconds(dashDuration);

        // 대쉬 종료
        _rigidbody.velocity = Vector2.zero;
        isDashing = false;
        IsInvincible = false; // 무적 종료
    }

    protected virtual void Rotate(Vector2 direction)
    {
        float rotZ = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

        bool flipX;

        if (movementDirection.sqrMagnitude > 0.01f)
        {
            flipX = movementDirection.x > 0f;
        }
        else
        {
            flipX = lookDirection.x > 0f;
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float deltaX = mouseWorld.x - transform.position.x;

            flipX = deltaX < 0f;
        }
        
        characterRenderer.flipX = flipX;
        
        if (weaponPivot != null)
            weaponPivot.rotation = Quaternion.Euler(0f, 0f, rotZ);
        
        foreach (var weapon in activeWeapons)
            weapon.Rotate(flipX);
    }

    // 공격 사이의 딜레이 처리(없으면 매우 빠르게 발사됨)
    private void HandleAttackDelay()
    {
        foreach (var weapon in activeWeapons)
        {
            if (weapon == null)
            {
                return;
            }
            if (timeSinceLastAttack <= weapon.Delay)
            {
                timeSinceLastAttack += Time.deltaTime;
            }
            if (isAttacking && timeSinceLastAttack > weapon.Delay)
            {
                timeSinceLastAttack = 0;
                Attack();
            }
        }
    }

    protected virtual void Attack()
    {
        if (lookDirection != Vector2.zero)
        {
            // 리스트를 순회하며 모든 무기 공격 시도
            foreach (var weapon in activeWeapons)
            {
                weapon.Attack();
            }
        }
    }

    public List<WeaponHandler> GetActiveWeapons()
    {
        return activeWeapons;
    } //ui로 목록 가져가기
}

```
   </details>

<details>
  <summary> PlayerController.cs </summary>
  
```csharp
using UnityEngine;

public class PlayerController : BaseController
{
    private Camera camera;

    protected override void Start()
    {
        base.Start();
        camera = Camera.main;
    }

    protected override void HandleAction()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movementDirection = new Vector2(horizontal, vertical).normalized;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            AttemptDash();
        }

        Vector2 mousePosition = Input.mousePosition;
        Vector2 worldPos = camera.ScreenToWorldPoint(mousePosition);
        lookDirection = (worldPos - (Vector2)transform.position);

        if (lookDirection.magnitude < .9f)
        {
            lookDirection = Vector2.zero;
        }
        else
        {
            lookDirection = lookDirection.normalized;
        }

        isAttacking = true;
        //isAttacking = Input.GetMouseButton(0);
    }
}

```

</details>

<details>
  <summary> AnimationHandler.cs </summary>
  
```csharp
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("IsMove"); //이동시 애니메이션
    private static readonly int IsDamage = Animator.StringToHash("IsDamage"); //피격시 애니메이션

    protected Animator animator;

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Move(Vector2 obj)
    {
        animator.SetBool(IsMoving, obj.magnitude > .5f);
    }

    public void Damage()
    {
        animator.SetBool(IsDamage, true);
    }

    public void InvincibilityEnd()
    {
        animator.SetBool(IsDamage, false);
    }
}

```

</details>

<details>
  <summary> StatHandler.cs </summary>
  
```csharp
using UnityEngine;

public class StatHandler : MonoBehaviour
{
    [Range(1, 100)][SerializeField] private int health = 10;
    public int Health
    {
        get => health;
        set => health = Mathf.Clamp(value, 0, 100);
    }

    [SerializeField] private CharacterData characterData; // 캐릭터 데이터 연결

    // 실제 게임에서 변동되는 스탯들
    public int CurrentHealth { get; set; } // 현재 체력
    public int MaxHealth { get; set; }
    public float Speed { get; set; }
    public float Attack { get; set; }
    public int Defense { get; set; } // 방어력 추가
    public float AttackSpeed { get; set; } // 공격 속도 (기본 1.0f 기준, 높을수록 빠름 혹은 딜레이 감소)

    // 쿨타임 감소율 (0.0f = 0%, 0.1f = 10% 감소)
    public float CooldownReduction { get; set; } = 0.0f;

    //골드 획득량 증가 배율 (1.0f = 100%, 1.2f = 120%)
    public float GoldBonusMultiplier { get; set; } = 1.0f; // 1.0 = 100%

    // 패시브 효과용 플래그
    public bool HasExplosiveProjectile { get; set; } = false;
    public float ExplosiveChance { get; set; } = 0f;

    private ResouceController resouceController;

    private void Awake()
    {
        if (characterData != null)
        {
            InitializeStats();
        }

        resouceController = GetComponent<ResouceController>();
    }

    private void InitializeStats()
    {
        MaxHealth = characterData.baseHealth;
        CurrentHealth = MaxHealth;
        Speed = characterData.baseSpeed;
        Attack = characterData.baseAttack;
        AttackSpeed = characterData.baseAttackSpeed; // 예: 1.0f
        Defense = 0; // 기본 방어력
        CooldownReduction = 0f;
    }

    // 아이템 습득 시 스탯 업데이트용 메서드
    public void AddMaxHealth(int amount)
    {
        MaxHealth = resouceController.MaxHealth;
        CurrentHealth = resouceController.CurrentHealth;
        MaxHealth += amount;
        CurrentHealth += amount; // 최대 체력이 늘어나면 현재 체력도 같이 채워줌 (선택사항)

        UIManager.Instance.UpdateHP(CurrentHealth, MaxHealth);
    }

    public void AddAttack(float amount) => Attack += amount;
    public void AddDefense(int amount) => Defense += amount;

    // 퍼센트 증가 로직 (기본값에 곱할지, 합연산할지 정책에 따라 다름. 여기선 현재 값에 곱연산 적용)
    public void AddSpeedPercent(float percent) => Speed *= (1f + percent); // 0.1f = 10% 증가
    public void AddAttackSpeedPercent(float percent) => AttackSpeed *= (1f + percent);
    public void AddCooldownReduction(float percent) => CooldownReduction += percent; // 쿨감은 합연산 (예: 10% + 10% = 20%)
}

```

</details>

<details>
  <summary> PlayerSkillController.cs </summary>
  
  ```csharp
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

        // 공격력 복구
        statHandler.Attack = originalAttack;

        // 페이즈 2: 공격속도 감소 (1초) - 패널티
        // 공격 딜레이가 늘어나는 것이므로 수치를 더하거나 곱해서 느리게 만듦
        statHandler.AttackSpeed *= 1.5f;
        Debug.Log("마력 증강 부작용! 공격 속도 저하");

        yield return new WaitForSeconds(1f);

        // 공속 복구
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
```

</details>

<details>
  <summary> ResouceController.cs </summary>

  ```csharp
using UnityEngine;

public class ResouceController : MonoBehaviour
{
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private float healthChangeDelay = .5f;
    private BaseController baseController;
    private StatHandler statHandler;
    private AnimationHandler animationHandler;

    private float timeSinceLastChange = float.MaxValue;

    public int CurrentHealth {  get; private set; }
    public int MaxHealth { get; private set; }

    [SerializeField] private GameObject expOrbPrefab;

    [SerializeField] private bool isPlayer = true;

    private void Awake()
    {
        baseController = GetComponent<BaseController>();
        statHandler = GetComponent<StatHandler>();
        animationHandler = GetComponent<AnimationHandler>();
    }

    private void Start()
    {
        CurrentHealth = statHandler.Health;
        MaxHealth = statHandler.MaxHealth;
    }

    private void Update()
    {
        if (timeSinceLastChange < healthChangeDelay)
        {
            timeSinceLastChange += Time.deltaTime;
            if (timeSinceLastChange >= healthChangeDelay)
            {
                animationHandler.InvincibilityEnd(); //무적 해제
            }
        }
    }

    public bool ChangeHealth(int change)
    {
        if (baseController != null && baseController.IsInvincible)
        {
            // 무적이면 데미지 무시하고 리턴
            return false;
        }

        if (change == 0 || timeSinceLastChange < healthChangeDelay)
        {
            return false; //데미지를 받지 않음
        }
        int defense = statHandler.Defense; // 방어력 수치 가져오기
        if (change < 0)
        {
            // 데미지일 경우 방어력 적용
            int damageAfterDefense = -change - defense;
            change = damageAfterDefense < 1 ? 1 : -damageAfterDefense; //최소 데미지는 1로 설정
        }
        timeSinceLastChange = 0f;

        MaxHealth = statHandler.MaxHealth; //최대 체력 동기화
        CurrentHealth += change;
        CurrentHealth = CurrentHealth > MaxHealth ? MaxHealth : CurrentHealth;
        CurrentHealth = CurrentHealth < 0 ? 0 : CurrentHealth;
        Debug.Log("체력 " + CurrentHealth);
        if (change < 0)
        {
            animationHandler.Damage();
        }
        // 체력 UI 갱신 필요한 지점
        UIManager.Instance.UpdateHP(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0f)
        {
            Death(); //플레이어 사망
        }

        return true;
    }

    private void Death()
    {
        Debug.Log("사망");

        if (isPlayer)
        {
            // 플레이어면 바로 GameOver 호출
            GameManager.Instance.GameOver();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

```

</details>

<details>
  <summary> EquipmentController.cs </summary>

  ```csharp
using System.Collections.Generic;
using UnityEngine;

public class EquipmentController : MonoBehaviour
{
    public static EquipmentController Instance { get; private set; } //UI연결문제로 추가

    private StatHandler statHandler;

    // 획득한 장비 아이템들을 저장할 리스트
    public List<EquipmentData> equippedItems = new List<EquipmentData>();

    [SerializeField] private EquipmentData itemData; //테스트용: 장비 아이템 할당

    private void Awake()
    {
        Instance = this; //UI연결문제로 추가
        statHandler = GetComponent<StatHandler>();
    }

    // 아이템 획득 시 호출할 메서드
    public void EquipItem(EquipmentData data)
    {
        if (data == null) return;
        equippedItems.Add(data);

        Debug.Log($"아이템 습득 및 저장 완료: {data.itemName}");

        foreach (var modifier in data.modifiers)
        {
            ApplyStat(modifier);
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.RefreshEquipmentSlots(equippedItems);
        }
    }

    private void ApplyStat(StatModifier modifier)
    {
        switch (modifier.type)
        {
            case StatType.MaxHealth:
                statHandler.AddMaxHealth((int)modifier.value);
                break;

            case StatType.Attack:
                statHandler.AddAttack(modifier.value);
                break;

            case StatType.Defense:
                statHandler.AddDefense((int)modifier.value);
                break;

            case StatType.MoveSpeedPercent:
                // 10% 증가는 0.1f로 데이터에 입력됨
                statHandler.AddSpeedPercent(modifier.value);
                break;

            case StatType.AttackSpeedPercent:
                statHandler.AddAttackSpeedPercent(modifier.value);
                break;

            case StatType.CooldownReduction:
                statHandler.AddCooldownReduction(modifier.value);
                break;
        }
    }

    // 외부(UI 등)에서 현재 장착 중인 아이템 목록을 가져올 때 사용
    public List<EquipmentData> GetEquippedItems()
    {
        return equippedItems;
    }
}

```

</details>

<details>
  <summary> PlayerExp.cs </summary>

  ```csharp
using UnityEngine;

public class PlayerExp : MonoBehaviour, IExpReceiver
{
    [Header("현재 레벨")]
    [SerializeField] private int level = 1;           // 현재 레벨
    public int Level {  get { return level; } }

    [Header("현재 경험치")]
    [SerializeField] private int currentExp = 0;      // 현재 경험치
    public int CurrentExp { get { return currentExp; } }

    [Header("초기 레벨업 필요 경험치")]
    [SerializeField] private int maxExp = 100;        // 레벨업에 필요한 경험치
    public int MaxExp { get { return maxExp; } }

    [Header("레벨업 마다의 필요 경험치 증가량")]
    [SerializeField] private float expGrowthFactor = 1.2f; // 레벨업 할 때마다 필요 경험치 증가량 (1.2배)

    private void Awake()
    {

    }
    private void Start()
    {
        UIManager.Instance.UpdateLevel(level);
    }

    public void OnExpPickup(int amount)
    {
        currentExp += amount;
        Debug.Log($"경험치 획득: {amount} | 현재 경험치: {currentExp} / {maxExp}");

        // UI 업데이트 추가
        UIManager.Instance.UpdateEXP(currentExp, maxExp);

        // 경험치 먹는 효과음
        SoundManager.Instance.PlaySFX(SoundManager.Instance.expGain);

        if (currentExp >= maxExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        while (currentExp >= maxExp)
        {
            currentExp -= maxExp;
            level++;
            
            SoundManager.Instance.PlaySFX(SoundManager.Instance.levelUp);

            // 다음 레벨 필요 경험치 증가
            maxExp = Mathf.RoundToInt(maxExp * expGrowthFactor);

            Debug.Log($"<color=yellow>레벨 업! 현재 레벨: {level}</color>");
            // UI 갱신 추가
            UIManager.Instance.UpdateEXP(currentExp, maxExp);
            UIManager.Instance.UpdateLevel(level);

            // 게임매니저와 연결
            GameManager.Instance.OnPlayerLevelUp(level);
        }
    }
}

```

</details>

---
#### 무기 컨트롤

<details>
  <summary> WeaponHandler.cs </summary>

  ```csharp
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [Header("Attack Info")]
    [SerializeField] private float delay = 1f;
    public float Delay { get => delay; set => delay = value; }

    [SerializeField] private float weaponSize = 1f;
    public float WeaponSize { get => weaponSize; set => weaponSize = value; }

    [SerializeField] public float power = 1f;
    public float Power { get => power; set => power = value; }

    [SerializeField] private float speed = 1f;
    public float Speed { get => speed; set => speed = value; }

    [SerializeField] private float attackRange = 10f;
    public float AttackRange { get => attackRange; set => attackRange = value; }

    public LayerMask target;

    private static readonly int IsAttack = Animator.StringToHash("IsAttack");

    public BaseController Controller { get; private set; }

    private Animator animator;
    private SpriteRenderer weaponRenderer;
    public WeaponData weaponData;

    protected virtual void Awake()
    {
        Controller = GetComponentInParent<BaseController>();
        animator = GetComponentInChildren<Animator>();
        weaponRenderer = GetComponentInChildren<SpriteRenderer>();

        animator.speed = 1.0f / delay;
        transform.localScale = Vector3.one * weaponSize;
    }

    protected virtual void Start()
    {
        
    }

    public virtual void Attack()
    {
        AttackAnimation();
    }

    public virtual void AttackAnimation()
    {
        animator.SetTrigger(IsAttack);
    }

    public virtual void Rotate(bool isLeft)
    {
        weaponRenderer.flipY = isLeft;
    }
}

```

</details>

<details>
  <summary> RangeWeaponHandler.cs </summary>

  ```csharp
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
        // 사정거리(AttackRange) 내의 모든 적(enemyLayer)을 탐지
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, AttackRange, enemyLayer);

        if (hitColliders.Length == 0) return; // 적이 없으면 발사 안 함

        // 거리순으로 정렬 후, 발사체 개수(NumberOfPrijectilesPerShot)만큼만 가장 가까운 적을 선택
        // 거리 계산 성능을 위해 sqrMagnitude 사용 권장 (여기서는 가독성을 위해 Distance 사용)
        var closestEnemies = hitColliders
            .OrderBy(x => Vector2.Distance(transform.position, x.transform.position))
            .Take(numberOfPrijectilesPerShot)
            .ToList();

        // 선택된 적들을 향해 각각 발사
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

```

</details>

---
#### 발사체 컨트롤

<details>
  <summary> ProjectileController.cs </summary>

  ```csharp
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private LayerMask collisionLayer;
    private RangeWeaponHandler rangeWeaponHandler;

    private float currentDuration;
    private Vector2 direction;
    private bool isReady;
    private Transform pivot;

    private Rigidbody2D _rigidbody;
    private SpriteRenderer spriteRenderer;
    private StatHandler stats;

    private float damage;
    private bool isExplosive;

    public bool fxOnDestroy = true; //삭제시의 이펙트 출력 여부
    [SerializeField] private GameObject explosionFxPrefab;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        stats = FindAnyObjectByType<StatHandler>();
        pivot = transform.GetChild(0);
    }

    private void Update()
    {
        if (!isReady) return;

        currentDuration += Time.deltaTime;

        if (currentDuration > rangeWeaponHandler.Duration)
        {
            DestroyProjectile(transform.position, false);
            return;
        }
        _rigidbody.velocity = direction * rangeWeaponHandler.Speed;
    }

    private void OnEnable()
    {
        Invoke("Despawn", 1f);
    }

    private void Despawn()
    {
        DestroyProjectile(transform.position, false);
    }

    public void Setup(float dmg, bool explosive)
    {
        this.damage = dmg;
        this.isExplosive = explosive;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 폭발 확률 계산
        bool isExplosiveShot = false;
        if (stats.HasExplosiveProjectile)
        {
            float randomVal = Random.Range(0f, 1f); // 0.0 ~ 1.0
            if (randomVal <= stats.ExplosiveChance) // 확률 성공
            {
                isExplosiveShot = true;
            }
        }
        // 투사체 설정 (기본 공격력 + 폭발 여부)
        Setup(stats.Attack, isExplosiveShot);

        float finalDamage = damage;
        if (collisionLayer.value == (collisionLayer.value | (1 << collision.gameObject.layer))) //벽면 충돌체랑 같은 레이어인지 or 연산으로 확인
        {
            DestroyProjectile(collision.ClosestPoint(transform.position) - direction * .2f, fxOnDestroy);
        } //collision.ClosestPoint(transform.position) -> 충돌체랑 가장 가까운 부분
        else if (rangeWeaponHandler != null)
        {
            //타겟 충돌체와 같은지 확인 (웨폰 핸들러에서 지정)
            if (rangeWeaponHandler.target.value == (rangeWeaponHandler.target.value | (1 << collision.gameObject.layer)))
            {
                IDamagable damagableObject = collision.gameObject.GetComponent<IDamagable>();

                if (damagableObject != null)
                    damagableObject.TakeDamage(rangeWeaponHandler.power);

                // 패시브 효과: 폭발
                if (isExplosive)
                {
                    // 폭발 이펙트 생성 로직 추가 가능
                    finalDamage = damage * 2; // 데미지 2배
                    Debug.Log("패시브 발동! 으아아 이게 뭐야 (폭발 데미지)");

                    if (damagableObject != null)
                        damagableObject.TakeDamage(finalDamage);
                }

                DestroyProjectile(collision.ClosestPoint(transform.position), fxOnDestroy);
            }
        }
    }

    public void Init(Vector2 direction, RangeWeaponHandler weaponHandler)
    {
        rangeWeaponHandler = weaponHandler;

        this.direction = direction;
        currentDuration = 0;
        transform.localScale = Vector3.one * weaponHandler.BulletSize;
        spriteRenderer.color = weaponHandler.ProjectileColor;

        transform.right = this.direction;

        if (direction.x < 0)
            pivot.localRotation = Quaternion.Euler(180, 0, 0);
        else
            pivot.localRotation = Quaternion.Euler(0, 0, 0);

        isReady = true;
    }

    private void DestroyProjectile(Vector3 position, bool createFx)
    {
        // TODO: createFx가 true일 때 파괴 이펙트(Particle)를 생성하는 코드를 여기에 추가할 수 있습니다.
        if (createFx && explosionFxPrefab != null)
        {
            // 파티클 생성
            Instantiate(explosionFxPrefab, position, Quaternion.identity);
        }
        Destroy(this.gameObject);
    }
}

```

</details>

---
#### 발사체 관리

<details>
  <summary> ProjectileManager.cs </summary>

  ```csharp
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    private static ProjectileManager instance;
    public static ProjectileManager Instance {  get { return instance; } }

    [SerializeField] private GameObject[] projectilePrefabs;

    private void Awake()
    {
        instance = this;
    }

    public void ShootBullet(RangeWeaponHandler rangeWeaponHandler, Vector2 startPosition, Vector2 direction)
    {
        GameObject origin = projectilePrefabs[rangeWeaponHandler.BulletIndex];
        GameObject obj = Instantiate(origin, startPosition, Quaternion.identity);

        ProjectileController projectileController = obj.GetComponent<ProjectileController>();
        projectileController.Init(direction, rangeWeaponHandler);
    }
}

```

</details>

---
#### 캐릭터/무기/발사체 데이터 (SO)

<details>
  <summary> CharacterData.cs </summary>

  ```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Game Data/Character")]
public class CharacterData : ScriptableObject
{
    public string characterName; // 세실리
    public string weaponName;    // 마법화살
    public Sprite characterSprite;

    [Header("Base Stats")]
    public int baseHealth = 100;
    public float baseSpeed = 5f;
    public float baseAttack = 10f;
    public float baseAttackSpeed = 1f;

    [Header("Skills")]
    public SkillData passive1; // 도, 돈이다!
    public SkillData passive2; // 으아아 이게 뭐야
    public SkillData active1;  // 우왓, 저리 가!
    public SkillData active2;  // 마력 증강
    public SkillData ultimate; // 비장의 마법이야!

    [Header("Image (얼굴 전용)")]
    public Sprite hudSprite;

    [Header("Unique Weapon")]
    public WeaponData uniqueWeapon;
}
```

</details>

<details>
  <summary> SkillData.cs </summary>

  ```csharp
using UnityEngine;

public enum SkillsType { Passive, Active, Ultimate }

[CreateAssetMenu(fileName = "New Skill", menuName = "Game Data/Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;
    [TextArea] public string description;
    public SkillsType type;
    public float coolTime; // 쿨타임 (패시브는 0)
    public Sprite icon;

    [Header("Skill Values")]
    public float value1; // 스킬 효과 수치 (예: 50% 증가면 0.5)
    public float value2; // 보조 수치
    public float duration; // 지속 시간
}
```

</details>

<details>
  <summary> WeaponData.cs </summary>

  ```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Game Data/Weapon")]
public class WeaponData : ScriptableObject
{
    [Header("Info")]
    public string weaponName; // 무기 이름
    public Sprite icon;       // UI 표시용 아이콘
    [TextArea] public string description;

    [Header("Prefab")]
    public WeaponHandler weaponPrefab; // 실제 소환될 무기 프리팹
}

```

</details>

<details>
  <summary> EquipmentData.cs </summary>

  ```csharp
using System.Collections.Generic;
using UnityEngine;

// 스탯 종류 정의
public enum StatType
{
    MaxHealth,          // 최대 체력 (고정값)
    Attack,             // 공격력 (고정값)
    Defense,            // 방어력 (고정값)
    MoveSpeedPercent,   // 이동 속도 (%)
    AttackSpeedPercent, // 공격 속도 (%)
    CooldownReduction   // 쿨타임 감소 (%)
}

[System.Serializable]
public class StatModifier
{
    public StatType type;
    public float value; // 고정값 혹은 퍼센트(0.1 = 10%)
}

[CreateAssetMenu(fileName = "New Equipment", menuName = "Game Data/Equipment")]
public class EquipmentData : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon; // 아이콘 필요 시 사용

    [Header("Stats")]
    public List<StatModifier> modifiers = new List<StatModifier>();
}
```

</details>


## 기술스택
* Language: **C#**
* Engine: **Unity**
* Version Control: **Git, GitHub**
* IDE: **Visual Studio 2022**

## 사용에셋 목록
* 맵 타일셋: [Free Topdown Fantasy - Forest - Pixelart Tileset] (https://aamatniekss.itch.io/topdown-fantasy-forest)
* 그외: AI
