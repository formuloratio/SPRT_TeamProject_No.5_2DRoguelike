using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatHandler : MonoBehaviour
{
    [Range(1, 100)][SerializeField] private int health = 10;
    public int Health
    {
        get => health;
        set => health = Mathf.Clamp(value, 0, 100);
    }

    //[Range(1f, 20f)][SerializeField] private float speed = 3;
    //public float Speed
    //{
    //    get => speed;
    //    set => speed = Mathf.Clamp(value, 0, 20);
    //}

    [SerializeField] private CharacterData characterData; // 캐릭터 데이터 연결

    // 실제 게임에서 변동되는 스탯들
    public int CurrentHealth { get; set; } // 현재 체력
    public int MaxHealth { get; set; }
    public float Speed { get; set; }
    public float Attack { get; set; }
    public int Defense { get; set; } // 방어력 추가
    public float AttackSpeed { get; set; } // 공격 속도 (기본 1.0f 기준, 높을수록 빠름 혹은 딜레이 감소)

    // 쿨타임 감소율 (0.0f = 0%, 0.1f = 10% 감소)
    public float CooldownReduction { get; set; } = 0.0f;

    //골드 획득량 증가 배율 (1.0f = 100%, 1.2f = 120%)
    public float GoldBonusMultiplier { get; set; } = 1.0f; // 1.0 = 100%

    // 패시브 효과용 플래그
    public bool HasExplosiveProjectile { get; set; } = false;
    public float ExplosiveChance { get; set; } = 0f;

    private ResouceController resouceController;

    private void Awake()
    {
        if (characterData != null)
        {
            InitializeStats();
        }

        resouceController = GetComponent<ResouceController>();
    }

    private void InitializeStats()
    {
        MaxHealth = characterData.baseHealth;
        CurrentHealth = MaxHealth;
        Speed = characterData.baseSpeed;
        Attack = characterData.baseAttack;
        AttackSpeed = characterData.baseAttackSpeed; // 예: 1.0f
        Defense = 0; // 기본 방어력
        CooldownReduction = 0f;
    }

    // 아이템 습득 시 스탯 업데이트용 메서드
    public void AddMaxHealth(int amount)
    {
        MaxHealth = resouceController.MaxHealth;
        CurrentHealth = resouceController.CurrentHealth;
        MaxHealth += amount;
        CurrentHealth += amount; // 최대 체력이 늘어나면 현재 체력도 같이 채워줌 (선택사항)

        UIManager.Instance.UpdateHP(CurrentHealth, MaxHealth);
    }

    public void AddAttack(float amount) => Attack += amount;
    public void AddDefense(int amount) => Defense += amount;

    // 퍼센트 증가 로직 (기본값에 곱할지, 합연산할지 정책에 따라 다름. 여기선 현재 값에 곱연산 적용)
    public void AddSpeedPercent(float percent) => Speed *= (1f + percent); // 0.1f = 10% 증가
    public void AddAttackSpeedPercent(float percent) => AttackSpeed *= (1f + percent);
    public void AddCooldownReduction(float percent) => CooldownReduction += percent; // 쿨감은 합연산 (예: 10% + 10% = 20%)
}
