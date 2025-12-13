using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MultiSlotUI : MonoBehaviour
{
    [Header("Slot Type")]
    //public SlotType slotType; // Skill / Buff / Debuff
    //SlotType 구현 전으로 주석처리(데이터 추가되면 연결)

    [Header("UI")]
    public Image icon;
    public Image cooldownMask;
    public TMP_Text keyText;        // Skill 전용
    public TMP_Text cooldownText;   // 공통 (Cooldown / Duration)

    // 공통 쿨다운 / 지속시간
    private float currentTime;
    private float maxTime;

    public SkillData currentSkill; // HUD 슬롯에 실제 들어간 스킬
    public Sprite emptySprite;

    public void SetSkill(Sprite iconSprite, float cooldown, string key, SkillData data)
    {
        // 현재 스킬 데이터 저장
        currentSkill = data;

        // 아이콘 표시
        icon.sprite = iconSprite;
        icon.enabled = true;

        // 쿨타임 설정
        maxTime = cooldown;
        currentTime = 0f;

        // 키(Z/X/C) 표시
        keyText.text = key;
        keyText.gameObject.SetActive(true);

        // 쿨타임 마스크 초기화
        cooldownMask.fillAmount = 0f;
        cooldownText.gameObject.SetActive(false);
    }


    public void SetBuff(Sprite iconSprite, float duration)
    {
        //slotType = SlotType.Buff;

        icon.sprite = iconSprite;
        icon.enabled = true;

        maxTime = duration;
        currentTime = duration;

        keyText.gameObject.SetActive(false); // Buff는 키 표시 없음

        cooldownMask.fillAmount = 1f;
        cooldownText.text = Mathf.Ceil(duration).ToString();
        cooldownText.gameObject.SetActive(true);

        StartCoroutine(CountDownRoutine());
    }

    public void SetDebuff(Sprite iconSprite, float duration)
    {
        //slotType = SlotType.Debuff;

        icon.sprite = iconSprite;
        icon.enabled = true;

        maxTime = duration;
        currentTime = duration;

        keyText.gameObject.SetActive(false);

        cooldownMask.fillAmount = 1f;
        cooldownText.text = Mathf.Ceil(duration).ToString();
        cooldownText.gameObject.SetActive(true);

        StartCoroutine(CountDownRoutine());
    }

    public void StartCooldown()
    {
        currentTime = maxTime;
        cooldownMask.fillAmount = 1f;
        cooldownText.gameObject.SetActive(true);
        StartCoroutine(CountDownRoutine());
    }

    private IEnumerator CountDownRoutine()
    {
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            float ratio = currentTime / maxTime;

            cooldownMask.fillAmount = ratio;
            cooldownText.text = Mathf.Ceil(currentTime).ToString();
            cooldownText.gameObject.SetActive(true);

            yield return null;
        }

        cooldownMask.fillAmount = 0;
        cooldownText.gameObject.SetActive(false);
    }
    public bool IsEmpty()
    {
        // 아이콘이 null이거나, 아이콘 이미지가 비활성화되어 있으면 empty 취급
        return icon.sprite == null || icon.enabled == false;
    }
    public void SetEmpty()
    {
        icon.enabled = true;
        icon.sprite = emptySprite;

        keyText.text = "";
        cooldownMask.fillAmount = 0f;
        cooldownText.gameObject.SetActive(false);

        currentSkill = null;
    }
}
