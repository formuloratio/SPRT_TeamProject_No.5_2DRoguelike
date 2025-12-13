using UnityEngine;
using TMPro;

public class DamagePopupManager : MonoBehaviour
{
    public static DamagePopupManager Instance;

    [SerializeField] private GameObject damagePopupPrefab;

    private void Awake()
    {
        Instance = this;
    }

    //데미지를 입힐 때 호출할 함수
    //worldPos = 데미지 뜰 월드 좌표
    //damage = 데미지 수치
    public void ShowDamage(Vector3 worldPos, int damage)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        GameObject popupObj = Instantiate(damagePopupPrefab, transform);

        popupObj.transform.position = screenPos;

        TMP_Text text = popupObj.GetComponentInChildren<TMP_Text>();
        text.text = damage.ToString();

        //1초뒤삭제
        Destroy(popupObj, 1f);
    }
}
