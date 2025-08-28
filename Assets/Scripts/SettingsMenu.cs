using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Toggle setting1toggle;
    public Toggle setting2toggle;
    public Toggle setting3toggle;
    public Toggle setting4toggle;
    public Slider musicSlider;
    public Slider sfxSlider;
    public ColorPanel bgColorPanel;
    public Button bgPreviewModePanel;
    public GameObject settingsUI;

    private void Start()
    {
        bgColorPanel.Init(BazookaManager.Instance.GetColorSettingBackground(), new Color(58 / 255f, 58 / 255f, 58 / 255f));
        bgColorPanel.OnColorChanged += color =>
        {
            BazookaManager.Instance.SetColorSettingBackground(color);
            if (!settingsUI.activeSelf) Camera.main.backgroundColor = new Color((int)color[0] / 255f, (int)color[1] / 255f, (int)color[2] / 255f);
        };
        bgPreviewModePanel.onClick.AddListener(() =>
        {
            settingsUI.SetActive(!settingsUI.activeSelf);
            var value = BazookaManager.Instance.GetColorSettingBackground();
            Camera.main.backgroundColor = !settingsUI.activeSelf ? new Color((int)value[0] / 255f, (int)value[1] / 255f, (int)value[2] / 255f) : new Color(24 / 255f, 24 / 255f, 24 / 255f);
            bgPreviewModePanel.transform.GetChild(0).GetComponent<TMP_Text>().text = settingsUI.activeSelf ? "Preview On" : "Preview Off";
        });

        musicSlider.value = BazookaManager.Instance.GetSettingMusicVolume();
        sfxSlider.value = BazookaManager.Instance.GetSettingSFXVolume();
        if (!Application.isMobilePlatform)
        {
            setting1toggle.isOn = BazookaManager.Instance.GetSettingFullScreen() == true;
            setting2toggle.isOn = BazookaManager.Instance.GetSettingShowFPS() == true;
            setting3toggle.isOn = BazookaManager.Instance.GetSettingVsync() == true;
            setting4toggle.isOn = BazookaManager.Instance.GetSettingHideSocials() == true;
        }
        else
        {
            setting1toggle.interactable = false;
            setting2toggle.isOn = BazookaManager.Instance.GetSettingShowFPS() == true;
            setting3toggle.interactable = false;
            setting4toggle.isOn = BazookaManager.Instance.GetSettingHideSocials() == true;
        }
        setting1toggle.onValueChanged.AddListener(value =>
        {
            BazookaManager.Instance.SetSettingFullScreen(value);
            var width = Display.main.systemWidth;
            var height = Display.main.systemHeight;
            Screen.SetResolution(width, height, value);
        });
        setting2toggle.onValueChanged.AddListener(value =>
        {
            BazookaManager.Instance.SetSettingShowFPS(value);
        });
        setting3toggle.onValueChanged.AddListener(value =>
        {
            BazookaManager.Instance.SetSettingVsync(value);
            QualitySettings.vSyncCount = value ? 1 : -1;
        });
        setting4toggle.onValueChanged.AddListener(value => BazookaManager.Instance.SetSettingHideSocials(value));
        musicSlider.onValueChanged.AddListener(value =>
        {
            BazookaManager.Instance.SetSettingMusicVolume(value);
            MenuMusic.Instance.GetComponent<AudioSource>().volume = value;
        });
        sfxSlider.onValueChanged.AddListener(value => BazookaManager.Instance.SetSettingSFXVolume(value));
    }
}