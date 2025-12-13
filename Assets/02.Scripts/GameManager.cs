using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public enum GameState
{
    Ready,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Skill Data (ScriptableObjects)")]
    public SkillData ultimateSkillSO;       // 20레벨 궁극기
    public SkillData[] normalSkillPool;     // 일반 스킬 Pool

    // 이미 선택된 일반 스킬 리스트 (중복 방지)
    private List<SkillData> ownedNormalSkills = new List<SkillData>();

    [Header("Equipment Pool")]
    public EquipmentData[] equipmentPool;   // 레벨업 장비 선택 Pool (SO 폴더에서 연결)

    [Header("Weapon Pool")]
    public WeaponData[] weaponPool;

    public GameState State { get; private set; }
    public float playTime = 0f;

    private void OnEnable()
    {
        Enemy.OnBossDiedGlobal += GameOver;
    }

    private void OnDisable()
    {
        Enemy.OnBossDiedGlobal -= GameOver;
    }

    private void Awake()
    {
        Instance = this;
        State = GameState.Ready;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        StartGame();
    }

    private void Update()
    {
        if (State != GameState.Playing)
            return;

        playTime += Time.deltaTime;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateTimer(playTime);
    }

    // 게임 시작 시 첫 스킬 선택창 오픈
    public void StartGame()
    {
        State = GameState.Playing;

        SkillData[] startingSkills = GetRandomNormalSkills(2);
        UIManager.Instance.OpenSkillChoice(startingSkills);
        UIManager.Instance.RefreshWeaponSlots(BaseController.Instance.GetActiveWeapons());
    }

    // 스킬 선택 저장
    public void AddOwnedNormalSkill(SkillData data)
    {
        if (data == null) return;
        if (!ownedNormalSkills.Contains(data))
            ownedNormalSkills.Add(data);
    }

    // 일반 스킬 랜덤 추출
    private SkillData[] GetRandomNormalSkills(int count)
    {
        if (normalSkillPool == null || normalSkillPool.Length == 0)
            return new SkillData[0];

        List<SkillData> available = new List<SkillData>();

        foreach (var s in normalSkillPool)
        {
            if (!ownedNormalSkills.Contains(s))
                available.Add(s);
        }

        if (available.Count == 0)
            return new SkillData[0];

        // Shuffle
        for (int i = 0; i < available.Count; i++)
        {
            int rand = Random.Range(0, available.Count);
            (available[i], available[rand]) = (available[rand], available[i]);
        }

        int pick = Mathf.Min(count, available.Count);
        SkillData[] result = new SkillData[pick];

        for (int i = 0; i < pick; i++)
            result[i] = available[i];

        return result;
    }

    // 레벨업 트리거
    public void OnPlayerLevelUp(int level)
    {
        // 20레벨 → 궁극기 ->5로변경
        if (level == 5)
        {
            SkillData[] ult = { ultimateSkillSO };
            UIManager.Instance.OpenSkillChoice(ult);
            return;
        }

        // 40레벨 → 일반 스킬
        if (level == 40)
        {
            var normals = GetRandomNormalSkills(2);
            UIManager.Instance.OpenSkillChoice(normals);
            return;
        }

        // 나머지 모든 레벨 → 장비 + 무기 혼합 옵션 제공
        object[] options = GetMixedUpgradeOptions(3);
        LevelUpPanel.Instance.Open(options);
    }
    private object[] GetMixedUpgradeOptions(int count)
    {
        List<object> result = new List<object>();

        // 장비 랜덤 2개
        var equips = GetRandomEquipments(2);
        foreach (var e in equips)
            result.Add(e);

        // 무기 랜덤 1개
        var weapons = GetRandomWeapons(1);
        foreach (var w in weapons)
            result.Add(w);

        // 필요한 개수만큼 자르기
        while (result.Count > count)
            result.RemoveAt(Random.Range(0, result.Count));

        // 셔플
        for (int i = 0; i < result.Count; i++)
        {
            int r = Random.Range(0, result.Count);
            (result[i], result[r]) = (result[r], result[i]);
        }

        return result.ToArray();
    }
    // 장비 랜덤 선택
    private EquipmentData[] GetRandomEquipments(int count)
    {
        if (equipmentPool == null || equipmentPool.Length == 0)
            return new EquipmentData[0];

        List<EquipmentData> list = new List<EquipmentData>(equipmentPool);

        // Shuffle
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(0, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }

        int pick = Mathf.Min(count, list.Count);
        EquipmentData[] result = new EquipmentData[pick];

        for (int i = 0; i < pick; i++)
            result[i] = list[i];

        return result;
    }
    private WeaponData[] GetRandomWeapons(int count)
    {
        if (weaponPool == null || weaponPool.Length == 0)
            return new WeaponData[0];

        List<WeaponData> list = new List<WeaponData>(weaponPool);

        // Shuffle
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(0, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }

        int pick = Mathf.Min(count, list.Count);
        WeaponData[] result = new WeaponData[pick];

        for (int i = 0; i < pick; i++)
            result[i] = list[i];

        return result;
    }

    // 게임 오버
    public void GameOver()
    {
        if (State == GameState.GameOver)
            return;

        State = GameState.GameOver;

        int min = (int)(playTime / 60);
        int sec = (int)(playTime % 60);
        string playtimeStr = $"{min:00}:{sec:00}";

        UIManager.Instance.OpenGameOver(playtimeStr);

        Debug.Log("게임 오버!");
    }

    public void GameOver(Enemy diedBoss)
    {
        if (diedBoss.EnemyData.enemyType == EnemyType.Boss)
        {
            if (State == GameState.GameOver)
                return;

            State = GameState.GameOver;

            int min = (int)(playTime / 60);
            int sec = (int)(playTime % 60);
            string playtimeStr = $"{min:00}:{sec:00}";

            UIManager.Instance.OpenGameOver(playtimeStr);

            Debug.Log("게임 오버!");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
