using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Character")]
    public Image characterIllust;

    [Header("HUD")]
    public Slider hpBar;
    public Slider expBar;
    public TMP_Text timerText;

    [Header("HUD Slots")]
    public Transform weaponSlotParent;
    public Transform equipmentSlotParent;

    public GameObject weaponSlotPrefab;
    public GameObject equipmentSlotPrefab;

    [Header("HUD Text")]
    public TMP_Text hpText;
    public TMP_Text expText;

    [Header("Panels")]
    public LevelUpPanel levelUpPanel;
    public SkillChoicePanel skillChoicePanel;
    public PausePanel pausePanel;
    public GameOverPanel gameOverPanel;

    [Header("Skill / Dash Slot")]
    public MultiSlotUI dashSlot;
    public MultiSlotUI slotZ;
    public MultiSlotUI slotX;
    public MultiSlotUI slotC;
    
    [Header("player Mini StatusBar")]
    public PlayerStatusBar playerStatusBar;

    [Header("Ultimate Cut-in")]
    public Image ultimateCutInImage;
    public float ultimateCutInDuration = 0.5f;

    [Header("Gold")]
    public TMP_Text goldText;

    [Header("Gold")]
    public TMP_Text LV_Text;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    private void Start()
    {
        Debug.Log("Selected Character = " + CharacterSelectManager.selectedCharacter);

        if (CharacterSelectManager.selectedCharacter != null)
        {
            characterIllust.sprite = CharacterSelectManager.selectedCharacter.hudSprite;
        }
        CreateSlots(weaponSlotParent, weaponSlotPrefab, 6);
        CreateSlots(equipmentSlotParent, equipmentSlotPrefab, 6);

        // 대쉬 슬롯 초기 세팅 추가
        var player = FindObjectOfType<BaseController>();
        if (player != null)
        {
            dashSlot.SetSkill(dashSlot.icon.sprite, GetDashCooldown(player), "Shift", null);
        }

        if (CharacterSelectManager.selectedCharacter != null)
        {
            characterIllust.sprite = CharacterSelectManager.selectedCharacter.hudSprite;
            characterIllust.color = Color.white;
        }
    }
    private void Update()
    {
        HandleSkillInput();


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (SkillChoicePanel.Instance != null &&
                    SkillChoicePanel.Instance.isOpen)
                    return;

                if (LevelUpPanel.Instance != null &&
                    LevelUpPanel.Instance.isOpen)
                    return;

                TogglePause();
            }
        }
    }
    private void HandleSkillInput()
    {
        // Z
        if (Input.GetKeyDown(KeyCode.Z))
        {
            TryUseSkill(slotZ);
        }

        // X
        if (Input.GetKeyDown(KeyCode.X))
        {
            TryUseSkill(slotX);
        }

        // C (궁극기)
        if (Input.GetKeyDown(KeyCode.C))
        {
            TryUseSkill(slotC);
        }
    }

    private void TryUseSkill(MultiSlotUI slot)
    {
        if (slot == null) return;
        if (slot.currentSkill == null) return;

        bool cooling = slot.cooldownMask.fillAmount > 0;
        if (cooling) return;

        // PlayerSkillController에 직접 전달
        PlayerSkillController.Instance.UseSkillFromHUD(slot.currentSkill);

        // HUD 쿨다운 시작
        slot.StartCooldown();
    }
    public void PlayUltimateCutIn()
    {
        if (ultimateCutInImage == null)
            return;

        StartCoroutine(UltimateCutInRoutine());
    }
    // 얼티밋컷씬
    private IEnumerator UltimateCutInRoutine()
    {
        // 켜고
        ultimateCutInImage.gameObject.SetActive(true);

        // 0.5초(혹은 설정한 duration) 기다렸다가
        yield return new WaitForSeconds(ultimateCutInDuration);

        // 끄기
        ultimateCutInImage.gameObject.SetActive(false);
    }
    private void CreateSlots(Transform parent, GameObject prefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(prefab, parent);
        }
    }
    public void UpdateHP(float current, float max)
    {
        if (hpBar == null) return;
        hpBar.value = current / max;

        if (hpText != null)
            hpText.text = $"{(int)current} / {(int)max}";

        //minibar
        if (playerStatusBar != null)
            playerStatusBar.SetHp(current, max);
    }
    public void UpdateEXP(float current, float max)
    {
        if (expBar == null) return;
        expBar.value = current / max;

        if (expText != null)
        {
            float percent = (current / max) * 100f;
            expText.text = $"{percent:0}%";
        }

        // minibar
        if (playerStatusBar != null)
            playerStatusBar.SetExp(current, max);
    }
    public void UpdateTimer(float time)
    {
        int min = (int)(time / 60);
        int sec = (int)(time % 60);
        timerText.text = $"{min:00}:{sec:00}";
    }
    public void OpenLevelUp(object[] options)
    {
        LevelUpPanel.Instance.Open(options);
    }
    public void OpenSkillChoice(SkillData[] datas)
    {
        skillChoicePanel.Open(datas);
    }
    public void TogglePause()
    {
        if (pausePanel.isOpen)
            pausePanel.Close();
        else
            pausePanel.Open();
    }
    public void OpenGameOver(string playtime)
    {
        gameOverPanel.Open(playtime);
    }
    public void OnDashUsed()
    {
        dashSlot.StartCooldown();
    }
    float GetDashCooldown(BaseController player)
    {
        return player.GetDashCooldown();
    }
    public void SetSkillToHUD(SkillData data)
    {
        MultiSlotUI target = null;
        string key = "";

        // 어떤 슬롯인지 판단
        switch (data.type)
        {
            case SkillsType.Active:
            case SkillsType.Passive:
                if (slotZ.currentSkill == null)
                {
                    target = slotZ;
                    key = "Z";
                }
                else if (slotX.currentSkill == null)
                {
                    target = slotX;
                    key = "X";
                }
                break;

            case SkillsType.Ultimate:
                if (slotC.currentSkill == null)
                {
                    target = slotC;
                    key = "C";
                }
                break;
        }

        if (target == null)
        {
            Debug.Log("HUD 슬롯이 이미 가득 차 있습니다.");
            return;
        }

        // HUD 슬롯에 스킬 데이터 + 아이콘 + 쿨타임 + 키 전달
        target.SetSkill(data.icon,data.coolTime,key,data);
        PlayerSkillController.Instance.SetSkillFromHUD(key, data);
    }
    //골드 업데이트
    public void UpdateGold(int gold)
    {
        if (goldText != null)
            goldText.text = $"G: {gold}";
    }
    //레벨 업데이트
    public void UpdateLevel(int level)
    {
        if (LV_Text != null)
            LV_Text.text = $"Lv {level}";
    }
    public void RefreshEquipmentSlots(List<EquipmentData> items)
    {
        var slots = equipmentSlotParent.GetComponentsInChildren<ItemSlotController>();

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
    public void RefreshWeaponSlots(List<WeaponHandler> weapons)
    {
        var slots = weaponSlotParent.GetComponentsInChildren<ItemSlotController>();

        for (int i = 0; i < slots.Length; i++)
        {
            if (weapons != null && i < weapons.Count && weapons[i] != null)
            {
                // WeaponData에 아이콘이 있다고 가정
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
    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
