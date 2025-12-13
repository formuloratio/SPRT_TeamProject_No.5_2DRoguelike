using UnityEngine;

public class FoodDrop : MonoBehaviour
{
    public FoodItem data;       // ScriptableObject 데이터 연결
    public float lifeTime = 10f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SoundManager.Instance.PlaySFX(SoundManager.Instance.eatFood);
        var resource = other.GetComponent<ResouceController>();
        if (resource != null)
        {
            resource.ChangeHealth(data.healAmount);   // HP 회복 (정석)
            Destroy(gameObject);
        }
    }
}