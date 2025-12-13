using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossInfoUI : MonoBehaviour
{
    [SerializeField] private Image currentHpBar;
    [SerializeField] private TextMeshProUGUI bossNameText;
    [SerializeField] private TextMeshProUGUI bossHpText;

    public void SetBossInfo(Enemy boss)
    {
        if (boss != null)
        {
            currentHpBar.fillAmount = (boss.CurrentHealth / boss.EnemyData.maxHealth);
            bossNameText.text = boss.EnemyData.enemyName;
            bossHpText.text = $"{boss.CurrentHealth} / {boss.EnemyData.maxHealth}";
        }
    }
}
