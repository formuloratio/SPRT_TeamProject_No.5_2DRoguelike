using UnityEngine;

[CreateAssetMenu(fileName = "FoodItem", menuName = "Items/Food Item")]
public class FoodItem : ScriptableObject
{
    public string itemName = "Food";
    public int healAmount = 10;
}