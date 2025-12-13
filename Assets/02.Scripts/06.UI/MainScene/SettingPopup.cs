using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : MonoBehaviour
{
    public Button closeButton;

    private void Awake()
    {
        gameObject.SetActive(false);
        closeButton.onClick.AddListener(Close);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
