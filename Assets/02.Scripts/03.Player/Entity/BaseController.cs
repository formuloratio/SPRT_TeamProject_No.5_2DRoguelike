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

        // 2) lookDirection 업데이트
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
