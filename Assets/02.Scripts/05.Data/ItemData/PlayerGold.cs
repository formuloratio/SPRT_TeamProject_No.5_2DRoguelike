using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    public int gold = 0;

    public void AddGold(int amount)
    {
        gold += amount;

        UIManager.Instance.UpdateGold(gold);

        Debug.Log("골드 획득: " + amount + " | 현재 골드: " + gold);
    }
}