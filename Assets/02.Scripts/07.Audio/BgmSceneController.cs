using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMSceneController : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(Scene previous, Scene current)
    {
        string name = current.name;

        if (name == "MainScene")
            SoundManager.Instance.PlayBGM(SoundManager.Instance.mainBgm);

        else if (name == "SelectScene")
            SoundManager.Instance.PlayBGM(SoundManager.Instance.CrtselectBgm);

        else if (name == "GameScene")
            SoundManager.Instance.PlayBGM(SoundManager.Instance.gameBgm);
    }
}