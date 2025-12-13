using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemTooltipUI : MonoBehaviour
{
    public static ItemTooltipUI Instance;

    [Header("UI Elements")]
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text descText;

    [Header("Default Empty Text")]
    public string emptyName = "빈 슬롯";
    public string emptyDesc = "아직 아이템이 없습니다.";

    [Header("Offset from mouse")]
    public Vector2 offset = new Vector2(20f, -20f);

    private Canvas canvas;
    private RectTransform canvasRect;
    private RectTransform selfRect;

    private void Awake()
    {
        // 그냥 씬에 있는 이 오브젝트 하나만 싱글톤사용
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        selfRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();

        // 툴팁은 Raycast 안 받게
        foreach (var g in GetComponentsInChildren<Graphic>())
            g.raycastTarget = false;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void ShowEmpty()
    {
        // 디버그용
        Debug.Log("SHOW EMPTY CALLED");

        if (iconImage != null)
            iconImage.enabled = false;

        if (nameText != null) nameText.text = emptyName;
        if (descText != null) descText.text = emptyDesc;

        gameObject.SetActive(true);
        UpdatePosition();
    }
    public void ShowSkill(SkillData skill)
    {
        if (iconImage != null)
        {
            iconImage.enabled = skill.icon != null;
            iconImage.sprite = skill.icon;
        }

        if (nameText != null)
            nameText.text = skill.skillName;

        if (descText != null)
            descText.text = skill.description;

        gameObject.SetActive(true);
        UpdatePosition();
    }
    public void ShowEquipment(EquipmentData data)
    {
        if (data == null)
        {
            ShowEmpty();
            return;
        }

        if (iconImage != null)
        {
            iconImage.enabled = data.icon != null;
            iconImage.sprite = data.icon;
        }

        if (nameText != null)
            nameText.text = data.itemName;

        if (descText != null)
            descText.text = data.description;

        gameObject.SetActive(true);
        UpdatePosition();
    }

    public void ShowWeapon(WeaponData data)
    {
        if (data == null)
        {
            ShowEmpty();
            return;
        }

        if (iconImage != null)
        {
            iconImage.enabled = data.icon != null;
            iconImage.sprite = data.icon;
        }

        if (nameText != null)
            nameText.text = data.weaponName;

        if (descText != null)
            descText.text = data.description;

        gameObject.SetActive(true);
        UpdatePosition();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gameObject.activeSelf)
            UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (canvas == null) return;

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPos
        );

        localPos += offset;

        Vector2 size = selfRect.sizeDelta;

        float minX = -canvasRect.sizeDelta.x / 2f;
        float maxX = (canvasRect.sizeDelta.x / 2f) - size.x;
        float minY = (-canvasRect.sizeDelta.y / 2f) + size.y;
        float maxY = canvasRect.sizeDelta.y / 2f;

        localPos.x = Mathf.Clamp(localPos.x, minX, maxX);
        localPos.y = Mathf.Clamp(localPos.y, minY, maxY);

        selfRect.anchoredPosition = localPos;
    }
}
