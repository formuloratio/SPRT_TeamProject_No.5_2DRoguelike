using UnityEngine;

[CreateAssetMenu(fileName = "GoldItem", menuName = "Items/Gold Item")]
public class GoldItem : ScriptableObject
{
    public string itemName = "Gold";
    public int amount = 10;   //  골드량
}