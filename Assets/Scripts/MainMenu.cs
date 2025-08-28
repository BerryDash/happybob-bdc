using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public Button exitButton;
    public TMP_Text updateText;
    public Button updateButton;
    public GameObject selfBuiltPanel;

    void Awake()
    {
        LatestVersionText.Instance.text = updateText;
        LatestVersionText.Instance.updateButton = updateButton;
        LatestVersionText.Instance.RefreshText();

        if (Application.isMobilePlatform || Application.isEditor)
        {
            exitButton.gameObject.SetActive(false);
        }
        else
        {
            exitButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }
        if (SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY.Trim() == string.Empty || SensitiveInfo.SERVER_SEND_TRANSFER_KEY.Trim() == string.Empty) selfBuiltPanel.SetActive(false);
    }
}