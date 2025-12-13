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
