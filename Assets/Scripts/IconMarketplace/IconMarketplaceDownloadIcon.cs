using System.Collections;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class IconMarketplaceDownloadIcon : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds2 = new(2f);
    public IconMarketplaceManager marketplaceManager;
    public TMP_Text statusText;
    public TMP_Text balanceText;
    public Button backButton;
    public GameObject content;
    public GameObject sample;
    private string statusMessage;
    private Coroutine statusRoutine;

    void Awake()
    {
        backButton.onClick.AddListener(() => marketplaceManager.SwitchPanel(0));
    }

    internal void Load()
    {
        GetIcons();
        balanceText.text = "You have " + Tools.FormatWithCommas(BazookaManager.Instance.GetCustomBirdIconData().Balance) + " coins to spend";
    }

    async void GetIcons()
    {
        backButton.interactable = false;
        foreach (Transform item in content.transform)
        {
            if (item.gameObject.activeSelf)
            {
                Destroy(item.gameObject);
            }
        }
        ShowStatus("Loading...");
        using UnityWebRequest request = UnityWebRequest.Get(SensitiveInfo.SERVER_DATABASE_PREFIX + "getMarketplaceIcons.php");
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            backButton.interactable = true;
            ShowStatus("Failed to make HTTP request");
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        if (response == "-999")
        {
            backButton.interactable = true;
            ShowStatus("Server error while fetching data");
            return;
        }
        else if (response == "-998")
        {
            backButton.interactable = true;
            ShowStatus("Client version too outdated to access servers");
            return;
        }
        else if (response == "-997")
        {
            backButton.interactable = true;
            ShowStatus("Encryption/decryption issues");
            return;
        }
        else if (response == "-996")
        {
            backButton.interactable = true;
            ShowStatus("Can't send requests on self-built instance");
            return;
        }
        else
        {
            ShowStatus(null);
            var icons = JsonConvert.DeserializeObject<MarketplaceIconType[]>(response);
            var localUserID = BazookaManager.Instance.GetAccountID();
            foreach (var entry in icons)
            {
                GameObject newIcon = Instantiate(sample, content.transform);
                newIcon.name = "IconEntry";

                Tools.RenderFromBase64(entry.Data, newIcon.transform.GetChild(0).GetChild(0).GetComponent<Image>());
                newIcon.transform.GetChild(1).GetComponent<TMP_Text>().text = "Bird Name: " + entry.Name;
                newIcon.transform.GetChild(2).GetComponent<TMP_Text>().text = "Price " + Tools.FormatWithCommas(entry.Price) + " coin";
                newIcon.transform.GetChild(3).GetComponent<TMP_Text>().text = "Designer Name: " + entry.CreatorUsername;

                var btn = newIcon.transform.GetChild(4).GetComponent<Button>();
                var btnText = btn.transform.GetChild(0).GetComponent<TMP_Text>();

                bool alreadyBought = BazookaManager.Instance.GetCustomBirdIconData().Data.Any(d => d.UUID == entry.UUID);
                if (alreadyBought)
                {
                    btn.interactable = false;
                    if (localUserID != entry.CreatorUserID)
                    {
                        btnText.text = "Purchased";
                    }
                    else
                    {
                        btnText.text = "Claimed";
                    }
                }
                    else
                    {
                        btn.onClick.AddListener(() => HandlePurchase(entry, btn, localUserID));
                        if (localUserID != entry.CreatorUserID)
                        {
                            btnText.text = "Purchase";
                        }
                        else
                        {
                            btnText.text = "Claim";
                        }
                    }

                newIcon.SetActive(true);
            }
        }
        backButton.interactable = true;
    }

    void HandlePurchase(MarketplaceIconType data, Button button, BigInteger? localUserID)
    {
        button.interactable = false;
        MarketplaceIconStorageType marketplaceIconStorage = BazookaManager.Instance.GetCustomBirdIconData();
        if (localUserID != data.CreatorUserID)
        {
            if (data.Price > marketplaceIconStorage.Balance)
            {
                button.interactable = true;
                ShowStatus("You can't afford this icon! You need " + Tools.FormatWithCommas(data.Price - marketplaceIconStorage.Balance) + " more coins");
                return;
            }
            marketplaceIconStorage.Balance -= data.Price;
        }
        var list = marketplaceIconStorage.Data.ToList();
        list.Add(data);
        marketplaceIconStorage.Data = list.ToArray();
        if (localUserID != data.CreatorUserID)
        {
            button.transform.GetChild(0).GetComponent<TMP_Text>().text = "Purchased";
        }
        else
        {
            button.transform.GetChild(0).GetComponent<TMP_Text>().text = "Claimed";
        }
        balanceText.text = "You have " + Tools.FormatWithCommas(marketplaceIconStorage.Balance) + " coins to spend";
        BazookaManager.Instance.SetCustomBirdIconData(marketplaceIconStorage);
    }

    void ShowStatus(string content)
    {
        if (content == null)
        {
            if (statusRoutine != null) StopCoroutine(statusRoutine);
            statusText.gameObject.SetActive(false);
            statusText.text = "";
        }
        statusMessage = content;
        if (statusRoutine != null) StopCoroutine(statusRoutine);
        statusRoutine = StartCoroutine(StatusRoutine());
    }

    IEnumerator StatusRoutine()
    {
        statusText.gameObject.SetActive(true);
        statusText.text = statusMessage;
        statusText.color = new Color(statusText.color.r, statusText.color.g, statusText.color.b, 0f);

        float t = 0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            float a = t / 0.5f;
            statusText.color = new Color(statusText.color.r, statusText.color.g, statusText.color.b, a);
            yield return null;
        }

        yield return _waitForSeconds2;

        t = 0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            float a = 1f - (t / 0.5f);
            statusText.color = new Color(statusText.color.r, statusText.color.g, statusText.color.b, a);
            yield return null;
        }

        statusText.gameObject.SetActive(false);
        statusText.text = "";
        statusRoutine = null;
    }
}
