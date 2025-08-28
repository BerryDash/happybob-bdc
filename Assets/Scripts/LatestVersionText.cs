using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LatestVersionText : MonoBehaviour
{
    public static LatestVersionText Instance;
    public TMP_Text text;
    public Button updateButton;
    private string latest = null;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (updateButton != null)
        {
            updateButton.onClick.AddListener(() =>
            {
                updateButton.gameObject.SetActive(false);
            });
        }
    }

    void Start()
    {
        RefreshText();
        if (latest == null) GetLatestVersion();
    }

    void OnEnable()
    {
        RefreshText();
    }

    async void GetLatestVersion()
    {
        using UnityWebRequest request = UnityWebRequest.Get(SensitiveInfo.SERVER_DATABASE_PREFIX + "getLatestVersion.php");
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            latest = "-1";
        }
        else
        {
            latest = request.downloadHandler.text;
        }
        RefreshText();
    }

    internal void RefreshText()
    {
        if (text == null || updateButton == null) return;
        if (latest == null)
            text.text = "";
        else if (latest == "-1")
            text.text = "";
        else
            text.text = "";
        updateButton.gameObject.SetActive(false);
    }
}