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

</details>

<details>
  <summary> ResouceController.cs </summary>

</details>

<details>
  <summary> EquipmentController.cs </summary>

</details>

<details>
  <summary> PlayerExp.cs </summary>

</details>

#### 무기 컨트롤

<details>
  <summary> WeaponHandler.cs </summary>

</details>

<details>
  <summary> RangeWeaponHandler.cs </summary>

</details>


#### 발사체 컨트롤

<details>
  <summary> ProjectileController.cs </summary>

</details>

#### 발사체 관리

<details>
  <summary> ProjectileManager.cs </summary>

</details>

#### 캐릭터/무기/발사체 데이터 (SO)

<details>
  <summary> CharacterData.cs </summary>

</details>

<details>
  <summary> SkillData.cs </summary>

</details>

<details>
  <summary> WeaponData.cs </summary>

</details>

<details>
  <summary> EquipmentData.cs </summary>

</details>


## 기술스택
* Language: **C#**
* Engine: **Unity**
* Version Control: **Git, GitHub**
* IDE: **Visual Studio 2022**

## 사용에셋 목록
* 맵 타일셋: [Free Topdown Fantasy - Forest - Pixelart Tileset] (https://aamatniekss.itch.io/topdown-fantasy-forest)
* 그외: AI
