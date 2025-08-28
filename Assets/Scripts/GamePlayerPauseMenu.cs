using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayerPauseMenu : MonoBehaviour
{
    public static GamePlayerPauseMenu Instance;
    public Button backButton;
    public Button continueButton;
    public Button editUiButton;
    public Button resetUiButton;
    public AudioSource songLoop;
    public Slider musicSlider;
    public Slider sfxSlider;
    public TMP_Text musicSliderText;
    public TMP_Text sfxSliderText;
    public TMP_Text fpsText;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public TMP_Text boostText;
    public Button pauseButton;
    public Button restartButton;
    public Button jumpButton;
    public Button rightButton;
    public Button leftButton;
    internal bool editingUI = false;

    void Awake()
    {
        Instance = this;
        musicSlider.value = BazookaManager.Instance.GetSettingMusicVolume();
        sfxSlider.value = BazookaManager.Instance.GetSettingSFXVolume();
        backButton.onClick.AddListener(async () =>
        {
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenu");
        });
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
            ((RectTransform)fpsText.transform).anchoredPosition = new Vector2(210f, -35f);
            ((RectTransform)scoreText.transform).anchoredPosition = new Vector2(0f, -70f);
            ((RectTransform)highScoreText.transform).anchoredPosition = new Vector2(0f, -140f);
            ((RectTransform)boostText.transform).anchoredPosition = new Vector2(0f, -190f);
            PlayerPrefs.DeleteKey("DraggedUIFPSText");
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
    }

    public void ToggleEditingUI()
    {
        editingUI = !editingUI;
        musicSlider.gameObject.SetActive(!musicSlider.gameObject.activeSelf);
        sfxSlider.gameObject.SetActive(!sfxSlider.gameObject.activeSelf);
        musicSliderText.gameObject.SetActive(musicSlider.gameObject.activeSelf);
        sfxSliderText.gameObject.SetActive(sfxSlider.gameObject.activeSelf);
        backButton.gameObject.SetActive(!backButton.gameObject.activeSelf);
        continueButton.gameObject.SetActive(!continueButton.gameObject.activeSelf);
        editUiButton.transform.GetChild(0).GetComponent<TMP_Text>().text = editUiButton.transform.GetChild(0).GetComponent<TMP_Text>().text == "Edit UI" ? "Done" : "Edit UI";
        resetUiButton.gameObject.SetActive(!resetUiButton.gameObject.activeSelf);
        fpsText.GetComponent<DraggableUI>().canDrag = !fpsText.GetComponent<DraggableUI>().canDrag;
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
        }
    }
}