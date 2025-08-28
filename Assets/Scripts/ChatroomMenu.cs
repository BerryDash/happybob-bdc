using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChatroomMenu : MonoBehaviour
{
    private readonly static WaitForSeconds _waitForSeconds2 = new(2f);
    private readonly static WaitForSeconds _waitForSeconds3 = new(3f);
    public TMP_Text statusText;
    public Button backButton;
    public Button sendButton;
    public TMP_InputField messageInputField;
    public GameObject content;
    public GameObject sampleObject;
    private string statusMessage;
    private Coroutine statusRoutine;
    private Coroutine refreshLoopRoutine;
    private bool shouldScrollToBottom = true;
    public Button downButton;
    private bool isPaused;

    public GameObject optionsPanel;
    public Button optionsPanelExitButton;
    public Button optionsPanelDeleteButton;
    public Button optionsPanelEditButton;
    public Button optionsPanelReportButton;
    public Button optionsPanelCopyButton;
    private ChatroomMessage selectedMessageForOptions;

    public GameObject editMessagePanelSample;
    private GameObject editMessagePanelCurrent;

    public GameObject reportMessagePanel;
    public Button reportMessagePanelExitButton;
    public Button reportMessagePanelSubmitButton;
    public TMP_InputField reportMessagePanelReportReason;
    public Dictionary<string, string> customIcons;

    void Start()
    {
        reportMessagePanelReportReason.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().textWrappingMode = TextWrappingModes.Normal;
        if (BazookaManager.Instance.GetAccountID() == null || BazookaManager.Instance.GetAccountName() == null || BazookaManager.Instance.GetAccountSession() == null)
        {
            sendButton.interactable = false;
            messageInputField.interactable = false;
            ShowStatus("Warning: You are not logged in. Please log in to send messages.");
        }
        backButton.onClick.AddListener(async () => await SceneManager.LoadSceneAsync("MainMenu"));
        sendButton.onClick.AddListener(async () => await HandleMessageSubmit());
        downButton.onClick.AddListener(() => StartCoroutine(ScrollToBottom()));
        messageInputField.textComponent.textWrappingMode = TextWrappingModes.Normal;
        messageInputField.onSubmit.AddListener(async (_) => await HandleMessageSubmit());
        refreshLoopRoutine = StartCoroutine(Loop());

        optionsPanelExitButton.onClick.AddListener(() =>
        {
            optionsPanel.SetActive(false);
            selectedMessageForOptions = null;
        });
        optionsPanelDeleteButton.onClick.AddListener(async () =>
        {
            if (selectedMessageForOptions != null) await HandleDelete();
        });
        optionsPanelDeleteButton.onClick.AddListener(optionsPanelExitButton.onClick.Invoke);
        optionsPanelEditButton.onClick.AddListener(() =>
        {
            var editMessage = Instantiate(editMessagePanelSample, editMessagePanelSample.transform.parent);
            editMessagePanelCurrent = editMessage;
            var editMessageChild = editMessage.transform.GetChild(0);
            var editMessageChildExitButton = editMessageChild.GetChild(0).GetComponent<Button>();
            var editMessageChildSubmitButton = editMessageChild.GetChild(1).GetComponent<Button>();
            var editMessageChildOriginalPreview = editMessageChild.GetChild(2).gameObject;
            var editMessageChildEditedPreview = editMessageChild.GetChild(3).gameObject;
            var editMessageChildNewContentInputBox = editMessageChild.GetChild(4).GetComponent<TMP_InputField>();
            editMessageChildNewContentInputBox.text = Encoding.UTF8.GetString(Convert.FromBase64String(selectedMessageForOptions.Content));
            editMessageChildExitButton.onClick.AddListener(() =>
            {
                editMessagePanelCurrent = null;
                Destroy(editMessage);
                optionsPanel.SetActive(true);
            });
            editMessageChildSubmitButton.onClick.AddListener(async () =>
            {
                await HandleEdit();
                editMessagePanelCurrent = null;
                Destroy(editMessage);
                selectedMessageForOptions = null;
            });

            var rowInfo = editMessageChild.GetChild(2);
            var usernameText = rowInfo.transform.GetChild(2).GetComponent<TMP_Text>();
            var playerIcon = rowInfo.transform.GetChild(3).GetComponent<Image>();
            var playerOverlayIcon = playerIcon.transform.GetChild(0).GetComponent<Image>();
            var messageText = rowInfo.transform.GetChild(4).GetComponent<TMP_Text>();
            var optionsButton = rowInfo.transform.GetChild(5).GetComponent<Button>();

            usernameText.text = selectedMessageForOptions.Username;
            messageText.text = Encoding.UTF8.GetString(Convert.FromBase64String(selectedMessageForOptions.Content));
            if (selectedMessageForOptions.CustomIcon == null)
            {
                playerIcon.sprite = Resources.Load<Sprite>("Icons/Icons/bird_" + selectedMessageForOptions.Icon);
                if (selectedMessageForOptions.Icon == 1)
                {
                    playerIcon.sprite = Tools.GetIconForUser(selectedMessageForOptions.UserID);
                }
                playerOverlayIcon.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + selectedMessageForOptions.Overlay);
                if (selectedMessageForOptions.Overlay == 0)
                {
                    playerOverlayIcon.gameObject.SetActive(false);
                }
                else if (selectedMessageForOptions.Overlay == 8)
                {
                    playerOverlayIcon.transform.localPosition = new UnityEngine.Vector2(-16.56f, 14.81f);
                }
                else if (selectedMessageForOptions.Overlay == 11)
                {
                    playerOverlayIcon.transform.localPosition = new UnityEngine.Vector2(-14.74451f, 20.39122f);
                }
                else if (selectedMessageForOptions.Overlay == 13)
                {
                    playerOverlayIcon.transform.localPosition = new UnityEngine.Vector2(-16.54019f, 14.70365f);
                }
                try
                {
                    playerIcon.color = new Color(selectedMessageForOptions.BirdColor[0] / 255f, selectedMessageForOptions.BirdColor[1] / 255f, selectedMessageForOptions.BirdColor[2] / 255f);
                    playerOverlayIcon.color = new Color(selectedMessageForOptions.OverlayColor[0] / 255f, selectedMessageForOptions.OverlayColor[1] / 255f, selectedMessageForOptions.OverlayColor[2] / 255f);
                }
                catch (Exception)
                {
                    playerIcon.color = Color.white;
                    playerOverlayIcon.color = Color.white;
                }
            }
            else
            {
                Tools.RenderFromBase64(customIcons[selectedMessageForOptions.CustomIcon], playerIcon);
                playerOverlayIcon.gameObject.SetActive(false);
            }

            rowInfo = editMessageChild.GetChild(3);
            usernameText = rowInfo.transform.GetChild(2).GetComponent<TMP_Text>();
            playerIcon = rowInfo.transform.GetChild(3).GetComponent<Image>();
            playerOverlayIcon = playerIcon.transform.GetChild(0).GetComponent<Image>();
            messageText = rowInfo.transform.GetChild(4).GetComponent<TMP_Text>();
            optionsButton = rowInfo.transform.GetChild(5).GetComponent<Button>();

            usernameText.text = selectedMessageForOptions.Username;
            messageText.text = Encoding.UTF8.GetString(Convert.FromBase64String(selectedMessageForOptions.Content));
            editMessageChildNewContentInputBox.onValueChanged.AddListener((value) => messageText.text = value);
            if (selectedMessageForOptions.CustomIcon == null)
            {
                playerIcon.sprite = Resources.Load<Sprite>("Icons/Icons/bird_" + selectedMessageForOptions.Icon);
                if (selectedMessageForOptions.Icon == 1)
                {
                    playerIcon.sprite = Tools.GetIconForUser(selectedMessageForOptions.UserID);
                }
                playerOverlayIcon.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + selectedMessageForOptions.Overlay);
                if (selectedMessageForOptions.Overlay == 0)
                {
                    playerOverlayIcon.gameObject.SetActive(false);
                }
                else if (selectedMessageForOptions.Overlay == 8)
                {
                    playerOverlayIcon.transform.localPosition = new UnityEngine.Vector2(-16.56f, 14.81f);
                }
                else if (selectedMessageForOptions.Overlay == 11)
                {
                    playerOverlayIcon.transform.localPosition = new UnityEngine.Vector2(-14.74451f, 20.39122f);
                }
                else if (selectedMessageForOptions.Overlay == 13)
                {
                    playerOverlayIcon.transform.localPosition = new UnityEngine.Vector2(-16.54019f, 14.70365f);
                }
                try
                {
                    playerIcon.color = new Color(selectedMessageForOptions.BirdColor[0] / 255f, selectedMessageForOptions.BirdColor[1] / 255f, selectedMessageForOptions.BirdColor[2] / 255f);
                    playerOverlayIcon.color = new Color(selectedMessageForOptions.OverlayColor[0] / 255f, selectedMessageForOptions.OverlayColor[1] / 255f, selectedMessageForOptions.OverlayColor[2] / 255f);
                }
                catch (Exception)
                {
                    playerIcon.color = Color.white;
                    playerOverlayIcon.color = Color.white;
                }
            }
            else
            {
                Tools.RenderFromBase64(customIcons[selectedMessageForOptions.CustomIcon], playerIcon);
                playerOverlayIcon.gameObject.SetActive(false);
            }

            optionsPanel.SetActive(false);
            editMessage.SetActive(true);
        });
        optionsPanelReportButton.onClick.AddListener(() => {
            reportMessagePanelReportReason.text = "";
            optionsPanel.SetActive(false);
            reportMessagePanel.SetActive(true);
        });
        reportMessagePanelExitButton.onClick.AddListener(() =>
        {
            editMessagePanelCurrent = null;
            reportMessagePanel.SetActive(false);
            optionsPanel.SetActive(true);
        });
        reportMessagePanelSubmitButton.onClick.AddListener(async () =>
        {
            reportMessagePanel.SetActive(false);
            await HandleReport();
            editMessagePanelCurrent = null;
            selectedMessageForOptions = null;
        });
        optionsPanelCopyButton.onClick.AddListener(() =>
        {
            if (selectedMessageForOptions != null) GUIUtility.systemCopyBuffer = Encoding.UTF8.GetString(Convert.FromBase64String(selectedMessageForOptions.Content));
        });
        optionsPanelCopyButton.onClick.AddListener(optionsPanelExitButton.onClick.Invoke);
    }

    IEnumerator Loop()
    {
        while (true)
        {
            Refresh();
            yield return _waitForSeconds3;
        }
    }

    async Task HandleMessageSubmit()
    {
        if (!sendButton.interactable) return;
        backButton.interactable = false;
        var text = messageInputField.text.Clone() as string;
        messageInputField.text = string.Empty;
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        EncryptedWWWForm dataForm = new();
        dataForm.AddField("content", text);
        dataForm.AddField("token", BazookaManager.Instance.GetAccountSession());
        dataForm.AddField("username", BazookaManager.Instance.GetAccountName());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "sendChatroomMessage.php", dataForm.form);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            backButton.interactable = true;
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
            var jsonResponse = JObject.Parse(response);
            if ((bool)jsonResponse["success"])
            {
                shouldScrollToBottom = true;
                StopCoroutine(refreshLoopRoutine);
                refreshLoopRoutine = StartCoroutine(Loop());
            }
            else
            {
                ShowStatus((string)jsonResponse["message"]);
            }
        }
        backButton.interactable = true;
    }

    void ShowStatus(string content)
    {
        statusMessage = content;
        if (statusRoutine != null) StopCoroutine(statusRoutine);
        statusRoutine = StartCoroutine(StatusRoutine());
    }

    void Update()
    {
        var max = content.GetComponent<RectTransform>().sizeDelta.y;
        var current = content.transform.localPosition.y;
        downButton.gameObject.SetActive(Mathf.Abs(max - current) > 0.1f * max);
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

    async void Refresh()
    {
        if (isPaused) return;
        using UnityWebRequest request = UnityWebRequest.Get(SensitiveInfo.SERVER_DATABASE_PREFIX + "getChatroomMessages.php");
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch chatroom messages: " + request.error);
            return;
        }
        string response = SensitiveInfo.Decrypt(request.downloadHandler.text, SensitiveInfo.SERVER_RECEIVE_TRANSFER_KEY);
        var shouldClear = true;
        switch (response)
        {
            case "-999":
                ShowStatus("Server error while fetching data");
                break;
            case "-998":
                ShowStatus("Client version too outdated to access servers");
                break;
            case "-997":
                ShowStatus("Encryption/decryption issues");
                break;
            case "-996":
                ShowStatus("Can't send requests on self-built instance");
                break;
            default:
                shouldClear = false;
                var jsonResponse = JObject.Parse(response);
                var messages = jsonResponse["messages"].ToObject<ChatroomMessage[]>();
                customIcons = jsonResponse["customIcons"].ToObject<Dictionary<string, string>>();
                var localUserId = BazookaManager.Instance.GetAccountID();
                var sortedMessages = messages.OrderBy(m => m.ID).ToArray();
                for (int i = 0; i < sortedMessages.Length; i++)
                {
                    var message = sortedMessages[i];
                    var obj = content.transform.Find("ChatroomRow_" + message.ID);
                    if (obj != null || message.Deleted)
                    {
                        if (message.Deleted && obj != null)
                        {
                            Destroy(obj.gameObject);
                        }
                        else if (obj != null)
                        {
                            obj.SetSiblingIndex(i + 1);
                            if (obj.GetChild(3).GetComponent<TMP_Text>().text != Encoding.UTF8.GetString(Convert.FromBase64String(message.Content)))
                            {
                                obj.GetChild(3).GetComponent<TMP_Text>().text = Encoding.UTF8.GetString(Convert.FromBase64String(message.Content));
                            }
                            var button = obj.GetChild(4).GetComponent<Button>();
                            if (!button.interactable)
                            {
                                button.interactable = true;
                                button.onClick.AddListener(() => OptionsButtonClick(message, localUserId ?? 0));
                            }
                        }
                        continue;
                    }
                    if (content.transform.childCount > 50)
                    {
                        var firstChild = content.transform.GetChild(1);
                        Destroy(firstChild.gameObject);
                    }

                    var rowInfo = Instantiate(sampleObject, content.transform);
                    var bgImg = rowInfo.transform.GetChild(0).GetComponent<Image>();
                    var usernameText = rowInfo.transform.GetChild(1).GetComponent<TMP_Text>();
                    var playerIcon = rowInfo.transform.GetChild(2).GetComponent<Image>();
                    var playerOverlayIcon = playerIcon.transform.GetChild(0).GetComponent<Image>();
                    var messageText = rowInfo.transform.GetChild(3).GetComponent<TMP_Text>();
                    var optionsButton = rowInfo.transform.GetChild(4).GetComponent<Button>();

                    usernameText.text = message.Username;
                    messageText.text = Encoding.UTF8.GetString(Convert.FromBase64String(message.Content));
                    if (message.CustomIcon == null)
                    {
                        playerIcon.sprite = Resources.Load<Sprite>("Icons/Icons/bird_" + message.Icon);
                        if (message.Icon == 1)
                        {
                            playerIcon.sprite = Tools.GetIconForUser(message.UserID);
                        }
                        playerOverlayIcon.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + message.Overlay);
                        if (message.Overlay == 0)
                        {
                            playerOverlayIcon.gameObject.SetActive(false);
                        }
                        else if (message.Overlay == 8)
                        {
                            playerOverlayIcon.transform.localPosition = new UnityEngine.Vector2(-16.56f, 14.81f);
                        }
                        else if (message.Overlay == 11)
                        {
                            playerOverlayIcon.transform.localPosition = new UnityEngine.Vector2(-14.74451f, 20.39122f);
                        }
                        else if (message.Overlay == 13)
                        {
                            playerOverlayIcon.transform.localPosition = new UnityEngine.Vector2(-16.54019f, 14.70365f);
                        }
                        try
                        {
                            playerIcon.color = new Color(message.BirdColor[0] / 255f, message.BirdColor[1] / 255f, message.BirdColor[2] / 255f);
                            playerOverlayIcon.color = new Color(message.OverlayColor[0] / 255f, message.OverlayColor[1] / 255f, message.OverlayColor[2] / 255f);
                        }
                        catch (Exception)
                        {
                            playerIcon.color = Color.white;
                            playerOverlayIcon.color = Color.white;
                        }
                    }
                    else
                    {
                        Tools.RenderFromBase64(customIcons[message.CustomIcon], playerIcon);
                        playerOverlayIcon.gameObject.SetActive(false);
                    }
                    optionsButton.onClick.AddListener(() => OptionsButtonClick(message, localUserId ?? 0));
                    rowInfo.name = "ChatroomRow_" + message.ID;
                    var entryComponet = rowInfo.AddComponent<ChatroomMenuEntry>();
                    entryComponet.Init(bgImg, optionsButton);
                    rowInfo.SetActive(true);
                }
                break;
        }
        if (shouldClear)
        {
            foreach (Transform item in content.transform)
            {
                if (item.gameObject.activeSelf)
                {
                    Destroy(item.gameObject);
                }
            }
        }
        if (shouldScrollToBottom)
        {
            shouldScrollToBottom = false;
            StartCoroutine(ScrollToBottom());
        }
    }

    async Task HandleDelete()
    {
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("id", selectedMessageForOptions.ID.ToString());
        dataForm.AddField("token", BazookaManager.Instance.GetAccountSession());
        dataForm.AddField("username", BazookaManager.Instance.GetAccountName());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "deleteChatroomMessage.php", dataForm.form);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        shouldScrollToBottom = true;
        StopCoroutine(refreshLoopRoutine);
        refreshLoopRoutine = StartCoroutine(Loop());
    }

    async Task HandleEdit()
    {
        var newContent = editMessagePanelCurrent.transform.GetChild(0).GetChild(4).GetComponent<TMP_InputField>().text;
        selectedMessageForOptions.Content = newContent;
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("id", selectedMessageForOptions.ID.ToString());
        dataForm.AddField("content", newContent);
        dataForm.AddField("token", BazookaManager.Instance.GetAccountSession());
        dataForm.AddField("username", BazookaManager.Instance.GetAccountName());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "editChatroomMessage.php", dataForm.form);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        shouldScrollToBottom = true;
        StopCoroutine(refreshLoopRoutine);
        refreshLoopRoutine = StartCoroutine(Loop());
        var button = content.transform.Find("ChatroomRow_" + selectedMessageForOptions.ID).GetChild(4).GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.interactable = false;
    }

    async Task HandleReport()
    {
        EncryptedWWWForm dataForm = new();
        dataForm.AddField("id", selectedMessageForOptions.ID.ToString());
        dataForm.AddField("reason", reportMessagePanelReportReason.text);
        dataForm.AddField("token", BazookaManager.Instance.GetAccountSession());
        dataForm.AddField("username", BazookaManager.Instance.GetAccountName());
        using UnityWebRequest request = UnityWebRequest.Post(SensitiveInfo.SERVER_DATABASE_PREFIX + "reportChatroomMessage.php", dataForm.form);
        request.SetRequestHeader("Requester", "BerryDashClient");
        request.SetRequestHeader("ClientVersion", Application.version);
        request.SetRequestHeader("ClientPlatform", Application.platform.ToString());
        await request.SendWebRequest();
        shouldScrollToBottom = true;
        StopCoroutine(refreshLoopRoutine);
        refreshLoopRoutine = StartCoroutine(Loop());
    }

    IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        content.transform.localPosition = new UnityEngine.Vector2(0f, content.GetComponent<RectTransform>().sizeDelta.y);
    }

    void OptionsButtonClick(ChatroomMessage message, BigInteger localUserId)
    {
        selectedMessageForOptions = message;
        optionsPanelDeleteButton.transform.parent.gameObject.SetActive(localUserId != 0 && message.UserID == localUserId);
        optionsPanelEditButton.transform.parent.gameObject.SetActive(localUserId != 0 && message.UserID == localUserId);
        optionsPanelReportButton.transform.parent.gameObject.SetActive(localUserId != 0 && message.UserID != localUserId);
        optionsPanel.SetActive(true);
    }

    void OnApplicationPause(bool pause)
    {
        isPaused = pause;
    }
}