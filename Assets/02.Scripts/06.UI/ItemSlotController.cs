using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotController : MonoBehaviour
{
    //[Header("Slot Settings")]
    //public SlotType slotType; // Weapon, Equipment
    //public Image icon;
    //public TMP_Text levelText;

    //private ItemData currentItem;    
    public Image icon;
    public TMP_Text levelText;
    public Sprite emptySprite;

    public void SetEmpty()
    {
        icon.enabled = true;               // 아이콘은 항상 켬
        icon.sprite = emptySprite;         // 빈 슬롯 이미지
        levelText.text = "Lv -";
    }

    //public void SetItem(ItemData item)
    //{
    //    // 타입 검사
    //    if (slotType == SlotType.Weapon && item.itemType != ItemType.Weapon)
    //    {
    //        Debug.Log($"이 슬롯은 무기 전용입니다: {item.itemName} 불가");
    //        return;
    //    }

    //    if (slotType == SlotType.Equipment && item.itemType != ItemType.Equipment)
    //    {
    //        Debug.Log($"이 슬롯은 장비 전용입니다: {item.itemName} 불가");
    //        return;
    //    }

    //    currentItem = item;

    //    // 아이콘 표시
    //    icon.sprite = item.icon;
    //    icon.enabled = true;

    //    // 레벨 표시
    //    SetLevel(level);
    //}

    //public void SetLevel(int level)
    //{
    //    levelText.text = $"Lv {level}";
    //}

    //public void Clear()
    //{
    //    currentItem = null;
    //    icon.sprite = null;
    //    icon.enabled = false;
    //} //아이템 타입 데이터 준비되어 있어야함
}
