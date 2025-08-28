using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingMenu : MonoBehaviour
{
    public TMP_Text text;
    public Button updateButton;
    public Button continueButton;

    async void Start()
    {
        QualitySettings.vSyncCount = BazookaManager.Instance.GetSettingVsync() ? 1 : -1;
        if (!Application.isMobilePlatform)
        {
            Screen.fullScreen = BazookaManager.Instance.GetSettingFullScreen();
        }
        else
        {
            Application.targetFrameRate = 360;
            QualitySettings.vSyncCount = 0;
        }
        PlayerPrefs.SetString("latestVersion", Application.version);
        await SceneManager.LoadSceneAsync("MainMenu");
    }
}