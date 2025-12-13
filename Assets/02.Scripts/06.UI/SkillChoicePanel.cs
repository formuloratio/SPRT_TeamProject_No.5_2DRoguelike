using System.Collections.Generic;
using TMPro;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillChoicePanel : MonoBehaviour
{
    public static SkillChoicePanel Instance;

    [Header("Card")]
    public SkillCardUI cardPrefab;
    public Transform cardParent;

    private SkillData[] currentOptions;

    [Header("Owned Item Slots")]
    public TItemSlotUI slotPrefab;

    public Transform weaponSlotParent;   // OwnedWeaponGroup
    public Transform equipSlotParent;    // OwnedEquipGroup

    [Header("캐릭터 스탯")]
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

    [Header("스킬 슬롯")]
    public SkillChoiceSlotUI slotZ;
    public SkillChoiceSlotUI slotX;
    public SkillChoiceSlotUI slotC;
    
    [Header("그 외")]
    public GameObject window;
    public bool isOpen { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        window.SetActive(false);
    }

    // 패널 열기
    public void Open(SkillData[] datas)
    {
        isOpen = true;

        window.SetActive(true);
        Time.timeScale = 0f;

        CreateCards(datas);

        slotZ.SetEmpty();
        slotX.SetEmpty();
        slotC.SetEmpty();

        RefreshStats();
        CreateEmptyWeaponSlots();
        CreateEmptyEquipSlots();

        RefreshEquipSlots(EquipmentController.Instance.equippedItems);
        RefreshWeaponSlots(BaseController.Instance.GetActiveWeapons());
    }

    // 카드 생성
    private void CreateCards(SkillData[] datas)
    {
        foreach (Transform child in cardParent)
            Destroy(child.gameObject);

        foreach (var data in datas)
        {
            var card = Instantiate(cardPrefab, cardParent);
            card.SetCard(data);
        }
    }

    // 카드 선택
    public void SelectCard(SkillData data)
    {
        // 궁극기
        if (data.type == SkillsType.Ultimate)
        {
            if (slotC.IsEmpty())
            {
                slotC.SetSkill(data);
                UIManager.Instance.SetSkillToHUD(data);
            }
            else
                Debug.Log("궁극기 슬롯이 이미 찼습니다!");

            ClosePanel();
            return;
        }

        GameManager.Instance.AddOwnedNormalSkill(data);

        // 일반 스킬 Z → X
        if (slotZ.IsEmpty())
        {
            slotZ.SetSkill(data);
            UIManager.Instance.SetSkillToHUD(data);
        }
        else if (slotX.IsEmpty())
        {
            slotX.SetSkill(data);
            UIManager.Instance.SetSkillToHUD(data);
        }
        else
            Debug.Log("일반 스킬 슬롯 두 개가 이미 찼습니다!");

        ClosePanel();
    }

    private void ClosePanel()
    {
        isOpen = false;

        window.SetActive(false);
        Time.timeScale = 1f;
    }
    private void RefreshStats()
    {
        var player = GameObject.FindWithTag("Player");
        if (player == null) return;

        var stat = player.GetComponent<StatHandler>();
        var resource = player.GetComponent<ResouceController>();

        hpText.text = $"HP : {(int)resource.CurrentHealth} / {stat.MaxHealth}";
        spdText.text = $"SPD : {stat.Speed}";
        atkText.text = $"ATK : {stat.Attack}";
        atkspdText.text = $"ATK SPD : {stat.AttackSpeed}";

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
    private void CreateEmptyWeaponSlots()
    {
        foreach (Transform child in weaponSlotParent)
            Destroy(child.gameObject);

        for (int i = 0; i < 6; i++)
        {
            var slot = Instantiate(slotPrefab, weaponSlotParent);
            slot.SetEmpty();
        }
    }

    private void CreateEmptyEquipSlots()
    {
        foreach (Transform child in equipSlotParent)
            Destroy(child.gameObject);

        for (int i = 0; i < 6; i++)
        {
            var slot = Instantiate(slotPrefab, equipSlotParent);
            slot.SetEmpty();
        }
    }
    private void RefreshEquipSlots(List<EquipmentData> items)
    {
        var slots = equipSlotParent.GetComponentsInChildren<TItemSlotUI>();

        for (int i = 0; i < slots.Length; i++)
        {
            if (items != null && i < items.Count && items[i] != null)
            {
                slots[i].icon.enabled = true;
                slots[i].icon.sprite = items[i].icon;
                slots[i].levelText.text = "Lv -";
            }
            else
            {
                slots[i].SetEmpty();
            }
        }
    }
    private void RefreshWeaponSlots(List<WeaponHandler> weapons)
    {
        var slots = weaponSlotParent.GetComponentsInChildren<TItemSlotUI>();

        for (int i = 0; i < slots.Length; i++)
        {
            if (weapons != null && i < weapons.Count && weapons[i] != null)
            {
                slots[i].icon.enabled = true;
                slots[i].icon.sprite = weapons[i].weaponData.icon;
                slots[i].levelText.text = "Lv -";
            }
            else
            {
                slots[i].SetEmpty();
            }
        }
    }

}
