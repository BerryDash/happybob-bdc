using System;
using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardsMenu : MonoBehaviour
{
    public TMP_Text statusText;
    public Button backButton;
    public Button refreshButton;

    public GameObject selectionPanel;
    public Button selectionScoreButton;
    public Button selectionBerryButton;
    public Button selectionCoinButton;

    public GameObject scorePanel;
    public GameObject scoreContent;
    public GameObject scoreSampleObject;

    public GameObject berryPanel;
    public GameObject berryContent;
    public TMP_Dropdown berryShowTypeDropdown;
    public GameObject berrySampleObject;

    public GameObject coinPanel;
    public GameObject coinContent;
    public GameObject coinSampleObject;

    public Dictionary<string, string> customIcons;

    void Awake()
    {
        selectionScoreButton.onClick.AddListener(() => SwitchMenu(1));
        selectionBerryButton.onClick.AddListener(() => SwitchMenu(2));
        selectionCoinButton.onClick.AddListener(() => SwitchMenu(3));

        berryShowTypeDropdown.onValueChanged.AddListener(value => GetTopPlayersBerry(value));


        backButton.onClick.AddListener(async () =>
        {
            if (selectionPanel.activeSelf) await SceneManager.LoadSceneAsync("MainMenu");
            else if (scorePanel.activeSelf || berryPanel.activeSelf || coinPanel.activeSelf) SwitchMenu(0);
        });
        refreshButton.onClick.AddListener(() =>
        {
            if (scorePanel.activeSelf) GetTopPlayersScore();
            else if (berryPanel.activeSelf) GetTopPlayersBerry(berryShowTypeDropdown.value);
            else if (coinPanel.activeSelf) GetTopPlayersCoin();
        });
    }

    void SwitchMenu(int menu)
    {
        UpdateStatus(false, "");
        if (scorePanel.activeSelf)
        {
            foreach (Transform item in scoreContent.transform)
            {
                if (item.gameObject.activeSelf)
                {
                    Destroy(item.gameObject);
                }
            }
        }
        else if (berryPanel.activeSelf)
        {
            foreach (Transform item in berryContent.transform)
            {
                if (item.gameObject.activeSelf)
                {
                    Destroy(item.gameObject);
                }
            }
        }
        else if (coinPanel.activeSelf)
        {
            foreach (Transform item in coinContent.transform)
            {
                if (item.gameObject.activeSelf)
                {
                    Destroy(item.gameObject);
                }
            }
        }

        switch (menu)
        {
            case 0:
                refreshButton.gameObject.SetActive(false);
                selectionPanel.SetActive(true);
                scorePanel.SetActive(false);
                berryPanel.SetActive(false);
                coinPanel.SetActive(false);
                break;
            case 1:
                refreshButton.transform.localPosition = new UnityEngine.Vector2(-402.5f, -282.33f);
                refreshButton.gameObject.SetActive(true);
                GetTopPlayersScore();
                selectionPanel.SetActive(false);
                scorePanel.SetActive(true);
                berryPanel.SetActive(false);
                coinPanel.SetActive(false);
                break;
            case 2:
                refreshButton.transform.localPosition = new UnityEngine.Vector2(402.5f, 282.33f);
                refreshButton.gameObject.SetActive(true);
                berryShowTypeDropdown.value = 0;
                GetTopPlayersBerry(0);
                selectionPanel.SetActive(false);
                scorePanel.SetActive(false);
                berryPanel.SetActive(true);
                coinPanel.SetActive(false);
                break;
            case 3:
                refreshButton.transform.localPosition = new UnityEngine.Vector2(-402.5f, -282.33f);
                refreshButton.gameObject.SetActive(true);
                GetTopPlayersCoin();
                selectionPanel.SetActive(false);
                scorePanel.SetActive(false);
                berryPanel.SetActive(false);
                coinPanel.SetActive(true);
                break;
        }
    }

    async void GetTopPlayersScore()
    {
        backButton.interactable = false;
        refreshButton.interactable = false;
        foreach (Transform item in scoreContent.transform)
        {
            if (item.gameObject.activeSelf)
            {
                Destroy(item.gameObject);
            }
        }
        UpdateStatus(true, "Loading...");
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("type", "0");
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "getTopPlayers.php", dataForm.form);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            UpdateStatus(false);
            string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
            if (response == "-999")
            {
                UpdateStatus(true, "Server error while fetching data");
            }
            else if (response == "-998")
            {
                UpdateStatus(true, "Client version too outdated to access servers");
            }
            else if (response == "-997")
            {
                UpdateStatus(true, "Encryption/decryption issues");
            }
            else if (response == "-996")
            {
                UpdateStatus(true, "Can't send requests on self-built instance");
            }
            else if (response == "-1")
            {
                UpdateStatus(true, "No entries for this leaderboard found!");
            }
            else
            {
                var jsonResponse = JObject.Parse(response);
                var entries = (JArray)jsonResponse["entries"];
                customIcons = jsonResponse["customIcons"].ToObject<Dictionary<string, string>>();
                for (int i = 0; i < entries.Count; i++)
                {
                    var entry = JObject.Parse(entries[i].ToString());
                    var username = (string)entry["username"];
                    var highScore = BigInteger.Parse((string)entry["value"]);
                    var icon = (int)entry["icon"];
                    var overlay = (int)entry["overlay"];
                    var uid = BigInteger.Parse((string)entry["userid"]);
                    var birdColor = (JArray)entry["birdColor"];
                    var overlayColor = (JArray)entry["overlayColor"];
                    var customIcon = (string)entry["customIcon"];

                    var entryInfo = Instantiate(scoreSampleObject, scoreContent.transform);
                    var usernameText = entryInfo.transform.GetChild(0).GetComponent<TMP_Text>();
                    var playerIcon = usernameText.transform.GetChild(0).GetComponent<Image>();
                    var playerOverlayIcon = playerIcon.transform.GetChild(0).GetComponent<Image>();
                    var highScoreText = entryInfo.transform.GetChild(1).GetComponent<TMP_Text>();

                    if (BazookaManager.Instance.GetAccountID() == uid)
                    {
                        usernameText.color = Color.aquamarine;
                        highScoreText.color = Color.aquamarine;
                    }

                    usernameText.text = $"{username} (#{i + 1})";
                    highScoreText.text += Tools.FormatWithCommas(highScore);
                    if (customIcon == null)
                    {
                        playerIcon.sprite = Resources.Load<Sprite>("Icons/Icons/bird_" + icon);
                        if (icon == 1)
                        {
                            playerIcon.sprite = Tools.GetIconForUser(uid);
                        }
                        playerOverlayIcon.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + overlay);
                        if (overlay == 0)
                        {
                            playerOverlayIcon.gameObject.SetActive(false);
                        }
                        else if (overlay == 8)
                        {
                            playerOverlayIcon.transform.localPosition = new UnityEngine.Vector2(-16.56f, 14.81f);
                        }
                        else if (overlay == 11)
                        {
                            playerOverlayIcon.transform.localPosition = new UnityEngine.Vector2(-14.74451f, 20.39122f);
                        }
                        else if (overlay == 13)
                        {
                            playerOverlayIcon.transform.localPosition = new UnityEngine.Vector2(-16.54019f, 14.70365f);
                        }
                        try
                        {
                            playerIcon.color = new Color((int)birdColor[0] / 255f, (int)birdColor[1] / 255f, (int)birdColor[2] / 255f);
                            playerOverlayIcon.color = new Color((int)overlayColor[0] / 255f, (int)overlayColor[1] / 255f, (int)overlayColor[2] / 255f);
                        }
                        catch (Exception)
                        {
                            playerIcon.color = Color.white;
                            playerOverlayIcon.color = Color.white;
                        }
                    }
                    else
                    {
                        Tools.RenderFromBase64(customIcons[customIcon], playerIcon);
                        playerOverlayIcon.gameObject.SetActive(false);
                    }
                    entryInfo.SetActive(true);
                }
            }
        }
        else
        {
            UpdateStatus(true, "Failed to fetch leaderboard stats");
        }
        backButton.interactable = true;
        refreshButton.interactable = true;
    }

    async void GetTopPlayersBerry(int showAmount)
    {
        berryShowTypeDropdown.interactable = false;
        backButton.interactable = false;
        refreshButton.interactable = false;
        foreach (Transform item in berryContent.transform)
        {
            if (item.gameObject.activeSelf)
            {
                Destroy(item.gameObject);
            }
        }
        UpdateStatus(true, "Loading...");
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("showType", showAmount.ToString());
        dataForm.AddField("type", "1");
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "getTopPlayers.php", dataForm.form);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            UpdateStatus(false);
            string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
            if (response == "-999")
            {
                UpdateStatus(true, "Server error while fetching data");
            }
            else if (response == "-998")
            {
                UpdateStatus(true, "Client version too outdated to access servers");
            }
            else if (response == "-997")
            {
                UpdateStatus(true, "Encryption/decryption issues");
            }
            else if (response == "-996")
            {
                UpdateStatus(true, "Can't send requests on self-built instance");
            }
            else if (response == "-1")
            {
                UpdateStatus(true, "No entries for this leaderboard found!");
            }
            else
            {
                var jsonResponse = JObject.Parse(response);
                var entries = (JArray)jsonResponse["entries"];
                customIcons = jsonResponse["customIcons"].ToObject<Dictionary<string, string>>();
                for (int i = 0; i < entries.Count; i++)
                {
                    var entry = JObject.Parse(entries[i].ToString());
                    var username = (string)entry["username"];
                    var highScore = BigInteger.Parse((string)entry["value"]);
                    var icon = (int)entry["icon"];
                    var overlay = (int)entry["overlay"];
                    var uid = BigInteger.Parse((string)entry["userid"]);
                    var birdColor = (JArray)entry["birdColor"];
                    var overlayColor = (JArray)entry["overlayColor"];
                    var customIcon = (string)entry["customIcon"];

                    var entryInfo = Instantiate(berrySampleObject, berryContent.transform);
                    var usernameText = entryInfo.transform.GetChild(0).GetComponent<TMP_Text>();
                    var playerIcon = usernameText.transform.GetChild(0).GetComponent<Image>();
                    var playerOverlayIcon = playerIcon.transform.GetChild(0).GetComponent<Image>();
                    var highScoreText = entryInfo.transform.GetChild(1).GetComponent<TMP_Text>();

                    if (BazookaManager.Instance.GetAccountID() == uid)
                    {
                        usernameText.color = Color.aquamarine;
                        highScoreText.color = Color.aquamarine;
                    }

                    usernameText.text = $"{username} (#{i + 1})";
                    highScoreText.text += Tools.FormatWithCommas(highScore);
                    if (customIcon == null)
                    {
                        playerIcon.sprite = Resources.Load<Sprite>("Icons/Icons/bird_" + icon);
                        if (icon == 1)
                        {
                            playerIcon.sprite = Tools.GetIconForUser(uid);
                        }
                        playerOverlayIcon.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + overlay);
                        if (overlay == 0)
                        {
                            playerOverlayIcon.gameObject.SetActive(false);
                        }
                        else if (overlay == 8)
                        {
                            playerOverlayIcon.transform.localPosition = new UnityEngine.Vector2(-16.56f, 14.81f);
                        }
                        else if (overlay == 11)
                        {
                            playerOverlayIcon.transform.localPosition = new UnityEngine.Vector2(-14.74451f, 20.39122f);
                        }
                        else if (overlay == 13)
                        {
                            playerOverlayIcon.transform.localPosition = new UnityEngine.Vector2(-16.54019f, 14.70365f);
                        }
                        try
                        {
                            playerIcon.color = new Color((int)birdColor[0] / 255f, (int)birdColor[1] / 255f, (int)birdColor[2] / 255f);
                            playerOverlayIcon.color = new Color((int)overlayColor[0] / 255f, (int)overlayColor[1] / 255f, (int)overlayColor[2] / 255f);
                        }
                        catch (Exception)
                        {
                            playerIcon.color = Color.white;
                            playerOverlayIcon.color = Color.white;
                        }
                    }
                    else
                    {
                        Tools.RenderFromBase64(customIcons[customIcon], playerIcon);
                        playerOverlayIcon.gameObject.SetActive(false);
                    }
                    entryInfo.SetActive(true);
                }
            }
        }
        else
        {
            UpdateStatus(true, "Failed to fetch leaderboard stats");
        }
        berryShowTypeDropdown.interactable = true;
        backButton.interactable = true;
        refreshButton.interactable = true;
    }

    async void GetTopPlayersCoin()
    {
        backButton.interactable = false;
        refreshButton.interactable = false;
        foreach (Transform item in coinContent.transform)
        {
            if (item.gameObject.activeSelf)
            {
                Destroy(item.gameObject);
            }
        }
        UpdateStatus(true, "Loading...");
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("type", "2");
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "getTopPlayers.php", dataForm.form);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            UpdateStatus(false);
            string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
            if (response == "-999")
            {
                UpdateStatus(true, "Server error while fetching data");
            }
            else if (response == "-998")
            {
                UpdateStatus(true, "Client version too outdated to access servers");
            }
            else if (response == "-997")
            {
                UpdateStatus(true, "Encryption/decryption issues");
            }
            else if (response == "-996")
            {
                UpdateStatus(true, "Can't send requests on self-built instance");
            }
            else if (response == "-1")
            {
                UpdateStatus(true, "No entries for this leaderboard found!");
            }
            else
            {
                var jsonResponse = JObject.Parse(response);
                var entries = (JArray)jsonResponse["entries"];
                customIcons = jsonResponse["customIcons"].ToObject<Dictionary<string, string>>();
                for (int i = 0; i < entries.Count; i++)
                {
                    var entry = JObject.Parse(entries[i].ToString());
                    var username = (string)entry["username"];
                    var highScore = BigInteger.Parse((string)entry["value"]);
                    var icon = (int)entry["icon"];
                    var overlay = (int)entry["overlay"];
                    var uid = BigInteger.Parse((string)entry["userid"]);
                    var birdColor = (JArray)entry["birdColor"];
                    var overlayColor = (JArray)entry["overlayColor"];
                    var customIcon = (string)entry["customIcon"];

                    var entryInfo = Instantiate(coinSampleObject, coinContent.transform);
                    var usernameText = entryInfo.transform.GetChild(0).GetComponent<TMP_Text>();
                    var playerIcon = usernameText.transform.GetChild(0).GetComponent<Image>();
                    var playerOverlayIcon = playerIcon.transform.GetChild(0).GetComponent<Image>();
                    var highScoreText = entryInfo.transform.GetChild(1).GetComponent<TMP_Text>();

                    if (BazookaManager.Instance.GetAccountID() == uid)
                    {
                        usernameText.color = Color.aquamarine;
                        highScoreText.color = Color.aquamarine;
                    }

                    usernameText.text = $"{username} (#{i + 1})";
                    highScoreText.text += Tools.FormatWithCommas(highScore);
                    if (customIcon == null)
                    {
                        playerIcon.sprite = Resources.Load<Sprite>("Icons/Icons/bird_" + icon);
                        if (icon == 1)
                        {
                            playerIcon.sprite = Tools.GetIconForUser(uid);
                        }
                        playerOverlayIcon.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + overlay);
                        if (overlay == 0)
                        {
                            playerOverlayIcon.gameObject.SetActive(false);
                        }
                        else if (overlay == 8)
                        {
                            playerOverlayIcon.transform.localPosition = new UnityEngine.Vector2(-16.56f, 14.81f);
                        }
                        else if (overlay == 11)
                        {
                            playerOverlayIcon.transform.localPosition = new UnityEngine.Vector2(-14.74451f, 20.39122f);
                        }
                        else if (overlay == 13)
                        {
                            playerOverlayIcon.transform.localPosition = new UnityEngine.Vector2(-16.54019f, 14.70365f);
                        }
                        try
                        {
                            playerIcon.color = new Color((int)birdColor[0] / 255f, (int)birdColor[1] / 255f, (int)birdColor[2] / 255f);
                            playerOverlayIcon.color = new Color((int)overlayColor[0] / 255f, (int)overlayColor[1] / 255f, (int)overlayColor[2] / 255f);
                        }
                        catch (Exception)
                        {
                            playerIcon.color = Color.white;
                            playerOverlayIcon.color = Color.white;
                        }
                    }
                    else
                    {
                        Tools.RenderFromBase64(customIcons[customIcon], playerIcon);
                        playerOverlayIcon.gameObject.SetActive(false);
                    }
                    entryInfo.SetActive(true);
                }
            }
        }
        else
        {
            UpdateStatus(true, "Failed to fetch leaderboard stats");
        }
        backButton.interactable = true;
        refreshButton.interactable = true;
    }

    private void UpdateStatus(bool enabled, string message = "")
    {
        statusText.gameObject.SetActive(enabled);
        statusText.text = message;
    }
}