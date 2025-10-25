using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingMenu : MonoBehaviour
{
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