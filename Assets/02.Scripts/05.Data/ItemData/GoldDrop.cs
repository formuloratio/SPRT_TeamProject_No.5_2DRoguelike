using UnityEngine;

public class GoldDrop : MonoBehaviour
{
    public GoldItem data;  // ScriptableObject 연결
    public float lifeTime = 10f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var gold = other.GetComponent<PlayerGold>();
        if (gold != null)
        {
            gold.AddGold(data.amount);
            Destroy(gameObject);
        }
    }
}