using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusBar : MonoBehaviour
{
    [Header("HP UI")]
    public Image hpFillImage;

    [Header("EXP UI")]
    public Image expFillImage;

    public void SetHp(float current, float max)
    {
        if (hpFillImage == null || max <= 0f) return;
        hpFillImage.fillAmount = Mathf.Clamp01(current / max);
    }

    public void SetExp(float current, float max)
    {
        if (expFillImage == null || max <= 0f) return;
        expFillImage.fillAmount = Mathf.Clamp01(current / max);
    }
}
