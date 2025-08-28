using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconMarketplaceManager : MonoBehaviour
{
    public GameObject normalPanel;
    public GameObject downloadPanel;
    public GameObject uploadPanel;

    public IconMarketplaceUploadIcon uploadPanelScript;
    public IconMarketplaceDownloadIcon downloadPanelScript;

    public Button downloadButton;
    public Button uploadButton;
    public TMP_Text coinText;

    void Start()
    {
        downloadButton.onClick.AddListener(() => SwitchPanel(1));
        if (BazookaManager.Instance.GetAccountID() != null && BazookaManager.Instance.GetAccountName() != null && BazookaManager.Instance.GetAccountSession() != null)
        {
            uploadButton.onClick.AddListener(() => SwitchPanel(2));
        }
        else
        {
            uploadButton.interactable = false;
            uploadButton.transform.GetChild(0).transform.localPosition = new Vector3(0, 10, 0);
            uploadButton.transform.GetChild(1).GetComponent<TMP_Text>().text = "Not logged in to an account";
        }
        SwitchPanel(0);
    }

    internal void SwitchPanel(int panelIndex)
    {
        switch (panelIndex)
        {
            case 0:
                coinText.text = "You have " + Tools.FormatWithCommas(BazookaManager.Instance.GetCustomBirdIconData().Balance) + " coins";
                normalPanel.SetActive(true);
                downloadPanel.SetActive(false);
                uploadPanel.SetActive(false);
                break;
            case 1:
                foreach (Transform item in downloadPanelScript.content.transform)
                {
                    if (item.gameObject.activeSelf)
                    {
                        Destroy(item.gameObject);
                    }
                }
                normalPanel.SetActive(false);
                downloadPanel.SetActive(true);
                uploadPanel.SetActive(false);
                downloadPanelScript.Load();
                break;
            case 2:
                uploadPanelScript.Reset();
                normalPanel.SetActive(false);
                downloadPanel.SetActive(false);
                uploadPanel.SetActive(true);
                break;
        }
    }
}