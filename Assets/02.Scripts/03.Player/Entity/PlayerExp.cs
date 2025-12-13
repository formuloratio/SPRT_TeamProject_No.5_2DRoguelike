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

    private ResouceController resourceController;

    private void Awake()
    {
        resourceController = GetComponent<ResouceController>();
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
