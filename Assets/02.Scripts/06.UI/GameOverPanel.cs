using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverPanel : MonoBehaviour
{
    public static GameOverPanel Instance;

    [Header("Window")]
    public GameObject window;   // Window 오브젝트

    [Header("Playtime")]
    public TMP_Text playtimeText;

    [Header("Used Item Groups")]
    public Transform usedWeaponGroup;   // UsedWeaponGroup
    public Transform usedEquipGroup;    // UsedEquipGroup
    public ItemSlotController slotPrefab;

    [Header("Buttons")]
    public Button restartButton;
    public Button MainButton;

    public bool isOpen { get; private set; }

    private void Awake()
    {
        Instance = this;

        // GameOverPanel은 켜져 있지만, 안의 window는 꺼둔다
        window.SetActive(false);

        restartButton.onClick.AddListener(OnRetryClicked);
        MainButton.onClick.AddListener(OnMainClicked);
    }

    /// 게임 오버 패널 열기
    public void Open(string playtime)
    {
        isOpen = true;
        playtimeText.text = playtime;

        // 게임 정지
        Time.timeScale = 0f;

        window.SetActive(true);

        RefreshUsedWeapons();
        RefreshUsedEquip();
    }


    //패널 닫기 (재시작 시 사용)
    public void Close()
    {
        isOpen = false;

        window.SetActive(false);

        // 재시작 시 다시 시간 흐르게
        Time.timeScale = 1f;
    }

    void OnRetryClicked()
    {
        Time.timeScale = 1f;
        SceneLoader.Load(SceneType.GameScene);
    }

    void OnMainClicked()
    {
        Time.timeScale = 1f;
        SceneLoader.Load(SceneType.MainScene);
    }

    private void RefreshUsedWeapons()
    {
        foreach (Transform child in usedWeaponGroup)
            Destroy(child.gameObject);

        var weapons = BaseController.Instance.GetActiveWeapons();

        foreach (var w in weapons)
        {
            var slot = Instantiate(slotPrefab, usedWeaponGroup);
            slot.icon.sprite = w.weaponData.icon;
            slot.levelText.text = "Lv -";
        }
    }

    private void RefreshUsedEquip()
    {
        foreach (Transform child in usedEquipGroup)
            Destroy(child.gameObject);

        var equips = EquipmentController.Instance.equippedItems;

        foreach (var e in equips)
        {
            var slot = Instantiate(slotPrefab, usedEquipGroup);
            slot.icon.sprite = e.icon;
            slot.levelText.text = "Lv -";
        }
    }

}
