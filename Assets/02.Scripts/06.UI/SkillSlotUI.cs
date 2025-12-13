using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
    public Image skillIcon;
    public TMP_Text skillName;
    public TMP_Text skillDesc;
    public TMP_Text skillOption;
    public Sprite xMark;

    public void SetSlot(SkillData data)
    {
        if (data == null)
        {
            skillIcon.enabled = true;
            skillIcon.sprite = xMark;     // ∫Û ΩΩ∑‘ ¿ÃπÃ¡ˆ

            skillName.text = "";
            skillDesc.text = "";
            skillOption.text = "";
            return;
        }

        skillIcon.enabled = true;
        skillIcon.sprite = data.icon;

        skillName.text = data.skillName;
        skillDesc.text = data.description;

        skillOption.text =
            $"Type: {data.type}\n" +
            $"Cooldown: {data.coolTime}\n" +
            $"Value: {data.value1}\n" +
            $"Duration: {data.duration}";
    }
}
