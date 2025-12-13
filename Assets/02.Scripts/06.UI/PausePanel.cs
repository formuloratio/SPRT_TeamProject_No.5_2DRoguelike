using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    [Header("Skill Select Slots")]
    public int skillSlotCount = 15;
    public Transform skillSlotParent;
    public GameObject skillSlotPrefab;
    private SkillChoiceSlotUI[] pauseSkillSlots;

    [Header("Owned Item Slots")]
    public int itemSlotCount = 48;
    public Transform itemSlotParent;
    public GameObject itemSlotPrefab;

    [Header("Character Stats")]
    public TMP_Text hpText;
    public TMP_Text hpgenText;
    public TMP_Text defText;
    public TMP_Text spdText;
    public TMP_Text atkText;
    public TMP_Text atkspdText;
    public TMP_Text atkareaText;
    public TMP_Text cri;
    public TMP_Text cridmg;
    public TMP_Text projectilespd;
    public TMP_Text dur;
    public TMP_Text cd;
    public TMP_Text projectilenum;

    [Header("Buttons")]
    public Button resumeButton;   // 이어하기
    public Button retryButton;    // 재시작
    public Button mainButton;     // 메인 화면

    [Header("OpenClose")]
    public GameObject window;
    public bool isOpen = false;

    private void Awake()
    {
        resumeButton.onClick.AddListener(OnResumeClicked);
        retryButton.onClick.AddListener(OnRetryClicked);
        mainButton.onClick.AddListener(OnMainClicked);
    }

    private void Start()
    {
        GenerateSkillSlots();
        GenerateItemSlots();
        RefreshStatsDummy();
        window.SetActive(false);
        pauseSkillSlots = skillSlotParent.GetComponentsInChildren<SkillChoiceSlotUI>();
    }

    // 자동 슬롯 생성
    void GenerateSkillSlots()
    {
        foreach (Transform child in skillSlotParent)
            Destroy(child.gameObject);

        for (int i = 0; i < skillSlotCount; i++)
            Instantiate(skillSlotPrefab, skillSlotParent);
    }

    void GenerateItemSlots()
    {
        foreach (Transform child in itemSlotParent)
            Destroy(child.gameObject);

        for (int i = 0; i < itemSlotCount; i++)
            Instantiate(itemSlotPrefab, itemSlotParent);
    }

    // 더미 스탯 (데이터 없을 때)
    public void RefreshStatsDummy()
    {
        hpText.text = "-";
        hpgenText.text = "-";
        defText.text = "-";
        spdText.text = "-";
        atkText.text = "-";
        atkspdText.text = "-";
        atkareaText.text = "-";
        cri.text = "-";
        cridmg.text = "-";
        projectilespd.text = "-";
        dur.text = "-";
        cd.text = "-";
        projectilenum.text = "-";
    }


    // 실제 스탯 출력
    public void RefreshStats(StatHandler stat, ResouceController resource)
    {
        // 실제 존재하는 스탯만 표시
        hpText.text = $"HP : {resource.CurrentHealth} / {stat.MaxHealth}";
        spdText.text = $"SPD : {stat.Speed}";
        atkText.text = $"ATK : {stat.Attack}";
        atkspdText.text = $"ATK SPD : {stat.AttackSpeed}";

        // StatHandler에 없는 값은 "-" 처리
        hpgenText.text = "-";
        defText.text = "-";
        atkareaText.text = "-";
        cri.text = "-";
        cridmg.text = "-";
        projectilespd.text = "-";
        dur.text = "-";
        cd.text = "-";
        projectilenum.text = "-";
    }

    public void RefreshSkillSlots()
    {
        var ui = UIManager.Instance;

        // 플레이어가 현재 보유 중인 스킬들
        SkillData[] ownedSkills = new SkillData[]
        {
        ui.slotZ.currentSkill,
        ui.slotX.currentSkill,
        ui.slotC.currentSkill
        };

        int index = 0;

        // 1) 실제 사용 스킬들 먼저 채우기
        for (int i = 0; i < ownedSkills.Length; i++)
        {
            if (ownedSkills[i] != null)
            {
                pauseSkillSlots[index].SetSkill(ownedSkills[i]);
                index++;
            }
        }

        // 2) 남은 칸은 전부 빈 슬롯
        for (; index < pauseSkillSlots.Length; index++)
        {
            pauseSkillSlots[index].SetEmpty();
        }
    }
    public void Open()
    {
        if (isOpen) return;

        window.SetActive(true);
        Time.timeScale = 0f;

        // 플레이어 찾기
        var player = FindObjectOfType<BaseController>();
        if (player != null)
        {
            var stat = player.GetComponent<StatHandler>();
            var resource = player.GetComponent<ResouceController>();

            if (stat != null && resource != null)
                RefreshStats(stat, resource);
            else
                RefreshStatsDummy();
        }
        else
            RefreshStatsDummy();

        isOpen = true;

        RefreshSkillSlots();
        RefreshItemSlots();
    }
    private void RefreshItemSlots()
    {
        var equips = EquipmentController.Instance.equippedItems; // List<EquipmentData>
        var weapons = BaseController.Instance.GetActiveWeapons(); // List<WeaponHandler>

        var slots = itemSlotParent.GetComponentsInChildren<TItemSlotUI>();

        int index = 0;

        foreach (var e in equips)
        {
            if (index >= slots.Length) break;

            slots[index].icon.enabled = true;
            slots[index].icon.sprite = e.icon;
            slots[index].levelText.text = "Lv -";

            // 여기서 반드시 data 설정
            slots[index].equipmentData = e;
            slots[index].weaponData = null;

            index++;
        }

        // 2) 무기 채우기
        foreach (var w in weapons)
        {
            if (index >= slots.Length) break;

            // w.weaponData 는 WeaponData
            var data = w.weaponData;

            slots[index].icon.enabled = true;
            slots[index].icon.sprite = data.icon;
            slots[index].levelText.text = "Lv -";

            // 여기서 data 설정
            slots[index].weaponData = data;
            slots[index].equipmentData = null;

            index++;
        }

        // 남는 칸은 비우기
        for (; index < slots.Length; index++)
        {
            slots[index].equipmentData = null;
            slots[index].weaponData = null;
            slots[index].SetEmpty();
        }
    }

    public void Close()
    {
        window.SetActive(false);
        Time.timeScale = 1f;
        isOpen = false;
    }
    
    void OnResumeClicked()
    {
        Close();
    }

    void OnRetryClicked()
    {
        window.SetActive(false);
        isOpen = false;
        Time.timeScale = 1f;
        SceneLoader.Load(SceneType.GameScene);
    }

    void OnMainClicked()
    {
        SceneLoader.Load(SceneType.MainScene);
    }
}
