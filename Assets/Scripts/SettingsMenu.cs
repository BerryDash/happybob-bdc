using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public GameObject settingsMenu;
    public ColorPanel colorMenu;
    public Button toggleButton;

    public Toggle setting1toggle;
    public Toggle setting2toggle;
    public Toggle setting3toggle;
    public Toggle setting4toggle;
    public Slider musicSlider;
    public Slider sfxSlider;
    public GameObject settingsUI;

    public Button switchColorTypeButton;
    public int colorType = 0;
    public bool colorCanSave = true;

    private void Start()
    {
        colorMenu.Init(Color.white, Color.white);
        colorMenu.OnColorChanged += color =>
        {
            if (!colorCanSave) return;

            if (colorType == 0) BazookaManager.Instance.SetColorSettingBackground(color);
            else if (colorType == 1) BazookaManager.Instance.SetColorSettingMenuBackground(color);
            else if (colorType == 2) BazookaManager.Instance.SetColorSettingButton(color);
            else if (colorType == 3) BazookaManager.Instance.SetColorSettingButtonContent(color);
            else if (colorType == 4) BazookaManager.Instance.SetColorSettingText(color);

            foreach (CustomColorObject customColorObject in FindObjectsByType<CustomColorObject>(FindObjectsSortMode.None)) customColorObject.SetColor();
        };
        toggleButton.onClick.AddListener(() =>
        {
            settingsMenu.SetActive(!settingsMenu.activeSelf);
            colorMenu.gameObject.SetActive(!colorMenu.gameObject.activeSelf);
            toggleButton.transform.GetChild(0).GetComponent<TMP_Text>().text = settingsMenu.activeSelf ? "Colors" : "Settings";
            SwitchColorType(4);
            foreach (CustomColorObject customColorObject in FindObjectsByType<CustomColorObject>(FindObjectsSortMode.None)) customColorObject.SetColor();
        });
        switchColorTypeButton.onClick.AddListener(() => SwitchColorType());

        musicSlider.value = BazookaManager.Instance.GetSettingMusicVolume();
        sfxSlider.value = BazookaManager.Instance.GetSettingSFXVolume();
        if (!Application.isMobilePlatform)
        {
            setting1toggle.isOn = BazookaManager.Instance.GetSettingFullScreen() == true;
            setting2toggle.isOn = BazookaManager.Instance.GetSettingShowFPS() == true;
            setting3toggle.isOn = BazookaManager.Instance.GetSettingVsync() == true;
            setting4toggle.isOn = BazookaManager.Instance.GetSettingRandomMusic() == true;
            setting1toggle.onValueChanged.AddListener(value =>
            {
                BazookaManager.Instance.SetSettingFullScreen(value);
                var width = Display.main.systemWidth;
                var height = Display.main.systemHeight;
                Screen.SetResolution(width, height, value);
            });
            setting3toggle.onValueChanged.AddListener(value =>
            {
                BazookaManager.Instance.SetSettingVsync(value);
                QualitySettings.vSyncCount = value ? 1 : -1;
            });
        }
        else
        {
            setting1toggle.gameObject.SetActive(false);
            setting2toggle.isOn = BazookaManager.Instance.GetSettingShowFPS() == true;
            setting3toggle.gameObject.SetActive(false);
            setting4toggle.isOn = BazookaManager.Instance.GetSettingRandomMusic() == true;
        }
        setting2toggle.onValueChanged.AddListener(value =>
        {
            BazookaManager.Instance.SetSettingShowFPS(value);
        });
        setting4toggle.onValueChanged.AddListener(value => BazookaManager.Instance.SetSettingRandomMusic(value));
        musicSlider.onValueChanged.AddListener(value =>
        {
            BazookaManager.Instance.SetSettingMusicVolume(value);
            MenuMusic.Instance.GetComponent<AudioSource>().volume = value;
        });
        sfxSlider.onValueChanged.AddListener(value => BazookaManager.Instance.SetSettingSFXVolume(value));
        Tools.RefreshHierarchy(setting1toggle.transform.parent.gameObject);
    }

    void SwitchColorType(int color = -1)
    {
        var type = color == -1 ? colorType : color;
        var text = colorMenu.transform.GetChild(0).GetComponent<TMP_Text>();
        var colorToSet = new JArray(255, 255, 255);
        switch (type)
        {
            case 0: //IGBGColor
                text.text = "Menu background color";
                colorType = 1;
                colorToSet = BazookaManager.Instance.GetColorSettingMenuBackground();
                colorMenu.defaultColor = new(24f / 255f, 24f / 255f, 24f / 255f);
                break;
            case 1: //MBGColor
                text.text = "Button color";
                colorType = 2;
                colorToSet = BazookaManager.Instance.GetColorSettingButton();
                colorMenu.defaultColor = new(1f, 1f, 1f);
                break;
            case 2: //BCColor
                text.text = "Button content color";
                colorType = 3;
                colorToSet = BazookaManager.Instance.GetColorSettingButtonContent();
                colorMenu.defaultColor = new(0f, 0f, 0f);
                break;
            case 3: //TColor
                text.text = "Text color";
                colorType = 4;
                colorToSet = BazookaManager.Instance.GetColorSettingText();
                colorMenu.defaultColor = new(1f, 1f, 1f);
                break;
            case 4: //IGColor
                text.text = "In game background color";
                colorType = 0;
                colorToSet = BazookaManager.Instance.GetColorSettingBackground();
                colorMenu.defaultColor = new(58f / 255f, 58f / 255f, 58f / 255f);
                break;
        }
        colorCanSave = false;
        colorMenu.SetColor(new Color((int)colorToSet[0] / 255f, (int)colorToSet[1] / 255f, (int)colorToSet[2] / 255f));
        colorCanSave = true;
    }
}