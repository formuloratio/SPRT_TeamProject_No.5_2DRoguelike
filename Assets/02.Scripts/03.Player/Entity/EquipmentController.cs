using System.Collections.Generic;
using UnityEngine;

public class EquipmentController : MonoBehaviour
{
    public static EquipmentController Instance { get; private set; } //UI연결문제로 추가

    private StatHandler statHandler;

    // 획득한 장비 아이템들을 저장할 리스트
    public List<EquipmentData> equippedItems = new List<EquipmentData>();


    [SerializeField] private EquipmentData itemData; //테스트용: 장비 아이템 할당

    private void Awake()
    {
        Instance = this; //UI연결문제로 추가
        statHandler = GetComponent<StatHandler>();
    }

    private void Start()
    {
        //// 테스트용: 게임 시작 시 아이템 1개 장착
        //if (itemData != null)
        //{
        //    EquipItem(itemData);
        //}
    }

    // 아이템 획득 시 호출할 메서드
    public void EquipItem(EquipmentData data)
    {
        if (data == null) return;
        equippedItems.Add(data);

        Debug.Log($"아이템 습득 및 저장 완료: {data.itemName}");

        foreach (var modifier in data.modifiers)
        {
            ApplyStat(modifier);
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.RefreshEquipmentSlots(equippedItems);
        }
    }

    private void ApplyStat(StatModifier modifier)
    {
        switch (modifier.type)
        {
            case StatType.MaxHealth:
                statHandler.AddMaxHealth((int)modifier.value);
                break;

            case StatType.Attack:
                statHandler.AddAttack(modifier.value);
                break;

            case StatType.Defense:
                statHandler.AddDefense((int)modifier.value);
                break;

            case StatType.MoveSpeedPercent:
                // 10% 증가는 0.1f로 데이터에 입력됨
                statHandler.AddSpeedPercent(modifier.value);
                break;

            case StatType.AttackSpeedPercent:
                statHandler.AddAttackSpeedPercent(modifier.value);
                break;

            case StatType.CooldownReduction:
                statHandler.AddCooldownReduction(modifier.value);
                break;
        }
    }

    // 외부(UI 등)에서 현재 장착 중인 아이템 목록을 가져올 때 사용
    public List<EquipmentData> GetEquippedItems()
    {
        return equippedItems;
    }
}
