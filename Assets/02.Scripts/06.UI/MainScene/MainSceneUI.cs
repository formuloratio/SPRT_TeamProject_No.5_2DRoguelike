using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainSceneUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button btnStart;
    public Button btnUpgrade;
    public Button btnBook;
    public Button btnSettings;
    public Button btnQuit;

    [Header("setting")]
    public GameObject settingsPopup;

    [Header("Setting Sliders")]
    public Slider sliderBGM;
    public Slider sliderSFX;

    private void Start()
    {
        btnStart.onClick.AddListener(OnClickStart);
        btnUpgrade.onClick.AddListener(OnClickUpgrade);
        btnBook.onClick.AddListener(OnClickBook);
        btnSettings.onClick.AddListener(OnClickSettings);
        btnQuit.onClick.AddListener(OnClickQuit);

        sliderBGM.value = SoundManager.Instance.bgmVolume;
        sliderSFX.value = SoundManager.Instance.sfxVolume;

        sliderBGM.onValueChanged.AddListener(OnChangeBGM);
        sliderSFX.onValueChanged.AddListener(OnChangeSFX);
    }

    private void OnChangeBGM(float value)
    {
        SoundManager.Instance.bgmVolume = value;
    }

    private void OnChangeSFX(float value)
    {
        SoundManager.Instance.sfxVolume = value;
    }

    private void OnClickStart()
    {
        SceneLoader.Load(SceneType.CharacterSelectScene);
    }

    private void OnClickUpgrade()
    {
        Debug.Log("���׷��̵�(������) - ���� ��� ����");
    }

    private void OnClickBook()
    {
        Debug.Log("����(������) - ���� ��� ����");
    }

    private void OnClickSettings()
    {
        settingsPopup.SetActive(true);
    }

    private void OnClickQuit()
    {
        Application.Quit();
        Debug.Log("���� ����");
    }
}
