using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayerPauseMenu : MonoBehaviour
{
    public static GamePlayerPauseMenu Instance;
    public Button backButton;
    public Button continueButton;
    public Button statsButton;
    public Button editUiButton;
    public Button resetUiButton;
    public AudioSource songLoop;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public TMP_Text boostText;
    public Button pauseButton;
    public Button restartButton;
    public Button jumpButton;
    public Button rightButton;
    public Button leftButton;
    internal bool editingUI = false;

    public GameObject statsMenu;
    public Button statsMenuExitButton;
    public TMP_Text statsText;

    public Toggle settingFullscreenToggle;
    public Toggle settingVSyncToggle;
    public Toggle settingRandomMusicToggle;
    public Slider musicSlider;
    public Slider sfxSlider;
    public TMP_Text musicSliderText;
    public TMP_Text sfxSliderText;

    void Awake()
    {
        Instance = this;
        continueButton.onClick.AddListener(GamePlayer.instance.DisablePause);
        musicSlider.onValueChanged.AddListener(value =>
        {
            BazookaManager.Instance.SetSettingMusicVolume(value);
            songLoop.volume = value;
        });
        sfxSlider.onValueChanged.AddListener(value =>
        {
            BazookaManager.Instance.SetSettingSFXVolume(value);
        });
        editUiButton.onClick.AddListener(() =>
        {
            ToggleEditingUI();
        });
        resetUiButton.onClick.AddListener(() =>
        {
            ((RectTransform)scoreText.transform).anchoredPosition = new Vector2(0f, -70f);
            ((RectTransform)highScoreText.transform).anchoredPosition = new Vector2(0f, -140f);
            ((RectTransform)boostText.transform).anchoredPosition = new Vector2(0f, -190f);
            PlayerPrefs.DeleteKey("DraggedUIScoreText");
            PlayerPrefs.DeleteKey("DraggedUIHighScoreText");
            PlayerPrefs.DeleteKey("DraggedUIBoostText");
            if (Application.isMobilePlatform)
            {
                ((RectTransform)pauseButton.transform).anchoredPosition = new Vector2(128f, -128f);
                ((RectTransform)restartButton.transform).anchoredPosition = new Vector2(-128f, -128f);
                ((RectTransform)jumpButton.transform).anchoredPosition = new Vector2(-128f, 288f);
                ((RectTransform)rightButton.transform).anchoredPosition = new Vector2(-128f, 128f);
                ((RectTransform)leftButton.transform).anchoredPosition = new Vector2(128f, 128f);
                PlayerPrefs.DeleteKey("DraggedUIPauseButton");
                PlayerPrefs.DeleteKey("DraggedUIRestartButton");
                PlayerPrefs.DeleteKey("DraggedUIJumpButton");
                PlayerPrefs.DeleteKey("DraggedUIRightButton");
                PlayerPrefs.DeleteKey("DraggedUILeftButton");
            }
        });
        statsButton.onClick.AddListener(() =>
        {
            statsMenu.SetActive(true);
            var text = new StringBuilder();
            text.AppendLine("High Score: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreHighScore()));
            text.AppendLine("Total Normal Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalNormalBerries()));
            text.AppendLine("Total Poison Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalPoisonBerries()));
            text.AppendLine("Total Slow Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalSlowBerries()));
            text.AppendLine("Total Ultra Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalUltraBerries()));
            text.AppendLine("Total Speedy Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalSpeedyBerries()));
            text.AppendLine("Total Random Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalRandomBerries()));
            text.AppendLine("Total Anti Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalAntiBerries()));
            text.AppendLine("Total Time Slow Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalTimeSlowBerries()));
            text.AppendLine("Total Purple Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalPurpleBerries()));
            text.AppendLine("Total Time Freeze Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalTimeFreezeBerries()));
            text.AppendLine("Total Evil Berries: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalEvilBerries()));
            text.AppendLine("Total Attempts: " + Tools.FormatWithCommas(BazookaManager.Instance.GetGameStoreTotalAttepts()));
            statsText.text = text.ToString();
        });
        statsMenuExitButton.onClick.AddListener(() =>
        {
            statsMenu.SetActive(false);
            statsText.text = string.Empty;
        });

        musicSlider.value = BazookaManager.Instance.GetSettingMusicVolume();
        sfxSlider.value = BazookaManager.Instance.GetSettingSFXVolume();
        if (!Application.isMobilePlatform)
        {
            settingFullscreenToggle.isOn = BazookaManager.Instance.GetSettingFullScreen() == true;
            settingVSyncToggle.isOn = BazookaManager.Instance.GetSettingVsync() == true;
            settingRandomMusicToggle.isOn = BazookaManager.Instance.GetSettingRandomMusic() == true;
            settingFullscreenToggle.onValueChanged.AddListener(value =>
            {
                BazookaManager.Instance.SetSettingFullScreen(value);
                var width = Display.main.systemWidth;
                var height = Display.main.systemHeight;
                Screen.SetResolution(width, height, value);
            });
            settingVSyncToggle.onValueChanged.AddListener(value =>
            {
                BazookaManager.Instance.SetSettingVsync(value);
                QualitySettings.vSyncCount = value ? 1 : -1;
            });
        }
        else
        {
            settingFullscreenToggle.interactable = false;
            settingVSyncToggle.interactable = false;
            settingRandomMusicToggle.isOn = BazookaManager.Instance.GetSettingRandomMusic() == true;
        }
        settingRandomMusicToggle.onValueChanged.AddListener(value => BazookaManager.Instance.SetSettingRandomMusic(value));
        musicSlider.onValueChanged.AddListener(value =>
        {
            BazookaManager.Instance.SetSettingMusicVolume(value);
            GamePlayer.instance.backgroundMusic.volume = value;
        });
        sfxSlider.onValueChanged.AddListener(value => BazookaManager.Instance.SetSettingSFXVolume(value));
    }

    public void ToggleEditingUI()
    {
        editingUI = !editingUI;
        musicSlider.gameObject.SetActive(!musicSlider.gameObject.activeSelf);
        sfxSlider.gameObject.SetActive(!sfxSlider.gameObject.activeSelf);
        musicSliderText.gameObject.SetActive(!musicSliderText.gameObject.activeSelf);
        sfxSliderText.gameObject.SetActive(!sfxSliderText.gameObject.activeSelf);
        settingFullscreenToggle.gameObject.SetActive(!settingFullscreenToggle.gameObject.activeSelf);
        settingVSyncToggle.gameObject.SetActive(!settingVSyncToggle.gameObject.activeSelf);
        settingRandomMusicToggle.gameObject.SetActive(!settingRandomMusicToggle.gameObject.activeSelf);
        backButton.gameObject.SetActive(!backButton.gameObject.activeSelf);
        continueButton.gameObject.SetActive(!continueButton.gameObject.activeSelf);
        statsButton.gameObject.SetActive(!statsButton.gameObject.activeSelf);
        editUiButton.transform.GetChild(0).GetComponent<TMP_Text>().text = editUiButton.transform.GetChild(0).GetComponent<TMP_Text>().text == "Edit UI" ? "Done" : "Edit UI";
        resetUiButton.gameObject.SetActive(!resetUiButton.gameObject.activeSelf);
        scoreText.GetComponent<DraggableUI>().canDrag = !scoreText.GetComponent<DraggableUI>().canDrag;
        highScoreText.GetComponent<DraggableUI>().canDrag = !highScoreText.GetComponent<DraggableUI>().canDrag;
        boostText.GetComponent<DraggableUI>().canDrag = !boostText.GetComponent<DraggableUI>().canDrag;
        if (Application.isMobilePlatform)
        {
            var pauseDraggableUI = pauseButton.GetComponent<DraggableUI>();
            var restartDraggableUI = restartButton.GetComponent<DraggableUI>();
            var jumpDraggableUI = jumpButton.GetComponent<DraggableUI>();
            var rightDraggableUI = rightButton.GetComponent<DraggableUI>();
            var leftDraggableUI = leftButton.GetComponent<DraggableUI>();
            pauseButton.transform.parent.SetSiblingIndex(pauseDraggableUI.canDrag ? 0 : 2);
            pauseDraggableUI.canDrag = !pauseDraggableUI.canDrag;
            restartDraggableUI.canDrag = !restartDraggableUI.canDrag;
            jumpDraggableUI.canDrag = !jumpDraggableUI.canDrag;
            rightDraggableUI.canDrag = !rightDraggableUI.canDrag;
            leftDraggableUI.canDrag = !leftDraggableUI.canDrag;
            pauseButton.interactable = !pauseDraggableUI.canDrag;
            restartButton.interactable = !restartDraggableUI.canDrag;
            jumpButton.interactable = !jumpDraggableUI.canDrag;
            rightButton.interactable = !rightDraggableUI.canDrag;
            leftButton.interactable = !leftDraggableUI.canDrag;
            jumpButton.transform.GetChild(0).GetComponent<TMP_Text>().color = !jumpDraggableUI.canDrag ? GamePlayer.instance.isGrounded ? Color.white : Color.red : Color.white;
            restartButton.interactable = GamePlayer.instance.score != 0;
        }
    }
}