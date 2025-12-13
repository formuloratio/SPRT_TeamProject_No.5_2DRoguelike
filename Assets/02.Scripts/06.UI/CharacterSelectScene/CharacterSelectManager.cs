using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterSelectManager : MonoBehaviour
{
    public static CharacterData selectedCharacter;

    [Header("=== Character Data (SO) ===")]
    public CharacterData[] characterDataList;

    private int currentIndex = 0;

    [Header("=== UI References ===")]
    public Image characterIllust;
    public TMP_Text characterNameText;
    public TMP_Text paramText;

    [Header("Weapon")]
    public TMP_Text weaponNameText;
    public Image weaponIcon;
    public TMP_Text weaponDescText;

    [Header("Skill Slots (8개)")]
    public SkillSlotUI[] skillSlots; // 8칸

    [Header("X 아이콘(비활성 슬롯용)")]
    public Sprite xMark;  // 공통 2칸 + 궁극기 1칸에 들어갈 X모양 아이콘

    [Header("캐릭터 셀렉트 슬롯 18칸")]
    public Image[] characterSlots;
    public Sprite XMark;




    void Start()
    {
        UpdateUI(0);
    }

    public void UpdateUI(int index)
    {
        currentIndex = index;
        CharacterData data = characterDataList[index];

        // 기본 정보
        characterNameText.text = data.characterName;
        characterIllust.sprite = data.characterSprite;

        // 파라미터
        paramText.text =
            $"HP: {data.baseHealth}\n" +
            $"ATK: {data.baseAttack}\n" +
            $"SPD: {data.baseSpeed}\n" +
            $"ASPD: {data.baseAttackSpeed}";

        // 무기 이름
        if (data.uniqueWeapon != null)
            weaponNameText.text = data.uniqueWeapon.weaponName;
        else
            weaponNameText.text = data.weaponName;
        // 무기 아이콘
        if (data.uniqueWeapon != null)
        {
            weaponIcon.sprite = data.uniqueWeapon.icon;
            weaponIcon.color = Color.white;
        }
        // 무기 텍스트
        if (data.uniqueWeapon != null)
            weaponDescText.text = data.uniqueWeapon.description;

        // 스킬 슬롯 세팅
        ApplySkillSlots(data);
        ApplyCharacterSlots(data);
    }

    private void ApplySkillSlots(CharacterData data)
    {
        SkillData[] skills = new SkillData[8];

        // 0,4번 : 액티브
        skills[0] = data.active1;
        skills[4] = data.active2;

        // 1,5번 : 패시브
        skills[1] = data.passive1;
        skills[5] = data.passive2;

        // 3번 : 궁극기
        skills[3] = data.ultimate;

        // 나머지(2,6,7번)는 X박스용 → null로 둠
        skills[2] = null;
        skills[6] = null;
        skills[7] = null;

        // 슬롯에 데이터 적용
        for (int i = 0; i < 8; i++)
        {
            skillSlots[i].SetSlot(skills[i]);   // null이면 SkillSlotUI에서 X 처리
        }

        Debug.Log($"0번 = {skills[0]?.skillName}, 4번 = {skills[4]?.skillName}");
    }

    private void ApplyCharacterSlots(CharacterData data)
    {
        // 0번 슬롯: 캐릭터 대표 아이콘
        if (characterSlots.Length > 0)
        {
            characterSlots[0].sprite = data.hudSprite;
            characterSlots[0].color = Color.white;
        }

        // 나머지 슬롯은 X박스 처리
        for (int i = 1; i < characterSlots.Length; i++)  // ★★ i = 1부터 시작
        {
            characterSlots[i].sprite = XMark;
            characterSlots[i].color = Color.white;
        }
    }
    public void OnClickSlot(int index)
    {
        UpdateUI(index);
    }

    public void OnSelectComplete()
    {
        selectedCharacter = characterDataList[currentIndex]; // 선택 데이터 저장
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void OnBack()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }
}
