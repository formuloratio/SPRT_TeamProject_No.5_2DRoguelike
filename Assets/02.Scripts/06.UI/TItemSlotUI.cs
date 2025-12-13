using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TItemSlotUI : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    public TMP_Text levelText;
    public Sprite emptySprite;
    public EquipmentData equipmentData;  // 장비용
    public WeaponData weaponData;        // 무기용

    public void SetEmpty()
    {
        icon.enabled = true;            // 끄지 않음
        icon.sprite = emptySprite;      // empty 슬롯 이미지
        levelText.text = "Lv -";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ItemTooltipUI.Instance == null)
            return;

        // 장비가 들어있다면 장비 툴팁
        if (equipmentData != null)
        {
            ItemTooltipUI.Instance.ShowEquipment(equipmentData);
            return;
        }

        // 무기가 들어있다면 무기 툴팁
        if (weaponData != null)
        {
            ItemTooltipUI.Instance.ShowWeapon(weaponData);
            return;
        }

        // 둘 다 없으면 빈 슬롯
        ItemTooltipUI.Instance.ShowEmpty();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer EXIT on SLOT");

        if (ItemTooltipUI.Instance != null)
            ItemTooltipUI.Instance.Hide();
    }
}
