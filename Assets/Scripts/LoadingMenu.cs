using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingMenu : MonoBehaviour
{
    async void Start()
    {
        if (!Application.isMobilePlatform)
        {
            var width = Display.main.systemWidth;
            var height = Display.main.systemHeight;
            Screen.SetResolution(width, height, BazookaManager.Instance.GetSettingFullScreen());
            QualitySettings.vSyncCount = BazookaManager.Instance.GetSettingVsync() ? 1 : -1;
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