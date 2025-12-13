using UnityEngine;

public class DifficultyScaler : MonoBehaviour
{
    public static DifficultyScaler Instance { get; private set; }

    [Header("난이도 계수 증가 설정")]
    [Tooltip("기본 체력 배수. 게임 시작 시 적용.")]
    public float initialHealthMultiplier = 1.0f;
    [Tooltip("게임 시간 1분마다 체력 배수가 얼마나 증가하는지 설정. 예) 0.03f = 1분당 3% 증가")]
    public float healthMultiplierIncreasePerMinute;
    [Tooltip("기본 공격력 배수. 게임 시작 시 적용.")]
    public float initialDamageMultiplier = 1.0f;
    [Tooltip("게임 시간 1분마다 공격력 배수가 얼마나 증가하는지 설정. 예) 0.03f = 1분당 3% 증가")]
    public float damageMultiplierIncreasePerMinute;

    private GameManager gameManager;

    public float CurrentHealthMultiplier { get; private set; }
    public float CurrentDamageMultiplier { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        if(gameManager == null)
        {
            Debug.LogError("DifficultyScaler: GameManager를 찾을 수 없음");
            enabled = false;
        }

        CurrentHealthMultiplier = initialHealthMultiplier;
        CurrentDamageMultiplier = initialDamageMultiplier;
    }

    private void Update()
    {
        if (gameManager == null) return;

        float minutes = gameManager.playTime / 60f;

        CurrentHealthMultiplier = initialHealthMultiplier + (minutes * healthMultiplierIncreasePerMinute);
        CurrentDamageMultiplier = initialDamageMultiplier + (minutes * damageMultiplierIncreasePerMinute);
    }
}
