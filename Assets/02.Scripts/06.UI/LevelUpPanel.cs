using System.Collections.Generic;
using TMPro;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpPanel : MonoBehaviour
{
    public static LevelUpPanel Instance;

    [Header("Card")]
    public OptionCardUI cardPrefab;
    public Transform cardParent;

    [Header("Buttons")]
    public Button rerollButton;
    public Button deleteButton;   // Delete 버튼 (UI만 존재)

    private LevelUpOptionData[] currentOptions;

    [Header("Owned Item Slots")]
    public TItemSlotUI slotPrefab;


    public Transform weaponSlotParent;   // OwnedWeaponGroup
    public Transform equipSlotParent;    // OwnedEquipGroup

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

    public bool isOpen { get; private set; }
    public GameObject window;

    private void Awake()
    {
        Instance = this;

        //gameObject.SetActive(false);

        rerollButton.interactable = false;

        // Delete 버튼은 이번 프로젝트에서 비활성화
        deleteButton.interactable = false;
    }

    private void Start()
    {
        // TestOpen();
        window.SetActive(false);
    }

    // 패널 열기
    public void Open(object[] options)
    {
        isOpen = true;
        window.SetActive(true);
        Time.timeScale = 0f;

        CreateCards(options);
        RefreshStats();

        CreateEmptyEquipSlots();
        CreateEmptyWeaponSlots();

        RefreshEquipSlots(EquipmentController.Instance.equippedItems);
        RefreshWeaponSlots(BaseController.Instance.GetActiveWeapons());
    }
    // 카드 생성
    private void CreateCards(object[] options)
    {
        foreach (Transform child in cardParent)
            Destroy(child.gameObject);

        foreach (var obj in options)
        {
            var card = Instantiate(cardPrefab, cardParent);

            if (obj is EquipmentData e)
                card.SetCard(e);
            else if (obj is WeaponData w)
                card.SetCard(w);
        }
    }
    // 카드 선택
    public void SelectEquipment(EquipmentData data)
    {
        EquipmentController.Instance.EquipItem(data);

        RefreshEquipSlots(EquipmentController.Instance.equippedItems);
        RefreshWeaponSlots(BaseController.Instance.GetActiveWeapons());

        ClosePanel();
    }
    public void SelectWeapon(WeaponData data)
    {
        BaseController.Instance.EquipWeapon(data);

        RefreshEquipSlots(EquipmentController.Instance.equippedItems);
        RefreshWeaponSlots(BaseController.Instance.GetActiveWeapons());

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
    // 오른쪽 OwnedEquipGroup 갱신
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
            if (i < weapons.Count && weapons[i] != null)
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
